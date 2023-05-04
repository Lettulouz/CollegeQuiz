import React, { createContext } from 'react';
import ReactDOM from 'react-dom/client';

import QuizQuestionsRootComponent from "./quiz-questions/QuizQuestionsRootComponent";

export const QuestionsContext = createContext(null);
export const MainContext = createContext(null);

export let QUIZ_NAME = document.getElementById("quiz-questions-embed-content").dataset.quizName;
export const QUIZ_ID = document.getElementById("quiz-questions-embed-content").dataset.quizId;

const path = window.location.pathname.split('/');
export const id = path[path.length - 1];

export const generateAnswers = length => Array.from({ length }).map((_, i) => generateDiffAnswer(i, i === 0, false, ""));
export const generateTrueFalse = () => Array.from({ length: 2 }).map((_, i) => generateDiffAnswer(i, i === 0, false, `BOOLEAN${i}`));
export const generateMultAnswers = length => Array.from({ length }).map((_, i) => generateDiffAnswer(i, false, false, ""));
export const generateRangeAnswer = () => [ generateDiffAnswer(1, true, true, "RANGE") ];

export const generateDiffAnswer = (id, isCorrect, isRange, text) => ({
    id: id + 1, text, isCorrect, isRange, max: 0, maxCounted: 0, min: 0, minCounted: 0, step: 0, correctAns: 0
});

export const setQuizOriginalName = name => {
    QUIZ_NAME = name;
}

export const initialQuestions = [
    {
        id: 1,
        text: '',
        timeMin: '0',
        timeSec: '',
        imageUrl: '',
        type: 'SINGLE_FOUR_ANSWERS',
        answers: generateAnswers(4),
    },
];

ReactDOM
    .createRoot(document.getElementById('quiz-questions-embed-content'))
    .render(<QuizQuestionsRootComponent/>);
