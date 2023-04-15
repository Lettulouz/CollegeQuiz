import { useLoadableContent } from "./Hooks.jsx";
import { alertInfo, alertDanger, alertOff, getCommonFetchObj } from "./Utils.jsx";
const { useEffect, useState, createContext, useContext } = React;

const SessionContext = createContext(null);

const QR_CODE_BLOB = document.getElementById('inject-qr-code-blob').innerText;
const SESS_TOKEN = document.getElementById('inject-sess-code').innerText;

const QuizManagerLeftContentComponent = () => {
    const { connection } = useContext(SessionContext);
    const [ lobbyData, setLobbyData ] = useState({ name: '', host: '' });
    const [ allParticipants, setAllParticipants ] = useState({ Connected: [], Disconnected: [] });
    
    useEffect(() => {
        connection.on('GetAllParticipants', data => {
            setAllParticipants(JSON.parse(data));
        });
        fetch(`/api/v1/dotnet/QuizSessionAPI/GetLobbyData/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(({ name, host }) => setLobbyData({ name, host }))
            .then(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    }, []);
    
    const allConnected = allParticipants.Connected.map(name => <li key={name}>{name}</li>);
    const allDisconnected = allParticipants.Disconnected.map(name => <li key={name}>{name}</li>);
    
    return (
        <div className="col-lg-3 px-0 mb-2 mb-lg-0 order-lg-0 order-1">
            <div className="card px-3 py-3 h-100">
                <h3 className="mb-2">Poczekalnia</h3>
                <h6 className="text-black-50 mb-0">Nazwa quizu</h6>
                <h5 className="mb-2 lh-sm">{lobbyData.name}</h5>
                <h6 className="text-black-50 mb-0">Host</h6>
                <h5 className="mb-4 lh-sm">{lobbyData.host}</h5>
                <h6 className="text-black-50 mb-0">Połączeni: ({allParticipants.Connected.length})</h6>
                {allParticipants.Connected.length
                    ? <ul className="fw-bold">{allConnected}</ul>
                    : <li className="fw-bold">-</li>}
                <h6 className="text-black-50 mb-0">Rozłączeni: ({allParticipants.Disconnected.length})</h6>
                {allParticipants.Disconnected.length
                    ? <ul className="fw-bold">{allDisconnected}</ul>
                    : <li className="fw-bold">-</li>}
            </div>
        </div>
    );
};

const QuizManagerCenterComponent = () => {
    
    const copyToClipboard = () => {
        navigator.clipboard.writeText(SESS_TOKEN);
        toastr.success(`Skopiowano kod ${SESS_TOKEN} do schowka.`);
    };
    
    return (
        <div className="col-lg-6 px-0 px-lg-2 mb-2 mb-lg-0 order-lg-0 order-3">
            <div className="d-flex justify-content-center">
                <div className="card px-3 py-3 h-100">
                    <img src={`data:image/gif;base64,${QR_CODE_BLOB}`} alt=""/>
                </div>
            </div>
            <div className="row mt-2">
                <div className="col-9">
                    <span className="form-control text-center" id="lobbyCode" style={{ fontSize: 38, fontWeight: 'bolder' }}>
                        {SESS_TOKEN}
                    </span>
                </div>
                <div className="col-3">
                    <a className="btn btn-lg btn-dark w-100 h-100 text-white" type="button" data-bs-toggle="tooltip"
                       data-bs-placement="left" data-bs-title="Kopiuj do schowka" onClick={copyToClipboard}>
                        <i className="bi bi-clipboard"></i>
                    </a>
                </div>
            </div>
        </div>
    );
};

const QuizManagerRightContentComponent = () => {
    const { connection, setAlert } = useContext(SessionContext);
    const [ counting, setCounting ] = useState(5);
    
    const startQuiz = () => {
        if (counting === 0) return;
        let i = counting;
        const interval = setInterval(() => {
            connection.invoke('INIT_GAME_SEQUENCER_P2P', i, SESS_TOKEN).then(r => setCounting(r));
            if (i === 0) {
                clearInterval(interval);
                connection.invoke('START_GAME_P2P', SESS_TOKEN).then(_ => {
                    setAlert(alertInfo('GRA WŁAŚNIE SIĘ ROZPOCZĘŁA!'));
                    setTimeout(() => setAlert(alertOff()), 3000);
                });
            }
            --i;
        }, 1000);
    };
    
    return (
        <div className="col-lg-3 px-0 mb-2 mb-lg-0 order-lg-0 order-2">
            <div className="card px-3 py-3 h-100">
                {counting !== 0 && <button className="btn btn-info mt-1 mx-1" onClick={startQuiz}>
                    {counting !== 5 ? `Zaczyna się za ${counting}...` : 'Rozpocznij'}
                </button>}
                <button className="btn btn-danger text-white mt-1 mx-1" onClick={() => window.location.reload()}>
                    Zakończ quiz
                </button>
            </div>
        </div>
    );
};

const QuizManagerRootComponent = () => {
    const [ isActive, setActiveCallback ] = useLoadableContent();
    const [ isJoinable, setIsJoinable ] = useState(false);
    const [ connectionId, setConnectionId ] = useState('');
    const [ connection, setConnection ] = useState(null);
    const [ alert, setAlert ] = useState({ active: false, style: 'alert-success', message: '' });
    
    const estabilishedRoomConnection = () => {
        fetch(`/api/v1/dotnet/QuizSessionAPI/EstabilishedHostRoom/${connectionId}/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsJoinable(true);
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
        const connectionObj = new signalR.HubConnectionBuilder()
            .withUrl('/quizManagerSessionHub')
            .build();
        connectionObj.start()
            .then(() => connectionObj.invoke('getConnectionId').then(connId => {
                setConnectionId(connId);
                setConnection(connectionObj);
                setActiveCallback();
            }))
            .catch(() => setAlert(alertDanger('Nieudane dołączenie do sesji.')));
    }, []);
    
    return (
        <SessionContext.Provider value={{
            connection, setAlert
        }}>
            {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-4`} role="alert">
                <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
            </div>}
            {isActive && <div className="container">
                {isJoinable ? <div className="row position-relative">
                    <QuizManagerLeftContentComponent/>
                    <QuizManagerCenterComponent/>
                    <QuizManagerRightContentComponent/>
                </div> : <div className="text-center">
                    <button className="btn click-me mx-auto mt-5" onClick={estabilishedRoomConnection}>Stwórz nowy pokój</button>
                </div>}
            </div>}
        </SessionContext.Provider>
    );
};

ReactDOM
    .createRoot(document.getElementById('quizManagerRoot'))
    .render(<QuizManagerRootComponent/>);
