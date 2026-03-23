import { useState } from "react";
import type { Story } from "../dto/Story";
import { postAddStory } from "../requests/postAddStory";

export const Add = () => {
    const [name, setName] = useState("");
    const [structure, setStructure] = useState("");
    const [startingSummary, setStartingSummary] = useState("");
    const [introduction, setIntroduction] = useState("");
    const [nodes, setNodes] = useState([{ content: "", transitionTurns: 0, contentTurns: 0 }]);

    const submitStory = async (e: React.SubmitEvent) => {
        e.preventDefault();

        const story: Story = {
            name,
            structure,
            startingSummary,
            introduction,
            nodes: nodes.map(n => { return { content: n.content, transitionTurns: n.transitionTurns, contentTurns: n.contentTurns } })
        };

        await postAddStory(story);

        setName("");
        setStructure("");
        setStartingSummary("");
        setIntroduction("");
        setNodes([{ content: "", transitionTurns: 0, contentTurns: 0 }]);
    };

    const addNode = () => {
        setNodes([...nodes, { content: "", transitionTurns: 0, contentTurns: 0 }]);
    };

    const removeNode = (index: number) => {
        setNodes(nodes.filter((_, i) => i !== index));
    };

    const updateNode = (index: number, field: "content" | "transitionTurns" | "contentTurns", value: string) => {
        const updated = [...nodes];
        updated[index] = {
            ...updated[index],
            [field]: field === "content" ? value : Number(value)
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
                   <textarea value={structure} onChange={e => setStructure(e.target.value)} />
               </label>

                <label>
                    Introduction:
                    <textarea value={introduction} onChange={e => setIntroduction(e.target.value)} />
                </label>

                <label>
                    Starting Summary:
                    <textarea value={startingSummary} onChange={e => setStartingSummary(e.target.value)} />
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
                            Transition Turns:
                            <input
                                type="number"
                                placeholder="Transition Turns"
                                value={node.transitionTurns}
                                onChange={e => updateNode(i, "transitionTurns", e.target.value)}
                            />
                        </label>
                        <label>
                            Content Turns:
                            <input
                                type="number"
                                placeholder="Content Turns"
                                value={node.contentTurns}
                                onChange={e => updateNode(i, "contentTurns", e.target.value)}
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