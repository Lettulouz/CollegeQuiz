import { InGameViewContext } from "./QuizManagerInGameViewComponent.jsx";
import { SessionContext } from "./QuizManagerRenderer.jsx";
import { ANSWER_LETTERS } from "../Utils.jsx";

import QuizPlayerViewResultComponent from "./QuizPlayerViewResultComponent.jsx";
import RemoveUserFromSessionButtonComponent from "./RemoveUserFromSessionButtonComponent.jsx";
import QuizPlayerViewRangeAnswerTypeComponent from "./QuizPlayerViewRangeAnswerTypeComponent.jsx";
import QuizPlayerViewUniversalAnswerTypeComponent from "./QuizPlayerViewUniversalAnswerTypeComponent.jsx";
import QuizPlayerViewTrueFalseAnswerTypeComponent from "./QuizPlayerViewTrueFalseAnswerTypeComponent.jsx";
import QuizPlayerViewUniversalQuestionTypeComponent from "./QuizPlayerViewUniversalQuestionTypeComponent.jsx";

const QuizManagerInGameTableComponent = () => {
    const { nextQuestionIsActive, allParticipants, resultTable } = React.useContext(SessionContext);
    const { questionType, answers, respondedUsers } = React.useContext(InGameViewContext);
    
    const answerColor = r => r.IsGood !== "none" ? r.IsGood === "true" ? 'text-success' : 'text-danger' : '';
    
    const renderQuestionTypeSection = () => {
        switch (questionType) {
            case 5: return <QuizPlayerViewRangeAnswerTypeComponent/>;
            case 2: return (
                <QuizPlayerViewUniversalQuestionTypeComponent>
                    {answers.map((answer, idx) => (
                        <QuizPlayerViewTrueFalseAnswerTypeComponent key={idx} number={idx} answer={answer}/>
                    ))}
                </QuizPlayerViewUniversalQuestionTypeComponent>
            );
            default: return  (
                <QuizPlayerViewUniversalQuestionTypeComponent>
                    {answers.map((answer, idx) => (
                        <QuizPlayerViewUniversalAnswerTypeComponent key={answer.Text} number={idx} answer={answer}/>
                    ))}
                </QuizPlayerViewUniversalQuestionTypeComponent>
            );
        }
    };
    
    const generateAnswer = r => {
        if (r.Answer === "-") return r.Answer;
        switch (questionType) {
            case 1: case 3: case 4: return ANSWER_LETTERS[Number(r.Answer)];        // 4 odpowiedzi, 6 odpowidzi
            case 2:                 return r.Answer === "0" ? "P" : "F";            // prawda/fałsz
            default:                return r.Answer;                                // zakresowe
        }
    };
    
    return (
        <div className="row" style={{ minHeight: 608 }}>
            <div className="col-lg-8 px-1 mb-2">
                <div className="card trsp p-3 h-100">
                    <h4 className="mb-2 quiz-color-text">Widok gracza</h4>
                    {nextQuestionIsActive ? <QuizPlayerViewResultComponent/> : renderQuestionTypeSection()}
                </div>
            </div>
            <div className="col-lg-4 px-1 mb-2">
                <div className="card trsp p-3 h-100">
                    <h4 className="mb-2 quiz-color-text">Tabela wyników</h4>
                    <p className="my-1">
                        Udzieliło odpowiedzi
                        <strong className="mx-2">{respondedUsers}/{allParticipants.Connected.length}</strong>
                        graczy
                    </p>
                    <table className="table">
                        <thead>
                        <tr>
                            <th scope="col">Nick</th>
                            <th scope="col">Odp</th>
                            <th scope="col">Punkty</th>
                            <th scope="col">Akcja</th>
                        </tr>
                        </thead>
                        <tbody>
                        {resultTable.map(r => (
                            <tr key={r.Username}>
                                <td>{r.Username}</td>
                                <td className={`fw-bold ${answerColor(r)}`}>{generateAnswer(r)}</td>
                                <td>{r.Points}</td>
                                <td><RemoveUserFromSessionButtonComponent name={r.Username}/></td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default QuizManagerInGameTableComponent;