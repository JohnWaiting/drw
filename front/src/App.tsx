import './App.css';
import * as signalR from "@microsoft/signalr";
import * as React from "react";
import { 
    Switch,
    Route,
    Router
} from "react-router-dom";
import { createBrowserHistory } from 'history';


const history = createBrowserHistory();

interface GamePageState {
}

class LobbyPage extends React.Component<any, GamePageState> {
    render() {
        return (
            <div>
                Lobby page
            </div>
        );
    }
}

const messagesRecievedInfo: string[] = [];

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://"+ TODO +"/hub")
    .build();

function formatToLogTime(date: Date): string {
    return date.getFullYear() + ":" 
        + (date.getMonth()+1) + ":" 
        + date.getDate() + ":"
        + date.getHours() + ":"  
        + date.getMinutes() + ":" 
        + date.getSeconds() + ":" 
        + date.getMilliseconds();
}

connection.on("onConnected", function () {
    const logString = `${formatToLogTime(new Date())}: onConnected.\nargs: ${JSON.stringify(arguments)}`;

    console.log(logString);

    messagesRecievedInfo.push(logString);

    history.push("/lobby");
});

connection.start().catch(err => document.write(err));

interface HomeState {
    nickname: string;
    gameKey: string;
}

class HomePage extends React.Component<any, HomeState> {
    constructor(props: {}) {
        super(props);
        
        this.state = {
            nickname: "",
            gameKey: ""
        }
        
        this.handleNicknameChange = this.handleNicknameChange.bind(this);
        this.handleGameKeyChange = this.handleGameKeyChange.bind(this);
        this.handleCreateGameRequest = this.handleCreateGameRequest.bind(this);
        this.handleJoinGameRequest = this.handleJoinGameRequest.bind(this);
    }

    handleNicknameChange(event: React.ChangeEvent<HTMLInputElement>) {
        const input = event.target as HTMLInputElement;
        this.setState({nickname: input.value});
    }

    handleGameKeyChange(event: React.ChangeEvent<HTMLInputElement>) {
        const input = event.target as HTMLInputElement;
        this.setState({gameKey: input.value});
    }

    handleCreateGameRequest() {
        this.requestGameConnectRequest(this.state.nickname);
    }

    handleJoinGameRequest() {
        this.requestGameConnectRequest(this.state.nickname, this.state.gameKey);
    }

    requestGameConnectRequest(nickname: string, gameKey: string | null = null) {
        connection.send("connect", nickname, gameKey).then(() => console.log('game connect request finished'));
    }

    render() {
        return (
            <div className="box">
                <h1 className="title is-1">DRW</h1>

                <div className="field has-addons">
                    <div className="control">
                        <input className="input" type="text" value={this.state.nickname} placeholder="Enter nickname" onChange={this.handleNicknameChange} />
                    </div>
                    <div className="control">
                        <a className="button is-info" onClick={this.handleCreateGameRequest}>
                            Create new game
                        </a>
                    </div>
                </div>
                
                <div className="field has-addons">
                    <div className="control">
                        <input className="input" type="text" value={this.state.gameKey} placeholder="Enter game key" onChange={this.handleGameKeyChange} />
                    </div>
                    <div className="control">
                        <a className="button is-info" onClick={this.handleJoinGameRequest}>
                            Join game
                        </a>
                    </div>
                </div>
            </div>
        )
    }
}

interface AppState {
}

class App extends React.Component<any, AppState> {
    render() {
        return (
            <Router history={history}>
                <div className="App">
                    <header className="App-header">
                        <Switch>
                            <Route exact path="/">
                                <HomePage />
                            </Route>
                            <Route path="/lobby">
                                <LobbyPage />
                            </Route>
                        </Switch>
                    </header>
                </div>
            </Router>
        );
    }
}

export default App;
