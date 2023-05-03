import { SESS_TOKEN, SessionContext } from "./QuizManagerRenderer.jsx";
import { alertDanger, getCommonFetchObj } from "../Utils.jsx";

const RemoveUserFromSessionButtonComponent = ({ name }) => {
    const { setAlert } = React.useContext(SessionContext);
    
    const onRemoveUser = () => {
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
        <button className="border-0 bg-transparent" onClick={onRemoveUser}
                title="Usuwanie użytkownika z sesji wraz z usunięciem wszystkich zdobytych przez niego punktów">
            <i className="bi bi-x-circle-fill text-danger"></i>
        </button>
    );
};

export default RemoveUserFromSessionButtonComponent;