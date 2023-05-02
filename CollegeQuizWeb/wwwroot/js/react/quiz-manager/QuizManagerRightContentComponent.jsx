import { alertInfo, alertOff } from "../Utils.jsx";
import { SessionContext, SESS_TOKEN } from "./QuizManagerRenderer.jsx";

const QuizManagerRightContentComponent = () => {
    const {
        connection, setAlert, allParticipants, nextQuestionIsActive, startBtnText, isEnded, setStartBtnText,
        nextQuestionBtnText
    } = React.useContext(SessionContext);
    
    const [ counting, setCounting ] = React.useState(5);
    const [ isStartClicked, setIsStartClicked ] = React.useState(false);

    const startQuiz = () => {
        if (counting === 0 || isStartClicked || allParticipants.Connected.length === 0) return;
        setIsStartClicked(true);
        
        let i = counting;
        const interval = setInterval(() => {
            connection.invoke('INIT_GAME_SEQUENCER_P2P', i, SESS_TOKEN).then(r => setCounting(r));
            setStartBtnText(`Zaczyna się za ${i}...`);
            if (i === 0) {
                setStartBtnText('Aktywny');
                setAlert(alertInfo('GRA WŁAŚNIE SIĘ ROZPOCZĘŁA!'));
                setTimeout(() => setAlert(alertOff()), 3000);
                clearInterval(interval);
                connection.invoke('START_GAME_P2P', SESS_TOKEN).then(_ => _);
            }
            --i;
        }, 1000);
    };
    
    const sendSignal = () => {
        if (!nextQuestionIsActive) return;
        connection.invoke('START_GAME_P2P', SESS_TOKEN);
    };
    
    return (
        <div className="col-lg-3 px-0 mb-2 mb-lg-0 order-lg-0 order-2">
            <div className="card px-3 py-3 h-100 d-flex flex-column justify-content-between">
                <div className="w-100">
                    <button className="btn btn-color-second-short mt-2 w-100 btn-std" onClick={startQuiz}
                        disabled={allParticipants.Connected.length === 0 || counting === 0 || isEnded}>
                        {startBtnText}
                    </button>
                    <button className="btn btn-color-one-short text-white mt-2 w-100 btn-std" onClick={() => sendSignal()}
                            disabled={!nextQuestionIsActive || isEnded}>
                        {nextQuestionBtnText}
                    </button>
                </div>
                <div className="w-100">
                    <button className="btn btn-danger mt-2 text-white w-100" onClick={() => window.location.reload()}>
                        Zakończ quiz
                    </button>
                </div>
            </div>
        </div>
    );
};

export default QuizManagerRightContentComponent;