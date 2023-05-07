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
    const [ isJoinable, setIsJoinable ] = useState(false);
    const [ connectionId, setConnectionId ] = useState('');
    const [ connection, setConnection ] = useState(null);
    const [ alert, setAlert ] = useState(alertOff());
    const [ isEstabiblishedClicked, setIsEstabilishedClicked ] = useState(false);
    const [ allParticipants, setAllParticipants ] = useState({ Connected: [], Disconnected: [], Banned: [] });
    const [ nextQuestionIsActive, setNextQuestionIsActive ] = useState(false);
    const [ tick, setTick ] = useState(0);
    const [ nextQuestionBtnText, setNextQuestionBtnText ] = useState("Następne pytanie");
    const [ startBtnText, setStartBtnText ] = useState("Rozpocznij");
    const [ isEnded, setIsEnded ] = useState(false);
    const [ progressWidth, setProgressWidth ] = useState(100);
    const [ lobbyData, setLobbyData ] = useState({ name: '', host: '', questionsCount: 0 });
    const [ inGameViewActive, setInGameViewActive ] = useState(false);
    const [ counting, setCounting ] = useState(5);
    const [ countingActive, setCountingActive ] = useState(false);
    const [ afterQuestionResults, setAfterQuestionResults ] = useState([]);
    const [ resultTable, setResultTable ] = useState([]);
    const [ isAnswersVisible, setIsAnswersVisible ] = useState(false);
    const [ respondedUsers, setRespondedUsers ] = useState(0);
    const [ questionType, setQuestionType ] = useState(1);
    const [ answersIsSetted, setAnswersIsSetted ] = useState(false);
    
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
            resultTable, setResultTable, isAnswersVisible, setIsAnswersVisible, respondedUsers, setRespondedUsers,
            questionType, setQuestionType, answersIsSetted, setAnswersIsSetted
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