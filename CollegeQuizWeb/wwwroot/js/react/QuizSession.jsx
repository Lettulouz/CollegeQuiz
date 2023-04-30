import { useLoadableContent } from "./Hooks.jsx";
import {
    alertInfo, alertDanger, alertOff, WAITING_SCREEN, getCommonFetchObj, COUNTING_SCREEN, IN_GAME, QUESTION_RESULT_SCREEN,
    CORRECT_ANSWERS_SCREEN
} from "./Utils.jsx";

const { useEffect, useState, createContext, useContext, useRef } = React;

const SessionContext = createContext(null);

const ANSWER_LETTERS = [ "A", "B", "C", "D", "E", "F" ];
const ANSWER_SVGS = [ "/gfx/blueCard.svg", "/gfx/greenCard.svg", "/gfx/darkblueCard.svg", 
    "/gfx/tealCard.svg", "/gfx/oliveCard.svg", "/gfx/darkgreenCard.svg" ];

const LeaveSessionButtonComponent = props => {
    const {
        token, connectionId, setIsConnect, setAlert, setScreenAction, isLeaveClicked, setIsLeaveClicked, setIsJoinClicked,
        connection
    } = useContext(SessionContext);
    const modalRef = useRef();
    
    const leaveRoom = () => {
        if (isLeaveClicked) return;
        setIsLeaveClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/LeaveRoom/${connectionId}/${token.toUpperCase()}`, getCommonFetchObj('POST'))
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
    
    useEffect(() => {
        connection.on("OnDisconectedSession", _ => {
            hideModal();
        });
    }, []);
    
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
                            <button type="button" className="btn-color-one bg-danger text-white" data-bs-dismiss="modal" onClick={leaveRoom}>
                                Opuść sesję
                            </button>
                            <button type="button" className="btn-color-one" data-bs-dismiss="modal" onClick={hideModal}>
                                Zamknij okno
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <button className="btn-color-one bg-danger text-white w-100 rounded" onClick={showModal}>{props.text}</button>
        </>
    );
}

const MainWindowGameComponent = () => {
    const {
        connection, setScreenAction, screenAction, setIsConnect, setAlert, quizName, setIsJoinClicked, 
        setIsLeaveClicked, setAnswers, setQuestion, setQuestionTimer,
        setQuestionNumber, setIsAnswerSet, setAfterQuestionResults, setCurrentQuestionLeader, setCurrentAnswer,
        setIsLast, setAnswerSett, answerSett, questionType, setQuestionType, answRange, setAnswRange
    } = useContext(SessionContext);
    
    const [ counting, setCounting ] = useState(5);
    
    useEffect(() => {
        connection.on("INIT_GAME_SEQUENCER_P2P", counter => {
            setScreenAction(COUNTING_SCREEN);
            setCounting(counter);
        });
        connection.on("START_GAME_P2P", () => setScreenAction(IN_GAME));
        connection.on("QUESTION_P2P", answ=>{
            const parsedAnswers = JSON.parse(answ);
            setQuestion(parsedAnswers.question);
            setAnswerSett(
                {
                    step: parsedAnswers.step, 
                    min: parsedAnswers.min, 
                    max: parsedAnswers.max, 
                    min_counted: parsedAnswers.min_counted, 
                    max_counted: parsedAnswers.max_counted
                }
            );
            setAnswRange({
                min: parsedAnswers.min,
                max: parsedAnswers.max
            });
            setAnswers(parsedAnswers.answers.map(q=> q));
            console.log(parsedAnswers.answers.map(q=> q));
            setQuestionType(parsedAnswers.questionType);
            setQuestionTimer(parsedAnswers.time_sec);
            setQuestionNumber(parsedAnswers.questionId);
            setIsAnswerSet(false);
            setCurrentAnswer("");
        });
        connection.on("QUESTION_TIMER_P2P", counter => {
            setQuestionTimer(counter);
        });
        connection.on("QUESTION_RESULT_P2P", questionAnsw => {
            setScreenAction(QUESTION_RESULT_SCREEN);
            const parsedAnswers = JSON.parse(questionAnsw);
            setAfterQuestionResults(parsedAnswers);
            console.log(parsedAnswers);
            console.log(parsedAnswers.reduce((max, dict) => max.newPoints > dict.newPoints ? max : dict).Username);
            setCurrentQuestionLeader(parsedAnswers.reduce((max, dict) => max.newPoints > dict.newPoints ? max : dict).Username);
            setIsLast(parsedAnswers.isLast);
        });
        connection.on("CORRECT_ANSWERS_SCREEN", currentAnsw => {
            setCurrentAnswer(JSON.parse(currentAnsw));
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
            case QUESTION_RESULT_SCREEN: return (
                <QuestionResultComponent/>
            );
            default:

                switch(questionType){
                    case 1: return <QuestionType1Component/>;
                    case 2: return <QuestionType2Component/>;
                    case 3: return <QuestionType3Component/>;
                    case 4: return <QuestionType4Component/>;
                    case 5: return <QuestionType5Component/>;
                }
        }
    };
    
    return (
        <div className="row">
            {renderComponentSection()}
        </div>
    );
};

const QuestionResultComponent = () => {
    const { afterQuestionResults, currentQuestionLeader } = useContext(SessionContext);
    
    const containerUsernamesRef = useRef(null);
    const containerScoresRef = useRef(null);
    const leaderRef = useRef(null);
    const timeline = anime.timeline({ easing: 'easeOutExpo' });
    
    useEffect(() => {
        if (!afterQuestionResults[0].isLast) return;
        timeline
            .add({
                targets: containerUsernamesRef.current.children,
                translateX: [ -80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
                delay: anime.stagger(100),
            })
            .add({
                targets: containerScoresRef.current.children,
                translateX: [ 80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
                delay: anime.stagger(100),
            }, "-=800")
            .add({
                targets: leaderRef.current,
                translateY: [ 80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
            }, "-=800");
    }, []);
    
    return (
        <div className="container">
            <div className="row mb-2">
                <div className="col-md-6" ref={containerUsernamesRef}>
                    {afterQuestionResults.map(m => (
                        <div className="leaderboard text-white fw-bold mb-2 fs-1 mx-2 px-5 col-sm d-flex align-items-center justify-content-center"
                             key={m.Username}>
                            {m.Username}
                        </div>
                    ))}
                </div>
                <div className="col-md-6" ref={containerScoresRef}>
                    {afterQuestionResults.map(m => (
                        <div className="leaderboard gold-leaderboard fw-bold mb-2 fs-1 mx-2 px-5 col-sm d-flex align-items-center justify-content-center"
                             key={m.Username}>
                            {m.Score} + {m.newPoints}
                        </div>
                    ))}
                </div>
            </div>
            <div className="leaderboard text-white fw-bold fs-1 mx-2 px-5 col-sm d-flex align-items-center justify-content-center"
                 ref={leaderRef}>
               {currentQuestionLeader}
            </div>
        </div>
    );
}


// 4 ANSWERS, ONE CORRECT
const QuestionType1Component = () => {
    const { question, questionTimer } = useContext(SessionContext);
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <img src="/gfx/1.png" width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={0}/>
                        <QuestionCardComponent number={1}/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={2}/>
                        <QuestionCardComponent number={3}/>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
}


// TRUE/FALSE
const QuestionType2Component = () => {
    const { question, questionTimer } = useContext(SessionContext);
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <img src="/gfx/timer.svg" width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={0}/>
                        <QuestionCardComponent number={1}/>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
}

// FOUR ANSWERS, MANY CORRECT
const QuestionType3Component = () => {
    const { question, questionTimer } = useContext(SessionContext);
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <img src="/gfx/timer.svg" width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={0}/>
                        <QuestionCardComponent number={1}/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={2}/>
                        <QuestionCardComponent number={3}/>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
}

// SIX ANSWERS, ONE CORRECT
const QuestionType4Component = () => {
    const { question, questionTimer } = useContext(SessionContext);
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <img src="/gfx/timer.svg" width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={0}/>
                        <QuestionCardComponent number={1}/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={2}/>
                        <QuestionCardComponent number={3}/>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <QuestionCardComponent number={4}/>
                        <QuestionCardComponent number={5}/>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
}


// RANGE
const QuestionType5Component = () => {
    const {
        connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer, answerSett, answRange, setAnswRange
    } = useContext(SessionContext);
    const { question, questionTimer } = useContext(SessionContext);
    const stepsSlider = useRef(null);
    const handleClick = () => {
        if(isAnswerSet || currentAnswer !== "") return;
        
        const answer = "r" + parseInt(answRange.min) + "," + parseInt(answRange.max);
        console.log(answer);
        fetch(`/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}`,
            getCommonFetchObj("POST")).then(r => r)
        console.log("test");
        setIsAnswerSet(true);
    }
    
    useEffect(() => {

        noUiSlider.create(stepsSlider.current, {
            start: [answerSett.min, answerSett.max],
            behaviour: 'drag',
            connect: true,
            tooltips: [wNumb({decimals: 0}), wNumb({decimals: 0})],
            range: {
                'min': [answerSett.min, answerSett.step],
                'max' :answerSett.max
            }
        });

        stepsSlider.current.noUiSlider.on('update', function (values, handle) {
            setAnswRange({min: values[0], max: values[1]});
        });
    }, []);
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <img src="/gfx/timer.svg" width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSlider}></div>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <div className="col-12 d-flex m-0 justify-content-center text-center">
                            <div onClick={() => handleClick()}>
                                <button className={`btn btn-success border-0 w-100 m-0 text-white rounded cursor-default ${isAnswerSet && 'cursor-not-allowed'} `}>
                                    Zatwierdź
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
}

const QuestionCardComponent = ({ number }) => {
    const {
        answers, connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer
    } = useContext(SessionContext);
    const [clickedIndex, setClickedIndex] = useState(null);
    
    const handleClick = answer => {
        if(isAnswerSet || currentAnswer !== "") return;
        fetch(`/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}`,
            getCommonFetchObj("POST")).then(r => r)
        setIsAnswerSet(true);
        setClickedIndex(answer);
    }
    
    const incClassAns = () => currentAnswer !== answers[number] && currentAnswer !== "" ? 'incorrectAnswer' : '';
    const isClicked = () => isAnswerSet && clickedIndex !== number && 'notClicked';
    
    return (
        <div className={`col-6 d-flex m-0 ${incClassAns()}`}>
            <div className={`card bg-dark text-white card-img-custom ${clickedIndex === number && 'clicked'} ${isClicked()}`}
                 onClick={() => handleClick(number)}>
                <button className={`bg-transparent border-0 p-0 m-0 cursor-default ${isAnswerSet && 'cursor-not-allowed'} `}>
                    <img src={ANSWER_SVGS[number]} className="card-img" alt="image_answer_D" />
                    <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                        <h5 className="card-title text-center">Odpowiedź {ANSWER_LETTERS[number]}</h5>
                        <p className="card-text text-center">{answers[number]}</p>
                    </div>
                </button>
            </div>
        </div>
    );
}

const HeaderPanelComponent = () => {
    const { screenAction } = useContext(SessionContext);
    return (
        <>
            {(screenAction === "COUNTING_SCREEN" || screenAction === "WAITING_SCREEN") && 
             <div className="row">
                <LeaveSessionButtonComponent text={"Opuść pokój"}/>
            </div>}
        </>
    );
};

const JoinToSessionComponent = () => {
    const {
        setIsConnect, setConnection, connectionId, setConnectionId, token, setToken, alert, setAlert, setQuizName,
        setScreenAction, isJoinClicked, setIsJoinClicked, question
    } = useContext(SessionContext);
    
    const [ joinDisabled, setJoinDisabled ] = useState(true);
    
    const onSubmitJoinToSession = e => {
        e.preventDefault();
        if (isJoinClicked) return;
        setIsJoinClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/JoinRoom/${connectionId}/${token.toUpperCase()}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setQuizName(r.quizName);
                    setScreenAction(question !== "" ? r.screenType : WAITING_SCREEN);
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
            .withAutomaticReconnect()
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
                    <div className="card px-5 py-5 h-100 justify-content-center">
                        <form className="form-data mt-3" onSubmit={onSubmitJoinToSession}>
                            <label id="mainName">Dołącz do quizu</label>
                            {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between mb-4`} role="alert">
                                <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                                <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                            </div>}
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
    const [ answerSett, setAnswerSett ] = useState ({step: "", min: "", max: "", min_counted: "", max_counted: ""});
    const [ question, setQuestion ] = useState('');
    const [ questionType, setQuestionType ] = useState(null);
    const [ questionTimer, setQuestionTimer ] = useState(null);
    const [ questionNumber, setQuestionNumber ] = useState(null);
    const [ isAnswerSet, setIsAnswerSet ] = useState(false);
    const [ afterQuestionResults, setAfterQuestionResults ] = useState([]);
    const [ currentQuestionLeader, setCurrentQuestionLeader ] = useState("");
    const [ currentAnswer, setCurrentAnswer ] = useState("");
    const [ isLast, setIsLast ] = useState(false);
    const [ answRange, setAnswRange ] = useState({min: "", max: ""});
    
    const [ isActive, setActiveCallback ] = useLoadableContent();
    useEffect(() => setActiveCallback(), []);
    
    return (
        <SessionContext.Provider value={{
            connection, setConnection, isConnect,  setIsConnect, connectionId, setConnectionId, token, setToken, alert, setAlert,
            screenAction, setScreenAction, quizName, setQuizName, isJoinClicked, setIsJoinClicked, isLeaveClicked,
            setIsLeaveClicked, quizStarted, setQuizStarted, answers, setAnswers, question, 
            setQuestion, questionTimer, setQuestionTimer, questionNumber, setQuestionNumber, isAnswerSet, setIsAnswerSet,
            afterQuestionResults, setAfterQuestionResults, currentQuestionLeader, setCurrentQuestionLeader,
            currentAnswer, setCurrentAnswer, isLast, setIsLast, answerSett, setAnswerSett, questionType, setQuestionType, answRange, setAnswRange
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
