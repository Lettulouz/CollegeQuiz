import { SessionContext } from "./QuizManagerRenderer.jsx";

const QuizManagerQuestionTickComponent = () => {
    const {
        connection, setNextQuestionBtnText, progressWidth, setProgressWidth, setTick
    } = React.useContext(SessionContext);
    
    React.useEffect(() => {
        // uruchamiane przy odliczaniu do następnego pytania
        connection.on("LOBBY_QUESTION_TICK_P2P", tickObject => {
            const parsedTickObject = JSON.parse(tickObject);
            
            setTick(parsedTickObject.Elapsed);
            setNextQuestionBtnText(`Do następnego pytania: ${parsedTickObject.Elapsed}...`);
            
            setProgressWidth((parsedTickObject.Elapsed / parsedTickObject.Total) * 100);
        });
    }, []);
    
    return (
        <div className="row w-100 mx-0 mb-3">
            <div className="question-progress-bar w-100">
                <div className="question-progress-bar-inner" style={{ width: `${progressWidth}%` }}></div>
            </div>
        </div>
    );
};

export default QuizManagerQuestionTickComponent;