import { QR_CODE_BLOB, SESS_TOKEN } from "./QuizManagerRenderer.jsx";

const QuizManagerQrCodeModalComponent = () => {

    const modalRef = React.useRef();
    
    const showModal = () => new bootstrap.Modal(modalRef.current, { backdrop: 'static', keyboard: false }).show();
    const hideModal = () => bootstrap.Modal.getInstance(modalRef.current).hide();
    
    return (
        <>
            <div className="modal fade" id="confirm-leave-session-modal" tabIndex="-1" aria-hidden="false" ref={modalRef}>
                <div className="modal-dialog modal-lg">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5">OR kod ({SESS_TOKEN})</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" onClick={hideModal}></button>
                        </div>
                        <div className="modal-body fw-normal">
                            <img src={`data:image/gif;base64,${QR_CODE_BLOB}`} alt="" width="100%" height="auto"/>
                        </div>
                    </div>
                </div>
            </div>
            <div className="col-md-2 mb-1 d-flex justify-content-center align-items-center">
                <button className="background-primary-logo d-flex justify-content-center align-items-center"
                        title="Kliknij aby pokazać kod QR" onClick={showModal}>
                    <i className="bi bi-qr-code text-white fs-2"></i>
                </button>
            </div>
        </>
    );
};

export default QuizManagerQrCodeModalComponent;