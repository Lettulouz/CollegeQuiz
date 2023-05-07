import { useRef, useEffect, useContext } from "react";
import { alertDanger, alertInfo, getCommonFetchObj, WAITING_SCREEN } from "../utils/common";
import { SessionContext } from "../quiz-session-renderer";

const QuizQuestionSessionIcon = ({ text }) => {
    const {
        questionType
    } = useContext(SessionContext);
    
    return (
        <>
            {questionType === 3 &&
                <i className="bi bi-collection session-icon"></i>
            }
            {(questionType === 1 || questionType === 4) &&
                <i className="bi bi-1-square session-icon"></i>
            }
        </>
    );
};

export default QuizQuestionSessionIcon;