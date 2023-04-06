import { CounterComponent } from "./CounterComponent.jsx";
const { useEffect, useState } = React;

const AddQuizQuestionsRoot = () => {
    const [ counter, useCounter ] = useState(0);
    
    useEffect(() => {
        console.log(`Zwiększenie licznika. Aktualna wartość: ${counter}`);
    }, [ counter ]);
    
    const increaseCounter = () => {
        useCounter(counter + 1);
    };
    
    return (
        <>
            <button onClick={increaseCounter}>Zwiększ licznik</button>
            <CounterComponent counterState={counter}/>
        </>
    );
};

ReactDOM
    .createRoot(document.getElementById('addQuizQuestionsRoot'))
    .render(<AddQuizQuestionsRoot/>);
