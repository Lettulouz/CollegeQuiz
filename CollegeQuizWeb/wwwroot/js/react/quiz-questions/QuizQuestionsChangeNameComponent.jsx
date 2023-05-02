import { alertDanger, alertInfo, getCommonFetchObj } from "../Utils.jsx";
import { MainContext, QUIZ_NAME } from "./QuizQuestionsRenderer.jsx";

const QuizQuestionsChangeNameComponent = () => {
    const { setAlert } = React.useContext(MainContext);
    const [ quizName, setQuizName ] = React.useState(QUIZ_NAME);
    const [ quizNameInvalid, setQuizNameInvalid ] = React.useState(false);

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

    React.useEffect(() => {
        setQuizNameInvalid(quizName.length < 3);
    }, [ quizName ]);

    return (
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
    );
};

export default QuizQuestionsChangeNameComponent;