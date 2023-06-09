import { useContext, useState } from "react";
import { alertDanger, ANSWER_SVGS, generateErrorMessage, getCommonFetchObj } from "../utils/common";
import { SessionContext } from "../quiz-session-renderer";

const QuizQuestionTrueFalseAnswerTypeComponent = ({ number }) => {
    const {
        answers, connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer
    } = useContext(SessionContext);
    
    const [ clickedIndex, setClickedIndex ] = useState([]);
    
    const handleClick = () => {
        if (isAnswerSet || currentAnswer !== "") return;
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${number}/false`,
            getCommonFetchObj("POST")
        ).then(r => {
            if (r.ok) {
                return r;
            }
            throw new Error(r.status);
        }).catch(e => {
            if (e === undefined) return;
            setAlert(alertDanger(generateErrorMessage(e.message)));
        });
        setIsAnswerSet(true);
        setClickedIndex(number);
    };
    
    const incClassAns = () => !Object.values(currentAnswer)
        .find(x=> x.AnswerName === answers[number]) && Object.keys(currentAnswer).length !== 0 ? 'incorrectAnswer' : '';
    
    const cssDisabledClickEvent = isAnswerSet && 'cursor-not-allowed';
    const cssClickedClass = clickedIndex === number && 'clicked';
    
    return (
        <div className={`col-6 d-flex m-0 mt-3 ${incClassAns()}`}>
            <div className={`card text-white card-img-custom ${cssClickedClass}`}
                onClick={handleClick}>
                <button className={`bg-transparent border-0 p-0 m-0 cursor-default ${cssDisabledClickEvent}`}>
                    <img src={ANSWER_SVGS[number]} className="card-img" alt="image_answer_D" />
                    <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                        <h5 className="card-title text-center text-white">{number === 0 ? 'Prawda' : 'Fałsz'}</h5>
                    </div>
                </button>
            </div>
        </div>
    );
};

export default QuizQuestionTrueFalseAnswerTypeComponent;