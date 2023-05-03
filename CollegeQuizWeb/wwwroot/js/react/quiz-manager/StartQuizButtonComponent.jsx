import { SESS_TOKEN, SessionContext } from "./QuizManagerRenderer.jsx";
import { alertInfo, alertOff } from "../Utils.jsx";

const StartQuizButtonComponent = () => {
    const {
        connection, setAlert, allParticipants, startBtnText, isEnded, setStartBtnText, setInGameViewActive,
        inGameViewActive, counting, setCounting, setCountingActive
    } = React.useContext(SessionContext);
    
    const [ isStartClicked, setIsStartClicked ] = React.useState(false);
    
    const startQuiz = () => {
        if (counting === 0 || isStartClicked || allParticipants.Connected.length === 0) return;
        
        setIsStartClicked(true);
        
        let i = counting;
        const interval = setInterval(() => {
            connection.invoke('INIT_GAME_SEQUENCER_P2P', i, SESS_TOKEN).then(r => setCounting(r));
            setStartBtnText(`Zaczyna się za ${i}...`);
            setCountingActive(true);
            if (i === 0) {
                setStartBtnText('Aktywny');
                setCountingActive(false);
                setInGameViewActive(true);
                setAlert(alertInfo('GRA WŁAŚNIE SIĘ ROZPOCZĘŁA!'));
                setTimeout(() => setAlert(alertOff()), 3000);
                clearInterval(interval);
                connection.invoke('START_GAME_P2P', SESS_TOKEN).then(_ => _);
            }
            --i;
        }, 1000);
    };
    
    return (
        <button className="btn btn-color-second-short w-100 btn-std" onClick={startQuiz}
                disabled={allParticipants.Connected.length === 0 || counting === 0 
                    || isEnded || isStartClicked || inGameViewActive}>
            {startBtnText}
        </button>
    );
};

export default StartQuizButtonComponent;