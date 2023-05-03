import { useLoadableContent } from "../Hooks.jsx";
import { alertOff, getCommonFetchObj } from "../Utils.jsx";
import { id, initialQuestions, QuestionsContext, MainContext } from "./QuizQuestionsRenderer.jsx";

import QuizQuestionComponent from "./QuizQuestionComponent.jsx";
import QuizQuestionsChangeNameComponent from "./QuizQuestionsChangeNameComponent.jsx";
import AddUpdateQuizQuestionComponent from "./AddUpdateQuizQuestionComponent.jsx";

const QuizQuestionsRootComponent = () => {
    const [ questions, setQuestions ] = React.useState(initialQuestions);
    const [ allGood, setAllGood ] = React.useState(true);
    const [ alert, setAlert ] = React.useState(alertOff());
    const [ isActive, setActiveCallback, setInactiveCallback ] = useLoadableContent();
    const [ isNotValid, setIsNotValid ] = React.useState(false);
    const [ availableModes, setAvailableModes ] = React.useState([]);
    const [ infoModesAlert, setInfoModesAlert ] = React.useState('');
    const [ uploadedImages, setUploadedImages ] = React.useState([]);
    
    const onSetQuestionProperty = (questionId, property, data) => {
        const qst = [ ...questions ];
        const idx = qst.findIndex(qs => qs.id === questionId);
        qst[idx][property] = data;
        setQuestions(qst);
    };
    
    React.useEffect(() => {
        fetch(`/api/v1/dotnet/QuizAPI/GetQuizQuestions/${id}`, getCommonFetchObj('GET')).then(r => {
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

    React.useEffect(() => {
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
                <QuizQuestionsChangeNameComponent/>
                {generateQuestionsComponents}
                <AddUpdateQuizQuestionComponent/>
            </>}
        </MainContext.Provider>
    );
};

export default QuizQuestionsRootComponent;