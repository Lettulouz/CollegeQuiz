import { useContext } from "react";
import { SESS_TOKEN, SessionContext } from "../quiz-manager-renderer";
import { alertDanger, generateErrorMessage, getCommonFetchObj } from "../utils/common";

const UnbanUserFromSessionButtonComponent = ({ name }) => {
    const { setAlert, countingActive } = useContext(SessionContext);
    
    const unbanUserFromSession = () => {
        if (countingActive) return;
        
        fetch(`/api/v1/dotnet/QuizSessionAPI/UnbanFromSession/${SESS_TOKEN}/${name}`, getCommonFetchObj("POST"))
            .then(r => {
                if (r.ok) {
                    return r.json();
                }
                throw new Error(r.status);
            })
            .then(({ isGood, message }) => {
                if (isGood) {
                    new RetroNotify({
                        contentHeader: 'Informacja',
                        contentText: message,
                        style: 'black',
                        animate: 'slideTopRight'
                    })
                } else {
                    setAlert(alertDanger(message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger(generateErrorMessage(e.message)));
            });
    };
    
    return (
        <button onClick={unbanUserFromSession} disabled={countingActive}
            className="border-0 bg-transparent on-hover-good-darker" title="Odbanowywanie użytkownika z sesji">
            <i className="bi bi-arrow-counterclockwise quiz-color-text fs-5"></i>
        </button>
    );
};

export default UnbanUserFromSessionButtonComponent;