import { useContext, useRef, useState, useEffect } from "react";
import {
    generateAnswers, generateMultAnswers, generateRangeAnswer, generateTrueFalse, MainContext, QuestionsContext
} from "../quiz-questions-renderer";

import QuizQuestionTextContentComponent from "./QuizQuestionTextContentComponent";
import QuizQuestionsRangeAnswerComponent from "./QuizQuestionsRangeAnswerComponent";
import QuizQuestionsTrueFalseAnswerComponent from "./QuizQuestionsTrueFalseAnswerComponent";
import QuizQuestionsImageManipulatorComponent from "./QuizQuestionsImageManipulatorComponent";
import QuizQuestionsSingleGoodAnswerComponent from "./QuizQuestionsSingleGoodAnswerComponent";

const QuizQuestionComponent = () => {
    const {
        setAlert, setQuestions, uploadedImages, setUploadedImages, onSetQuestionProperty, questions
    } = useContext(MainContext);
    
    const { q, availableModes } = useContext(QuestionsContext);

    const modalRef = useRef(null);
    const imageRef = useRef(null);
    const cropperRef = useRef(null);
    const inputImageRef = useRef(null);

    const [ mainImageVisible, setMainImageVisible ] = useState(false);
    const [ imageSrc, setImageSrc ] = useState('');
    const [ imagePreview, setImagePreview ] = useState('');

    const showModal = () => new bootstrap.Modal(modalRef.current, {backdrop: 'static', keyboard: false}).show();
    const hideModal = () => bootstrap.Modal.getInstance(modalRef.current).hide();
    
    const generateOptions = availableModes.map(o => <option key={o.slug} value={o.slug}>{o.title}</option>);

    const answersComponents = q.answers.map((answer, idx) => (
        <QuizQuestionsSingleGoodAnswerComponent
            key={idx} id={idx + 1} answer={answer}
            isMultipleAnswers={q.type === "MULTIPLE_FOUR_ANSWERS"}
        />
    ));
    
    const onRemoveImage = () => {
        const images = [ ...uploadedImages ];
        const withoutImage = images.filter(i => i.id !== q.id);
        setUploadedImages(withoutImage);
        setMainImageVisible(false);
        URL.revokeObjectURL(imageSrc);
        onSetQuestionProperty(q.id, "urlImage", "");
        setImageSrc('');
        inputImageRef.current.value = "";
        cropperRef.current.destroy();
    };

    const acceptChanges = () => {
        const canvas = cropperRef.current.getCroppedCanvas({ width: 500, height: 500 });
        setImagePreview(canvas.toDataURL());
        canvas.toBlob(function (blob) {
            const images = [ ...uploadedImages ];
            let updatedImage = images.findIndex(i => i.id === q.id);
            if (updatedImage !== -1) {
                images[updatedImage] = { id: q.id, image: blob };
                setUploadedImages(images);
            } else {
                setUploadedImages(prevState => [ ...prevState, { id: q.id, image: blob } ]);
            }
        });
        setMainImageVisible(true);
    };

    const onSetQuestionType = e => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qstData => qstData.id === q.id);
        qst[idx].type = e.target.value;
        switch (e.target.value) {
            case "TRUE_FALSE":              qst[idx].answers = generateTrueFalse();      break;
            case "RANGE":                   qst[idx].answers = generateRangeAnswer();   break;
            case "SINGLE_SIX_ANSWERS":      qst[idx].answers = generateAnswers(6);      break;
            case "MULTIPLE_FOUR_ANSWERS":   qst[idx].answers = generateMultAnswers(4);  break;
            default:                        qst[idx].answers = generateAnswers(4);
        }
        setQuestions(qst);
    };
    
    const refuseChanges = () => {
        hideModal();
        onRemoveImage();
    };

    const onLoadImage = e => {
        const { files } = e.target;
        if (!files || !files[0]) return;
        setImageSrc(URL.createObjectURL(files[0]));
        showModal();
    };

    const generateAnswersComponent = () => {
        switch (q.type) {
            case "RANGE": return <QuizQuestionsRangeAnswerComponent/>;
            case "TRUE_FALSE": return q.answers.map((answer, idx) => (
                <QuizQuestionsTrueFalseAnswerComponent key={idx} id={idx + 1} answer={answer}/>
            ));
            default: return answersComponents;
        }
    };
    
    useEffect(() => {
        if (!q.imageUrl) return;
        setImageSrc(q.imageUrl);
        setImagePreview(q.imageUrl);
        fetch(q.imageUrl)
            .then(r => {
                if (r.ok) {
                    return r.blob();
                }
                throw new Error(r.status);
            })
            .then(b => {
                if (uploadedImages.length === 0) {
                    setUploadedImages(prevState => ([ ...prevState, { id: q.id, image: b } ]));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger(generateErrorMessage(e.message)));
            })
        setMainImageVisible(true);
    }, [ q.imageUrl ]);
    
    useEffect(() => {
        return () => {
            setImageSrc("");
            setImagePreview("");
            setMainImageVisible(false);
        };
    }, [ q.imageUrl ]);
    
    return (
        <>
            <QuizQuestionsImageManipulatorComponent
                modalRef={modalRef} acceptChanges={acceptChanges} refuseChanges={refuseChanges} imageSrc={imageSrc}
                imageRef={imageRef} cropperRef={cropperRef}
            />
            <div className="row g-2 mb-4">
                <div className="col-12">
                    <div className="p-3 card card-glass-effect">
                        {mainImageVisible && <div className="d-flex justify-content-center mb-3">
                            <button onClick={showModal} className="bg-transparent border-image rounded">
                                <img src={imagePreview} alt="loadedImage" className="align-middle rounded" width="100%" height="100%"/>
                            </button>
                        </div>}
                        <div className="mb-2 hstack gap-3">
                            <select className="form-select" value={q.type}
                                    onChange={onSetQuestionType}>
                                {generateOptions}
                            </select>
                            <input ref={inputImageRef} type="file" className="form-control" onChange={onLoadImage}
                                accept="image/png,image/jpeg,image/jpg" disabled={mainImageVisible}/>
                            {mainImageVisible && <button className="btn btn-danger text-white" onClick={onRemoveImage}>X</button>}
                        </div>
                        <QuizQuestionTextContentComponent/>
                    </div>
                </div>
                {generateAnswersComponent()}
            </div>
        </>
    );
};

export default QuizQuestionComponent;