import { useLoaderData } from "react-router"

export const Stories = () => {
    let data = useLoaderData();
    return (
        <div>
            <h1>Stories </h1>
            <div>{data.stories}</div>
        </div>
    )
}
// TODO, display all stories with links to edit and play
// and delete