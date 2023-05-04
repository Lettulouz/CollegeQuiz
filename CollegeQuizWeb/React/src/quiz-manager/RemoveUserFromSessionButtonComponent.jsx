import { useContext } from "react";
import { SESS_TOKEN, SessionContext } from "../quiz-manager-renderer";
import { alertDanger, getCommonFetchObj } from "../utils/common";

const RemoveUserFromSessionButtonComponent = ({ name }) => {
    const { setAlert, countingActive } = useContext(SessionContext);
    
    const onRemoveUser = () => {
        if (countingActive) return;
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/RemoveFromSession/${SESS_TOKEN}/${name}`,
            getCommonFetchObj("POST")
        ).then(r => r)
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił błąd podczas usuwania użytkownika z sesji.'));
            });
    };
    
    return (
        <button className="border-0 bg-transparent on-hover-darker" onClick={onRemoveUser}
            title="Usuwanie użytkownika z sesji wraz z usunięciem wszystkich zdobytych przez niego punktów"
            disabled={countingActive}>
            <i className="bi bi-x-circle-fill text-danger fs-5"></i>
        </button>
    );
};

export default RemoveUserFromSessionButtonComponent;