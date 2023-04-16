import { useLoadableContent } from "./Hooks.jsx";
import {
    alertInfo, alertDanger, alertOff, WAITING_SCREEN, getCommonFetchObj, COUNTING_SCREEN, IN_GAME
} from "./Utils.jsx";

const { useEffect, useState, createContext, useContext, useRef } = React;

const SessionContext = createContext(null);

const LeaveSessionButtonComponent = () => {
    const {
        token, connectionId, setIsConnect, setAlert, setScreenAction, isLeaveClicked, setIsLeaveClicked, setIsJoinClicked
    } = useContext(SessionContext);
    const modalRef = useRef()
    
    const leaveRoom = () => {
        if (isLeaveClicked) return;
        setIsLeaveClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/LeaveRoom/${connectionId}/${token}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsConnect(false);
                    setScreenAction(WAITING_SCREEN);
                    setIsJoinClicked(false);
                    setIsLeaveClicked(false);
                    setAlert(alertInfo(r.message));
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
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
    const {
        connection, setScreenAction, screenAction, setIsConnect, setAlert, quizName, setIsJoinClicked, 
        setIsLeaveClicked, answers, setAnswers, question, setQuestion, questionTimer, setQuestionTimer,
    } = useContext(SessionContext);
    const [ counting, setCounting ] = useState(5);
    const [ questionDataJSON, setQuestionDataJSON ] = useState([]);
    
    
    useEffect(() => {
        connection.on("INIT_GAME_SEQUENCER_P2P", counter => {
            setScreenAction(COUNTING_SCREEN);
            setCounting(counter);
        });
        connection.on("START_GAME_P2P", () => setScreenAction(IN_GAME));
        connection.on("QUESTION_P2P", answ=>{
            const test = JSON.parse(answ);
            console.log(test); 
            setQuestion(test.question);
            setAnswers(test.answers.map(q=> q));
            console.log(test.answers.map(q=> q));
            setQuestionTimer(test.time_sec);
        });
        connection.on("QUESTION_TIMER_P2P", counter => {
            setQuestionTimer(counter);
        });
        connection.on("OnDisconectedSession", data => {
            setIsJoinClicked(false);
            setIsLeaveClicked(false);
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
                <QuestionType1/>
            );
        }
    };
    
    return (
        <div className="row">
            {renderComponentSection()}
        </div>
    );
};


const QuestionType1 = () => {
    const {
        question, questionTimer
    } = useContext(SessionContext);
    return (
        <div className="container">
            <div className="row d-flex justify-content-center">
                <div className="col-10">
                    <div className="card px-3 py-3">
                        <h3>{question} {questionTimer}</h3>
                        <img src={"/gfx/qrCodeLogo.png"} width="300px" height="300px"/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCard number={0}/>
                        <QuestionCard number={1}/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCard number={2}/>
                        <QuestionCard number={3}/>
                    </div>
                </div>
            </div>
        </div>
    );
}


const QuestionCard = (props) => {
    const {
        answers, answerLetter, answerSVG
    } = useContext(SessionContext);
    return (
        <div className="col-6 d-flex m-0">
            <div className="card bg-dark text-white card-img-custom">
                <img src={answerSVG[props.number]} className="card-img" alt="image_answer_D"/>
                <div
                    className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                    <h5 className="card-title text-center">Odpowiedź {answerLetter[props.number]}</h5>
                    <p className="card-text text-center">{answers[props.number]}</p>
                </div>
            </div>
        </div>
    );
}

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
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert, setQuizName,
        setScreenAction, isJoinClicked, setIsJoinClicked
    } = useContext(SessionContext);
    
    const [ joinDisabled, setJoinDisabled ] = useState(true);
    
    const onSubmitJoinToSession = e => {
        e.preventDefault();
        if (isJoinClicked) return;
        setIsJoinClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setQuizName(r.quizName);
                    setScreenAction(r.screenType);
                    setIsConnect(true);
                } else {
                    setIsJoinClicked(false);
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                setIsJoinClicked(false);
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };
    
    useEffect(() => {
        setJoinDisabled(token.length !== 5);
    }, [ token ]);
    
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/quizUserSessionHub')
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

const QuizSessionRootComponent = () => {
    const [ connection, setConnection ] = useState(null)
    const [ connectionId, setConnectionId ] = useState('');
    const [ isConnect, setIsConnect ] = useState(false);
    const [ token, setToken ] = useState('');
    const [ alert, setAlert ] = useState({ active: false, style: 'alert-success', message: '' });
    const [ screenAction, setScreenAction ] = useState(WAITING_SCREEN);
    const [ quizName, setQuizName ] = useState('');
    const [ isJoinClicked, setIsJoinClicked ] = useState(false);
    const [ isLeaveClicked, setIsLeaveClicked ] = useState(false);
    const [ quizStarted, setQuizStarted ] = useState(false);
    const [ answers, setAnswers ] = useState([]);
    const [ answerLetter, setAnswerLetter ] = useState(["A", "B", "C", "D"]);
    const [ answerSVG, setAnswerSVG ] 
        = useState(["/gfx/blueCard.svg", "/gfx/greenCard.svg", "/gfx/darkblueCard.svg", "/gfx/tealCard.svg"]);
    const [ question, setQuestion ] = useState();
    const [ questionTimer, setQuestionTimer ] = useState();

    const [ isActive, setActiveCallback ] = useLoadableContent();
    useEffect(() => setActiveCallback(), []);
    
    return (
        <SessionContext.Provider value={{
            connection, setConnection, setIsConnect, connectionId, setConnectionId, token, setToken, alert, setAlert,
            screenAction, setScreenAction, quizName, setQuizName, isJoinClicked, setIsJoinClicked, isLeaveClicked,
            setIsLeaveClicked, quizStarted, setQuizStarted, answers, setAnswers, answerLetter, answerSVG, question, 
            setQuestion, questionTimer, setQuestionTimer
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
