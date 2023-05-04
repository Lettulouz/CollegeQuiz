import { useEffect } from "react";
import { getCropperConfig } from "../utils/common";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";

const QuizQuestionsImageManipulatorComponent = ({ modalRef, acceptChanges, refuseChanges, imageRef, imageSrc, cropperRef }) => {
    
    useEffect(() => {
        cropperRef.current = new Cropper(imageRef.current, getCropperConfig());
        return () => { cropperRef.current.destroy() };
    }, [ imageSrc ]);

    return (
        <div className="modal fade" tabIndex="-1" aria-hidden="false" ref={modalRef} data-bs-backdrop="static" role="dialog">
            <div className="modal-dialog modal-lg" role="document">
                <div className="modal-content">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5">Kadrowanie zdjęcia</h1>
                    </div>
                    <div className="modal-body fw-normal d-flex justify-content-center w-100" style={{ width: 600 }}>
                        <img ref={imageRef} src={imageSrc} alt="Original" />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn-color-one" data-bs-dismiss="modal" onClick={acceptChanges}>
                            Zatwierdź zmiany
                        </button>
                        <button type="button" className="btn-color-one bg-danger text-white" data-bs-dismiss="modal" onClick={refuseChanges}>
                            Odrzuć i usuń zdjęcie
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuizQuestionsImageManipulatorComponent;