import { useContext, useState } from "react";
import { SESS_TOKEN, SessionContext } from "../quiz-manager-renderer";
import { alertInfo, alertOff } from "../utils/common";

const StartQuizButtonComponent = () => {
    const {
        connection, setAlert, allParticipants, startBtnText, isEnded, setStartBtnText, setInGameViewActive,
        inGameViewActive, counting, setCounting, setCountingActive
    } = useContext(SessionContext);
    
    const [ isStartClicked, setIsStartClicked ] = useState(false);
    
    const startQuiz = () => {
        if (counting === 0 || isStartClicked || allParticipants.Connected.length === 0) return;
        
        setIsStartClicked(true);
        
        let i = counting;
        const interval = setInterval(() => {
            connection.invoke('INIT_GAME_SEQUENCER_P2P', i, SESS_TOKEN).then(r => setCounting(r));
            setStartBtnText(`Zaczyna się za ${i}...`);
            playSound(i);
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

    const playSound = counter => {
        if(counter <= 5 && counter>0){
            var audio = new Audio("/sounds/counter/" + counter + ".mp4");
            audio.volume = 0.8
            audio.play();
        }
    }
    
    return (
        <button className="btn btn-color-second-short w-100 btn-std" onClick={startQuiz}
                disabled={allParticipants.Connected.length === 0 || counting === 0 
                    || isEnded || isStartClicked || inGameViewActive}>
            {startBtnText}
        </button>
    );
};

export default StartQuizButtonComponent;