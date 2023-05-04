import { useContext, useRef, useEffect } from "react";
import { SessionContext } from "../quiz-session-renderer";
import { alertDanger, getCommonFetchObj } from "../utils/common";
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
    
    useEffect(() => {
        if (!stepsSliderContainer.current || stepsSlider.current) return;

        stepsSlider.current = nouislider.create(stepsSliderContainer.current, {
            start: [ answerSett.min, answerSett.max ],
            behaviour: 'drag',
            connect: true,
            range: {
                'min': [ answerSett.min, answerSett.step ],
                'max': answerSett.max
            },
            tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
        });
        stepsSlider.current.on('change', values => setAnswRange({ min: values[0], max: values[1] }));
        
        return () => {
            if (stepsSlider.current) {
                stepsSlider.current.destroy();
                stepsSlider.current = null;
            }
        };
    }, [ stepsSliderContainer, answerSett ]);

    useEffect(() => {
        if (!resultSliderContainer.current || !currentAnswer || !resultSlider.current) return;
        
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
                'min': [ answerSett.min, answerSett.step ],
                'max': answerSett.max
            },
        });
    }, [ resultSliderContainer, currentAnswer, resultSlider ]);
    
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
                        <div ref={stepsSliderContainer} />
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={resultSliderContainer}></div>
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
                    </div>}
                </div>
                <div className="col-lg px-0 order-0 order-lg-2 mb-3">
                    <LeaveQuizSessionButtonComponent text="Wyjdź"/>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionRangeTypeComponent;