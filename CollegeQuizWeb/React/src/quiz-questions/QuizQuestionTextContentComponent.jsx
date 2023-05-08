import { useContext, useEffect } from "react";
import { MainContext, QuestionsContext } from "../quiz-questions-renderer";

const QuizQuestionTextContentComponent = () => {
    const { q } = useContext(QuestionsContext);
    const { setQuestions, onSetQuestionProperty, questions, uploadedImages } = useContext(MainContext);

    const setTimeMin = e => onSetQuestionProperty(q.id, "timeMin", e.target.value);
    const setTimeSec = e => onSetQuestionProperty(q.id, "timeSec", e.target.value);
    
    const isUploaded = !!uploadedImages.find(i => i.id === q.id);
    
    const onRemoveQuestion = () => {
        if (isUploaded) return;
        let copyQuestions = [ ...questions ];
        copyQuestions = copyQuestions
            .filter(qst => qst.id !== q.id)
            .map((d, idx) => ({ ...d, id: idx + 1 }));
        setQuestions(copyQuestions);
    };
    
    useEffect(() => {
        if (q.timeMin.length > 3) onSetQuestionProperty(q.id, "timeMin", q.timeMin.slice(0, 3));
    }, [ q.timeMin ]);

    useEffect(() => {
        if (q.timeSec.length > 2) onSetQuestionProperty(q.id, "timeSec",  q.timeSec.slice(0, 2));
        if (Number(q.timeSec) > 59) onSetQuestionProperty(q.id, "timeSec", "59");
    }, [ q.timeSec ]);
    
    return (
        <div className="hstack gap-2">
            <div className="me-2 fs-3">{q.id}</div>
            <textarea className="form-control h-100" placeholder="Treść pytania" value={q.text}
                onChange={e => onSetQuestionProperty(q.id, "text", e.target.value)}></textarea>
            <div>
                <label className="mb-1">Czas trwania pytania:</label>
                <div className="d-flex">
                    <input type="number" className="form-control time-control" placeholder="min" min="0" max="999"
                        maxLength="3" value={q.timeMin} onChange={setTimeMin}/>
                    <span className="mx-2 fw-bold pt-1">:</span>
                    <input type="number" className="form-control time-control" placeholder="sek" min="0" max="59"
                        maxLength="2" value={q.timeSec} onChange={setTimeSec}/>
                </div>
            </div>
            <button className={`btn btn-danger text-white ms-2 ${isUploaded && 'disabled'}`}
                onClick={onRemoveQuestion}>X</button>
        </div>
    );
};

export default QuizQuestionTextContentComponent;