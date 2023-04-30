import { useLoadableContent } from "./Hooks.jsx";
import { getCommonFetchObj, alertOff, alertInfo, alertDanger, getCommonFetchObjWithBody } from "./Utils.jsx";
const { useEffect, useState, createContext, useContext } = React;

const QuestionsContext = createContext(null);
const MainContext = createContext(null);
const QUESTION_TYPES = [
    { title: "4 odpowiedzi, jedna prawidłowa", slug: "SINGLE_FOUR_ANSWERS", },
    { title: "Prawda fałsz", slug: "TRUE_FALSE" },
    { title: "4 odpowiedzi, wiele prawidłowych", slug: "MULTIPLE_FOUR_ANSWERS" },
    { title: "6 odpowiedzi, jedna prawidłowa", slug: "SINGLE_SIX_ANSWERS" },
    { title: "Zakres", slug: "RANGE" },
];

let QUIZ_NAME = document.getElementById("addQuizQuestionsRoot").dataset.quizName;
const QUIZ_ID = document.getElementById("addQuizQuestionsRoot").dataset.quizId;

const QuizChangeNameComponent = () => {
    const { setAlert } = useContext(MainContext);
    const [ quizName, setQuizName ] = useState(QUIZ_NAME);
    const [ quizNameInvalid, setQuizNameInvalid ] = useState(false);

    const changeQuizName = e => {
        e.preventDefault();
        fetch(`/api/v1/dotnet/QuizAPI/ChangeQuizName/${QUIZ_ID}/${quizName}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setAlert(alertInfo(r.message));
                    QUIZ_NAME = quizName;
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };

    useEffect(() => {
        setQuizNameInvalid(quizName.length < 3); 
    }, [ quizName ]);
    
    return (
        <>
            <div className="hstack gap-3 w-100 mb-3">
                <form onSubmit={changeQuizName} className="hstack gap-2 w-100">
                    <input type="text" className="form-control" value={quizName} onChange={e => setQuizName(e.target.value)}/>
                    <button type="submit" className="btn btn-color-one fit-content" disabled={quizNameInvalid}>
                        <i className="bi bi-check-lg text-white"></i>
                    </button>
                    <button type="button" className="btn btn-color-one fit-content" onClick={() => setQuizName(QUIZ_NAME)}
                            data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Tooltip on top">
                        <i className="bi bi-arrow-counterclockwise text-white"></i>
                    </button>
                </form>
            </div>
        </>
    );
};


const QuizRangeAnswerComponent = ({ id }) => {
    const { q, onSetRangeProp, setIsNotValid } = useContext(QuestionsContext);
    const answerR = q.answers[0];
    const [ minInvalid, setMinInvalid ] = useState('');
    const [ minCountedInvalid, setMinCountedInvalid ] = useState('');
    const [ countedOutOfRange, setCountedOutOfRange ] = useState('');
    const [ stepIsInvalid, setStepIsInvalid ] = useState('');
    
    useEffect(() => {
        const minMax = answerR.min > answerR.max;
        const minMaxCounted = answerR.minCounted > answerR.maxCounted;
        const countedOutOfRange = answerR.minCounted > answerR.max || answerR.minCounted < answerR.min ||
            answerR.maxCounted < answerR.min || answerR.maxCounted > answerR.max;
        const stepIsInvalid = (answerR.min % answerR.step !== 0 || answerR.max % answerR.step !== 0 ||
            answerR.minCounted % answerR.step !== 0 || answerR.maxCounted % answerR.step !== 0 ||
            answerR.step > answerR.max || answerR.step < answerR.min) && answerR.step !== 1;

        setMinInvalid(minMax ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setMinCountedInvalid(minMaxCounted ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setCountedOutOfRange(countedOutOfRange ? "Wartość punktowana wykracza poza zakres" : "");
        setStepIsInvalid(stepIsInvalid ? "Wartość step musi być dzielnikiem pozostałych wartości" : "");
        
        setIsNotValid(minMax || minMaxCounted || countedOutOfRange || stepIsInvalid);
    }, [ answerR.max, answerR.min, answerR.step, answerR.minCounted, answerR.maxCounted ]);
    
    return (
        <div className="col-12">
            <div className="p-3 card">
                <div className="p-3 card">
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label htmlFor="minId" className="form-label">Min</label>
                            <input value={answerR.min} type="number" className={`form-control ${minInvalid && 'is-invalid'}`}
                                   id="minId" onChange={e => onSetRangeProp(id, e, "min")} min={0}/>
                            <div className="invalid-feedback">{minInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="stepId" className="form-label">Step(sister)</label>
                            <input value={answerR.step} type="number" className={`form-control ${stepIsInvalid && 'is-invalid'}`}
                                   id="stepId" onChange={e => onSetRangeProp(id, e, "step")} min={0}/>
                            <div className="invalid-feedback">{stepIsInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="maxId" className="form-label">Maks</label>
                            <input value={answerR.max} type="number" className="form-control" id="maxId"
                                   onChange={e => onSetRangeProp(id, e, "max")} min={0}/>
                        </div>
                        <div className="col-md-6">
                            <label htmlFor="minCountedId" className="form-label">Min punktowane</label>
                            <input value={answerR.minCounted} type="number"
                                   className={`form-control ${(countedOutOfRange || minCountedInvalid) && 'is-invalid'}`}
                                   id="minCountedId" onChange={e => onSetRangeProp(id, e, "minCounted")} min={0}/>
                            <div className="invalid-feedback">{minCountedInvalid}</div>
                        </div>
                        <div className="col-md-6">
                            <label htmlFor="maxCounterId" className="form-label">Maks punktowane</label>
                            <input value={answerR.maxCounted} type="number" className={`form-control ${countedOutOfRange && 'is-invalid'}`}
                                   id="maxCounterId" onChange={e => onSetRangeProp(id, e, "maxCounted")} min={0}/>
                            <div className="invalid-feedback">{countedOutOfRange}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

const QuizSingleGoodAnswerComponent = ({ id, answer, isMultipleAnswers }) => {
    const { q, onSetQuestionAnswer, onChangeCorrectAnswer } = useContext(QuestionsContext);
    
    const questionId = `question_${id}_group_${q.id}`;
    const groupId = `question_group_${q.id}`;
    
    return (
        <div className="col-6">
            <div className="p-3 card">
                <div className="p-3 card hstack">
                    <div className="me-2 fs-4">{answer.id}</div>
                    <input type="text" className="form-control" placeholder={`Treść odpowiedzi ${id}`}
                           onChange={e => onSetQuestionAnswer(e.target.value, q.id, answer.id)} value={answer.text}/>
                </div>
                <div className="form-check mt-2">
                    {isMultipleAnswers ? <>
                        <input type="checkbox" className="form-check-input" id={questionId} name={groupId}
                               checked={answer.isCorrect} onChange={e => onChangeCorrectAnswer(q.id, answer.id, isMultipleAnswers, e.target.checked)}/>
                    </> : <>
                        <input type="radio" className="form-check-input" id={questionId} name={groupId}
                               checked={answer.isCorrect} onChange={() => onChangeCorrectAnswer(q.id, answer.id, isMultipleAnswers, true)}/>
                    </>}
                    <label htmlFor={questionId} className="form-check-label">
                        {q.type === "TRUE_FALSE" ? id === 1 ? "Prawda" : "Fałsz" : "To jest poprawna odpowiedź"}
                    </label>
                </div>
            </div>
        </div>
    );
};

const QuizQuestionComponent = () => {
    const {
        q, onSetQuestionTitle, onRemoveQuestion, onSetMinutes, onSetSeconds, onSetQuestionType
    } = useContext(QuestionsContext);
    
    const generateOptions = QUESTION_TYPES.map(o => <option key={o.slug} value={o.slug}>{o.title}</option>);
    
    const answersComponents = q.answers.map((answer, idx) => (
        <QuizSingleGoodAnswerComponent
            key={idx} id={idx + 1} answer={answer}
            isMultipleAnswers={q.type === "MULTIPLE_FOUR_ANSWERS"}
        />
    ));
    
    useEffect(() => {
        if (q.timeMin.length > 3) onSetMinutes(q.id, q.timeMin.slice(0, 3));
    }, [ q.timeMin ]);

    useEffect(() => {
        if (q.timeSec.length > 2) onSetSeconds(q.id, e.timeSec.slice(0, 2));
        if (q.timeSec > 59) onSetSeconds(q.id, 59);
    }, [ q.timeSec ]);
    
    return (
        <div className="row g-2 mb-4">
            <div className="col-12">
                <div className="p-3 card">
                    <div className="mb-2">
                        <select className="form-select" value={q.type}
                                onChange={e => onSetQuestionType(e.target.value, q.id)}>
                            {generateOptions}
                        </select>
                    </div>
                    <div className="hstack gap-2">
                        <div className="me-2 fs-3">{q.id}</div>
                        <textarea className="form-control h-100" placeholder="Treść pytania" value={q.text}
                                  onChange={e => onSetQuestionTitle(e.target.value, q.id)}></textarea>
                        <div>
                            <label className="mb-1">Czas trwania pytania:</label>
                            <div className="d-flex">
                                <input type="number" className="form-control time-control" placeholder="min" min="0" max="999"
                                       maxLength="3" value={q.timeMin} onChange={e => onSetMinutes(q.id, e.target.value)}/>
                                <span className="mx-2 fw-bold pt-1">:</span>
                                <input type="number" className="form-control time-control" placeholder="sek" min="0" max="59"
                                       maxLength="2" value={q.timeSec} onChange={e => onSetSeconds(q.id, e.target.value)}/>
                            </div>
                        </div>
                        {q.id > 1 && <button className="btn btn-danger text-white ms-2"
                                             onClick={() => onRemoveQuestion(q.id)}>X</button>}
                    </div>
                </div>
            </div>
            {q.type === "RANGE" ? <QuizRangeAnswerComponent id={q.id}/> : answersComponents}
        </div>
    );
};

const initialQuestions = [
    {
        id: 1,
        text: '',
        timeMin: '0',
        timeSec: '',
        type: 'SINGLE_FOUR_ANSWERS',
        answers: [
            { id: 1, text: '', isCorrect: true, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
            { id: 2, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
            { id: 3, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
            { id: 4, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
        ],
    },
];

const AddQuizQuestionsRoot = () => {
    const [ questions, setQuestions ] = useState(initialQuestions);
    const [ allGood, setAllGood ] = useState(true);
    const [ alert, setAlert ] = useState(alertOff());
    const [ isActive, setActiveCallback ] = useLoadableContent();
    const [ isSended, setIsSended ] = useState(false);
    const [ isNotValid, setIsNotValid ] = useState(false);
    
    const onSetQuestionType = (type, questionId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].type = type;
        onChangeQuestionType(questionId, type);
        setQuestions(qst);
    };
    
    const onSetQuestionTitle = (text, questionId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].text = text;
        setQuestions(qst);
    };
    
    const onSetQuestionAnswer = (text, questionId, answerId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        const answIdx = qst[idx].answers.findIndex(a => a.id === answerId);
        qst[idx].answers[answIdx].text = text;
        setQuestions(qst);
    };
    
    const onChangeCorrectAnswer = (questionId, answerId, isMultipleAnswers, checked) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        if (!isMultipleAnswers) {
            qst[idx].answers.forEach(a => a.isCorrect = false);
        }
        const answIdx = qst[idx].answers.findIndex(a => a.id === answerId);
        qst[idx].answers[answIdx].isCorrect = checked;
        setQuestions(qst);
    };
    
    const onSetMinutes = (questionId, minutes) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].timeMin = minutes;
        setQuestions(qst);
    };

    const onSetSeconds = (questionId, seconds) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].timeSec = seconds;
        setQuestions(qst);
    };

    const onAddNewQuestion = () => {
        setQuestions([ ...questions, {
            id: questions.length + 1,
            text: '',
            timeMin: '0',
            timeSec: '',
            type: 'SINGLE_FOUR_ANSWERS',
            answers: [
                { id: 1, text: '', isCorrect: true, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
                { id: 2, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
                { id: 3, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 },
                { id: 4, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 }
            ]
        }]);
    };

    const generateAnswerObject = (id, isCorrect) => (
        { id: id + 1, text: '', isCorrect, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 }
    );
    
    const onChangeQuestionType = (questionId, type) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        switch (type) {
            case "TRUE_FALSE":
                qst[idx].answers = Array.from({ length: 2 }).map((_, i) => generateAnswerObject(i, i === 0));
                console.log(qst[idx].answers);
                break;
            case "RANGE":
                qst[idx].answers = [
                    { id: 1, text: '', isCorrect: true, isRange: true, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0 }
                ];
                break;
            case "SINGLE_SIX_ANSWERS":
                qst[idx].answers = Array.from({ length: 6 }).map((_, i) => generateAnswerObject(i, i === 0));
                break;
            case "MULTIPLE_FOUR_ANSWERS":
                qst[idx].answers = Array.from({ length: 4 }).map((_, i) => generateAnswerObject(i, false));
                break;
            default:
                qst[idx].answers = Array.from({ length: 4 }).map((_, i) => generateAnswerObject(i, i === 0));
        }
        setQuestions(qst);
    };

    const onSetRangeProp = (questionId, e, propType) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].answers[0][propType] = Number(e.target.value);
        setQuestions(qst);
    };
    
    const onRemoveQuestion = (id) => {
        const qst = [ ...questions ];
        setQuestions(qst.filter(q => q.id !== id));
    };
    
    const appendQuestionsTooQuiz = () => {
        if (isSended) return;
        setIsSended(true);
        const path = window.location.pathname.split('/');
        const id = path[path.length - 1];
        fetch(`/api/v1/dotnet/QuizAPI/AddQuizQuestions/${id}`, getCommonFetchObjWithBody('POST', { aggregate: questions }))
            .then(r => {
                setIsSended(false);
                window.scrollTo(0, 0);
                return r.json()
            })
            .then(r => {
                if (r.isGood) {
                    setAlert(alertInfo(r.message));
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };
    
    useEffect(() => {
        const loadableSpinner = document.getElementById('react-loadable-spinner-content');
        const path = window.location.pathname.split('/');
        const id = path[path.length - 1];
        fetch(`/api/v1/dotnet/QuizAPI/GetQuizQuestions/${id}`, getCommonFetchObj('GET')).then(r => {
            loadableSpinner.style.cssText = 'display:none !important';
            setActiveCallback();
            return r.json()
        }).then(r => { 
            if (r.aggregate.length !== 0) setQuestions(r.aggregate);
        }).catch(e => {
            if (e === undefined) return;
            setAlert({ active: true, style: 'alert-danger', message: 'Wystąpił nieznany błąd' });
        });
    }, []);
    
    useEffect(() => {
        const anyBad = questions.filter(q => {
            return q.text.length <= 2 || (q.answers
                    .filter(a => a.text.length >= 1).length !== q.answers.length && q.type !== "RANGE") ||
                q.timeMin.length === 0 || q.timeSec.length === 0 || 
                q.answers.filter(q => q.isCorrect).length === 0;
        });
        setAllGood(anyBad.length === 0 && !isNotValid);
    }, [ questions, isNotValid ]);
    
    const generateQuestionsComponents = questions.map((q, idx) => (
        <QuestionsContext.Provider key={idx} value={{
            q, setQuestions, onSetQuestionAnswer, onChangeCorrectAnswer, onSetQuestionTitle, onRemoveQuestion,
            onSetMinutes, onSetSeconds, onSetQuestionType, onSetRangeProp, setIsNotValid
        }}>
            <QuizQuestionComponent/>
        </QuestionsContext.Provider>
    ));
    
    return (
        <MainContext.Provider value={{ setAlert }}>
            {isActive && <>
                <div className="alert alert-warning">
                    Aby zaktualizowac nazwę quizu, musi się składać ono z przynajmniej 3 znaków. Aby poprawnie
                    dodać/zaktualizować pytania, każde pole musi zostać wypełnione przynajmniej dwoma znakami
                    a w przypadku odpowiedzi, musi być przynajmniej jedna zaznaczona.
                </div>
                {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between`} role="alert">
                    <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                    <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                </div>}
                <QuizChangeNameComponent/>
                {generateQuestionsComponents}
                <button onClick={onAddNewQuestion} className="btn btn-color-one w-100 mt-2">
                    Dodaj nowe pytanie
                </button>
                {allGood && <button className="btn btn-color-one mt-2 btn-light w-100" onClick={appendQuestionsTooQuiz}>
                    Zaktualizuj pytania quizu
                </button>}
            </>}
        </MainContext.Provider>
    );
};

ReactDOM
    .createRoot(document.getElementById('addQuizQuestionsRoot'))
    .render(<AddQuizQuestionsRoot/>);
