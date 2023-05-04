import { useContext, useEffect } from "react";
import { SessionContext } from "../quiz-manager-renderer";

const QuizManagerQuestionTickComponent = () => {
    const {
        connection, setNextQuestionBtnText, progressWidth, setProgressWidth, setTick
    } = useContext(SessionContext);
    
    useEffect(() => {
        // uruchamiane przy odliczaniu do następnego pytania
        connection.on("QUESTION_TIMER_P2P", tickObject => {
            const parsedTickObject = JSON.parse(tickObject);
            
            setTick(parsedTickObject.Elapsed);
            setNextQuestionBtnText(`Do następnego pytania: ${parsedTickObject.Elapsed}...`);
            
            setProgressWidth((parsedTickObject.Elapsed / parsedTickObject.Total) * 100);
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