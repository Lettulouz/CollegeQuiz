import { useContext } from "react";
import { SessionContext } from "../quiz-session-renderer";

const QuizQuestionProgressBarComponent = () => {
    const { progressWidth } = useContext(SessionContext);
    
    return (
        <div className="row w-100 mx-0 mb-3">
            <div className="question-progress-bar w-100">
                <div className="question-progress-bar-inner" style={{ width: `${progressWidth}%` }}></div>
            </div>
        </div>
    );
};

export default QuizQuestionProgressBarComponent;