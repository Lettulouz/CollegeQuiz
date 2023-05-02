import { ANSWER_LETTERS, ANSWER_SVGS, SessionContext } from "./QuizSessionRenderer.jsx";
import { getCommonFetchObj } from "../Utils.jsx";

const QuizQuestionAnswerComponent = ({ number, isMultiSelect }) => {
    const {
        answers, connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer
    } = React.useContext(SessionContext);
    
    const [ clickedIndex, setClickedIndex ] = React.useState([]);

    const handleClick = answer => {
        if (isMultiSelect && clickedIndex.includes(answer)) return;
        if (!isMultiSelect && (isAnswerSet || currentAnswer !== "")) return;
        
        const multi = isMultiSelect ? "true" : "false";
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}/${multi}`,
            getCommonFetchObj("POST")
        ).then(r => r);
        setIsAnswerSet(true);
        if (isMultiSelect) {
            setClickedIndex([...clickedIndex, answer]);
            return;
        }
        setClickedIndex(answer);
    };

    const incClassAns = () => !Object.values(currentAnswer)
        .find(x=> x.AnswerName === answers[number]) && Object.keys(currentAnswer).length !== 0 ? 'incorrectAnswer' : '';
    
    const isClicked = () => isAnswerSet && clickedIndex !== number && 'notClicked';
    const cssDisabledClickEvent = (!isMultiSelect && isAnswerSet) && 'cursor-not-allowed';
    
    const cssClickedClass = isMultiSelect
        ? clickedIndex.includes(number) && 'clicked'
        : clickedIndex === number && 'clicked';
    
    return (
        <div className={`col-6 d-flex m-0 mt-3 ${incClassAns()}`}>
            <div className={`card bg-dark text-white card-img-custom ${cssClickedClass} ${isMultiSelect && isClicked()}`}
                onClick={() => handleClick(number)}>
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