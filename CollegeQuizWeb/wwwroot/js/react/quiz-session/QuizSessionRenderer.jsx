import QuizSessionRootComponent from "./QuizSessionRootComponent.jsx";

export const SessionContext = React.createContext(null);

ReactDOM
    .createRoot(document.getElementById('quizSessionRoot'))
    .render(<QuizSessionRootComponent/>);
