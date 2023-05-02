import QuizSessionRootComponent from "./QuizSessionRootComponent.jsx";

export const SessionContext = React.createContext(null);

export const ANSWER_LETTERS = [ "A", "B", "C", "D", "E", "F" ];
export const ANSWER_SVGS = [
    "/gfx/blueCard.svg", "/gfx/greenCard.svg", "/gfx/darkblueCard.svg",
    "/gfx/tealCard.svg", "/gfx/oliveCard.svg", "/gfx/darkgreenCard.svg"
];

ReactDOM
    .createRoot(document.getElementById('quizSessionRoot'))
    .render(<QuizSessionRootComponent/>);
