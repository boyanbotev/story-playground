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
                const token = localStorage.getItem("token");
                if (token) {
                    await updateStory(data.story.id, token!, story);
                    navigate("/stories");
                } else {
                    navigate("/login");
                }
            }}
        />
    );
};