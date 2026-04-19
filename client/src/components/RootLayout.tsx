import { Outlet } from "react-router"
import { Navbar } from "./Navbar"
import { Container, Toolbar } from "@mui/material"

export const RootLayout = () => {
    return (
        <>
            <Navbar />
            <Toolbar /> 
            <Container sx={{ flexGrow: 1, py: 3 }}>
                <Outlet />
            </Container>
        </>
    )
}