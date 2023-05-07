import { useContext, useState, useEffect } from "react";
import { QuestionsContext, MainContext } from "../quiz-questions-renderer";

const QuizQuestionsRangeAnswerComponent = () => {
    const { questions, setQuestions, setIsNotValid } = useContext(MainContext);
    const { q } = useContext(QuestionsContext);
    
    const answerR = q.answers[0];
    
    const [ minInvalid, setMinInvalid ] = useState('');
    const [ minCountedInvalid, setMinCountedInvalid ] = useState('');
    const [ countedOutOfRange, setCountedOutOfRange ] = useState('');
    const [ stepIsInvalid, setStepIsInvalid ] = useState('');
    const [ isInvalidCorrectAns, setIsCorrectInvalid ] = useState('');

    const onSetRangeProp = (e, propType) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qstData => qstData.id === q.id);
        qst[idx].answers[0][propType] = Number(e.target.value);
        setQuestions(qst);
    };
    
    useEffect(() => {
        const minMax = answerR.min > answerR.max;
        const minMaxCounted = answerR.minCounted > answerR.maxCounted;
        const countedOutOfRange = answerR.minCounted > answerR.max || answerR.minCounted < answerR.min ||
            answerR.maxCounted < answerR.min || answerR.maxCounted > answerR.max;
        const stepIsInvalid =
            ((answerR.min - answerR.correctAns) % answerR.step !== 0 ||
                (answerR.max - answerR.correctAns) % answerR.step !== 0 ||
                (answerR.minCounted - answerR.correctAns) % answerR.step !== 0 ||
                (answerR.maxCounted - answerR.correctAns) % answerR.step !== 0) && answerR.step !== 1;
        const correctAnsIsInvalid = answerR.correctAns > answerR.max || answerR.correctAns < answerR.min;

        setMinInvalid(minMax ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setMinCountedInvalid(minMaxCounted ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setCountedOutOfRange(countedOutOfRange ? "Wartość punktowana wykracza poza zakres" : "");
        setStepIsInvalid(stepIsInvalid ? "Wartość przejścia musi być dzielnikiem pozostałych wartości" : "");
        setIsCorrectInvalid(correctAnsIsInvalid ? "Nieprawidłowa wartość prawidłowej odpowiedzi" : "");

        setIsNotValid(minMax || minMaxCounted || countedOutOfRange || stepIsInvalid || correctAnsIsInvalid);
    }, [ answerR.max, answerR.min, answerR.step, answerR.minCounted, answerR.maxCounted, answerR.correctAns ]);

    return (
        <div className="col-12">
            <div className="p-3 card card-glass-effect">
                <div className="p-1">
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label htmlFor="minId" className="form-label">Min</label>
                            <input value={answerR.min} type="number" className={`form-control ${minInvalid && 'is-invalid'}`}
                                id="minId" onChange={e => onSetRangeProp(e, "min")}/>
                            <div className="invalid-feedback">{minInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="stepId" className="form-label">Wartość przejścia</label>
                            <input value={answerR.step} type="number" className={`form-control ${stepIsInvalid && 'is-invalid'}`}
                                id="stepId" onChange={e => onSetRangeProp(e, "step")}/>
                            <div className="invalid-feedback">{stepIsInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="maxId" className="form-label">Maks</label>
                            <input value={answerR.max} type="number" className="form-control" id="maxId"
                                onChange={e => onSetRangeProp(e, "max")}/>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="minCountedId" className="form-label">Min punktowane</label>
                            <input value={answerR.minCounted} type="number"
                                className={`form-control ${(countedOutOfRange || minCountedInvalid) && 'is-invalid'}`}
                                id="minCountedId" onChange={e => onSetRangeProp(e, "minCounted")}/>
                            <div className="invalid-feedback">{minCountedInvalid}</div>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="correctAnsId" className="form-label">Prawidłowa odpowiedź</label>
                            <input value={answerR.correctAns} type="number"
                                className={`form-control ${(isInvalidCorrectAns) && 'is-invalid'}`}
                                id="correctAnsId" onChange={e => onSetRangeProp(e, "correctAns")}/>
                            <div className="invalid-feedback">{isInvalidCorrectAns}</div>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="maxCounterId" className="form-label">Maks punktowane</label>
                            <input value={answerR.maxCounted} type="number" className={`form-control ${countedOutOfRange && 'is-invalid'}`}
                                id="maxCounterId" onChange={e => onSetRangeProp(e, "maxCounted")}/>
                            <div className="invalid-feedback">{countedOutOfRange}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionsRangeAnswerComponent;
