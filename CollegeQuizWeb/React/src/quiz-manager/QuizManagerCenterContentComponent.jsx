import { useContext } from "react";
import { SESS_TOKEN, QR_CODE_BLOB, copyToClipboard, SessionContext } from "../quiz-manager-renderer";

import QuizPlayerViewResultComponent from "./QuizPlayerViewResultComponent.jsx";

const QuizManagerCenterContentComponent = () => {
    const { lobbyData, counting, countingActive, isEnded } = useContext(SessionContext);
    
    return (
        <div className="col-lg-6 px-0 px-lg-2 mb-2 mb-lg-0 order-lg-0 order-2">
            {!isEnded && <div className="row">
                <div className="col-9 pe-1">
                    <span className="form-control text-center" style={{fontSize: 38, fontWeight: 'bolder'}}>
                        {SESS_TOKEN}
                    </span>
                </div>
                <div className="col-3 ps-1">
                    <a className="btn btn-lg btn-dark w-100 h-100 text-white" type="button" data-bs-toggle="tooltip"
                        data-bs-placement="left" data-bs-title="Kopiuj do schowka" onClick={copyToClipboard}>
                        <i className="bi bi-clipboard"></i>
                    </a>
                </div>
            </div>}
            <div className="d-flex justify-content-center align-items-center mt-2">
                <div className="card px-3 py-3 w-100 h-100" style={{ minHeight: 608 }}>
                    {isEnded ? <QuizPlayerViewResultComponent/> : countingActive ? <>
                        <p className="mt-3 fs-4 text-prim-color text-center" style={{ paddingTop: 150 }}>
                            Uwaga! Twój quiz "<strong>{lobbyData.name}</strong>" uruchamia się za:
                        </p>
                        <h2 className="fw-bold fs-1 text-prim-color text-center">{counting}</h2>
                    </> : <img src={`data:image/gif;base64,${QR_CODE_BLOB}`} alt=""/>}
                </div>
            </div>
        </div>
    );
};

export default QuizManagerCenterContentComponent;