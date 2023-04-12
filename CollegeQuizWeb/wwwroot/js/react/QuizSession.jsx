const { useEffect, useState, createContext, useContext } = React;

const alertInfo = (message) => ({ active: true, style: 'alert-success', message });
const alertDanger = (message) => ({ active: true, style: 'alert-danger', message });
const alertOff = () => ({ active: false, style: 'alert-success', message: '' });

const SessionContext = createContext(null);

const ShowWaitingScreenComponent = () => {
    const { connection, token, connectionId, setIsConnect, setAlert } = useContext(SessionContext);
    
    const [ message, setMessage ] = useState('');
    const [ messages, setMessages ] = useState([]);
    
    const sendMessage = e => {
        e.preventDefault();
        fetch(`/api/v1/dotnet/QuizSessionAPI/SendMessage/${token}/${message}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(r => r)
    }
    
    const leaveRoom = () => {
        fetch(`/api/v1/dotnet/QuizSessionAPI/LeaveRoom/${connectionId}/${token}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsConnect(false);
                    setAlert(alertInfo(r.message));
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .then(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };
    
    useEffect(() => {
        connection.on('ReceivedMessage', data => {
            setMessages(oldArray => ([ ...oldArray, data ]));
        });
    }, []);
    
    return (
        <>
            <div>oczekiwanie na rozpoczęcie quizu</div>
            <p>wiadomości:</p>
            <ul>{messages.map(m => <li>{m}</li>)}</ul>
            <form onSubmit={sendMessage}>
                <input type="text" value={message} onChange={e => setMessage(e.target.value)}/>
                <button type="submit">wyslij</button>
            </form>
            <button onClick={leaveRoom}>Opuść pokój</button>
        </>
    );
};

const JoinToSessionComponent = () => {
    const {
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert
    } = useContext(SessionContext);
    
    const onSubmitJoinToSession = e => {
        e.preventDefault();
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setAlert(alertInfo(`dołączono do pokoju, nazwa quizu: ${r.quizName}`));
                    setIsConnect(true);
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .then(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };
    
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/quizSessionHub')
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
                    {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-4`} role="alert">
                        <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                        <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                    </div>}
                    <div className="card px-5 py-5 h-100 justify-content-center">
                        <form className="form-data mt-3" onSubmit={onSubmitJoinToSession}>
                            <label id="mainName">Dołącz do quizu</label>
                            <div className="forms-inputs mb-4">
                                <label id="username">Token</label>
                                <input type="text" className="form-control" value={token} onChange={e => setToken(e.target.value)}
                                       pattern="[a-zA-Z]{5}" placeholder="np. RGKQE"/>
                                <button type="submit" className="btn btn-color-one mt-4 text-white w-100">Dołącz</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

const QuizSessionRootComponent = () => {
    const [ showContent, setShowContent ] = useState(false);
    const [ connection, setConnection ] = useState(null)
    const [ connectionId, setConnectionId ] = useState('');
    const [ isConnect, setIsConnect ] = useState(false);
    const [ token, setToken ] = useState('');
    const [ alert, setAlert ] = useState({ active: false, style: 'alert-success', message: '' });

    useEffect(() => {
        document.getElementById('react-loadable-spinner-content').style.cssText = 'display:none !important';
        setShowContent(true);
    }, []);
    
    return (
        <SessionContext.Provider value={{
            connection, setConnection, setIsConnect, connectionId, setConnectionId, token, setToken, alert, setAlert
        }}>
            {showContent && <>
                {isConnect ? <ShowWaitingScreenComponent/> : <JoinToSessionComponent/>}
            </>}
        </SessionContext.Provider>
    );
}

ReactDOM
    .createRoot(document.getElementById('quizSessionRoot'))
    .render(<QuizSessionRootComponent/>);
