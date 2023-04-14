import { useLoadableContent } from "./Hooks.jsx";
import {
    alertInfo, alertDanger, alertOff, WAITING_SCREEN, getCommonFetchObj, COUNTING_SCREEN, IN_GAME
} from "./Utils.jsx";
const { useEffect, useState, createContext, useContext, useRef } = React;

const SessionContext = createContext(null);

const LeaveSessionButtonComponent = () => {
    const { token, connectionId, setIsConnect, setAlert, setScreenAction } = useContext(SessionContext);
    const modalRef = useRef()
    
    const leaveRoom = () => {
        fetch(`/api/v1/dotnet/QuizSessionAPI/LeaveRoom/${connectionId}/${token}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsConnect(false);
                    setScreenAction(WAITING_SCREEN);
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
    
    const showModal = () => new bootstrap.Modal(modalRef.current, { backdrop: 'static', keyboard: false }).show();
    const hideModal = () => bootstrap.Modal.getInstance(modalRef.current).hide();
    
    return (
        <>
            <div className="modal fade" id="confirm-leave-session-modal" tabIndex="-1" aria-hidden="false" ref={modalRef}>
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5">Opuszczenie aktywnej sesji</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div className="modal-body fw-normal">
                            Czy na pewno chcesz opuścić aktywną sesję? Jeśli sesja się jeszcze nie skończy, będziesz
                            mógł/mogła do niej ponownie dołączyć.
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn-color-one bg-danger text-white" data-bs-dismiss="modal"
                                onClick={leaveRoom}>
                                Opuść sesję
                            </button>
                            <button type="button" className="btn-color-one" data-bs-dismiss="modal"
                                onClick={hideModal}>
                                Zamknij okno
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <button className="btn-color-one bg-danger text-white" onClick={showModal}>Opuść pokój</button>
        </>
    );
}

const MainWindowGameComponent = () => {
    const { connection, setScreenAction, screenAction, setIsConnect, setAlert, quizName } = useContext(SessionContext);
    const [ counting, setCounting ] = useState(5);
    
    useEffect(() => {
        connection.on("INIT_GAME_SEQUENCER_P2P", counter => {
            setScreenAction(COUNTING_SCREEN);
            setCounting(counter);
        });
        connection.on("START_GAME_P2P", () => {
            setScreenAction(IN_GAME);
            // TODO: pobieranie pierwszego pytania
        });
        connection.on("OnDisconectedSession", data => {
            connection.stop().then(_ => {
                setIsConnect(false);
                setAlert(alertDanger(data));
            });
        });
    }, []);

    const renderComponentSection = () => {
        switch(screenAction) {
            case WAITING_SCREEN: return (
                <div className="mt-5">
                    <div className="mt-5 d-flex flex-column align-items-center">
                        <div className="spinner-border load-spinner-circle mt-4" role="status"></div>
                        <p className="mt-3 fs-4 text-prim-color text-center">
                            Oczekiwanie na uruchomienie quizu "<strong>{quizName}</strong>" przez hosta...
                        </p>
                    </div>
                </div>
            );
            case COUNTING_SCREEN: return (
                <div className="mt-5">
                    <div className="mt-5 d-flex flex-column align-items-center">
                        <p className="mt-3 fs-4 text-prim-color text-center">
                            Przygotuj się! Quiz "<strong>{quizName}</strong>" uruchamia się za:
                        </p>
                        <h2 className="fw-bold fs-1 text-prim-color">{counting}</h2>
                    </div>
                </div>
            );
            default: return (
                <div>
                    in game action
                </div>
            );
        }
    };
    
    return (
        <div className="row">
            {renderComponentSection()}
        </div>
    );
};

const HeaderPanelComponent = () => {
    const { isConnect } = useContext(SessionContext);
    return (
        <>
            {isConnect ? <div className="row justify-content-between">
                <div className="col-md-4">
                    123
                </div>
                <div className="col-md-4">
                    321
                </div>
                <div className="col-md-4">
                    <LeaveSessionButtonComponent/>
                </div>
            </div> : <div className="row">
                <LeaveSessionButtonComponent/>
            </div>}
        </>
    );
};

const JoinToSessionComponent = () => {
    const {
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert, setQuizName
    } = useContext(SessionContext);
    
    const onSubmitJoinToSession = e => {
        e.preventDefault();
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setQuizName(r.quizName);
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
    const [ connection, setConnection ] = useState(null)
    const [ connectionId, setConnectionId ] = useState('');
    const [ isConnect, setIsConnect ] = useState(false);
    const [ token, setToken ] = useState('');
    const [ alert, setAlert ] = useState({ active: false, style: 'alert-success', message: '' });
    const [ screenAction, setScreenAction ] = useState(WAITING_SCREEN);
    const [ quizName, setQuizName ] = useState('');

    const [ isActive, setActiveCallback ] = useLoadableContent();
    useEffect(() => setActiveCallback(), []);
    
    return (
        <SessionContext.Provider value={{
            connection, setConnection, setIsConnect, connectionId, setConnectionId, token, setToken, alert, setAlert,
            screenAction, setScreenAction, quizName, setQuizName
        }}>
            {isActive && <>
                {isConnect ? <>
                    <HeaderPanelComponent/>
                    <MainWindowGameComponent/>
                </> : <JoinToSessionComponent/>}
            </>}
        </SessionContext.Provider>
    );
};

ReactDOM
    .createRoot(document.getElementById('quizSessionRoot'))
    .render(<QuizSessionRootComponent/>);
