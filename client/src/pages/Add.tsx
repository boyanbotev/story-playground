import { useRef } from "react";
import { Story } from "../dto/Story";
import { postAddStory } from "../requests/postAddStory";

export const Add = () => {
    const name = useRef<HTMLInputElement | null>(null);
    const structure = useRef<HTMLInputElement | null>(null);

    const submitStory = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        
        const story = new Story(name.current!.value, structure.current!.value, []);
        await postAddStory(story);

        name.current!.value = "";
        structure.current!.value = "";
    }

    return (
        <div>
            <h1>Add</h1>
            <form className="form" onSubmit={submitStory}>
                <label>
                    Story Name:
                    <input type="text" name="name" ref={name} />
                </label>
               
               <label>
                   Structure:
                   <input type="text" name="structure" ref={structure}/>
               </label>
               <label>
                   Nodes:
                   // TODO: add nodes
                   
               </label>
                <button type="submit">Submit</button>
            </form>
        </div>
    )
}