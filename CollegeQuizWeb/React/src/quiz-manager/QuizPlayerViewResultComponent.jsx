import { useContext, useRef, useEffect, Fragment } from "react";
import { SessionContext } from "../quiz-manager-renderer";
import { generateColor } from "../utils/common";
import anime from "animejs";

const QuizPlayerViewResultComponent = () => {
    const { afterQuestionResults } = useContext(SessionContext);
    
    const containerUsernamesRef = useRef(null);
    const leaderRef = useRef(null);

    const timeline = anime.timeline({ easing: 'easeOutExpo' });
    const lastItem = afterQuestionResults.length - 1;

    useEffect(() => {
        if (!afterQuestionResults[0].isLast) return;
        timeline
            .add({
                targets: containerUsernamesRef.current.children,
                translateX: [ (el, i) => (i % 2 === 0 ? '-50px' : '50px'), 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
                delay: anime.stagger(100),
            })
            .add({
                targets: leaderRef.current,
                translateY: [ 80, 0 ],
                opacity: { value: [ 0, 1 ], easing: 'linear' },
            }, "-=800");
    }, []);
    
    return (
        <div className="container mt-3">
            <div className="row gx-5 leaderboard-container small" ref={containerUsernamesRef}>
                {afterQuestionResults.slice(0, -1).map((result, i) => (
                    <Fragment key={i}>
                        <div className="col-md-6 p-1">
                            <div className="leaderboard p-3 fs-4 text-break lh-1 fw-bold"
                                style={{ backgroundColor: generateColor(i) }}>
                                {result.Username}
                            </div>
                        </div>
                        <div className="col-md-6 p-1">
                            <div className="leaderboard p-3 fs-4 text-break lh-1 fw-bold"
                                style={{ backgroundColor: generateColor(i) }}>
                                {result.Score} (+ {result.newPoints})
                            </div>
                        </div>
                    </Fragment>
                ))}
            </div>
            {afterQuestionResults[lastItem].CurrentStreak > 0 && <div className="row gx-5 leaderboard-container small">
                <div className="col-12 p-1" ref={leaderRef}>
                    <div className="leaderboard streak p-3 fs-4 text-break lh-1 fw-bold d-flex justify-content-between">
                        <div className="icon-animation">
                            <i className="bi bi-lightning-charge-fill text-warning fs-3 icon-animation"></i>
                        </div>
                        {afterQuestionResults[lastItem].Username}: {afterQuestionResults[lastItem].CurrentStreak}
                        <div className="icon-animation">
                            <i className="bi bi-lightning-charge-fill text-warning fs-3 icon-animation"></i>
                        </div>
                    </div>
                </div>
            </div>}
        </div>
    );
};

export default QuizPlayerViewResultComponent;