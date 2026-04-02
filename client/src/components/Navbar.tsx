import { NavLink } from "react-router";

export function Navbar() {
    return (
        <nav>  
            <NavLink to="/stories">Stories</NavLink>
            <NavLink to="/stories/add">Add Story</NavLink>
            <NavLink to="/login">Login</NavLink>
            <NavLink to="/register">Register</NavLink>
        </nav>
    )
}