import { useContext, useEffect } from "react";
import { SessionContext } from "../quiz-manager-renderer";
import { convertSecondsToTime, playSound } from "../utils/common";

const QuizManagerQuestionTickComponent = () => {
    const {
        connection, setNextQuestionBtnText, progressWidth, setProgressWidth, setTick
    } = useContext(SessionContext);
    
    useEffect(() => {
        // uruchamiane przy odliczaniu do następnego pytania
        connection.on("QUESTION_TIMER_P2P", tickObject => {
            const parsedTickObject = JSON.parse(tickObject);
            
            setTick(parsedTickObject.Remaining);
            setNextQuestionBtnText(`Do następnego pytania: ${convertSecondsToTime(parsedTickObject.Remaining)}...`);
            
            setProgressWidth((parsedTickObject.Remaining / parsedTickObject.Total) * 100);
            playSound(parsedTickObject.Remaining);
        });
    }, []);
    
    return (
        <div className="row px-1 mb-3">
            <div className="question-progress-bar w-100">
                <div className="question-progress-bar-inner" style={{ width: `${progressWidth}%` }}></div>
            </div>
        </div>
    );
};

export default QuizManagerQuestionTickComponent;