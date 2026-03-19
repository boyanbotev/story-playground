import { Outlet } from "react-router"
import { Navbar } from "./Navbar"

export const RootLayout = () => {
    return (
        <>
            <Navbar />
            <Outlet />
        </>
    )
}