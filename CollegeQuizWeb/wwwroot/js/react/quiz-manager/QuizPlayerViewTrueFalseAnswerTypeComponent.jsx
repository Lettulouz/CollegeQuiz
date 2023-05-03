import { ANSWER_SVGS } from "../Utils.jsx";

const QuizPlayerViewTrueFalseAnswerTypeComponent = ({ number, answer }) => {
    return (
        <div className={`col-6 d-flex m-0 mt-3`}>
            <div className={`card bg-dark text-white card-img-custom-noanim ${!answer.IsCorrect && 'incorrectAnswer'}`}>
                <div className="bg-transparent border-0 p-0 m-0 cursor-default">
                    <img src={ANSWER_SVGS[number]} className="card-img" alt="image_answer_D" />
                    <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                        <h5 className="card-title text-center">{number === 0 ? 'Prawda' : 'Fałsz'}</h5>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuizPlayerViewTrueFalseAnswerTypeComponent;