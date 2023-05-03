import { InGameViewContext } from "./QuizManagerInGameViewComponent.jsx";
import { SessionContext } from "./QuizManagerRenderer.jsx";

const QuizPlayerViewUniversalQuestionTypeComponent = ({ children }) => {
    const { imageUrl, questionName } = React.useContext(InGameViewContext);
    const { tick } = React.useContext(SessionContext);
    
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {tick}
                    </p>
                    <img src="/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-10 order-2 order-lg-1">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{questionName}</h3>
                        <div className="bg-transparent">
                            <img src={imageUrl || "/gfx/def_qst.svg"} width="200px" height="auto" alt=""/>
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