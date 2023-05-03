import { MainContext, QuestionsContext } from "./QuizQuestionsRenderer.jsx";

const QuizQuestionsTrueFalseAnswerComponent = ({ id, answer }) => {
    const { questions, setQuestions } = React.useContext(MainContext);
    const { q } = React.useContext(QuestionsContext);

    const questionId = `question_${id}_group_${q.id}`;
    const groupId = `question_group_${q.id}`;
    
    const onChangeCorrectAnswer = () => {
        const qst = [ ...questions ];
        
        const idx = qst.findIndex(qstData => qstData.id === q.id);
        qst[idx].answers.forEach(a => a.isCorrect = false);
        
        const answIdx = qst[idx].answers.findIndex(a => a.id === answer.id);
        qst[idx].answers[answIdx].isCorrect = true;
        setQuestions(qst);
    };
    
    return (
        <div className="col-6">
            <div className="p-3 card">
                <div className="form-check">
                    <input type="radio" className="form-check-input form-check-input-radio" id={questionId} name={groupId}
                        checked={answer.isCorrect} onChange={onChangeCorrectAnswer}/>
                    <label htmlFor={questionId} className="form-check-label">
                        {id === 1 ? "Prawda" : "Fałsz"}
                    </label>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionsTrueFalseAnswerComponent;