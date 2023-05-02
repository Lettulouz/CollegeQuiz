import { alertDanger, alertOff, getCommonFetchObj, WAITING_SCREEN } from "../Utils.jsx";
import { SessionContext } from "./QuizSessionRenderer.jsx";

const JoinToQuizSessionComponent = () => {
    const {
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert, setQuizName,
        setScreenAction, isJoinClicked, setIsJoinClicked, question
    } = React.useContext(SessionContext);

    const [ joinDisabled, setJoinDisabled ] = React.useState(true);

    const onSubmitJoinToSession = e => {
        e.preventDefault();
        if (isJoinClicked) return;
        setIsJoinClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token.toUpperCase()}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setQuizName(r.quizName);
                    setScreenAction(question !== "" ? r.screenType : WAITING_SCREEN);
                    setToken("");
                    setIsConnect(true);
                } else {
                    setIsJoinClicked(false);
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                setIsJoinClicked(false);
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };

    React.useEffect(() => {
        setJoinDisabled(token.length !== 5);
    }, [ token ]);

    React.useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/quizUserSessionHub')
            .withAutomaticReconnect()
            .build();
        connection.start()
            .then(() => connection.invoke('getConnectionId').then(connId => setConnectionId(connId)))
            .catch(() => setAlert(alertDanger('Nieudane dołączenie do sesji.')));
        setConnection(connection);
    }, []);

    return (
        <div className="container mt-5 mb-1">
            <div className="row d-flex justify-content-center mt-5">
                <div className="col-12 col-md-6 mb-5">
                    <div className="card px-5 py-5 h-100 justify-content-center">
                        <form className="form-data mt-3" onSubmit={onSubmitJoinToSession}>
                            <label id="mainName">Dołącz do quizu</label>
                            {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-4`} role="alert">
                                <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                                <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                            </div>}
                            <div className="forms-inputs mb-4">
                                <label id="username">Token</label>
                                <input type="text" className="form-control" value={token} onChange={e => setToken(e.target.value)}
                                    pattern="[a-zA-Z]{5}" maxLength="5" placeholder="np. RGKQE"/>
                                <button className={`btn btn-color-one mt-4 text-white w-100 ${joinDisabled && 'disabled'}`}
                                        type="submit">Dołącz</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default JoinToQuizSessionComponent;