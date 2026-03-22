import { useLoaderData } from "react-router"
import { StoryCard } from "../components/StoryCard"

export type StoryData = {
    id: number,
    name: string,
    structure: string,
    startingSummary: string,
    introduction: string,
    nodes: StoryNodeData[]
}

export type StoryNodeData = {
    id: number,
    content: string,
    transitionTurns: number,
    contentTurns: number,
}

export const Stories = () => {
    let data = useLoaderData();

    return (
        <div>
            <h1>Stories </h1>
            <div>{(data.stories as StoryData[]).map(story => <StoryCard key={story.id} story={story} />)}</div>
        </div>
    )
}