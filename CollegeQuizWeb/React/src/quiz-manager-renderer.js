import React, { createContext } from 'react';
import ReactDOM from 'react-dom/client';

import QuizManagerRootComponent from "./quiz-manager/QuizManagerRootComponent";

export const SessionContext = createContext(null);
export const InGameViewContext = createContext(null);

export const QR_CODE_BLOB = document.getElementById('inject-qr-code-blob').innerText;
export const SESS_TOKEN = document.getElementById('inject-sess-code').innerText.toUpperCase();
export const QUIZ_NAME = document.getElementById('inject-quiz-name').innerText;

export const copyToClipboard = () => {
    navigator.clipboard.writeText(SESS_TOKEN);
    toastr.success(`Skopiowano kod ${SESS_TOKEN} do schowka.`);
};

ReactDOM
    .createRoot(document.getElementById('quiz-manager-embed-content'))
    .render(<QuizManagerRootComponent/>);