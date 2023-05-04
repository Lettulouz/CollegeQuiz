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
            
            setTick(parsedTickObject.Remaining);
            setNextQuestionBtnText(`Do następnego pytania: ${parsedTickObject.Remaining}...`);
            
            setProgressWidth((parsedTickObject.Remaining / parsedTickObject.Total) * 100);
            playSound(parsedTickObject.Remaining);
        });
    }, []);

    const playSound = counter => {
        if(counter <= 5 && counter>0){
            var audio = new Audio("/sounds/counter/" + counter + ".mp4");
            audio.volume = 0.8
            audio.play();
        }
    }
    
    return (
        <div className="row px-1 mb-3">
            <div className="question-progress-bar w-100">
                <div className="question-progress-bar-inner" style={{ width: `${progressWidth}%` }}></div>
            </div>
        </div>
    );
};

export default QuizManagerQuestionTickComponent;