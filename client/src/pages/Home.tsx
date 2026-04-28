import { Typography, Container, Card, List, ListItem } from "@mui/material";

export const Home = () => {
    return (
        <Container maxWidth="sm">
            <Typography
                component="h1"
                variant="h4"
                sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
            >
                Story Playground
            </Typography>
            <img src="book.png" alt="book" width="100%" className="decorative-image" />
            <Typography variant="h5" sx={{ width: '100%', fontSize: 'clamp(1rem, 10vw, 1.5rem)' }}>
                Features
            </Typography>
            <List disablePadding>
                <ListItem>
                    Preplanned events
                </ListItem>
                <ListItem>
                    Quests where the user can achieve a goal using their own ingenuity and their own ideas
                </ListItem>
                <ListItem>
                    Play the stories using an LLM running free and more environmentally friendly on your own computer
                </ListItem>    
            </List>
        </Container>
    )
}