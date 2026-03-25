import { NavLink, useRevalidator } from "react-router"
import type { Story } from "../dto/Story";
import { NodeCard } from "./NodeCard"
import { deleteStory } from "../requests/deleteStory"

export const StoryCard = ({ story }: { story: Story }) => {

    const { revalidate } = useRevalidator();

    const onClickDelete = async (id: number) => {
        await deleteStory(id);
        revalidate();
    }

    return (
        <div>
            <h2>{story.name}</h2>
            <p>Synopsis: {story.structure}</p>
            {/* <p>Starting Summary: {story.startingSummary}</p> */}
            {/* <p>Introduction: {story.introduction}</p> */}
            {/* <div className="nodes">{story.nodes.map(n => <NodeCard key={n.id} node={n} />)}</div> */}
            <div className="button-panel">
                <NavLink to={`/stories/${story.id}`}>Edit</NavLink>
                <NavLink to={`/stories/${story.id}/play`}>Play</NavLink>
                <button type="button" onClick={() => onClickDelete(story.id!)}>Delete</button>
            </div>

        </div>
    )
}