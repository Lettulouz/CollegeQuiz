import StartQuizButtonComponent from "./StartQuizButtonComponent.jsx";
import NextQuestionButtonComponent from "./NextQuestionButtonComponent.jsx";

const QuizManagerRightContentComponent = () => {
    return (
        <div className="col-lg-3 px-0 px-md-1 mb-2 mb-lg-0 order-lg-0 order-0">
            <div className="card trsp px-3 py-3 h-100 d-flex flex-column justify-content-between">
                <div className="w-100">
                    <div className="mb-2">
                        <StartQuizButtonComponent/>
                    </div>
                    <NextQuestionButtonComponent/>
                </div>
                <div className="w-100">
                    <button className="btn btn-danger mt-2 text-white w-100" onClick={() => window.location.reload()}>
                        Zakończ quiz
                    </button>
                </div>
            </div>
        </div>
    );
};

export default QuizManagerRightContentComponent;