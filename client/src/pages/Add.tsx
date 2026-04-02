import { StoryForm } from "../components/StoryForm";
import { postAddStory } from "../requests/postAddStory";
import { useNavigate } from "react-router";

export const Add = () => {
    const navigate = useNavigate();
    
    return (
        <StoryForm
            initialStory={{}}
            onSubmit={async (story) => {
                const token = localStorage.getItem("token");
                if (token) {
                    await postAddStory(story, token!);
                    navigate("/stories");
                } else {
                    navigate("/login");
                }
            }}
        />
    );
};