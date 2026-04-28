import { useLoaderData } from "react-router"
import { StoryCard } from "../components/StoryCard"
import type { Story } from "../dto/Story"
import { Box, Container, Stack } from "@mui/material"

export const Stories = () => {
    let data = useLoaderData();

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
            <Stack spacing={2}>
                <Container className="stories">{(data.stories as Story[]).map(story => <StoryCard key={story.id} story={story} />)}</Container>
            </Stack>
        </Box>

    )
}