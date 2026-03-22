import { useState } from 'react';
import { useLoaderData } from 'react-router';
import { requestProgress } from '../requests/requestProgress';
import { LoadingAnimation } from '../components/LoadingAnimation';

export const Game = () => {
    const { story } = useLoaderData();
    const [ runningSummary, setRunningSummary ] = useState<string>(story.startingSummary);
    const [ transitionTurnsRemaining, setTransitionTurnsRemaining ] = useState<number>(story.nodes[0].transitionTurns);
    const [ contentTurnsRemaining, setContentTurnsRemaining ] = useState<number>(story.nodes[0].contentTurns);
    const [ nodeIndex, setNodeIndex ] = useState<number>(0);
    const [ action, setAction ] = useState<string>("");
    const [ storyText, setStoryText ] = useState<string>(story.introduction);
    const [ isLoading, setIsLoading ] = useState<boolean>(false);
    const [ error, setError ] = useState<string>("");

    const submitAction = async (e: React.SubmitEvent) => {
        e.preventDefault();
        setIsLoading(true);

        const response = await requestProgress(story.id, nodeIndex, action, runningSummary, transitionTurnsRemaining, contentTurnsRemaining);

        if (response.error) {
            setIsLoading(false);
            setAction("");
            setError(response.error);
            return;
        }

        console.log("request", action, runningSummary, transitionTurnsRemaining, story.nodes[nodeIndex].content, story.structure);
        console.log("response", response);
        
        setRunningSummary(response.summarySoFar);
        setTransitionTurnsRemaining(response.transitionTurnsRemaining);
        setContentTurnsRemaining(response.contentTurnsRemaining);
        setStoryText(response.storyText);
        setAction("");
        setNodeIndex(response.nodeIndex);
        setIsLoading(false);
        setError("");
    }

    return isLoading ? (
        <LoadingAnimation />
    ) : (
        <div>
            <h1>{story.name}</h1>
            <p>{storyText}</p>
            <p className={"error"}>{error}</p>
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