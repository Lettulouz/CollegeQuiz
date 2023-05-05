import {useContext, useState} from "react";
import { SessionContext, InGameViewContext } from "../quiz-manager-renderer";
import { ANSWER_LETTERS } from "../utils/common";

import SessionParticipantsComponent from "./SessionParticipantsComponent";
import QuizPlayerViewResultComponent from "./QuizPlayerViewResultComponent";
import QuizPlayerViewRangeAnswerTypeComponent from "./QuizPlayerViewRangeAnswerTypeComponent";
import QuizPlayerViewUniversalAnswerTypeComponent from "./QuizPlayerViewUniversalAnswerTypeComponent";
import QuizPlayerViewTrueFalseAnswerTypeComponent from "./QuizPlayerViewTrueFalseAnswerTypeComponent";
import QuizPlayerViewUniversalQuestionTypeComponent from "./QuizPlayerViewUniversalQuestionTypeComponent";

const QuizManagerInGameTableComponent = () => {
    const { questionType, answers } = useContext(InGameViewContext);
    const {
        nextQuestionIsActive, allParticipants, resultTable, isAnswersVisible, respondedUsers, afterQuestionResults
    } = useContext(SessionContext);
    
    const [ showParticipants, setShowParticipants ] = useState(false);
    
    const answerColor = r => {
        if (isAnswersVisible) {
            return r.IsGood !== "none" ? r.IsGood === "true" ? 'text-success' : 'text-danger' : '';
        }
        return '';
    };
    
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
            case 1: case 4:         return ANSWER_LETTERS[Number(r.Answer)];        // 4 odpowiedzi, 6 odpowidzi
            case 2:                 return r.Answer === "0" ? "P" : "F";            // prawda/fałsz
            default:                return r.Answer;                                // zakresowe
        }
    };
    
    return (
        <div className="row" style={{ minHeight: 608 }}>
            <div className="col-lg-8 px-1 mb-2">
                <div className="card trsp p-3 h-100">
                    <h4 className="mb-2 quiz-color-text">Widok gracza</h4>
                    {nextQuestionIsActive ? afterQuestionResults.length > 0 
                        ? <QuizPlayerViewResultComponent/> : 
                        <>
                            <div className="alert alert-warning">
                                Brak aktywnych użytkowników sesji.
                            </div>
                        </> : renderQuestionTypeSection()}
                </div>
            </div>
            <div className="col-lg-4 px-1 mb-2">
                <div className="d-flex h-100">
                    <div className="card trsp p-3 h-100 flex-fill leaderboard-table-card scrollable-container">
                        {showParticipants ? <>
                            <h4 className="mb-2 quiz-color-text mb-3">Gracze w sesji</h4>
                            <SessionParticipantsComponent/>
                        </> : <>
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
                                </tr>
                                </thead>
                                <tbody>
                                {resultTable.map(r => (
                                    <tr key={r.Username}>
                                        <td>{r.Username}</td>
                                        <td className={`fw-bold ${answerColor(r)}`}>{generateAnswer(r)}</td>
                                        <td>{r.Points}</td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </>}
                    </div>
                    <div className="leaderboard-table-nav-container d-flex flex-column">
                        <button className={`border-0 flex-grow-1 position-relative leaderboard-toggle-button 
                            ${!showParticipants && 'active'}`} onClick={() => setShowParticipants(false)}>
                            <div className="leaderboard-table-nav-button-text fs-5">
                                Tabela wyników
                            </div>
                        </button>
                        <button className={`border-0 flex-grow-1 position-relative leaderboard-toggle-button 
                            ${showParticipants && 'active'}`} onClick={() => setShowParticipants(true)}>
                            <div className="leaderboard-table-nav-button-text fs-5">
                                Gracze w sesji
                            </div>
                        </button>
                    </div>
                </div>
                
            </div>
        </div>
    );
};

export default QuizManagerInGameTableComponent;