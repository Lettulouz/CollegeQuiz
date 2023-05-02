import { SESS_TOKEN, QR_CODE_BLOB } from "./QuizManagerRenderer.jsx";

const QuizManagerCenterContentComponent = () => {

    const copyToClipboard = () => {
        navigator.clipboard.writeText(SESS_TOKEN);
        toastr.success(`Skopiowano kod ${SESS_TOKEN} do schowka.`);
    };

    return (
        <div className="col-lg-6 px-0 px-lg-2 mb-2 mb-lg-0 order-lg-0 order-3">
            <div className="d-flex justify-content-center">
                <div className="card px-3 py-3 h-100">
                    <img src={`data:image/gif;base64,${QR_CODE_BLOB}`} alt=""/>
                </div>
            </div>
            <div className="row mt-2">
                <div className="col-9">
                    <span className="form-control text-center" id="lobbyCode" style={{ fontSize: 38, fontWeight: 'bolder' }}>
                        {SESS_TOKEN}
                    </span>
                </div>
                <div className="col-3">
                    <a className="btn btn-lg btn-dark w-100 h-100 text-white" type="button" data-bs-toggle="tooltip"
                        data-bs-placement="left" data-bs-title="Kopiuj do schowka" onClick={copyToClipboard}>
                        <i className="bi bi-clipboard"></i>
                    </a>
                </div>
            </div>
        </div>
    );
};

export default QuizManagerCenterContentComponent;