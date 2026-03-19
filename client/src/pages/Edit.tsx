import { use, useEffect } from "react";
import { useLoaderData } from "react-router"

export const Edit = () => {
    let data = useLoaderData();

    useEffect(() => {
        console.log(data.story);
    }, []);
    
    return (
        <div>
            <h1>Story Form</h1>
            <div>{data.story}</div>
        </div>
    )
}
// TODO: form to edit story