import QuizManagerRootComponent from "./QuizManagerRootComponent.jsx";

export const SessionContext = React.createContext(null);

export const QR_CODE_BLOB = document.getElementById('inject-qr-code-blob').innerText;
export const SESS_TOKEN = document.getElementById('inject-sess-code').innerText.toUpperCase();
export const QUIZ_NAME = document.getElementById('inject-quiz-name').innerText;

export const copyToClipboard = () => {
    navigator.clipboard.writeText(SESS_TOKEN);
    toastr.success(`Skopiowano kod ${SESS_TOKEN} do schowka.`);
};

ReactDOM
    .createRoot(document.getElementById('quizManagerRoot'))
    .render(<QuizManagerRootComponent/>);