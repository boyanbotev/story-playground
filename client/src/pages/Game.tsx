import { useState } from 'react';
import { useLoaderData } from 'react-router';
import { requestProgress } from '../requests/requestProgress';

export const Game = () => {
    const { story } = useLoaderData();
    const [ runningSummary, setRunningSummary ] = useState<string>(story.startingSummary);
    const [ turnsRemaing, setTurnsRemaining ] = useState<number>(story.nodes[0].turns);
    const [ nodeIndex, setNodeIndex ] = useState<number>(0);
    const [ action, setAction ] = useState<string>("");
    const [ storyText, setStoryText ] = useState<string>(story.introduction);

    const submitAction = async (e: React.SubmitEvent) => {
        e.preventDefault();

        const response = await requestProgress(story.id, nodeIndex, action, runningSummary, turnsRemaing);
        console.log("request", action, runningSummary, turnsRemaing, story.nodes[nodeIndex].content, story.structure);
        console.log("response", response);
        
        setRunningSummary(response.summarySoFar);
        setTurnsRemaining(response.turnsRemaining);
        setStoryText(response.storyText);
        setAction("");
        setNodeIndex(response.nodeIndex);
        // TODO:
        // loading graphic while waiting for response
    }

    return (
        <div>
            <h1>{story.name}</h1>
            <p>{storyText}</p>
            <form className="form" onSubmit={submitAction}>
                <label>
                    Write your action here:
                    <input value={action} onChange={e => setAction(e.target.value)} />
                </label>
                <button type="submit">Submit</button>
            </form>
        </div>
    )
}