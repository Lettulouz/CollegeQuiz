import { useContext, useEffect } from "react";
import { alertDanger, alertOff, alertWarning, getCommonFetchObj } from "../utils/common";
import { SessionContext, SESS_TOKEN } from "../quiz-manager-renderer";

import RemoveUserFromSessionButtonComponent from "./RemoveUserFromSessionButtonComponent.jsx";

const QuizManagerLeftContentComponent = () => {
    const {
        connection, allParticipants, setAllParticipants, lobbyData, setLobbyData, setAlert, resultTable, setResultTable,
        setRespondedUsers
    } = useContext(SessionContext);

    useEffect(() => {
        
        // dołączenie użytkownika do sesji
        connection.on('GetAllParticipants', data => {
            const allParticipants = JSON.parse(data);
            
            allParticipants.Connected.sort();
            allParticipants.Disconnected.sort();
            setAllParticipants(allParticipants);
            if (allParticipants.Connected.length === 0) {
                setAlert(alertWarning("Rozgrywka jest możliwa tylko wtedy, gdy uczestniczy w niej przynajmniej jeden gracz."));
            } else {
                setAlert(alertOff());
            }
        });
        
        // dołączenie użytkownika do sesji, dodanie go jeśli miał jakieś punkty
        connection.on('USER_JOINABLE_POINTS_P2P', joinableUser => {
            const parsedJoinableUser = JSON.parse(joinableUser);
            if (resultTable.some(r => r.Username === parsedJoinableUser.Username)) return;
            
            const { Username, Points, IsGood } = parsedJoinableUser;
            setResultTable(prevState => ([ ...prevState, { Username, Points, IsGood, Answer: '-' }]));
        });
        
        // usunięcie użytkownika z sesji
        connection.on('USER_ON_LEAVE_ROOM_P2P', leavingUsername => {
            const copy = [ ...resultTable ];
            const newState = copy.filter(r => r.username !== leavingUsername);
            setResultTable(newState);
            setRespondedUsers(prevState => prevState === 0 ? 0 : prevState - 1);
        });
        
        fetch(`/api/v1/dotnet/QuizSessionAPI/GetLobbyData/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(({ name, host, questionsCount }) => setLobbyData({ name, host, questionsCount }))
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    }, []);
    
    const allConnected = allParticipants.Connected.map(name => (
        <li className="h6 list-group-item bg-transparent border-0 px-1 py-1 mb-0" key={name}>
            <div className="container p-0">
                <div className="d-flex justify-content-between text-break">
                    <div className="quiz-color-text d-flex align-items-center lh-1">{name}</div>
                    <RemoveUserFromSessionButtonComponent name={name}/>
                </div>
            </div>
        </li>
    ));
    
    const allDisconnected = allParticipants.Disconnected.map(name => (
        <li className="h6 list-group-item bg-transparent border-0 px-1 py-1 mb-0 quiz-color-text" key={name}>{name}</li>
    ));

    return (
        <div className="col-lg-3 px-0 px-md-1 mb-2 mb-lg-0 order-lg-0 order-2">
            <div className="card trsp px-3 py-3 h-100 scrollable-container">
                <h3 className="mb-2 quiz-color-text">Poczekalnia</h3>
                <h6 className="text-black-50 mb-0">Nazwa quizu</h6>
                <h5 className="mb-2 lh-sm quiz-color-text">{lobbyData.name}</h5>
                <h6 className="text-black-50 mb-0">Host</h6>
                <h5 className="mb-4 lh-sm quiz-color-text">{lobbyData.host}</h5>
                <h6 className="text-black-50 mb-1">Połączeni: ({allParticipants.Connected.length})</h6>
                {allParticipants.Connected.length > 0 && 
                    <ul className="fw-bold list-group" style={{ minHeight: 38 }}>{allConnected}</ul>}
                <h6 className="text-black-50 mt-3 mb-1">Rozłączeni: ({allParticipants.Disconnected.length})</h6>
                {allParticipants.Disconnected.length  > 0 && <ul className="fw-bold list-group">{allDisconnected}</ul>}
            </div>
        </div>
    );
};

export default QuizManagerLeftContentComponent;