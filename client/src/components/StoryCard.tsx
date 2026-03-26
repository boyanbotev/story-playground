import { NavLink, useRevalidator } from "react-router"
import type { Story } from "../dto/Story";
import { deleteStory } from "../requests/deleteStory"

export const StoryCard = ({ story }: { story: Story }) => {

    const { revalidate } = useRevalidator();

    const onClickDelete = async (id: number) => {
        const isConfirmed = window.confirm("Are you sure you want to delete this story?");

        if (isConfirmed) {
            await deleteStory(id);
            revalidate();
        }
    }

    return (
        <div>
            <h2>{story.name}</h2>
            <p>Synopsis: {story.structure}</p>
            <div className="button-panel">
                <NavLink to={`/stories/${story.id}`}>Edit</NavLink>
                <NavLink to={`/stories/${story.id}/play`}>Play</NavLink>
                <button type="button" onClick={() => onClickDelete(story.id!)}>Delete</button>
            </div>

        </div>
    )
}