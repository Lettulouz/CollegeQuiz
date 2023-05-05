import { useContext, useEffect } from "react";
import { alertDanger, alertOff, alertWarning, getCommonFetchObj } from "../utils/common";
import { SessionContext, SESS_TOKEN } from "../quiz-manager-renderer";

import SessionParticipantsComponent from "./SessionParticipantsComponent";

const QuizManagerLeftContentComponent = () => {
    const {
        connection, setAllParticipants, lobbyData, setLobbyData, setAlert, resultTable, setResultTable
    } = useContext(SessionContext);

    useEffect(() => {
        // dołączenie użytkownika do sesji
        connection.on('SEEDING_PARTICIPANTS_P2P', data => {
            const allParticipants = JSON.parse(data);
            
            allParticipants.Connected.sort();
            allParticipants.Disconnected.sort();
            allParticipants.Banned.sort();
            
            setAllParticipants(allParticipants);
            setResultTable(prevState => prevState.filter(u => allParticipants.Connected.includes(u.Username)));
            
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
        
        fetch(`/api/v1/dotnet/QuizSessionAPI/GetLobbyData/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(({ name, host, questionsCount }) => setLobbyData({ name, host, questionsCount }))
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    }, []);
    
    return (
        <div className="col-lg-3 px-0 px-md-1 mb-2 mb-lg-0 order-lg-0 order-2">
            <div className="card trsp px-3 py-3 h-100 scrollable-container">
                <h3 className="mb-2 quiz-color-text">Poczekalnia</h3>
                <h6 className="text-black-50 mb-0">Nazwa quizu</h6>
                <h5 className="mb-2 lh-sm quiz-color-text">{lobbyData.name}</h5>
                <h6 className="text-black-50 mb-0">Host</h6>
                <h5 className="mb-4 lh-sm quiz-color-text">{lobbyData.host}</h5>
                <SessionParticipantsComponent/>
            </div>
        </div>
    );
};

export default QuizManagerLeftContentComponent;