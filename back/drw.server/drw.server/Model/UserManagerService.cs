using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace drw.server.Model
{
    public class UserManagerService
    {
        private readonly Dictionary<string, User> _users = new Dictionary<string, User>();

        public void AddOrUpdate(string user, string game, string connectionId)
        {
            var player = _users.Values.FirstOrDefault(x => x.Name == user && x.Game == game);
            _users[connectionId] = player == null
                ? new User { Name = user, Game = game }
                : player;
        }

        public string[] GetActiveGames()
        {
            return _users.Values.Select(x => x.Game).Distinct().ToArray();
        }

        public string[] GetActivePlayers(string game)
        {
            return _users.Values.Where(x => x.Game == game).Select(x => x.Name).ToArray();
        }

        public bool TryGetUserByConnectionId(string connectionId, out User user)
        {
            return _users.TryGetValue(connectionId, out user);
        }

        public void RemoveUserByConnectionId(string connectionId)
        {
            _users.Remove(connectionId);
        }
    }

    public class User
    {
        public string Name { get; set; }

        public string Game { get; set; }
    }

    public class PreGamePhase
    {
        // эта фаза появляется при создании игры
        // она заканчивается когда число игроков > 1 и все нажали "готов"

        public async Task Start()
        {
            // фаза закончится успешно если все нажмут "начать"
            // фаза закончится неуспешно если все выйдут



        }

        public async Task AskToStart(string groupName)
        {

        }
    }
}
