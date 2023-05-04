import { useContext, useRef } from "react";
import { SessionContext } from "../quiz-manager-renderer";

const AnswersVisibilityButtonComponent = () => {
    const { isAnswersVisible, setIsAnswersVisible } = useContext(SessionContext);
    const modalRef = useRef();

    const onChangeVisibility = () => setIsAnswersVisible(prevState => !prevState);
    const btnColor = () => isAnswersVisible ? 'btn-color-second-short' : 'btn-color-second-disabled'

    const showModal = () => new bootstrap.Modal(modalRef.current, { backdrop: 'static', keyboard: false }).show();
    const hideModal = () => bootstrap.Modal.getInstance(modalRef.current).hide();
    
    return (
        <>
            <div className="modal fade" tabIndex="-1" aria-hidden="false" ref={modalRef}>
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5">Pokazanie odpowiedzi</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div className="modal-body fw-normal">
                            Czy na pewno chcesz pokazać odpowiedzi? Opcje tą stosuj jedynie wtedy, gdy uczestnicy quizu
                            nie widzą Twojego panelu zarządzania quizem.
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn-color-one bg-danger text-white" data-bs-dismiss="modal"
                                    onClick={onChangeVisibility}>
                                Pokaż odpowiedzi
                            </button>
                            <button type="button" className="btn-color-one" data-bs-dismiss="modal" onClick={hideModal}>
                                Zamknij okno
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <button className={`btn ${btnColor()} w-100 btn-std d-flex align-items-center`}
                onClick={isAnswersVisible ? onChangeVisibility : showModal}>
                <div className="form-check ps-0 position-relative form-switch mb-0 d-flex justify-content-center w-100">
                    <input className="form-check-input position-absolute" type="checkbox" role="switch"
                        onChange={isAnswersVisible ? onChangeVisibility : showModal} checked={isAnswersVisible}
                        style={{ left: 40, cursor: 'pointer' }}/>
                    <label className="form-check-label" htmlFor="flexSwitchCheckDefault" style={{ cursor: 'pointer' }}>
                        Odpowiedzi {isAnswersVisible ? 'widoczne' : 'niewidoczne'}
                    </label>
                </div>
            </button>
        </>
    );
};

export default AnswersVisibilityButtonComponent;