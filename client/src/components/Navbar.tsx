import { NavLink } from "react-router";
import { AppBar, Container, Box, Toolbar, Button } from "@mui/material";
import { StyledButton } from "./StyledButton";
import { styled, alpha } from '@mui/material/styles';

export function Navbar() {

    const StyledToolbar = styled(Toolbar)(({ theme }) => ({
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        flexShrink: 0,
        borderRadius: `calc(${theme.shape.borderRadius}px + 8px)`,
        backdropFilter: 'blur(24px)',
        border: '1px solid',
        borderColor: (theme.vars || theme).palette.divider,
        backgroundColor: theme.vars
            ? `rgba(${theme.vars.palette.background.defaultChannel} / 0.4)`
            : alpha(theme.palette.background.default, 0.4),
        boxShadow: (theme.vars || theme).shadows[1],
        padding: '8px 12px',
    }));

    return (
        <AppBar
        position="fixed"
        enableColorOnDark
        sx={{
            boxShadow: 0,
            bgcolor: 'transparent',
            backgroundImage: 'none',
            mt: 'calc(var(--template-frame-height, 0px) + 28px)',
        }}
        >
            <Container maxWidth="lg">
                <StyledToolbar>
                    <Box sx={{ flexGrow: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', px: 0 }}>
                        <Box sx={{ display: { xs: 'none', md: 'flex' } }}>
                            <StyledButton component={NavLink} to={"/stories"}>
                                Stories
                            </StyledButton>
                            <StyledButton component={NavLink} to={"/stories/add"}>
                                Add Story
                            </StyledButton>
                            <StyledButton component={NavLink} to={"/login"}>
                                Login
                            </StyledButton>
                            <StyledButton component={NavLink} to={"/register"}>
                                Register
                            </StyledButton>
                        </Box>
                    </Box>
                </StyledToolbar>    
            </Container>
        </AppBar>
    )
}