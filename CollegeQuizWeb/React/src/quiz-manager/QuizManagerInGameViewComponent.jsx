import { useContext, useRef, useEffect, useState } from "react";
import { copyToClipboard, InGameViewContext, SESS_TOKEN, SessionContext } from "../quiz-manager-renderer";
import { ANSWER_LETTERS } from "../utils/common";

import StartQuizButtonComponent from "./StartQuizButtonComponent";
import NextQuestionButtonComponent from "./NextQuestionButtonComponent";
import QuizManagerInGameTableComponent from "./QuizManagerInGameTableComponent";
import QuizManagerQrCodeModalComponent from "./QuizManagerQrCodeModalComponent";
import QuizManagerQuestionTickComponent from "./QuizManagerQuestionTickComponent";
import AnswersVisibilityButtonComponent from "./AnswersVisibilityButtonComponent";

const QuizManagerInGameViewComponent = () => {
    const {
        connection, lobbyData, allParticipants, setNextQuestionIsActive, resultTable, setResultTable, setRespondedUsers
    } = useContext(SessionContext);

    const [ questionType, setQuestionType ] = useState(1);
    const [ imageUrl, setImageUrl ] = useState('');
    const [ questionName, setQuestionName ] = useState('');
    const [ answers, setAnswers ] = useState([]);
    const [ currentQuestion, setCurrentQuestion ] = useState(1);
    
    const [ rangeData, setRangeData ] = React.useState({
        Step: 0, Min: 0, Max: 0, MinCounted: 0, MaxCounted: 0, CorrectAnswerRange: 0
    });
    
    useEffect(() => {
        // odbieranie pytań z koncentratora
        connection.on("QUESTION_P2P", answ => {
            const parsedAnswers = JSON.parse(answ);
            const { Step, Min, Max, MinCounted, MaxCounted, CorrectAnswerRange } = parsedAnswers;

            const newState = [ ...resultTable ].map(r => {
                r.Answer = "-";
                r.IsGood = "none";
                r.Points = r.Points.replace(/ *\([^)]*\) */g, "");
                return r;
            });
            setResultTable(newState);
            setRespondedUsers(0);
            
            setNextQuestionIsActive(false);
            setRangeData({ Step, Min, Max, MinCounted, MaxCounted, CorrectAnswerRange });
            setQuestionType(parsedAnswers.QuestionType);
            setImageUrl(parsedAnswers.ImageUrl);
            setQuestionName(parsedAnswers.QuestionName);
            setAnswers(parsedAnswers.Answers);
            setCurrentQuestion(parsedAnswers.QuestionId);
        });
        
        // użytkownik wysyłający odpowiedź (uruchamia się w momencie zaznaczenia przez niego odpowiedzi)
        connection.on("USER_SELECT_ANSWER_P2P", answer => {
            const parsedAnswer = JSON.parse(answer);

            const stateCopy = [ ...resultTable ];
            const parcitipantIndex = stateCopy.findIndex(e => e.Username === parsedAnswer.Username);
            if (!parcitipantIndex === -1) return;
            
            stateCopy[parcitipantIndex].Answer = parsedAnswer.SelectedAnswer;
            setResultTable(stateCopy);
            setRespondedUsers(prevState => prevState + 1);
        });
        
        // wykona się gdy serwer obliczy punkty po każdej turze
        connection.on("COMPUTE_ALL_POINTS_P2P", usersPoints => {
            const parsedUserPoints = JSON.parse(usersPoints);
            
            const stateCopy = [ ...resultTable ];
            const newState = stateCopy.map(u => {
                const user = parsedUserPoints.find(usr => usr.Username === u.Username);
                if (!user) return u;
                u.Points = user.Points;
                u.IsGood = user.IsGood;
                return u;
            });
            setResultTable(newState);
        });
    }, []);
    
    return (
        <>
            <QuizManagerQuestionTickComponent/>
            <div className="row mb-2">
                <div className="col-lg-3 px-1">
                    <div className="mb-2">
                        <StartQuizButtonComponent/>
                    </div>
                    <NextQuestionButtonComponent/>
                </div>
                <div className="col-lg-6 px-1 px-lg-2 py-2 py-lg-0">
                    <div className="card trsp px-3 py-3">
                        <div className="row">
                            <div className="col-md-5 mb-1 d-flex flex-column justify-content-center">
                                <h6 className="text-black-50 mb-0 text-center text-lg-start">
                                    Nazwa quizu:
                                    <span className="quiz-color-text fw-bold ms-2">{lobbyData.name}</span>
                                </h6>
                                <h6 className="text-black-50 mb-0 text-center text-lg-start">
                                    Pytanie:
                                    <span className="quiz-color-text fw-bold ms-2">
                                        {currentQuestion}/{lobbyData.questionsCount}
                                    </span>
                                </h6>
                            </div>
                            <QuizManagerQrCodeModalComponent/>
                            <div className="col-md-5 mb-1 d-flex flex-column justify-content-center">
                                <h6 className="text-black-50 mb-0 text-center text-lg-end">
                                    Host:
                                    <span className="quiz-color-text fw-bold ms-2">{lobbyData.host}</span>
                                </h6>
                                <h6 className="text-black-50 mb-0 text-center text-lg-end">
                                    Liczba graczy:
                                    <span className="quiz-color-text fw-bold ms-2">
                                        {allParticipants.Connected.length}
                                    </span>
                                </h6>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-lg-3 px-1">
                    <div className="hstack gap-2 mb-2">
                        <span className="form-control text-center" style={{ fontWeight: 'bolder' }}>
                            {SESS_TOKEN}
                        </span>
                        <a className="btn btn-dark text-white" type="button" data-bs-toggle="tooltip"
                            data-bs-placement="left" data-bs-title="Kopiuj do schowka" onClick={copyToClipboard}>
                            <i className="bi bi-clipboard"></i>
                        </a>
                    </div>
                    <button className="btn btn-danger text-white mb-2 w-100 btn-std" onClick={() => window.location.reload()}>
                        Zakończ quiz
                    </button>
                </div>
            </div>
            <InGameViewContext.Provider value={{
                imageUrl, questionName, rangeData, questionType, answers, respondedUsers
            }}>
                <QuizManagerInGameTableComponent/>
            </InGameViewContext.Provider>
        </>
    );
};

export default QuizManagerInGameViewComponent;