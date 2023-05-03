import { SESS_TOKEN, SessionContext } from "./QuizManagerRenderer.jsx";

const NextQuestionButtonComponent = () => {
    const { connection, nextQuestionIsActive, isEnded, nextQuestionBtnText, allParticipants } = React.useContext(SessionContext);
    
    const sendSignal = () => {
        if (!nextQuestionIsActive) return;
        connection.invoke('START_GAME_P2P', SESS_TOKEN);
    };
    
    return (
        <button className="btn btn-color-one-short text-white w-100 btn-std" onClick={() => sendSignal()}
                disabled={!nextQuestionIsActive || isEnded || allParticipants.Connected.length === 0}>
            {nextQuestionBtnText}
        </button>
    );
};

export default NextQuestionButtonComponent;