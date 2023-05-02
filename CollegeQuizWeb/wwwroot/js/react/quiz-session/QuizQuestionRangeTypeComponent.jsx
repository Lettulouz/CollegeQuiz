import { SessionContext } from "./QuizSessionRenderer.jsx";
import { getCommonFetchObj } from "../Utils.jsx";

import LeaveQuizSessionButtonComponent from "./LeaveQuizSessionButtonComponent.jsx";

const QuizQuestionRangeTypeComponent = () => {
    const {
        connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer, answerSett, answRange, setAnswRange,
        questionImage, question, questionTimer
    } = React.useContext(SessionContext);

    const stepsSlider = React.useRef(null);

    const handleClick = () => {
        if(isAnswerSet || currentAnswer !== "") return;

        const answer = "r" + parseInt(answRange.min) + "," + parseInt(answRange.max);
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}/false`,
            getCommonFetchObj("POST")
        ).then(r => r);
        setIsAnswerSet(true);
    };

    React.useEffect(() => {
        noUiSlider.create(stepsSlider.current, {
            start: [ answerSett.min, answerSett.max ],
            behaviour: 'drag',
            connect: true,
            tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
            range: {
                'min': [ answerSett.min, answerSett.step ],
                'max' :answerSett.max
            },
        });
        stepsSlider.current.noUiSlider.on('update', function (values, _) {
            setAnswRange({ min: values[0], max: values[1] });
        });
    }, []);
    
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
                        <img src={questionImage || "/gfx/timer.svg"} width="200px" height="200px" alt=""/>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSlider}></div>
                    </div>
                    <div className="row d-flex mt-3 px-3">
                        <div className="col-12 d-flex m-0 justify-content-center text-center">
                            <div onClick={() => handleClick()}>
                                <button className={`btn btn-success border-0 w-100 m-0 text-white rounded cursor-default
                                    ${isAnswerSet && 'cursor-not-allowed'} `}>
                                    Zatwierdź
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-2 px-0">
                    <LeaveQuizSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionRangeTypeComponent;