import { SessionContext } from "./QuizSessionRenderer.jsx";

import LeaveSessionButtonComponent from "./LeaveQuizSessionButtonComponent.jsx";

const QuizSessionQuestionResultComponent = () => {
    const { afterQuestionResults } = React.useContext(SessionContext);

    const containerUsernamesRef = React.useRef(null);
    const containerScoresRef = React.useRef(null);
    const leaderRef = React.useRef(null);
    
    const timeline = anime.timeline({ easing: 'easeOutExpo' });
    const lastItem = afterQuestionResults.length - 1;

    React.useEffect(() => {
        if (!afterQuestionResults[0].isLast) return;
        timeline
            .add({
                targets: containerUsernamesRef.current.children,
                translateX: [ -80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
                delay: anime.stagger(100),
            })
            .add({
                targets: containerScoresRef.current.children,
                translateX: [ 80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
                delay: anime.stagger(100),
            }, "-=800")
            .add({
                targets: leaderRef.current,
                translateY: [ 80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
            }, "-=800");
    }, []);

    return (
        <div className="container">
            <div className="row mb-4">
                <LeaveSessionButtonComponent text="Opuść quiz" customClass="danger-button-fixed"/>
            </div>
            <div className="row mb-2">
                <div className="col-md-6" ref={containerUsernamesRef}>
                    {afterQuestionResults.slice(0, -1).map(m => (
                        <div className="leaderboard colors-leaderboard fw-bold mb-2 fs-1 mx-2 px-5 col-sm" key={m.Username}>
                            {m.Username}
                        </div>
                    ))}
                </div>
                <div className="col-md-6" ref={containerScoresRef}>
                    {afterQuestionResults.slice(0, -1).map(m => (
                        <div className="leaderboard fw-bold mb-2 fs-1 mx-2 px-5 col-sm" key={m.Username}>
                            {m.Score} (+ {m.newPoints})
                        </div>
                    ))}
                </div>
            </div>
            {afterQuestionResults[lastItem].CurrentStreak > 0 &&
            <div className="leaderboard streak fw-bold fs-1 mx-2 px-5 col-sm" ref={leaderRef}>
                <div className="icon-animation"><i className="bi bi-lightning-charge-fill text-warning fs-1 icon-animation"></i></div>
                <div className="flex-fill text-center">
                    {afterQuestionResults[lastItem].Username}: {afterQuestionResults[lastItem].CurrentStreak}
                </div>
                <div className="icon-animation"><i className="bi bi-lightning-charge-fill text-warning fs-1"></i></div>
            </div>
                
            }
        </div>
    );
};

export default QuizSessionQuestionResultComponent;