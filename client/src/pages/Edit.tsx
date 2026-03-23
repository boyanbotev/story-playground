import { useNavigate, useLoaderData } from "react-router"
import type { Story } from "../dto/Story";
import type { StoryNode } from "../dto/StoryNode";
import { useState } from "react";
import { updateStory } from "../requests/updateStory";

export const Edit = () => {
    let data = useLoaderData();
    let navigate = useNavigate();
    const [name, setName] = useState<string>(data.story.name);
    const [structure, setStructure] = useState<string>(data.story.structure);
    const [startingSummary, setStartingSummary] = useState<string>(data.story.startingSummary);
    const [introduction, setIntroduction] = useState<string>(data.story.introduction);
    const [nodes, setNodes] = useState<StoryNode[]>(data.story.nodes);

    const submitStory = async (e: React.SubmitEvent) => {
        e.preventDefault();

        const story: Story = {
            id: data.story.id,
            name,
            structure,
            startingSummary,
            introduction,
            nodes: nodes.map(n => { return { id: n.id, content: n.content, transitionTurns: n.transitionTurns, contentTurns: n.contentTurns } })
        };

        await updateStory(data.story.id, story);
        navigate("/stories");
    };

    const addNode = () => {
        setNodes([...nodes, { id: nodes.length, content: "", transitionTurns: 0, contentTurns: 0 }]);
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
            <h1>Story Form</h1>
                <form className="form" onSubmit={submitStory}>
                <label>
                    Story Name:
                    <textarea value={name} onChange={e => setName(e.target.value)} />
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

                {nodes.map((node: StoryNode, i: number) => (
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
                                placeholder="TransitionTurns"
                                value={node.transitionTurns}
                                onChange={e => updateNode(i, "transitionTurns", e.target.value)}
                            />
                        </label>
                        <label>
                            Content Turns:
                            <input
                                type="number"
                                placeholder="ContentTurns"
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
