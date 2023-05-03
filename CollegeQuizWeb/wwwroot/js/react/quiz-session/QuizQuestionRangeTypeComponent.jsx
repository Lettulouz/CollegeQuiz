import { SessionContext } from "./QuizSessionRenderer.jsx";
import { alertDanger, getCommonFetchObj } from "../Utils.jsx";

import LeaveQuizSessionButtonComponent from "./LeaveQuizSessionButtonComponent.jsx";
import QuizQuestionProgressBarComponent from "./QuizQuestionProgressBarComponent.jsx";

const QuizQuestionRangeTypeComponent = () => {
    const {
        connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer, answerSett, answRange, setAnswRange,
        questionImage, question, questionTimer
    } = React.useContext(SessionContext);

    
    const stepsSlider = React.useRef(null);
    const stepsSliderResult = React.useRef(null);
    const componentReady = React.useRef(false);

    const handleClick = () => {
        if (isAnswerSet || currentAnswer !== "") return;

        const answer = "r" + parseInt(answRange.min) + "," + parseInt(answRange.max);
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}/false`,
            getCommonFetchObj("POST")
        ).then(r => r)
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił błąd podczas wysyłania odpowiedzi przez użytkownika.'));
            });
        setIsAnswerSet(true);
        stepsSlider.current.noUiSlider.disable();
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

    React.useEffect(() => {
        if (currentAnswer !== "") {
            if(currentAnswer[0].AnswerMinCounted === currentAnswer[0].AnswerCorrect){
                noUiSlider.create(stepsSliderResult.current, {
                    start: [ currentAnswer[0].AnswerMinCounted, currentAnswer[0].AnswerCorrect, currentAnswer[0].AnswerMaxCounted ],
                    connect: true,
                    tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
                    handleAttributes: [
                        { 'style': 'background:#DC3545' },
                        { 'style': 'background:#DC3545' },
                        { 'aria-label': 'upper' },
                    ],
                    range: {
                        'min': [ answerSett.min, answerSett.step ],
                        'max' :answerSett.max
                    },
                });
            }else if(currentAnswer[0].AnswerMaxCounted === currentAnswer[0].AnswerCorrect){
                noUiSlider.create(stepsSliderResult.current, {
                    start: [ currentAnswer[0].AnswerMinCounted, currentAnswer[0].AnswerCorrect, currentAnswer[0].AnswerMaxCounted ],
                    connect: true,
                    tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
                    handleAttributes: [
                        { 'aria-label': 'upper' },
                        { 'style': 'background:#DC3545' },
                        { 'style': 'background:#DC3545' },
                    ],
                    range: {
                        'min': [ answerSett.min, answerSett.step ],
                        'max' :answerSett.max
                    },
                });
            }else{
                noUiSlider.create(stepsSliderResult.current, {
                    start: [ currentAnswer[0].AnswerMinCounted, currentAnswer[0].AnswerCorrect, currentAnswer[0].AnswerMaxCounted ],
                    connect: true,
                    tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
                    handleAttributes: [
                        { 'aria-label': 'upper' },
                        { 'style': 'background:#DC3545' },
                        { 'aria-label': 'upper' },
                    ],
                    range: {
                        'min': [ answerSett.min, answerSett.step ],
                        'max' :answerSett.max
                    },
                });
            }
            stepsSliderResult.current.noUiSlider.disable(0);
            stepsSliderResult.current.noUiSlider.disable(1);
            stepsSliderResult.current.noUiSlider.disable(2);
        }
           
    }, [currentAnswer]);


    
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {questionTimer}
                    </p>
                    <img src="/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-9 order-2 order-lg-1">
                    <QuizQuestionProgressBarComponent/>
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{question}</h3>
                        <div className="bg-transparent">
                            <img src={questionImage || "/gfx/def_qst.svg"} width="300px" height="auto" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSlider}></div>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSliderResult}></div>
                    </div>
                    {!isAnswerSet &&
                    <div className="row d-flex mt-3 px-3">
                        <div className="col-12 d-flex m-0 justify-content-center text-center">
                            <div onClick={() => handleClick()}>
                                <button className={`btn btn-success border-0 w-100 m-0 text-white rounded cursor-default `}>
                                    Zatwierdź
                                </button>
                            </div>
                        </div>
                    </div>
                    }
                </div>
                <div className="col-lg px-0 order-0 order-lg-2 mb-3">
                    <LeaveQuizSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionRangeTypeComponent;