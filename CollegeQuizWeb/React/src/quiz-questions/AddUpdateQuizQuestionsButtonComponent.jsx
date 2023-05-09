import { useContext, useState } from "react";
import { id, MainContext } from "../quiz-questions-renderer";
import {
    alertDanger, alertInfo, generateErrorMessage, getCommonFetchObjWithBody, getCommonFetchObjWithFormData
} from "../utils/common";

const AddUpdateQuizQuestionsButtonComponent = () => {
    const {
        questions, setQuestions, setInactiveCallback, setActiveCallback, setAlert, allGood
    } = useContext(MainContext);

    const [ isSended, setIsSended ] = useState(false);

    const appendQuizImages = () => {
        const formData = new FormData();
        const onlyBlobs = questions.map(e => ({ id: e.id, blobImage: e.blobImage }));
        for (const uploadedImage of onlyBlobs) {
            if (!uploadedImage.blobImage) continue;
            const type = uploadedImage.blobImage.type.substring(uploadedImage.blobImage.type.indexOf('/') + 1);
            formData.append("uploads", uploadedImage.blobImage, `question${uploadedImage.id}.${type}`);
        }
        fetch(`/api/v1/dotnet/QuizAPI/UpdateQuizImages/${id}`, getCommonFetchObjWithFormData('POST', formData))
            .then(r => {
                if (r.ok) {
                    setIsSended(false);
                    window.scrollTo(0, 0);
                    return r.json();
                }
                throw new Error(r.status);
            })
            .then(r => {
                if (r.isGood) {
                    setAlert(alertInfo(r.message));
                    const qst = [ ...questions ];
                    const updatedImages = qst.map(qst2 => {
                        const quizImage = r.quizImages.find(i => i.id === qst2.id);
                        qst2.imageUrl = quizImage ? quizImage.url : "";
                        return qst2;
                    });
                    setQuestions(updatedImages);
                } else {
                    setAlert(alertDanger(r.message));
                }
                setActiveCallback();
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger(generateErrorMessage(e.message)));
                setActiveCallback();
            });
    };
    
    const appendQuestionsTooQuiz = () => {
        setInactiveCallback();
        if (isSended) return;
        setIsSended(true);
        const questionsWithoutBlob = questions.map(({ id, text, timeMin, timeSec, imageUrl, type, answers }) => ({
            id, text, timeMin, timeSec, imageUrl, type, answers
        }));
        fetch(`/api/v1/dotnet/QuizAPI/AddQuizQuestions/${id}`, getCommonFetchObjWithBody('POST', { aggregate: questionsWithoutBlob }))
            .then(r => {
                if (r.ok) {
                    setIsSended(false);
                    return r.json();
                }
                throw new Error(r.status);
            })
            .then(r => {
                if (r.isGood) {
                    appendQuizImages();
                } else {
                    setAlert(alertDanger(r.message));
                    setActiveCallback();
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger(generateErrorMessage(e.message)));
                setActiveCallback();
            });
    };
    
    return (
        <>
            {allGood ? <button className="btn btn-color-one mt-2 btn-light w-100" onClick={appendQuestionsTooQuiz}>
                Zaktualizuj pytania quizu
            </button> : <div className="btn btn-outline-danger btn-error-danger text-danger mt-2 w-100">
                Przed zaktualizowaniem quizu popraw błędy.
            </div>}
        </>
    );
};

export default AddUpdateQuizQuestionsButtonComponent;