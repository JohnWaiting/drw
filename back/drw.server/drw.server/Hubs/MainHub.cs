using drw.server.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace drw.server.Hubs
{
    public class MainHub : Hub
    {
        private UserManagerService _userManagerService;

        public MainHub(UserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public async Task Connect(string nickname, string gameId = null)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                gameId = Guid.NewGuid().ToString().Substring(0, 5);
            }

            await Clients.Client(Context.ConnectionId).SendAsync("onConnected", gameId);
            _userManagerService.AddOrUpdate(nickname, gameId, Context.ConnectionId);

            string userGroup = gameId + "_" + nickname;

            // отключаем пользователей с тем же ником в той же игре
            await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
            await Clients.OthersInGroup(userGroup)
                .SendAsync("onDisconnected", "Duplicate session");

            // оповещаем пользователей в той же игре о подключении нового игрока
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId)
                .SendAsync("onUsersChanged", _userManagerService.GetActivePlayers(gameId));
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if( _userManagerService.TryGetUserByConnectionId(Context.ConnectionId, out var user))
            {
                // оповещаем пользователей, что игрок покинул их
                await Clients.OthersInGroup(user.Game).SendAsync("onUsersChanged", _userManagerService.GetActivePlayers(user.Game));
            }

            _userManagerService.RemoveUserByConnectionId(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task PlayerReadyToStart(bool ready)
        {
            if (!_userManagerService.TryGetUserByConnectionId(Context.ConnectionId, out var user))
            {
                return;
            }
        }

        // Фаза 0 - все должны нажать кнопку "Готов"
        // Если кто-то не нажал и прошло N сек, фаза сбрасывается
        // Если кто-то отвалился в процессе, фаза сбрасывается

        // Фаза 1 - нужно написать фразу.
        // Если игрок отваливается, его должно вернуть в эту же фазу
        // Если все игроки отваливаются, игра окончена
        // Фаза считается законченной, если прошло N секунд или все отправили слово

        // Фаза 2 - нужно нарисовать картинку
        // Каждому пользователю приходит фраза от одного из соседей (нужно хранить последовательность, чтобы случайно не переслать тому же)
        // Он должен нарисовать картинку. Состояние картинки раз в X секунд синкается с сервером (на случай если чел отвалится)
        // Фаза заканчивается когда все нарисовали картинку и нажали "закончить" или вышло время

        // Фаза П - пауза
        // В любой момент времени можно поставить игру на паузу
        // Если все отвалятся, игра закончится
        // если сняться с паузы, игра продолжится
        // в режим паузы можно переходить в фазе 1 или 2

        // Фаза К - конец игры
        // 
    }

    // Стейт игрока: 
    // - код игры
    // - имя игрока 
    // - ConnectionID

    // Стейт игры:
    // - фазы (1 и 2)
    // - состояние паузы

    // Стейт фазы 1 и 2:
    // - источник работы игроков
    // - результат работы игроков
    // - флаг готовности результата

    public class MessageBus
    {

    }
}
