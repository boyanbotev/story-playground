import { NavLink, useRevalidator } from "react-router"
import type { StoryData } from "../pages/Stories"
import { NodeCard } from "./NodeCard"
import { deleteStory } from "../requests/deleteStory"

export const StoryCard = ({ story }: { story: StoryData }) => {

    const { revalidate } = useRevalidator();

    const onClickDelete = async (id: number) => {
        await deleteStory(id);
        revalidate();
    }

    return (
        <div>
            <h2>{story.name}</h2>
            <p>{story.structure}</p>
            <div className="nodes">{story.nodes.map(n => <NodeCard key={n.id} node={n} />)}</div>
            <NavLink to={`/stories/${story.id}`}>Edit</NavLink>
            <NavLink to={`/stories/${story.id}/play`}>Play</NavLink>
            <button type="button" onClick={() => onClickDelete(story.id)}>Delete</button>
        </div>
    )
}