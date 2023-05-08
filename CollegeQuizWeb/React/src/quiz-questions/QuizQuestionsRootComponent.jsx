import { useState, useEffect } from "react";
import { id, initialQuestions, QuestionsContext, MainContext } from "../quiz-questions-renderer";
import useLoadableContent from "../hooks/useLoadableContent";
import { alertDanger, alertOff, generateErrorMessage, getCommonFetchObj } from "../utils/common";

import QuizQuestionComponent from "./QuizQuestionComponent";
import AddNewMemoryQuestionComponent from "./AddNewMemoryQuestionComponent";
import QuizQuestionsChangeNameComponent from "./QuizQuestionsChangeNameComponent";
import AddUpdateQuizQuestionsButtonComponent from "./AddUpdateQuizQuestionsButtonComponent";

const QuizQuestionsRootComponent = () => {
    const [ questions, setQuestions ] = useState(initialQuestions);
    const [ allGood, setAllGood ] = useState(true);
    const [ alert, setAlert ] = useState(alertOff());
    const [ isActive, setActiveCallback, setInactiveCallback ] = useLoadableContent();
    const [ isNotValid, setIsNotValid ] = useState(false);
    const [ availableModes, setAvailableModes ] = useState([]);
    const [ infoModesAlert, setInfoModesAlert ] = useState('');
    const [ uploadedImages, setUploadedImages ] = useState([]);
    
    const onSetQuestionProperty = (questionId, property, data) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qs => qs.id === questionId);
        qst[idx][property] = data;
        setQuestions(qst);
    };
    
    useEffect(() => {
        fetch(`/api/v1/dotnet/QuizAPI/GetQuizQuestions/${id}`, getCommonFetchObj('GET')).then(r => {
            if (r.ok) {
                setActiveCallback();
                return r.json();
            }
            throw new Error(r.status);
        }).then(r => {
            if (r.aggregate.length !== 0) setQuestions(r.aggregate);
            setAvailableModes(r.availableModes);
            setInfoModesAlert(r.permissionModesMessage);
        }).catch(e => {
            if (e === undefined) return;
            setAlert(alertDanger(generateErrorMessage(e.message)));
        });
    }, []);

    useEffect(() => {
        const anyBad = questions.filter(q => {
            return q.text.length <= 2 || (q.answers
                    .filter(a => a.text.length >= 1).length !== q.answers.length && q.type !== "RANGE"
                        && q.type !== "TRUE_FALSE") ||
                q.timeMin.length === 0 || q.timeSec.length === 0 ||
                q.answers.filter(q => q.isCorrect).length === 0;
        });
        setAllGood(anyBad.length === 0 && !isNotValid);
    }, [ questions, isNotValid ]);

    const generateQuestionsComponents = questions.map((q, idx) => (
        <QuestionsContext.Provider key={idx} value={{ q, availableModes }}>
            <QuizQuestionComponent/>
        </QuestionsContext.Provider>
    ));

    return (
        <MainContext.Provider value={{
            setAlert, questions, setQuestions, setActiveCallback, setInactiveCallback, allGood, uploadedImages,
            setUploadedImages, onSetQuestionProperty, setIsNotValid
        }}>
            {isActive && <>
                <div className="alert alert-warning">
                    Aby zaktualizowac nazwę quizu, musi się składać ono z przynajmniej 3 znaków. Aby poprawnie
                    dodać/zaktualizować pytania, każda treść pytania musi posiadać przynajmniej dwa znaki, a sama odpowiedź
                    może mieć jeden. Dodatkowo w przypadku odpowiedzi, musi być przynajmniej jedna zaznaczona.
                </div>
                <div className="alert alert-info">
                    <span dangerouslySetInnerHTML={{ __html: infoModesAlert }}></span>
                </div>
                {alert.active && <div className={`alert ${alert.style} d-flex justify-content-between`} role="alert">
                    <span dangerouslySetInnerHTML={{ __html: alert.message }}></span>
                    <button type="button" className="btn-close" onClick={() => setAlert(alertOff())}></button>
                </div>}
                <QuizQuestionsChangeNameComponent/>
                <div className="mb-2">
                    <AddUpdateQuizQuestionsButtonComponent/>
                </div>
                {generateQuestionsComponents}
                <AddNewMemoryQuestionComponent/>
                <AddUpdateQuizQuestionsButtonComponent/>
            </>}
        </MainContext.Provider>
    );
};

export default QuizQuestionsRootComponent;