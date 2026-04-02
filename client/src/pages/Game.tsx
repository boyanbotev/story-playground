import { useState, useRef, useEffect } from 'react';
import { useLoaderData } from 'react-router';
import { requestProgress, type ProgressRequestProps } from '../requests/requestProgress';
import { LoadingAnimation } from '../components/LoadingAnimation';

export const Game = () => {
    const { story } = useLoaderData();
    const [ runningSummary, setRunningSummary ] = useState<string>(story.startingSummary);
    const [ transitionTurnsRemaining, setTransitionTurnsRemaining ] = useState<number>(story.nodes[0].transitionTurns);
    const [ contentTurnsRemaining, setContentTurnsRemaining ] = useState<number>(story.nodes[0].contentTurns);
    const [ goal, setGoal ] = useState<string>(story.nodes[0].userGoal);
    const [ questCompleteText, setQuestCompleteText ] = useState<string | null>(null);
    const [ nodeIndex, setNodeIndex ] = useState<number>(0);
    const [ action, setAction ] = useState<string>("");
    const [ storyText, setStoryText ] = useState<string>(story.introduction);
    const [ isLoading, setIsLoading ] = useState<boolean>(false);
    const [ isStoryComplete, setIsStoryComplete ] = useState<boolean>(false);
    const [ error, setError ] = useState<string>("");
    const controllerRef = useRef<AbortController | null>(null);

    const submitAction = async (e: React.SubmitEvent) => {
        if (!action.trim()) return;

        e.preventDefault();

        if (controllerRef.current) controllerRef.current.abort();
        var controller = new AbortController();
        controllerRef.current = controller;
        setIsLoading(true);

        try {
            var currentNode = story.nodes[nodeIndex];
            let progressRequest: ProgressRequestProps = {                 
                storyId: story.id,
                nodeIndex: nodeIndex,
                userAction: action,
                summarySoFar: runningSummary,
            };
            if (currentNode.$type == "story") {
                progressRequest = {...progressRequest, transitionTurnsRemaining, contentTurnsRemaining };
            }
            
            console.log("request", progressRequest);

            const token = localStorage.getItem("token");
            var response = await requestProgress(progressRequest, token!);

            if (controller.signal.aborted) return;

            if (response.error) {
                setIsLoading(false);
                setAction("");
                setError(response.error);
                return;
            }

            if (response.completed) {
                setIsLoading(false);
                setAction("");
                setStoryText(response.storyText);
                setIsStoryComplete(true);
                return;
            }

            if (response.questCompleteText) setQuestCompleteText(response.questCompleteText);
            else setQuestCompleteText(null);

            console.log("response", response);
            
            setRunningSummary(response.summarySoFar);
            setTransitionTurnsRemaining(response.transitionTurnsRemaining);
            setContentTurnsRemaining(response.contentTurnsRemaining);
            setStoryText(response.storyText);
            setAction("");
            setNodeIndex(response.nodeIndex);
            setIsLoading(false);
            setError("");
            setGoal(response.userGoal);
        } catch (err: any) {
            if (err.name === "AbortError") {
                console.log("aborted");
                return;
            }        
            console.error(err);
            setError("Something went wrong");
        } finally {
            if (!controller.signal.aborted) {
                setIsLoading(false);
            }
        }
    }

    useEffect(() => {
        return () => {
            if (controllerRef.current) {
                controllerRef.current.abort();
            }
        };
    }, []);

    return isLoading ? (
        <LoadingAnimation />
    ) : isStoryComplete ? (
        <div>
            <h1>{story.name}</h1>
            <p>{storyText}</p>
            <b>THE END</b>
            <p><i>You have completed the story!</i></p>
        </div>
    ) : (
        <div>
            <h1>{story.name}</h1>
            {story.nodes[nodeIndex].$type == "quest" ? (
                <div className='goal'><p>Goal: {goal}</p></div>
            ) : null}
            {questCompleteText ? (
                <div className='quest-complete'><p>{questCompleteText}</p></div>
            ) : null}
            <p className={"error"}>{error}</p>
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