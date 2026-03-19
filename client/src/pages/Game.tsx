import { useState } from 'react';

export const Game = () => {
    const [ runningSummary, setRunningSummary ] = useState<string>();
    const [ turnsRemaing, setTurnsRemaining ] = useState<number>();
    const [ nodeIndex, setNodeIndex ] = useState<number>();

    return (
        <div>
            <h1>Game</h1>
        </div>
    )
}