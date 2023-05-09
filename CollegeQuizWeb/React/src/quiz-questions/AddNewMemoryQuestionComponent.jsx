import { useContext } from "react";
import { generateAnswers, MainContext } from "../quiz-questions-renderer";

const AddNewMemoryQuestionComponent = () => {
    const { questions, setQuestions } = useContext(MainContext);
    
    const onAddNewQuestion = () => {
        setQuestions([ ...questions, {
            id: questions.length + 1,
            text: '',
            timeMin: '0',
            timeSec: '',
            imageUrl: '',
            type: 'SINGLE_FOUR_ANSWERS',
            blobImage: null,
            answers: generateAnswers(4)
        }]);
    };
    
    return (
        <button onClick={onAddNewQuestion} className="btn btn-color-one w-100 mt-2">
            Dodaj nowe pytanie
        </button>
    );
};

export default AddNewMemoryQuestionComponent;