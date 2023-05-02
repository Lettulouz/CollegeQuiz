import { SessionContext } from "./QuizSessionRenderer.jsx";

const QuizSessionQuestionResultComponent = () => {
    const { afterQuestionResults, currentQuestionLeader } = React.useContext(SessionContext);

    const containerUsernamesRef = React.useRef(null);
    const containerScoresRef = React.useRef(null);
    const leaderRef = React.useRef(null);
    const timeline = anime.timeline({ easing: 'easeOutExpo' });

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
            <div className="row mb-2">
                <div className="col-md-6" ref={containerUsernamesRef}>
                    {afterQuestionResults.map(m => (
                        <div className="leaderboard text-white fw-bold mb-2 fs-1 mx-2 px-5 col-sm" key={m.Username}>
                            {m.Username}
                        </div>
                    ))}
                </div>
                <div className="col-md-6" ref={containerScoresRef}>
                    {afterQuestionResults.map(m => (
                        <div className="leaderboard gold-leaderboard fw-bold mb-2 fs-1 mx-2 px-5 col-sm" key={m.Username}>
                            {m.Score} + {m.newPoints}
                        </div>
                    ))}
                </div>
            </div>
            <div className="leaderboard text-white fw-bold fs-1 mx-2 px-5 col-sm" ref={leaderRef}>
                {currentQuestionLeader}
            </div>
        </div>
    );
};

export default QuizSessionQuestionResultComponent;