import { SessionContext } from "./QuizSessionRenderer.jsx";

import LeaveQuizSessionButtonComponent from "./LeaveQuizSessionButtonComponent.jsx";

const QuizQuestionUniversalTypeComponent = ({ children }) => {
    const { question, questionTimer, questionImage } = React.useContext(SessionContext);
    
    return (
        <div className="container d-flex">
            <div className="row d-flex justify-content-center">
                <div className="col-1 px-0">
                    <div className="card card-img-custom">
                        <img src="/gfx/timer.svg" alt="image_answer_D"/>
                        <div className="card-body card-img-overlay d-flex flex-column align-items-center justify-content-center">
                            <p className="card-title text-center m-0 text-prim-color fs-5 fw-bold">{questionTimer}</p>
                        </div>
                    </div>
                </div>
                <div className="col-9">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3>{question}</h3>
                        <div className="bg-transparent border-image nohov">
                            <img src={questionImage || "/gfx/timer.svg"} width="100%" height="100%" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex px-3">
                        {children}
                    </div>
                </div>
                <div className="col-lg px-0 order-0 order-lg-2 mb-3">
                    <LeaveQuizSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionUniversalTypeComponent;