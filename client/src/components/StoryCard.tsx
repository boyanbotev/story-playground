import { NavLink } from "react-router"
import type { StoryData } from "../pages/Stories"
import { NodeCard } from "./NodeCard"
import { deleteStory } from "../requests/deleteStory"

export const StoryCard = ({ story }: { story: StoryData }) => {
    return (
        <div>
            <h2>{story.name}</h2>
            <p>{story.structure}</p>
            {story.nodes.map(n => <NodeCard key={n.id} node={n} />)}
            <NavLink to={`/stories/${story.id}`}>Edit</NavLink>
            <NavLink to={`/stories/${story.id}/play`}>Play</NavLink>
            <button type="button" onClick={() => deleteStory(story.id)}>Delete</button>
        </div>
    )
}