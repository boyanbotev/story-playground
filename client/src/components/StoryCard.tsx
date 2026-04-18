import { useRevalidator } from "react-router"
import type { Story } from "../dto/Story";
import { deleteStory } from "../requests/deleteStory"
import { Button, ButtonGroup, Card, Container } from "@mui/material";
import { useNavigate } from "react-router";

export const StoryCard = ({ story }: { story: Story }) => {

    const { revalidate } = useRevalidator();
    const navigate = useNavigate();

    const onClickDelete = async (id: number) => {
        const isConfirmed = window.confirm("Are you sure you want to delete this story?");
        const token = localStorage.getItem("token");

        if (isConfirmed && token) {
            await deleteStory(id, token);
            revalidate();
        }
    }

    return (
        <Card variant="outlined" className="story-card">
            <Container>
                <h2>{story.name}</h2>
                <p>Synopsis: {story.structure}</p>
            </Container>
            <Container className="button-panel" aria-label="Basic button group">
                <Button variant="contained" onClick={() => navigate(`/stories/${story.id}/play`)}>Play</Button>
                <Button variant="contained" onClick={() => navigate(`/stories/${story.id}`)}>Edit</Button>
                <Button variant="contained" onClick={() => onClickDelete(story.id!)}>Delete</Button>
            </Container>
        </Card>
    )
}