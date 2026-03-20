import { useState } from 'react';
import { useLoaderData } from 'react-router';
import { requestProgress } from '../requests/requestProgress';

export const Game = () => {
    const { story } = useLoaderData();
    const [ runningSummary, setRunningSummary ] = useState<string>("");
    const [ turnsRemaing, setTurnsRemaining ] = useState<number>(0);
    const [ nodeIndex, setNodeIndex ] = useState<number>(0);
    const [ action, setAction ] = useState<string>("");
    const [ storyText, setStoryText ] = useState<string>("");

    const submitAction = async (e: React.SubmitEvent) => {
        e.preventDefault();

        const response = await requestProgress(story.id, action, runningSummary, turnsRemaing, story.nodes[nodeIndex].content, story.structure);
        setRunningSummary(response.summarySoFar);
        setTurnsRemaining(response.turnsRemaining);
        setStoryText(response.storyText);
        setAction("");
        // TODO: nodeIndex
        // starting introduction
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