import { alertDanger, alertInfo, getCommonFetchObj, WAITING_SCREEN } from "../Utils.jsx";
import { SessionContext } from "./QuizSessionRenderer.jsx";

const LeaveQuizSessionButtonComponent = ({ text }) => {
    const {
        token, connectionId, setIsConnect, setAlert, setScreenAction, isLeaveClicked, setIsLeaveClicked, setIsJoinClicked,
        connection, setToken
    } = React.useContext(SessionContext);
    
    const modalRef = React.useRef();

    const leaveRoom = () => {
        if (isLeaveClicked) return;
        setIsLeaveClicked(true);
        fetch(`/api/v1/dotnet/QuizSessionAPI/LeaveRoom/${connectionId}/${token.toUpperCase()}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setIsConnect(false);
                    setScreenAction(WAITING_SCREEN);
                    setIsJoinClicked(false);
                    setIsLeaveClicked(false);
                    setAlert(alertInfo(r.message));
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };

    const showModal = () => new bootstrap.Modal(modalRef.current, { backdrop: 'static', keyboard: false }).show();
    const hideModal = () => {
        setToken("");
        bootstrap.Modal.getInstance(modalRef.current).hide();
    };

    React.useEffect(() => {
        connection.on("OnDisconnectedSession", _ => hideModal());
    }, []);

    return (
        <>
            <div className="modal fade" id="confirm-leave-session-modal" tabIndex="-1" aria-hidden="false" ref={modalRef}>
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5">Opuszczenie aktywnej sesji</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div className="modal-body fw-normal">
                            Czy na pewno chcesz opuścić aktywną sesję? Jeśli sesja się jeszcze nie skończy, będziesz
                            mógł/mogła do niej ponownie dołączyć.
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn-color-one bg-danger text-white" data-bs-dismiss="modal"
                                onClick={leaveRoom}>
                                Opuść sesję
                            </button>
                            <button type="button" className="btn-color-one" data-bs-dismiss="modal" onClick={hideModal}>
                                Zamknij okno
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <button className="btn bg-danger text-white rounded mx-auto w-100 danger-button-fixed" onClick={showModal}>
                {text}
            </button>
        </>
    );
};

export default LeaveQuizSessionButtonComponent;