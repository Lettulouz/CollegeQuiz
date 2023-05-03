import { useLoadableContent } from "../Hooks.jsx";
import { alertDanger, alertInfo, alertOff, getCommonFetchObj } from "../Utils.jsx";
import { SessionContext, SESS_TOKEN, QUIZ_NAME } from "./QuizManagerRenderer.jsx";

import QuizManagerLeftContentComponent from "./QuizManagerLeftContentComponent.jsx";
import QuizManagerCenterContentComponent from "./QuizManagerCenterContentComponent.jsx";
import QuizManagerRightContentComponent from "./QuizManagerRightContentComponent.jsx";
import QuizManagerQuestionTickComponent from "./QuizManagerQuestionTickComponent.jsx";

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

    React.useEffect(() => {
        const connectionObj = new signalR.HubConnectionBuilder()
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

        // chowanie i pokazywanie przycisku następnego pytania
        connectionObj.on("ON_NEXT_QUESTION_P2P", isActive => {
            setNextQuestionBtnText("Następne pytanie");
            setTimeout(() => setProgressWidth(100), 2000);
            setNextQuestionIsActive(isActive);
        });
        
        // przy ostatnim pytaniu, chowanie przycisków
        connectionObj.on("ON_END_QUESTIONS_P2P", _ => {
            setNextQuestionBtnText("Koniec pytań");
            setStartBtnText("Zakończony");
            setIsEnded(true);
            setProgressWidth(100);
            setAlert(alertInfo("Quiz został zakończony."));
            setTimeout(() => setAlert(alertOff()), 3000);
        });
    }, []);

    return (
        <SessionContext.Provider value={{
            connection, setAlert, allParticipants, setAllParticipants, nextQuestionIsActive, tick, setTick,
            nextQuestionBtnText, setNextQuestionBtnText, startBtnText, isEnded, setStartBtnText,
            progressWidth, setProgressWidth,
        }}>
            {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-4`} role="alert">
                <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
            </div>}
            {isActive && <div className="container px-0 mx-0">
                {isJoinable ? <div>
                    <QuizManagerQuestionTickComponent/>
                    <div className="row w-100 position-relative mx-0">
                        <QuizManagerLeftContentComponent/>
                        <QuizManagerCenterContentComponent/>
                        <QuizManagerRightContentComponent/>
                    </div>
                </div> : <div className="text-center" style={{ marginTop: '25vh' }}>
                    <p>Quiz: <strong>{QUIZ_NAME}</strong></p>
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