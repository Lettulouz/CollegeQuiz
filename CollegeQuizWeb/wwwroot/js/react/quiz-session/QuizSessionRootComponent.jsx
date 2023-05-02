import { WAITING_SCREEN } from "../Utils.jsx";
import { useLoadableContent } from "../Hooks.jsx";
import { SessionContext } from "./QuizSessionRenderer.jsx";

import JoinToQuizSessionComponent from "./JoinToQuizSessionComponent.jsx";
import LeaveSessionButtonComponent from "./LeaveQuizSessionButtonComponent.jsx";
import QuizSessionMainGameWindowComponent from "./QuizSessionMainGameWindowComponent.jsx";

const QuizSessionRootComponent = () => {
    const [ connection, setConnection ] = React.useState(null)
    const [ connectionId, setConnectionId ] = React.useState('');
    const [ isConnect, setIsConnect ] = React.useState(false);
    const [ token, setToken ] = React.useState('');
    const [ alert, setAlert ] = React.useState({ active: false, style: 'alert-success', message: '' });
    const [ screenAction, setScreenAction ] = React.useState(WAITING_SCREEN);
    const [ quizName, setQuizName ] = React.useState('');
    const [ isJoinClicked, setIsJoinClicked ] = React.useState(false);
    const [ isLeaveClicked, setIsLeaveClicked ] = React.useState(false);
    const [ quizStarted, setQuizStarted ] = React.useState(false);
    const [ answers, setAnswers ] = React.useState([]);
    const [ answerSett, setAnswerSett ] = React.useState ({step: "", min: "", max: "", min_counted: "", max_counted: ""});
    const [ question, setQuestion ] = React.useState('');
    const [ questionImage, setQuestionImage ] = React.useState('');
    const [ questionType, setQuestionType ] = React.useState(null);
    const [ questionTimer, setQuestionTimer ] = React.useState(null);
    const [ questionNumber, setQuestionNumber ] = React.useState(null);
    const [ isAnswerSet, setIsAnswerSet ] = React.useState(false);
    const [ afterQuestionResults, setAfterQuestionResults ] = React.useState([]);
    const [ currentAnswer, setCurrentAnswer ] = React.useState([]);
    const [ isLast, setIsLast ] = React.useState(false);
    const [ answRange, setAnswRange ] = React.useState({ min: "", max: "" });
    const [ progressWidth, setProgressWidth ] = React.useState(100);

    const [ isActive, setActiveCallback ] = useLoadableContent();
    React.useEffect(() => setActiveCallback(), []);

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
                    {(screenAction === "COUNTING_SCREEN" || screenAction === "WAITING_SCREEN") && <div className="row">
                        <LeaveSessionButtonComponent text="Opuść pokój"/>
                    </div>}
                    <QuizSessionMainGameWindowComponent/>
                </> : <JoinToQuizSessionComponent/>}
            </>}
        </SessionContext.Provider>
    );
};

export default QuizSessionRootComponent;