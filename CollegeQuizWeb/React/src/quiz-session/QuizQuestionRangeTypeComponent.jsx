import { useContext, useRef, useEffect } from "react";
import { SessionContext } from "../quiz-session-renderer";
import { alertDanger, convertSecondsToTime, generateErrorMessage, getCommonFetchObj } from "../utils/common";
import wNumb from "wnumb";
import nouislider from "nouislider";
import "nouislider/dist/nouislider.css";

import LeaveQuizSessionButtonComponent from "./LeaveQuizSessionButtonComponent";
import QuizQuestionProgressBarComponent from "./QuizQuestionProgressBarComponent";

const QuizQuestionRangeTypeComponent = () => {
    const {
        connectionId, questionNumber, isAnswerSet, setIsAnswerSet, currentAnswer, answerSett, answRange, setAnswRange,
        questionImage, question, questionTimer
    } = useContext(SessionContext);
    
    const stepsSliderContainer = useRef(null);
    const resultSliderContainer = useRef(null);
    const stepsSlider = useRef(null);
    const resultSlider = useRef(null);

    const leftToRight = () => {
        var values = stepsSlider.current.get();
        answRange.Min = values[1];
        stepsSlider.current.set([values[1], null]);
    }
    
    const rightToLeft = () => {
        var values = stepsSlider.current.get();
        answRange.Max = values[0];
        stepsSlider.current.set([null, values[0]]);
    }
    
    const handleClick = () => {
        if (isAnswerSet || currentAnswer !== "") return;

        const answer = "r" + parseInt(answRange.Min) + "," + parseInt(answRange.Max);
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/SendAnswer/${connectionId}/${questionNumber}/${answer}/false`,
            getCommonFetchObj("POST")
        ).then(r => {
            if (r.ok) {
                return r;
            }
            throw new Error(r.status);
        }).catch(e => {
            if (e === undefined) return;
            setAlert(alertDanger(generateErrorMessage(e.message)));
        });
        setIsAnswerSet(true);
        stepsSlider.current.disable();
    };
    
    useEffect(() => {
        if (!stepsSliderContainer.current || stepsSlider.current) return;

        stepsSlider.current = nouislider.create(stepsSliderContainer.current, {
            start: [ answerSett.Min, answerSett.Max ],
            behaviour: 'drag',
            connect: true,
            range: {
                'min': [ answerSett.Min, answerSett.Step ],
                'max': answerSett.Max
            },
            tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
        });
        stepsSlider.current.on('change', values => setAnswRange({ Min: values[0], Max: values[1] }));
    }, [ stepsSliderContainer, answerSett ]);

    useEffect(() => {
        if (!resultSliderContainer.current || !currentAnswer) return;
        
        let attributes;
        if(currentAnswer[0].AnswerMinCounted === currentAnswer[0].AnswerCorrect) {
            attributes = [
                { 'style': 'background:#DC3545' },
                { 'style': 'background:#DC3545' },
                { 'aria-label': 'upper' },
            ];
        } else if (currentAnswer[0].AnswerMaxCounted === currentAnswer[0].AnswerCorrect) {
            attributes = [
                { 'aria-label': 'upper' },
                { 'style': 'background:#DC3545' },
                { 'style': 'background:#DC3545' },
            ];
        } else {
            attributes = [
                { 'aria-label': 'upper' },
                { 'style': 'background:#DC3545' },
                { 'aria-label': 'upper' },
            ];
        }
        resultSlider.current = nouislider.create(resultSliderContainer.current, {
            start: [ currentAnswer[0].AnswerMinCounted, currentAnswer[0].AnswerCorrect, currentAnswer[0].AnswerMaxCounted ],
            connect: true,
            tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
            handleAttributes: attributes,
            range: {
                'min': [ answerSett.Min, answerSett.Step ],
                'max': answerSett.Max
            },
        });
        Array.from({ length: 3 }).forEach((_, i) => resultSlider.current.disable(i))
    }, [ resultSliderContainer, currentAnswer, resultSlider ]);
    
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {convertSecondsToTime(questionTimer)}
                    </p>
                    <img src="https://quizazu.cdn.miloszgilga.pl/static/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-10 order-2 order-lg-1">
                    <QuizQuestionProgressBarComponent/>
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{question}</h3>
                        <div className="bg-transparent">
                            <img src={questionImage || "https://quizazu.cdn.miloszgilga.pl/static/gfx/def_qst.svg"}
                                className="rounded" width="300px" height="auto" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSliderContainer} />
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={resultSliderContainer}></div>
                    </div>
                    {!isAnswerSet &&
                    <div>
                        <div className="row d-flex mt-3 px-3 justify-content-between">
                            <div className="col-3 btn btn-color-one text-white" onClick={() => leftToRight()}>Do prawej</div>
                            <div className="col-3 btn btn-color-one text-white" onClick={() => handleClick()}>Zatwierdź</div>
                            <div className="col-3 btn btn-color-one text-white" onClick={() => rightToLeft()}>Do lewej</div>
                        </div>
                    </div>}
                </div>
                <div className="col-lg px-0 order-0 order-lg-2 mb-3">
                    <LeaveQuizSessionButtonComponent text={<i className=" bi bi-box-arrow-left session-icon m-0"></i>}/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionRangeTypeComponent;