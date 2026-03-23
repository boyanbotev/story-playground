import { useNavigate, useLoaderData } from "react-router"
import { updateStory } from "../requests/updateStory";
import { StoryForm } from "../components/StoryForm";

export const Edit = () => {
    const data = useLoaderData();
    const navigate = useNavigate();

    return (
        <StoryForm
            initialStory={data.story}
            onSubmit={async (story) => {
                await updateStory(data.story.id, story);
                navigate("/stories");
            }}
        />
    );
};