import { useLoadableContent } from "./Hooks.jsx";
import {getCommonFetchObj, alertOff, alertInfo, alertDanger, getCommonFetchObjWithBody} from "./Utils.jsx";
const { useEffect, useState, createContext, useContext } = React;

const QuestionsContext = createContext(null);

const QuizAnswerComponent = ({ id, answer }) => {
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
                    <input type="radio" className="form-check-input" id={questionId} name={groupId}
                           checked={answer.isCorrect} onChange={() => onChangeCorrectAnswer(q.id, answer.id)}/>
                    <label htmlFor={questionId} className="form-check-label">
                        To jest poprawna odpowiedź
                    </label>
                </div>
            </div>
        </div>
    );
};

const QuizQuestionComponent = () => {
    const { q, onSetQuestionTitle, onRemoveQuestion, onSetMinutes, onSetSeconds } = useContext(QuestionsContext);
    
    const answersComponents = q.answers.map((answer, idx) => (
        <QuizAnswerComponent key={idx} id={idx + 1} answer={answer}/>
    ));
    
    return (
        <div className="row g-2 mb-4">
            <div className="col-12">
                <div className="p-3 card hstack gap-2">
                    <div className="me-2 fs-3">{q.id}</div>
                    <textarea className="form-control h-100" placeholder="Treść pytania" value={q.text}
                        onChange={e => onSetQuestionTitle(e.target.value, q.id)}></textarea>
                    <div>
                        <label className="mb-1">Czas trwania pytania:</label>
                        <div className="d-flex">
                            <input type="number" className="form-control time-control" placeholder="min"
                                value={q.timeMin} onChange={e => onSetMinutes(q.id, e.target.value)}/>
                            <span className="mx-2 fw-bold pt-1">:</span>
                            <input type="number" className="form-control time-control" placeholder="sek"
                                value={q.timeSec} onChange={e => onSetSeconds(q.id, e.target.value)}/>
                        </div>
                    </div>
                    {q.id > 1 && <button className="btn btn-danger text-white ms-2"
                                         onClick={() => onRemoveQuestion(q.id)}>X</button>}
                </div>
            </div>
            {answersComponents}
        </div>
    );
};

const initialQuestions = [
    {
        id: 1,
        text: '',
        timeMin: '',
        timeSec: '',
        answers: [
            { id: 1, text: '', isCorrect: true },
            { id: 2, text: '', isCorrect: false },
            { id: 3, text: '', isCorrect: false },
            { id: 4, text: '', isCorrect: false }
        ]
    }
];

const AddQuizQuestionsRoot = () => {
    const [ questions, setQuestions ] = useState(initialQuestions);
    const [ allGood, setAllGood ] = useState(true);
    const [ alert, setAlert ] = useState(alertOff());
    const [ isActive, setActiveCallback ] = useLoadableContent();
    
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
    
    const onChangeCorrectAnswer = (questionId, answerId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].answers.forEach(a => a.isCorrect = false);
        const answIdx = qst[idx].answers.findIndex(a => a.id === answerId);
        qst[idx].answers[answIdx].isCorrect = true;
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
            answers: [
                { id: 1, text: '', isCorrect: true },
                { id: 2, text: '', isCorrect: false },
                { id: 3, text: '', isCorrect: false },
                { id: 4, text: '', isCorrect: false }
            ]
        }]);
    };
    
    const onRemoveQuestion = (id) => {
        const qst = [ ...questions ];
        setQuestions(qst.filter(q => q.id !== id));
    };
    
    const appendQuestionsTooQuiz = () => {
        const path = window.location.pathname.split('/');
        const id = path[path.length - 1];
        fetch(`/api/v1/dotnet/quizapi/quiz-questions?id=${id}`, getCommonFetchObj('POST'))
            .then(r => r.json())
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
        fetch(`/api/v1/dotnet/quizapi/quiz-questions?id=${id}`, getCommonFetchObj('GET')).then(r => {
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
        let allGood = false;
        questions.forEach(q => {
            allGood = q.text.length > 2 && q.answers.filter(a => a.text.length > 2).length === q.answers.length;
        });
        setAllGood(allGood);
    }, [ questions ]);
    
    const generateQuestionsComponents = questions.map((q, idx) => (
        <QuestionsContext.Provider key={idx} value={{
            q, setQuestions, onSetQuestionAnswer, onChangeCorrectAnswer, onSetQuestionTitle, onRemoveQuestion,
            onSetMinutes, onSetSeconds
        }}>
            <QuizQuestionComponent/>
        </QuestionsContext.Provider>
    ));
    
    return (
        <>
            {isActive && <>
                {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between`} role="alert">
                    <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                    <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                </div>}
                {generateQuestionsComponents}
                <button onClick={onAddNewQuestion} className="btn btn-color-one w-100 mt-2">
                    Dodaj nowe pytanie
                </button>
                {allGood && <button className="btn btn-color-one mt-2 btn-light w-100" onClick={appendQuestionsTooQuiz}>
                    Zaktualizuj pytania quizu
                </button>}
            </>}
        </>
    );
};

ReactDOM
    .createRoot(document.getElementById('addQuizQuestionsRoot'))
    .render(<AddQuizQuestionsRoot/>);
