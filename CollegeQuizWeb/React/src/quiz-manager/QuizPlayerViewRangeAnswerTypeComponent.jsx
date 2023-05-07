import { useContext, useEffect, useRef } from "react";
import { SessionContext, InGameViewContext } from "../quiz-manager-renderer";
import { convertSecondsToTime } from "../utils/common";
import nouislider from "nouislider";
import wNumb from "wnumb";
import "nouislider/dist/nouislider.min.css"

const QuizPlayerViewRangeAnswerTypeComponent = () => {
    const { tick, isAnswersVisible } = useContext(SessionContext);
    const { imageUrl, questionName, rangeData } = useContext(InGameViewContext);

    const stepsSlider = useRef(null);
    const stepsSliderVisual = useRef(null);
    const stepsSliderVisualContainer = useRef(null);
    const stepsSliderContainer = useRef(null);

    useEffect(() => {
        if (!isAnswersVisible){
            stepsSliderVisual.current = nouislider.create(stepsSliderVisualContainer.current, {
                start: [ rangeData.Min, rangeData.Max ],
                connect: true,
                tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
                handleAttributes: [
                    { 'style': 'background:#9acf8a' },
                    { 'style': 'background:#9acf8a' },
                ],
                range: {
                    'min': [ rangeData.Min, rangeData.Step ],
                    'max' :rangeData.Max
                },
            });
            stepsSliderVisual.current.disable();
            return; 
        } 

        stepsSlider.current = nouislider.create(stepsSliderContainer.current, {
            start: [ rangeData.MinCounted, rangeData.CorrectAnswerRange, rangeData.MaxCounted ],
            connect: true,
            tooltips: [ wNumb({ decimals: 0 }), wNumb({ decimals: 0 }), wNumb({ decimals: 0 })],
            handleAttributes: [
                { 'style': 'background:#9acf8a' },
                { 'aria-label': 'upper' },
                { 'style': 'background:#9acf8a' },
            ],
            range: {
                'min': [ rangeData.Min, rangeData.Step ],
                'max' :rangeData.Max
            },
        });
        stepsSlider.current.disable();
    }, [ isAnswersVisible ]);
    
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {convertSecondsToTime(tick)}
                    </p>
                    <img src="https://quizazu.cdn.miloszgilga.pl/static/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-9 order-2 order-lg-1">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{questionName}</h3>
                        <div className="bg-transparent">
                            <img src={imageUrl || "https://quizazu.cdn.miloszgilga.pl/static/gfx/def_qst.svg"}
                                width="200px" height="auto" alt=""/>
                        </div>
                    </div>
                    {isAnswersVisible && <>
                        <div className="row d-flex mt-6 px-3">
                            <div ref={stepsSliderContainer}></div>
                        </div>
                        <div className="row mt-4">
                            <div className="col-md-6 mb-2">
                                <div className="btn btn-color-one-short text-white w-100 btn-std btn-nobtn">
                                    Wartość minimalna: <strong className="ms-2">{rangeData.Min}</strong>
                                </div>
                            </div>
                            <div className="col-md-6 mb-2">
                                <div className="btn btn-color-second-short text-white w-100 btn-std btn-nobtn">
                                    Wartość maksymalna: <strong className="ms-2">{rangeData.Max}</strong>
                                </div>
                            </div>
                            <div className="col-md-12 mb-2">
                                <div className="btn btn-color-one-short text-white w-100 btn-std btn-nobtn">
                                    Wartość prawidłowa: <strong className="ms-2">{rangeData.CorrectAnswerRange}</strong>
                                </div>
                            </div>
                            <div className="col-md-6 mb-2">
                                <div className="btn btn-color-one-short text-white w-100 btn-std btn-nobtn">
                                    Minimum punktowane: <strong className="ms-2">{rangeData.MinCounted}</strong>
                                </div>
                            </div>
                            <div className="col-md-6 mb-2">
                                <div className="btn btn-color-second-short text-white w-100 btn-std btn-nobtn">
                                    Maksimum punktowane: <strong className="ms-2">{rangeData.MaxCounted}</strong>
                                </div>
                            </div>
                        </div>
                    </>}
                    {!isAnswersVisible && <>
                        <div className="row d-flex mt-6 px-3">
                            <div ref={stepsSliderVisualContainer}></div>
                        </div>
                    </>}
                </div>
            </div>
        </div>
    );
};

export default QuizPlayerViewRangeAnswerTypeComponent;