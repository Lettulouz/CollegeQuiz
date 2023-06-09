import { useContext, useState, useEffect } from "react";
import { alertDanger, alertOff, generateErrorMessage, getCommonFetchObj, WAITING_SCREEN } from "../utils/common";
import { SessionContext } from "../quiz-session-renderer";
import { HubConnectionBuilder } from "@microsoft/signalr";

const JoinToQuizSessionComponent = () => {
    const {
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert, setQuizName,
        setScreenAction, isJoinClicked, setIsJoinClicked, currentAnswer
    } = useContext(SessionContext);

    const [ joinDisabled, setJoinDisabled ] = useState(true);

    const onSubmitJoinToSession = e => {
        e.preventDefault();
        if (isJoinClicked) return;
        setIsJoinClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token.toUpperCase()}`, getCommonFetchObj('POST'))
            .then(r => {
                if (r.ok) {
                    return r.json();
                }
                throw new Error(r.status);
            })
            .then(r => {
                if (r.isGood) {
                    setQuizName(r.quizName);
                    setScreenAction(currentAnswer === [] ? r.screenType : WAITING_SCREEN);
                    setIsConnect(true);
                } else {
                    setIsJoinClicked(false);
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                setIsJoinClicked(false);
                if (e === undefined) return;
                setAlert(alertDanger(generateErrorMessage(e.message)));
            });
    };

    useEffect(() => {
        setJoinDisabled(token.length !== 5);
    }, [ token ]);

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl('/quizUserSessionHub')
            .withAutomaticReconnect()
            .build();
        connection.start()
            .then(() => connection.invoke('getConnectionId').then(connId => setConnectionId(connId)))
            .catch(() => setAlert(alertDanger('Nieudane dołączenie do sesji.')));
        setConnection(connection);
    }, []);

    return (
        <div className="container margin-hide-menu-top mb-1">
            <div className="row d-flex justify-content-center">
                <div className="col-12 col-md-6 mb-5">
                    <div className="card px-5 py-5 h-100 justify-content-center">
                        <form className="form-data mt-3" onSubmit={onSubmitJoinToSession}>
                            <label className="mainName2">Dołącz do quizu</label>
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