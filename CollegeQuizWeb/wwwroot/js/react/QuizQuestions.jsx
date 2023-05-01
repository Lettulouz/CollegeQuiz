import { useLoadableContent } from "./Hooks.jsx";
import {
    getCommonFetchObj, alertOff, alertInfo, alertDanger, getCommonFetchObjWithBody, getCommonFetchObjWithFormData,
    getCropperConfig
} from "./Utils.jsx";
const { useEffect, useState, createContext, useContext, useRef } = React;

const QuestionsContext = createContext(null);
const MainContext = createContext(null);

let QUIZ_NAME = document.getElementById("addQuizQuestionsRoot").dataset.quizName;
const QUIZ_ID = document.getElementById("addQuizQuestionsRoot").dataset.quizId;
const loadableSpinner = document.getElementById('react-loadable-spinner-content');

const path = window.location.pathname.split('/');
const id = path[path.length - 1];

const QuizChangeNameComponent = () => {
    const { setAlert } = useContext(MainContext);
    const [ quizName, setQuizName ] = useState(QUIZ_NAME);
    const [ quizNameInvalid, setQuizNameInvalid ] = useState(false);

    const changeQuizName = e => {
        e.preventDefault();
        fetch(`/api/v1/dotnet/QuizAPI/ChangeQuizName/${QUIZ_ID}/${quizName}`, getCommonFetchObj('POST'))
            .then(r => r.json())
            .then(r => {
                if (r.isGood) {
                    setAlert(alertInfo(r.message));
                    QUIZ_NAME = quizName;
                } else {
                    setAlert(alertDanger(r.message));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert(alertDanger('Wystąpił nieznany błąd'));
            });
    };

    useEffect(() => {
        setQuizNameInvalid(quizName.length < 3); 
    }, [ quizName ]);
    
    return (
        <>
            <div className="hstack gap-3 w-100 mb-3">
                <form onSubmit={changeQuizName} className="hstack gap-2 w-100">
                    <input type="text" className="form-control" value={quizName} onChange={e => setQuizName(e.target.value)}/>
                    <button type="submit" className="btn btn-color-one fit-content" disabled={quizNameInvalid}>
                        <i className="bi bi-check-lg text-white"></i>
                    </button>
                    <button type="button" className="btn btn-color-one fit-content" onClick={() => setQuizName(QUIZ_NAME)}
                            data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Tooltip on top">
                        <i className="bi bi-arrow-counterclockwise text-white"></i>
                    </button>
                </form>
            </div>
        </>
    );
};


const QuizRangeAnswerComponent = ({ id }) => {
    const { q, onSetRangeProp, setIsNotValid } = useContext(QuestionsContext);
    const answerR = q.answers[0];
    const [ minInvalid, setMinInvalid ] = useState('');
    const [ minCountedInvalid, setMinCountedInvalid ] = useState('');
    const [ countedOutOfRange, setCountedOutOfRange ] = useState('');
    const [ stepIsInvalid, setStepIsInvalid ] = useState('');
    const [ isInvalidCorrectAns, setIsCorrectInvalid ] = useState('');
    
    useEffect(() => {
        const minMax = answerR.min > answerR.max;
        const minMaxCounted = answerR.minCounted > answerR.maxCounted;
        const countedOutOfRange = answerR.minCounted > answerR.max || answerR.minCounted < answerR.min ||
            answerR.maxCounted < answerR.min || answerR.maxCounted > answerR.max;
        const stepIsInvalid = 
            ((answerR.min - answerR.correctAns) % answerR.step !== 0 || 
            (answerR.max - answerR.correctAns) % answerR.step !== 0 ||
            (answerR.minCounted - answerR.correctAns) % answerR.step !== 0 ||
            (answerR.maxCounted - answerR.correctAns) % answerR.step !== 0 ||
            answerR.step > answerR.max || answerR.step < answerR.min) && answerR.step !== 1;
        const correctAnsIsInvalid = answerR.correctAns > answerR.max || answerR.correctAns < answerR.min;
        
        setMinInvalid(minMax ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setMinCountedInvalid(minMaxCounted ? "Wartość minimalna nie może być większa od wartości maksymalnej" : "");
        setCountedOutOfRange(countedOutOfRange ? "Wartość punktowana wykracza poza zakres" : "");
        setStepIsInvalid(stepIsInvalid ? "Wartość step musi być dzielnikiem pozostałych wartości" : "");
        setIsCorrectInvalid(correctAnsIsInvalid ? "Nieprawidłowa wartość prawidłowej odpowiedzi" : "");
        
        setIsNotValid(minMax || minMaxCounted || countedOutOfRange || stepIsInvalid || correctAnsIsInvalid);
    }, [ answerR.max, answerR.min, answerR.step, answerR.minCounted, answerR.maxCounted, answerR.correctAns ]);
    
    return (
        <div className="col-12">
            <div className="p-3 card">
                <div className="p-3 card">
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label htmlFor="minId" className="form-label">Min</label>
                            <input value={answerR.min} type="number" className={`form-control ${minInvalid && 'is-invalid'}`}
                                id="minId" onChange={e => onSetRangeProp(id, e, "min")} min={0}/>
                            <div className="invalid-feedback">{minInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="stepId" className="form-label">Step(sister)</label>
                            <input value={answerR.step} type="number" className={`form-control ${stepIsInvalid && 'is-invalid'}`}
                                id="stepId" onChange={e => onSetRangeProp(id, e, "step")} min={0}/>
                            <div className="invalid-feedback">{stepIsInvalid}</div>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label htmlFor="maxId" className="form-label">Maks</label>
                            <input value={answerR.max} type="number" className="form-control" id="maxId"
                                onChange={e => onSetRangeProp(id, e, "max")} min={0}/>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="minCountedId" className="form-label">Min punktowane</label>
                            <input value={answerR.minCounted} type="number"
                                className={`form-control ${(countedOutOfRange || minCountedInvalid) && 'is-invalid'}`}
                                id="minCountedId" onChange={e => onSetRangeProp(id, e, "minCounted")} min={0}/>
                            <div className="invalid-feedback">{minCountedInvalid}</div>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="correctAnsId" className="form-label">Prawidłowa odpowiedź</label>
                            <input value={answerR.correctAns} type="number"
                                className={`form-control ${(isInvalidCorrectAns) && 'is-invalid'}`}
                                id="correctAnsId" onChange={e => onSetRangeProp(id, e, "correctAns")} min={0}/>
                            <div className="invalid-feedback">{isInvalidCorrectAns}</div>
                        </div>
                        <div className="col-md-4">
                            <label htmlFor="maxCounterId" className="form-label">Maks punktowane</label>
                            <input value={answerR.maxCounted} type="number" className={`form-control ${countedOutOfRange && 'is-invalid'}`}
                                id="maxCounterId" onChange={e => onSetRangeProp(id, e, "maxCounted")} min={0}/>
                            <div className="invalid-feedback">{countedOutOfRange}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

const QuizSingleGoodAnswerComponent = ({ id, answer, isMultipleAnswers }) => {
    const { q, onSetQuestionAnswer, onChangeCorrectAnswer } = useContext(QuestionsContext);
    
    const questionId = `question_${id}_group_${q.id}`;
    const groupId = `question_group_${q.id}`;
    
    return (
        <div className="col-6">
            <div className="p-3 card">
                <div className="p-3 card hstack">
                    <div className="me-2 fs-4">{answer.id}</div>
                    <input type="text" className="form-control" placeholder={`Treść odpowiedzi ${id}`}
                        onChange={e => onSetQuestionAnswer(e.target.value, q.id, answer.id)} value={answer.text}/>
                </div>
                <div className="form-check mt-2">
                    {isMultipleAnswers ? <>
                        <input type="checkbox" className="form-check-input" id={questionId} name={groupId}
                            checked={answer.isCorrect} onChange={e => onChangeCorrectAnswer(q.id, answer.id, isMultipleAnswers, e.target.checked)}/>
                    </> : <>
                        <input type="radio" className="form-check-input form-check-input-radio" id={questionId} name={groupId}
                            checked={answer.isCorrect} onChange={() => onChangeCorrectAnswer(q.id, answer.id, isMultipleAnswers, true)}/>
                    </>}
                    <label htmlFor={questionId} className="form-check-label">
                        {q.type === "TRUE_FALSE" ? id === 1 ? "Prawda" : "Fałsz" : "To jest poprawna odpowiedź"}
                    </label>
                </div>
            </div>
        </div>
    );
};

const QuizImageManipulator = ({ modalRef, acceptChanges, refuseChanges, imageRef, imageSrc, cropperRef }) => {

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
                    <div className="modal-body fw-normal d-flex justify-content-center w-100"
                        style={{ width: 600 }}>
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

const QuizQuestionComponent = () => {
    const { setAlert } = useContext(MainContext);
    const {
        q, onSetQuestionTitle, onRemoveQuestion, onSetMinutes, onSetSeconds, onSetQuestionType, availableModes,
        uploadedImages, setUploadedImages, onSetQuestionImage
    } = useContext(QuestionsContext);
    
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
        <QuizSingleGoodAnswerComponent
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
        onSetQuestionImage('', q.id);
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

    useEffect(() => {
        if (!q.imageUrl) return;
        setImageSrc(q.imageUrl);
        setImagePreview(q.imageUrl);
        fetch(q.imageUrl)
            .then(r => r.blob())
            .then(b => {
                if (uploadedImages.length === 0) {
                    setUploadedImages(prevState => ([ ...prevState, { id: q.id, image: b } ]));
                }
            })
            .catch(e => {
                if (e === undefined) return;
                setAlert({ active: true, style: 'alert-danger', message: 'Wystąpił problem z załadowaniem grafiki' });
            })
        setMainImageVisible(true);
    }, [ q.imageUrl ]);
    
    useEffect(() => {
        if (q.timeMin.length > 3) onSetMinutes(q.id, q.timeMin.slice(0, 3));
    }, [ q.timeMin ]);

    useEffect(() => {
        if (q.timeSec.length > 2) onSetSeconds(q.id, e.timeSec.slice(0, 2));
        if (q.timeSec > 59) onSetSeconds(q.id, 59);
    }, [ q.timeSec ]);
    
    return (
        <>
            <QuizImageManipulator
                modalRef={modalRef} acceptChanges={acceptChanges} refuseChanges={refuseChanges} imageSrc={imageSrc}
                imageRef={imageRef} cropperRef={cropperRef}
            />
            <div className="row g-2 mb-4">
                <div className="col-12">
                    <div className="p-3 card">
                        {mainImageVisible && <div className="d-flex justify-content-center mb-3">
                            <button onClick={showModal} className="bg-transparent border-image">
                                <img src={imagePreview} alt="loadedImage" className="align-middle" width="100%" height="100%"/>
                            </button>
                        </div>}
                        <div className="mb-2 hstack gap-3">
                            <select className="form-select" value={q.type}
                                    onChange={e => onSetQuestionType(e.target.value, q.id)}>
                                {generateOptions}
                            </select>
                            <input ref={inputImageRef} type="file" className="form-control" onChange={onLoadImage}
                                accept="image/png,image/jpeg,image/jpg" disabled={mainImageVisible}/>
                            {mainImageVisible && <button className="btn btn-danger text-white" onClick={onRemoveImage}>X</button>}
                        </div>
                        <div className="hstack gap-2">
                            <div className="me-2 fs-3">{q.id}</div>
                            <textarea className="form-control h-100" placeholder="Treść pytania" value={q.text}
                                onChange={e => onSetQuestionTitle(e.target.value, q.id)}></textarea>
                            <div>
                                <label className="mb-1">Czas trwania pytania:</label>
                                <div className="d-flex">
                                    <input type="number" className="form-control time-control" placeholder="min" min="0" max="999"
                                        maxLength="3" value={q.timeMin} onChange={e => onSetMinutes(q.id, e.target.value)}/>
                                    <span className="mx-2 fw-bold pt-1">:</span>
                                    <input type="number" className="form-control time-control" placeholder="sek" min="0" max="59"
                                        maxLength="2" value={q.timeSec} onChange={e => onSetSeconds(q.id, e.target.value)}/>
                                </div>
                            </div>
                            <button className="btn btn-danger text-white ms-2" onClick={() => onRemoveQuestion(q.id)}>X</button>
                        </div>
                    </div>
                </div>
                {q.type === "RANGE" ? <QuizRangeAnswerComponent id={q.id}/> : answersComponents}
            </div>
        </>
    );
};

const initialQuestions = [
    {
        id: 1,
        text: '',
        timeMin: '0',
        timeSec: '',
        imageUrl: '',
        type: 'SINGLE_FOUR_ANSWERS',
        answers: [
            { id: 1, text: '', isCorrect: true, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
            { id: 2, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
            { id: 3, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
            { id: 4, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
        ],
    },
];

const AddQuizQuestionsRoot = () => {
    const [ questions, setQuestions ] = useState(initialQuestions);
    const [ allGood, setAllGood ] = useState(true);
    const [ alert, setAlert ] = useState(alertOff());
    const [ isActive, setActiveCallback, setInactiveCallback ] = useLoadableContent();
    const [ isSended, setIsSended ] = useState(false);
    const [ isNotValid, setIsNotValid ] = useState(false);
    const [ availableModes, setAvailableModes ] = useState([]);
    const [ infoModesAlert, setInfoModesAlert ] = useState('');
    const [ uploadedImages, setUploadedImages ] = useState([]);

    const onSetQuestionImage = (image, questionId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qs => qs.id === questionId);
        qst[idx].imageUrl = image;
        setQuestions(qst);
    };
    
    const onSetQuestionType = (type, questionId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].type = type;
        onChangeQuestionType(questionId, type);
        setQuestions(qst);
    };
    
    const onSetQuestionTitle = (text, questionId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].text = text;
        setQuestions(qst);
    };
    
    const onSetQuestionAnswer = (text, questionId, answerId) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        const answIdx = qst[idx].answers.findIndex(a => a.id === answerId);
        qst[idx].answers[answIdx].text = text;
        setQuestions(qst);
    };
    
    const onChangeCorrectAnswer = (questionId, answerId, isMultipleAnswers, checked) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        if (!isMultipleAnswers) {
            qst[idx].answers.forEach(a => a.isCorrect = false);
        }
        const answIdx = qst[idx].answers.findIndex(a => a.id === answerId);
        qst[idx].answers[answIdx].isCorrect = checked;
        setQuestions(qst);
    };
    
    const onSetMinutes = (questionId, minutes) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].timeMin = minutes;
        setQuestions(qst);
    };

    const onSetSeconds = (questionId, seconds) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].timeSec = seconds;
        setQuestions(qst);
    };

    const onAddNewQuestion = () => {
        setQuestions([ ...questions, {
            id: questions.length + 1,
            text: '',
            timeMin: '0',
            timeSec: '',
            imageUrl: '',
            type: 'SINGLE_FOUR_ANSWERS',
            answers: [
                { id: 1, text: '', isCorrect: true, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
                { id: 2, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
                { id: 3, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 },
                { id: 4, text: '', isCorrect: false, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 }
            ]
        }]);
    };

    const generateAnswerObject = (id, isCorrect) => (
        { id: id + 1, text: '', isCorrect, isRange: false, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 }
    );
    
    const onChangeQuestionType = (questionId, type) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        switch (type) {
            case "TRUE_FALSE":
                qst[idx].answers = Array.from({ length: 2 }).map((_, i) => generateAnswerObject(i, i === 0));
                break;
            case "RANGE":
                qst[idx].answers = [
                    { id: 1, text: '', isCorrect: true, isRange: true, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0 }
                ];
                break;
            case "SINGLE_SIX_ANSWERS":
                qst[idx].answers = Array.from({ length: 6 }).map((_, i) => generateAnswerObject(i, i === 0));
                break;
            case "MULTIPLE_FOUR_ANSWERS":
                qst[idx].answers = Array.from({ length: 4 }).map((_, i) => generateAnswerObject(i, false));
                break;
            default:
                qst[idx].answers = Array.from({ length: 4 }).map((_, i) => generateAnswerObject(i, i === 0));
        }
        setQuestions(qst);
    };

    const onSetRangeProp = (questionId, e, propType) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(q => q.id === questionId);
        qst[idx].answers[0][propType] = Number(e.target.value);
        setQuestions(qst);
    };
    
    const onRemoveQuestion = (id) => {
        const qst = [ ...questions ];
        setQuestions(qst.filter(q => q.id !== id));
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
                return r.json()
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
    
    useEffect(() => {
        const path = window.location.pathname.split('/');
        const id = path[path.length - 1];
        fetch(`/api/v1/dotnet/QuizAPI/GetQuizQuestions/${id}`, getCommonFetchObj('GET')).then(r => {
            loadableSpinner.style.cssText = 'display:none !important';
            setActiveCallback();
            return r.json()
        }).then(r => { 
            if (r.aggregate.length !== 0) setQuestions(r.aggregate);
            setAvailableModes(r.availableModes);
            setInfoModesAlert(r.permissionModesMessage);
        }).catch(e => {
            if (e === undefined) return;
            setAlert({ active: true, style: 'alert-danger', message: 'Wystąpił nieznany błąd' });
        });
    }, []);
    
    useEffect(() => {
        const anyBad = questions.filter(q => {
            return q.text.length <= 2 || (q.answers
                    .filter(a => a.text.length >= 1).length !== q.answers.length && q.type !== "RANGE") ||
                q.timeMin.length === 0 || q.timeSec.length === 0 || 
                q.answers.filter(q => q.isCorrect).length === 0;
        });
        setAllGood(anyBad.length === 0 && !isNotValid);
    }, [ questions, isNotValid ]);
    
    const generateQuestionsComponents = questions.map((q, idx) => (
        <QuestionsContext.Provider key={idx} value={{
            q, setQuestions, onSetQuestionAnswer, onChangeCorrectAnswer, onSetQuestionTitle, onRemoveQuestion,
            onSetMinutes, onSetSeconds, onSetQuestionType, onSetRangeProp, setIsNotValid, availableModes,
            uploadedImages, setUploadedImages, onSetQuestionImage
        }}>
            <QuizQuestionComponent/>
        </QuestionsContext.Provider>
    ));
    
    return (
        <MainContext.Provider value={{ setAlert }}>
            {isActive && <>
                <div className="alert alert-warning">
                    Aby zaktualizowac nazwę quizu, musi się składać ono z przynajmniej 3 znaków. Aby poprawnie
                    dodać/zaktualizować pytania, każde pole musi zostać wypełnione przynajmniej dwoma znakami
                    a w przypadku odpowiedzi, musi być przynajmniej jedna zaznaczona.
                </div>
                <div className="alert alert-info">
                    <span dangerouslySetInnerHTML={{ __html: infoModesAlert }}></span>
                </div>
                {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between`} role="alert">
                    <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                    <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                </div>}
                <QuizChangeNameComponent/>
                {generateQuestionsComponents}
                <button onClick={onAddNewQuestion} className="btn btn-color-one w-100 mt-2">
                    Dodaj nowe pytanie
                </button>
                {allGood && <button className="btn btn-color-one mt-2 btn-light w-100" onClick={appendQuestionsTooQuiz}>
                    Zaktualizuj pytania quizu
                </button>}
            </>}
        </MainContext.Provider>
    );
};

ReactDOM
    .createRoot(document.getElementById('addQuizQuestionsRoot'))
    .render(<AddQuizQuestionsRoot/>);
