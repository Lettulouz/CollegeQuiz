import { useState, useEffect } from "react";
import { WAITING_SCREEN } from "../utils/common";
import useLoadableContent from "../hooks/useLoadableContent";
import { SessionContext } from "../quiz-session-renderer";

import JoinToQuizSessionComponent from "./JoinToQuizSessionComponent";
import LeaveSessionButtonComponent from "./LeaveQuizSessionButtonComponent";
import QuizSessionMainGameWindowComponent from "./QuizSessionMainGameWindowComponent";

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
    const [ questionImage, setQuestionImage ] = useState('');
    const [ questionType, setQuestionType ] = useState(null);
    const [ questionTimer, setQuestionTimer ] = useState(null);
    const [ questionNumber, setQuestionNumber ] = useState(null);
    const [ isAnswerSet, setIsAnswerSet ] = useState(false);
    const [ afterQuestionResults, setAfterQuestionResults ] = useState([]);
    const [ currentAnswer, setCurrentAnswer ] = useState([]);
    const [ isLast, setIsLast ] = useState(false);
    const [ answRange, setAnswRange ] = useState({ min: "", max: "" });
    const [ progressWidth, setProgressWidth ] = useState(100);

    const [ isActive, setActiveCallback ] = useLoadableContent();
    useEffect(() => setActiveCallback(), []);

    return (
        <SessionContext.Provider value={{
            connection, setConnection, isConnect,  setIsConnect, connectionId, setConnectionId, token, setToken, alert,
            setAlert, screenAction, setScreenAction, quizName, setQuizName, isJoinClicked, setIsJoinClicked, isLeaveClicked,
            setIsLeaveClicked, quizStarted, setQuizStarted, answers, setAnswers, question, setQuestion, questionTimer,
            setQuestionTimer, questionNumber, setQuestionNumber, isAnswerSet, setIsAnswerSet, afterQuestionResults,
            setAfterQuestionResults, currentAnswer, setCurrentAnswer,
            isLast, setIsLast, answerSett, setAnswerSett, questionType, setQuestionType, answRange, setAnswRange,
            questionImage, setQuestionImage, progressWidth, setProgressWidth
        }}>
            {isActive && <>
                {isConnect ? <>
                    {(screenAction === "COUNTING_SCREEN" || screenAction === "WAITING_SCREEN") && <div className="row px-2">
                        <LeaveSessionButtonComponent text="Opuść pokój"/>
                    </div>}
                    <QuizSessionMainGameWindowComponent/>
                </> : <JoinToQuizSessionComponent/>}
            </>}
        </SessionContext.Provider>
    );
};

export default QuizSessionRootComponent;