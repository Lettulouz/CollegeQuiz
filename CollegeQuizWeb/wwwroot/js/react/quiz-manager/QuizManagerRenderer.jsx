import QuizManagerRootComponent from "./QuizManagerRootComponent.jsx";

export const SessionContext = React.createContext(null);

export const QR_CODE_BLOB = document.getElementById('inject-qr-code-blob').innerText;
export const SESS_TOKEN = document.getElementById('inject-sess-code').innerText.toUpperCase();
export const QUIZ_NAME = document.getElementById('inject-quiz-name').innerText;

ReactDOM
    .createRoot(document.getElementById('quizManagerRoot'))
    .render(<QuizManagerRootComponent/>);