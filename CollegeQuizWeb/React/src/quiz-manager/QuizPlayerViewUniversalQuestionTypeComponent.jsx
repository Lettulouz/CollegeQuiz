import { useContext } from "react";
import { InGameViewContext } from "../quiz-manager-renderer";

const QuizPlayerViewUniversalQuestionTypeComponent = ({ children }) => {
    const { imageUrl, questionName } = useContext(InGameViewContext);
    
    return (
        <div className="container">
            <div className="row">
                <div className="col-12">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{questionName}</h3>
                        <div className="bg-transparent">
                            <img src={imageUrl || "https://cdn.quizazu.com/static/gfx/def_qst.svg"}
                                className="rounded" width="200px" height="auto" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex px-3">
                        {children}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuizPlayerViewUniversalQuestionTypeComponent;