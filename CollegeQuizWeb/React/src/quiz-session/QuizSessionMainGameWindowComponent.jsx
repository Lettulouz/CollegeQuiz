import { useContext, useState, useEffect } from "react";
import {
    alertWarning, COUNTING_SCREEN, IN_GAME, playSound, QUESTION_RESULT_SCREEN, WAITING_SCREEN
} from "../utils/common";
import { SessionContext } from "../quiz-session-renderer";

import QuizQuestionAnswerComponent from "./QuizQuestionAnswerComponent";
import QuizQuestionRangeTypeComponent from "./QuizQuestionRangeTypeComponent";
import QuizSessionQuestionResultComponent from "./QuizSessionQuestionResultComponent";
import QuizQuestionUniversalTypeComponent from "./QuizQuestionUniversalTypeComponent";
import QuizQuestionTrueFalseAnswerTypeComponent from "./QuizQuestionTrueFalseAnswerTypeComponent";

const QuizSessionMainGameWindowComponent = () => {
    const {
        connection, setScreenAction, screenAction, setIsConnect, setAlert, quizName, setIsJoinClicked,
        setIsLeaveClicked, setAnswers, setQuestion, setQuestionTimer, setQuestionNumber, setIsAnswerSet,
        setAfterQuestionResults, setCurrentAnswer, setIsLast, setAnswerSett, questionType, setQuestionType,
        setAnswRange, setQuestionImage, setProgressWidth
    } = useContext(SessionContext);

    const [ counting, setCounting ] = useState(5);
    
    useEffect(() => {
        
        // start gry, uruchomienie timera odliczającego
        connection.on("INIT_GAME_SEQUENCER_P2P", counter => {
            setScreenAction(COUNTING_SCREEN);
            setCounting(counter);
            playSound(counter);
        });
        
        // rozpoczęcie gry
        connection.on("START_GAME_P2P", () => {});
        
        // odbieranie pytań z koncentratora serwera
        connection.on("QUESTION_P2P", answ => {
            const parsedAnswers = JSON.parse(answ);
            const { step, min, max, min_counted, max_counted } = parsedAnswers;
            setAnswerSett({ step, min, max, min_counted, max_counted });
            setScreenAction(IN_GAME)
            setQuestion(parsedAnswers.question);
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
            setProgressWidth((parsedCounter.Remaining / parsedCounter.Total) * 100);
            setQuestionTimer(parsedCounter.Remaining);
            playSound(parsedCounter.Remaining);
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

    const generateTrueFalseAnswers = () => (
        <QuizQuestionUniversalTypeComponent>
            {Array.from({ length: 2 }).map((_, i) => (
                <QuizQuestionTrueFalseAnswerTypeComponent key={i} number={i}/>
            ))}
        </QuizQuestionUniversalTypeComponent>
    );
    
    const renderQuestionTypeSection = () => {
        switch (questionType) {
            case 1: return generateUniversalAnswers(4, false);  // 4 odpowiedzi, jedna poprawna
            case 2: return generateTrueFalseAnswers();          // true/false
            case 3: return generateUniversalAnswers(4, true);   // 4 odpowiedzi, wiele poprawnych
            case 4: return generateUniversalAnswers(6, false);  // 6 odpowiedzi, jedna poprawna
            case 5: return <QuizQuestionRangeTypeComponent/>;
        }
    };
    
    const renderComponentSection = () => {
        switch(screenAction) {
            case WAITING_SCREEN: return (
                <div className="card card-glass-effect card-min-height mt-3 d-flex justify-content-center align-items-center">
                    <div className="d-flex flex-column align-items-center">
                        <div className="mb-4 spinner-border load-spinner-circle mt-4" role="status"></div>
                        <p className="mt-3 fs-4 text-prim-color text-center">
                            Oczekiwanie na uruchomienie quizu <strong className="custom-green-color">{quizName}</strong> przez hosta...
                        </p>
                    </div>
                </div>
            );
            case COUNTING_SCREEN: return (
                <div className="card card-glass-effect card-min-height mt-3 d-flex justify-content-center align-items-center">
                    <div className="d-flex flex-column align-items-center">
                        <p className="mt-3 fs-4 text-prim-color text-center">
                            Przygotuj się! Quiz <strong className="custom-green-color">{quizName}</strong> uruchamia się za:
                        </p>
                        <h2 className="fw-bold fs-1 custom-green-color">{counting}</h2>
                    </div>
                </div>
            );
            case QUESTION_RESULT_SCREEN: return <QuizSessionQuestionResultComponent/>;
            default: return renderQuestionTypeSection()
        }
    };
    
    return (
        <div className="row px-2">
            {renderComponentSection()}
        </div>
    );
};

export default QuizSessionMainGameWindowComponent;