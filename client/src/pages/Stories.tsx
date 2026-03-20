import { useLoaderData } from "react-router"
import { StoryCard } from "../components/StoryCard"

export type StoryData = {
    id: number,
    name: string,
    structure: string,
    nodes: StoryNodeData[]
}

export type StoryNodeData = {
    id: number,
    content: string,
    turns: number
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