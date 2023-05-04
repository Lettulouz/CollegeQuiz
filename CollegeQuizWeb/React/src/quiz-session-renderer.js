import React, { createContext } from 'react';
import ReactDOM from 'react-dom/client';

import QuizSessionRootComponent from "./quiz-session/QuizSessionRootComponent";

export const SessionContext = createContext(null);

ReactDOM
    .createRoot(document.getElementById('quiz-session-embed-content'))
    .render(<QuizSessionRootComponent/>);