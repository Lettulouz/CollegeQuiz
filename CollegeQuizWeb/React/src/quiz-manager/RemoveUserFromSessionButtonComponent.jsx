import { useContext } from "react";
import { SESS_TOKEN, SessionContext } from "../quiz-manager-renderer";
import { alertDanger, getCommonFetchObj } from "../utils/common";

const RemoveUserFromSessionButtonComponent = ({ name }) => {
    const { setAlert, countingActive, setRespondedUsers } = useContext(SessionContext);
    
    const onRemoveUser = () => {
        if (countingActive) return;
        fetch(
            `/api/v1/dotnet/QuizSessionAPI/RemoveFromSession/${SESS_TOKEN}/${name}`,
            getCommonFetchObj("POST")
        ).then(r => r)
            .then(_ => setRespondedUsers(prevState => prevState === 0 ? 0 : prevState - 1))
            .catch(_ => {
                setAlert(alertDanger('Wystąpił błąd podczas usuwania użytkownika z sesji.'));
            });
    };
    
    return (
        <button className="border-0 bg-transparent on-hover-darker" onClick={onRemoveUser}
            title="Usuwanie użytkownika z sesji" disabled={countingActive}>
            <i className="bi bi-x-circle-fill text-danger fs-5"></i>
        </button>
    );
};

export default RemoveUserFromSessionButtonComponent;