import { useContext } from "react";
import { MainContext, QuestionsContext } from "../quiz-questions-renderer";

const QuizQuestionsSingleGoodAnswerComponent = ({ id, answer, isMultipleAnswers }) => {
    const { questions, setQuestions } = useContext(MainContext);
    const { q } = useContext(QuestionsContext);

    const questionId = `question_${id}_group_${q.id}`;
    const groupId = `question_group_${q.id}`;

    const onCorrectMultipleAnswers = e => onChangeCorrectAnswer(e.target.checked);
    const onCorrentSingleAnswer = () => onChangeCorrectAnswer(true);

    const onChangeCorrectAnswer = checked => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qstData => qstData.id === q.id);
        if (!isMultipleAnswers) {
            qst[idx].answers.forEach(a => a.isCorrect = false);
        }
        const answIdx = qst[idx].answers.findIndex(a => a.id === answer.id);
        qst[idx].answers[answIdx].isCorrect = checked;
        setQuestions(qst);
    };
    
    const onSetQuestionAnswer = e => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qstData => qstData.id === q.id);
        const answIdx = qst[idx].answers.findIndex(a => a.id === answer.id);
        qst[idx].answers[answIdx].text = e.target.value;
        setQuestions(qst);
    };
    
    return (
        <div className="col-6">
            <div className="p-3 card">
                <div className="p-3 card hstack">
                    <div className="me-2 fs-4">{answer.id}</div>
                    <input type="text" className="form-control" placeholder={`Treść odpowiedzi ${id}`}
                        onChange={onSetQuestionAnswer} value={answer.text}/>
                </div>
                <div className="form-check mt-2">
                    {isMultipleAnswers ? <>
                        <input type="checkbox" className="form-check-input" id={questionId} name={groupId}
                            checked={answer.isCorrect} onChange={onCorrectMultipleAnswers}/>
                    </> : <>
                        <input type="radio" className="form-check-input form-check-input-radio" id={questionId} name={groupId}
                            checked={answer.isCorrect} onChange={onCorrentSingleAnswer}/>
                    </>}
                    <label htmlFor={questionId} className="form-check-label">
                        To jest poprawna odpowiedź
                    </label>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionsSingleGoodAnswerComponent;