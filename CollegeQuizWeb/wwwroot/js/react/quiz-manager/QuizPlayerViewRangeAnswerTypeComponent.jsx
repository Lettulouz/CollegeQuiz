import { SessionContext } from "./QuizManagerRenderer.jsx";
import { InGameViewContext } from "./QuizManagerInGameViewComponent.jsx";

const QuizPlayerViewRangeAnswerTypeComponent = () => {
    const { tick } = React.useContext(SessionContext);
    const { imageUrl, questionName, rangeData } = React.useContext(InGameViewContext);

    const stepsSlider = React.useRef(null);

    React.useEffect(() => {
        noUiSlider.create(stepsSlider.current, {
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
    }, []);
    
    return (
        <div className="container">
            <div className="row">
                <div className="col-lg px-0 order-1 order-lg-0 time-image position-relative mx-auto">
                    <p className="fw-bold time-text">
                        {tick}
                    </p>
                    <img src="/gfx/timer.svg" alt="image_answer_D" className="img-fluid"/>
                </div>
                <div className="col-lg-9 order-2 order-lg-1">
                    <div className="card px-3 py-3 d-flex align-items-center text-break">
                        <h3 className="quiz-color-text">{questionName}</h3>
                        <div className="bg-transparent">
                            <img src={imageUrl || "/gfx/def_qst.svg"} width="200px" height="auto" alt=""/>
                        </div>
                    </div>
                    <div className="row d-flex mt-6 px-3">
                        <div ref={stepsSlider}></div>
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
                </div>
            </div>
        </div>
    );
};

export default QuizPlayerViewRangeAnswerTypeComponent;