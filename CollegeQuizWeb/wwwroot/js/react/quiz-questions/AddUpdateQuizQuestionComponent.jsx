import { generateAnswers, id, MainContext } from "./QuizQuestionsRenderer.jsx";
import { alertDanger, alertInfo, getCommonFetchObjWithBody, getCommonFetchObjWithFormData } from "../Utils.jsx";

const AddUpdateQuizQuestionComponent = () => {
    const {
        questions, setQuestions, setInactiveCallback, setActiveCallback, setAlert, allGood, uploadedImages
    } = React.useContext(MainContext);

    const [ isSended, setIsSended ] = React.useState(false);
    
    const onAddNewQuestion = () => {
        setQuestions([ ...questions, {
            id: questions.length + 1,
            text: '',
            timeMin: '0',
            timeSec: '',
            imageUrl: '',
            type: 'SINGLE_FOUR_ANSWERS',
            answers: generateAnswers(4)
        }]);
    };

    const appendQuizImages = () => {
        const formData = new FormData();
        for (const uploadedImage of uploadedImages) {
            const type = uploadedImage.image.type.substring(uploadedImage.image.type.indexOf('/') + 1);
            formData.append("uploads", uploadedImage.image, `question${uploadedImage.id}.${type}`);
        }
        fetch(`/api/v1/dotnet/QuizAPI/UpdateQuizImages/${id}`, getCommonFetchObjWithFormData('POST', formData))
            .then(r => {
                setIsSended(false);
                window.scrollTo(0, 0);
                return r.json();
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
                setAlert(alertDanger('Wystąpił nieznany błąd'));
                setActiveCallback();
            });
    };
    
    const appendQuestionsTooQuiz = () => {
        setInactiveCallback();
        if (isSended) return;
        setIsSended(true);
        fetch(`/api/v1/dotnet/QuizAPI/AddQuizQuestions/${id}`, getCommonFetchObjWithBody('POST', { aggregate: questions }))
            .then(r => {
                setIsSended(false);
                return r.json()
            })
            .then(r => {
                if (r.isGood) {
                    appendQuizImages();
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
                setActiveCallback();
            });
    };
    
    return (
        <>
            <button onClick={onAddNewQuestion} className="btn btn-color-one w-100 mt-2">
                Dodaj nowe pytanie
            </button>
            {allGood && <button className="btn btn-color-one mt-2 btn-light w-100" onClick={appendQuestionsTooQuiz}>
                Zaktualizuj pytania quizu
            </button>}
        </>
    );
};

export default AddUpdateQuizQuestionComponent;