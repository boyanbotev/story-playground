import { useState } from "react";
import { Story } from "../dto/Story";
import { postAddStory } from "../requests/postAddStory";
import { StoryNode } from "../dto/StoryNode";

export const Add = () => {
    const [name, setName] = useState("");
    const [structure, setStructure] = useState("");
    const [startingSummary, setStartingSummary] = useState("");
    const [introduction, setIntroduction] = useState("");
    const [nodes, setNodes] = useState([{ content: "", turns: 0 }]);

    const submitStory = async (e: React.SubmitEvent) => {
        e.preventDefault();

        const story = new Story(
            name,
            structure,
            startingSummary,
            introduction,
            nodes.map(n => new StoryNode(n.content, n.turns)),
        );

        await postAddStory(story);

        setName("");
        setStructure("");
        setNodes([{ content: "", turns: 0 }]);
    };

    const addNode = () => {
        setNodes([...nodes, { content: "", turns: 0 }]);
    };

    const removeNode = (index: number) => {
        setNodes(nodes.filter((_, i) => i !== index));
    };

    const updateNode = (index: number, field: "content" | "turns", value: string) => {
        const updated = [...nodes];
        updated[index] = {
            ...updated[index],
            [field]: field === "turns" ? Number(value) : value
        };
        setNodes(updated);
    };

    return (
        <div>
            <h1>Add</h1>
            <form className="form" onSubmit={submitStory}>
                <label>
                    Story Name:
                    <input value={name} onChange={e => setName(e.target.value)} />
                </label>
                
                <label>
                   Structure:
                   <input value={structure} onChange={e => setStructure(e.target.value)} />
               </label>

                <label>
                    Introduction:
                    <input value={introduction} onChange={e => setIntroduction(e.target.value)} />
                </label>

                <label>
                    Starting Summary:
                    <input value={startingSummary} onChange={e => setStartingSummary(e.target.value)} />
                </label>

                <h3>Nodes</h3>

                {nodes.map((node, i) => (
                    <div key={i}>
                        <label>
                            Content:
                            <input
                                placeholder="Content"
                                value={node.content}
                                onChange={e => updateNode(i, "content", e.target.value)}
                            />
                        </label>
                        <label>
                            Turns:
                            <input
                                type="number"
                                placeholder="Turns"
                                value={node.turns}
                                onChange={e => updateNode(i, "turns", e.target.value)}
                            />
                        </label>

                        <button type="button" onClick={() => removeNode(i)}>Remove</button>
                    </div>
                ))}

                <button type="button" onClick={addNode}>Add Node</button>
                <button type="submit">Submit</button>
            </form>
        </div>
    )
}