import { useLoaderData } from "react-router"
import { StoryCard } from "../components/StoryCard"
import type { Story } from "../dto/Story"

export const Stories = () => {
    let data = useLoaderData();

    return (
        <div>
            <h1>Stories </h1>
            <div>{(data.stories as Story[]).map(story => <StoryCard key={story.id} story={story} />)}</div>
        </div>
    )
}