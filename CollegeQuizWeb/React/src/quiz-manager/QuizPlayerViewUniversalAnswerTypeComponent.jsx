import { useContext } from "react";
import { ANSWER_LETTERS, ANSWER_SVGS } from "../utils/common";
import { SessionContext } from "../quiz-manager-renderer";

const QuizPlayerViewUniversalAnswerTypeComponent = ({ number, answer }) => {
    const { isAnswersVisible } = useContext(SessionContext);
    
    const isIncorrect = isAnswersVisible && !answer.IsCorrect ? 'incorrectAnswer' : '';
    
    return (
        <div className={`col-6 d-flex m-0 mt-3`}>
            <div className={`card bg-dark text-white card-img-custom-noanim ${isIncorrect}`}>
                <div className="bg-transparent border-0 p-0 m-0 cursor-default">
                    <img src={ANSWER_SVGS[number]} className="card-img" alt="image_answer_D" />
                    <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                        <h5 className="card-title text-center">Odpowiedź {ANSWER_LETTERS[number]}</h5>
                        <p className="card-text text-center">{answer.Text}</p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuizPlayerViewUniversalAnswerTypeComponent;