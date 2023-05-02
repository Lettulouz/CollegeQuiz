import { alertDanger, getCommonFetchObj } from "../Utils.jsx";
import { SessionContext, SESS_TOKEN } from "./QuizManagerRenderer.jsx";

const QuizManagerLeftContentComponent = () => {
    const { connection, allParticipants, setAllParticipants } = React.useContext(SessionContext);
    const [ lobbyData, setLobbyData ] = React.useState({ name: '', host: '' });

    React.useEffect(() => {
        connection.on('GetAllParticipants', data => {
            setAllParticipants(JSON.parse(data));
        });
        fetch(`/api/v1/dotnet/QuizSessionAPI/GetLobbyData/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(({ name, host }) => setLobbyData({ name, host }))
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    }, []);

    const handleClick = username => {
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/RemoveFromSession/${SESS_TOKEN}/${username}`,
            getCommonFetchObj("POST")
        ).then(r => r);
    };

    const allConnected = allParticipants.Connected.map(name => <li className="h5 list-group-item bg-transparent border-0 mb-0" key={name}>
        <div className="container p-0">
            <div className="row d-inline">
                <div className="d-inline">{name}</div>
                <i onClick={() => handleClick(name)} className="bi bi-x-circle-fill text-danger d-inline"></i>
            </div>
        </div>
        
    </li>);
    const allDisconnected = allParticipants.Disconnected.map(name => <li className="h5 list-group-item bg-transparent border-0 mb-0" key={name}>{name}</li>);

    return (
        <div className="col-lg-3 px-0 mb-2 mb-lg-0 order-lg-0 order-1">
            <div className="card px-3 py-3 h-100">
                <h3 className="mb-2">Poczekalnia</h3>
                <h6 className="text-black-50 mb-0">Nazwa quizu</h6>
                <h5 className="mb-2 lh-sm">{lobbyData.name}</h5>
                <h6 className="text-black-50 mb-0">Host</h6>
                <h5 className="mb-4 lh-sm">{lobbyData.host}</h5>
                <h6 className="text-black-50 mb-1">Połączeni: ({allParticipants.Connected.length})</h6>
                {allParticipants.Connected.length > 0 && 
                    <ul className="fw-bold list-group">{allConnected}</ul>}
                <h6 className="text-black-50 mt-3 mb-1">Rozłączeni: ({allParticipants.Disconnected.length})</h6>
                {allParticipants.Disconnected.length  > 0 && <ul className="fw-bold list-group">{allDisconnected}</ul>}
                <h6></h6>
            </div>
        </div>
    );
};

export default QuizManagerLeftContentComponent;