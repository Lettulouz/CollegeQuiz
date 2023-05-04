import { useState, useEffect } from "react";
import useLoadableContent from "../hooks/useLoadableContent";
import { alertDanger, alertInfo, alertOff, alertWarning, getCommonFetchObj } from "../utils/common";
import { SessionContext, SESS_TOKEN, QUIZ_NAME } from "../quiz-manager-renderer";
import { HubConnectionBuilder } from "@microsoft/signalr";

import QuizManagerInGameViewComponent from "./QuizManagerInGameViewComponent";
import QuizManagerLeftContentComponent from "./QuizManagerLeftContentComponent";
import QuizManagerRightContentComponent from "./QuizManagerRightContentComponent";
import QuizManagerCenterContentComponent from "./QuizManagerCenterContentComponent";

const QuizManagerRootComponent = () => {
    const [ isActive, setActiveCallback ] = useLoadableContent();
    const [ isJoinable, setIsJoinable ] = React.useState(false);
    const [ connectionId, setConnectionId ] = React.useState('');
    const [ connection, setConnection ] = React.useState(null);
    const [ alert, setAlert ] = React.useState(alertOff());
    const [ isEstabiblishedClicked, setIsEstabilishedClicked ] = React.useState(false);
    const [ allParticipants, setAllParticipants ] = React.useState({ Connected: [], Disconnected: [] });
    const [ nextQuestionIsActive, setNextQuestionIsActive ] = React.useState(false);
    const [ tick, setTick ] = React.useState(0);
    const [ nextQuestionBtnText, setNextQuestionBtnText ] = React.useState("Następne pytanie");
    const [ startBtnText, setStartBtnText ] = React.useState("Rozpocznij");
    const [ isEnded, setIsEnded ] = React.useState(false);
    const [ progressWidth, setProgressWidth ] = React.useState(100);
    const [ lobbyData, setLobbyData ] = React.useState({ name: '', host: '', questionsCount: 0 });
    const [ inGameViewActive, setInGameViewActive ] = React.useState(false);
    const [ counting, setCounting ] = React.useState(5);
    const [ countingActive, setCountingActive ] = React.useState(false);
    const [ afterQuestionResults, setAfterQuestionResults ] = React.useState([]);
    const [ resultTable, setResultTable ] = React.useState([]);
    
    const estabilishedRoomConnection = () => {
        if (isEstabiblishedClicked) return;
        setIsEstabilishedClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/EstabilishedHostRoom/${connectionId}/${SESS_TOKEN}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsJoinable(true);
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };

    useEffect(() => {
        const connectionObj = new HubConnectionBuilder()
            .withUrl('/quizManagerSessionHub')
            .withAutomaticReconnect()
            .build();
        connectionObj.start()
            .then(() => connectionObj.invoke('getConnectionId').then(connId => {
                setConnectionId(connId);
                setConnection(connectionObj);
                setActiveCallback();
            }))
            .catch(() => setAlert(alertDanger('Nieudane dołączenie do sesji.')));
        
        // plansza po zakończeniu tury/rozgrywki
        connectionObj.on("QUESTION_RESULT_P2P", questionAnsw => {
            const parsedAnswers = JSON.parse(questionAnsw);
            
            setAfterQuestionResults(parsedAnswers);
            setNextQuestionBtnText("Następne pytanie");
            setNextQuestionIsActive(true);
            
            setTimeout(() => setProgressWidth(100), 2000);
            
            // jeśli to jest ostatnia odpowiedź
            if (!parsedAnswers[0].isLast) return;
            
            setNextQuestionBtnText("Koniec pytań");
            setStartBtnText("Zakończony");
            setIsEnded(true);
            setProgressWidth(100);
            setAlert(alertInfo("Quiz został zakończony."));
            setTimeout(() => setAlert(alertOff()), 3000);
        });
    }, []);

    useEffect(() => {
        if (isJoinable) {
            setAlert(alertWarning("Rozgrywka jest możliwa tylko wtedy, gdy uczestniczy w niej przynajmniej jeden gracz."));
        } else {
            setAlert(alertOff());
        }
    }, [ isJoinable ]);
    
    return (
        <SessionContext.Provider value={{
            connection, setAlert, allParticipants, setAllParticipants, nextQuestionIsActive, tick, setTick,
            nextQuestionBtnText, setNextQuestionBtnText, startBtnText, isEnded, setStartBtnText,
            progressWidth, setProgressWidth, lobbyData, setLobbyData, setInGameViewActive, inGameViewActive,
            counting, setCounting, countingActive, setCountingActive, afterQuestionResults, setNextQuestionIsActive,
            resultTable, setResultTable
        }}>
            {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-3 mx-1`} role="alert">
                <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
            </div>}
            {isActive && <div className="container mx-0">
                {isJoinable ? <div>
                    {inGameViewActive ? <QuizManagerInGameViewComponent/> :
                        <div className="row">
                            <QuizManagerLeftContentComponent/>
                            <QuizManagerCenterContentComponent/>
                            <QuizManagerRightContentComponent/>
                        </div>}
                </div> : <div className="text-center" style={{ marginTop: '25vh' }}>
                    <p>Quiz:<br/><strong>{QUIZ_NAME}</strong></p>
                    <button className="btn click-me mx-auto mt-5" onClick={estabilishedRoomConnection}>Stwórz nowy pokój</button>
                    <div className="hstack gap-3 align-items-center w-100 mx-auto my-5" style={{ maxWidth: 800 }}>
                        <hr className="flex-fill"/>
                        <span>LUB</span>
                        <hr className="flex-fill"/>
                    </div>
                    <div className="hstack gap-3 justify-content-center">
                        <a className="btn click-me darken" href="/Quiz/MyQuizes">
                            Wróć do listy quizów
                        </a>
                    </div>
                </div>}
            </div>}
        </SessionContext.Provider>
    );
};

export default QuizManagerRootComponent;