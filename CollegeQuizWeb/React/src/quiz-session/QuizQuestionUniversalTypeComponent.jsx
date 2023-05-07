import { useContext } from "react";
import { SessionContext } from "../quiz-session-renderer";

import LeaveQuizSessionButtonComponent from "./LeaveQuizSessionButtonComponent";
import QuizQuestionProgressBarComponent from "./QuizQuestionProgressBarComponent";
import QuizQuestionSessionIcon from "./QuizQuestionSessionIcon";

const QuizQuestionUniversalTypeComponent = ({ children }) => {
    const { question, questionTimer, questionImage } = useContext(SessionContext);
    console.log(question)
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {questionTimer}
                    </p>
                    <img src="/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-10 order-2 order-lg-1">
                    <QuizQuestionProgressBarComponent/>
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{question}</h3>
                        <div className="bg-transparent">
                            <img src={questionImage || "/gfx/def_qst.svg"} width="300px" height="auto" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex px-3">
                        {children}
                    </div>
                </div>
                <div className="col-lg px-0 order-0 order-lg-1 mb-3 text-center">
                    <LeaveQuizSessionButtonComponent text={<i className=" bi bi-box-arrow-left session-icon m-0"></i>}/>
                    <QuizQuestionSessionIcon/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionUniversalTypeComponent;