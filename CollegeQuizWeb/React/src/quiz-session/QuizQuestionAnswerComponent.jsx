import { useContext, useState } from "react";
import { SessionContext } from "../quiz-session-renderer";
import { alertDanger, ANSWER_LETTERS, ANSWER_SVGS, getCommonFetchObj } from "../utils/common";

const QuizQuestionAnswerComponent = ({ number, isMultiSelect }) => {
    const {
        answers, connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer, questionType
    } = useContext(SessionContext);
    
    const [ clickedIndex, setClickedIndex ] = useState([]);
    
    const handleClick = () => {
        if (isMultiSelect && clickedIndex.includes(number)) return;
        if (!isMultiSelect && (isAnswerSet || currentAnswer !== "")) return;
        
        const multi = isMultiSelect ? "true" : "false";
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${number}/${multi}`,
            getCommonFetchObj("POST")
        ).then(r => r)
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił błąd podczas wysyłania odpowiedzi przez użytkownika.'));
            });
        setIsAnswerSet(true);
        if (isMultiSelect) {
            setClickedIndex([ ...clickedIndex, number ]);
            return;
        }
        setClickedIndex(number);
    };

    const incClassAns = () => !Object.values(currentAnswer)
        .find(x=> x.AnswerName === answers[number]) && Object.keys(currentAnswer).length !== 0 ? 'incorrectAnswer' : '';
    
    const isClicked = () => isAnswerSet && clickedIndex !== number && 'notClicked';
    const cssDisabledClickEvent = (!isMultiSelect && isAnswerSet) && 'cursor-not-allowed';
    
    const cssClickedClass = isMultiSelect
        ? clickedIndex.includes(number) && 'clicked'
        : clickedIndex === number && 'clicked';

    const sixAnsw = questionType === 4
        ? 'col-4'
        : 'col-6';
    
    return (
        <div className={`${sixAnsw} d-flex m-0 mt-3 ${incClassAns()}`}>
            <div className={`card bg-dark text-white card-img-custom ${cssClickedClass} ${isMultiSelect && isClicked()}`}
                onClick={handleClick}>
                <button className={`bg-transparent border-0 p-0 m-0 cursor-default ${cssDisabledClickEvent}`}>
                    <img src={ANSWER_SVGS[number]} className="card-img" alt="image_answer_D" />
                    <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                        <h5 className="card-title text-center">Odpowiedź {ANSWER_LETTERS[number]}</h5>
                        <p className="card-text text-center">{answers[number]}</p>
                    </div>
                </button>
            </div>
        </div>
    );
};

export default QuizQuestionAnswerComponent;