import { useLoaderData } from "react-router"
import { StoryCard } from "../components/StoryCard"
import type { Story } from "../dto/Story"
import { Box, Container, Stack, Typography } from "@mui/material"

export const Stories = () => {
    let data = useLoaderData();

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
            {/* <Container sx={{ textAlign: 'center', marginTop: '2rem' }}>
                <Typography
                    component="h1"
                    variant="h4"
                    sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
                >
                    Stories
                </Typography>
            </Container> */}
            <Stack spacing={2}>
                <Container className="stories">{(data.stories as Story[]).map(story => <StoryCard key={story.id} story={story} />)}</Container>
            </Stack>
        </Box>

    )
}