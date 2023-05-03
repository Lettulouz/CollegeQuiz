import { alertWarning, COUNTING_SCREEN, IN_GAME, QUESTION_RESULT_SCREEN, WAITING_SCREEN } from "../Utils.jsx";
import { SessionContext } from "./QuizSessionRenderer.jsx";

import QuizQuestionAnswerComponent from "./QuizQuestionAnswerComponent.jsx";
import QuizQuestionRangeTypeComponent from "./QuizQuestionRangeTypeComponent.jsx";
import QuizSessionQuestionResultComponent from "./QuizSessionQuestionResultComponent.jsx";
import QuizQuestionUniversalTypeComponent from "./QuizQuestionUniversalTypeComponent.jsx";

const QuizSessionMainGameWindowComponent = () => {
    const {
        connection, setScreenAction, screenAction, setIsConnect, setAlert, quizName, setIsJoinClicked,
        setIsLeaveClicked, setAnswers, setQuestion, setQuestionTimer, setQuestionNumber, setIsAnswerSet,
        setAfterQuestionResults, setCurrentAnswer, setIsLast, setAnswerSett, questionType, setQuestionType,
        setAnswRange, setQuestionImage, setProgressWidth
    } = React.useContext(SessionContext);

    const [ counting, setCounting ] = React.useState(5);
    
    React.useEffect(() => {
        
        // start gry, uruchomienie timera odliczającego
        connection.on("INIT_GAME_SEQUENCER_P2P", counter => {
            setScreenAction(COUNTING_SCREEN);
            setCounting(counter);
        });
        
        // rozpoczęcie gry
        connection.on("START_GAME_P2P", () => setScreenAction(IN_GAME));
        
        // odbieranie pytań z koncentratora serwera
        connection.on("QUESTION_P2P", answ => {
            const parsedAnswers = JSON.parse(answ);
            const { step, min, max, min_counted, max_counted } = parsedAnswers;
            
            setQuestion(parsedAnswers.question);
            setAnswerSett({ step, min, max, min_counted, max_counted });
            setAnswRange({ min, max });
            setAnswers(parsedAnswers.answers);
            
            setQuestionType(parsedAnswers.questionType);
            setQuestionTimer(parsedAnswers.time_sec);
            setQuestionNumber(parsedAnswers.questionId);
            setQuestionImage(parsedAnswers.image_url);
            
            setIsAnswerSet(false);
            setCurrentAnswer("");
        });

        // timer odliczający czas pytania
        connection.on("QUESTION_TIMER_P2P", counter => {
            const parsedCounter = JSON.parse(counter);
            setProgressWidth((parsedCounter.Elapsed / parsedCounter.Total) * 100);
            setQuestionTimer(parsedCounter.Elapsed);
        });

        // ustawienie prawidłowego pytania
        connection.on("CORRECT_ANSWERS_SCREEN", currentAnsw => setCurrentAnswer(JSON.parse(currentAnsw)));
        
        // plansza po zakończeniu tury
        connection.on("QUESTION_RESULT_P2P", questionAnsw => {
            setScreenAction(QUESTION_RESULT_SCREEN);
            const parsedAnswers = JSON.parse(questionAnsw);
            setAfterQuestionResults(parsedAnswers);
            setIsLast(parsedAnswers.isLast);
        });
        
        // rozłączenie hosta
        connection.on("OnDisconnectedSession", data => {
            setIsJoinClicked(false);
            setIsLeaveClicked(false);
            connection.stop().then(_ => {
                setIsConnect(false);
                setAlert(alertWarning(data));
            });
        });
    }, []);
    
    const generateUniversalAnswers = (count, multiSelect) => (
        <QuizQuestionUniversalTypeComponent>
            {Array.from({ length: count }).map((_, i) => (
                <QuizQuestionAnswerComponent key={i} number={i} isMultiSelect={multiSelect}/>
            ))}
        </QuizQuestionUniversalTypeComponent>
    );

    const renderQuestionTypeSection = () => {
        switch (questionType) {
            case 1: return generateUniversalAnswers(4, false);  // 4 odpowiedzi, jedna poprawna
            case 2: return generateUniversalAnswers(2, false);  // true/false
            case 3: return generateUniversalAnswers(4, true);   // 4 odpowiedzi, wiele poprawnych
            case 4: return generateUniversalAnswers(6, false);  // 6 odpowiedzi, jedna poprawna
            case 5: return <QuizQuestionRangeTypeComponent/>;
        }
    };
    
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
            case QUESTION_RESULT_SCREEN: return <QuizSessionQuestionResultComponent/>;
            default: return renderQuestionTypeSection()
        }
    };
    
    return (
        <div className="row">
            {renderComponentSection()}
        </div>
    );
};

export default QuizSessionMainGameWindowComponent;