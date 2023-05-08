import { useContext } from "react";
import { SessionContext } from "../quiz-manager-renderer";

import BanUserFromSessionButtonComponent from "./BanUserFromSessionButtonComponent";
import UnbanUserFromSessionButtonComponent from "./UnbanUserFromSessionButtonComponent";
import RemoveUserFromSessionButtonComponent from "./RemoveUserFromSessionButtonComponent";

const SessionParticipantsComponent = () => {
    const { allParticipants } = useContext(SessionContext);
    
    const allConnected = allParticipants.Connected.map(name => (
        <li className="h6 list-group-item bg-transparent border-0 px-1 py-1 mb-0" key={name}>
            <div className="container p-0">
                <div className="d-flex justify-content-between text-break">
                    <div className="quiz-color-text d-flex align-items-center lh-1">{name}</div>
                    <div>
                        <BanUserFromSessionButtonComponent name={name}/>
                        <RemoveUserFromSessionButtonComponent name={name}/>
                    </div>
                </div>
            </div>
        </li>
    ));

    const allDisconnected = allParticipants.Disconnected.map(name => (
        <li className="h6 list-group-item bg-transparent border-0 px-1 py-1 mb-0 quiz-color-text" key={name}>{name}</li>
    ));

    const allBanned = allParticipants.Banned.map(name => (
        <li className="h6 list-group-item bg-transparent border-0 px-1 py-1 mb-0" key={name}>
            <div className="container p-0">
                <div className="d-flex justify-content-between text-break">
                    <div className="quiz-color-text d-flex align-items-center lh-1">{name}</div>
                    <UnbanUserFromSessionButtonComponent name={name}/>
                </div>
            </div>
        </li>
    ));
    
    return (
        <>
            <h6 className="text-black-50 mb-1">Połączeni: ({allParticipants.Connected.length})</h6>
            {allParticipants.Connected.length > 0 &&
                <ul className="fw-bold list-group">{allConnected}</ul>}
            <h6 className="text-black-50 mt-3 mb-1">Rozłączeni: ({allParticipants.Disconnected.length})</h6>
            {allParticipants.Disconnected.length > 0 && <ul className="fw-bold list-group">{allDisconnected}</ul>}
            <h6 className="text-black-50 mt-3 mb-1">Zbanowani: ({allParticipants.Banned.length})</h6>
            {allParticipants.Banned.length > 0 && <ul className="fw-bold list-group">{allBanned}</ul>}
        </>
    );
};

export default SessionParticipantsComponent;