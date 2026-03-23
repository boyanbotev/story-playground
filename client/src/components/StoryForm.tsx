import { useState } from "react";
import type { Story } from "../dto/Story";

type StoryFormProps = {
    initialStory: Partial<Story>;
    onSubmit: (story: Story) => Promise<void>;
};

export const StoryForm = ({ initialStory, onSubmit }: StoryFormProps) => {
    const [name, setName] = useState(initialStory.name ?? "");
    const [structure, setStructure] = useState(initialStory.structure ?? "");
    const [startingSummary, setStartingSummary] = useState(initialStory.startingSummary ?? "");
    const [introduction, setIntroduction] = useState(initialStory.introduction ?? "");
    const [nodes, setNodes] = useState(
        initialStory.nodes ?? [{ content: "", transitionTurns: 0, contentTurns: 0 }]
    );

    const submit = async (e: React.FormEvent) => {
        e.preventDefault();

        const story: Story = {
            ...initialStory, // keeps id if present
            name,
            structure,
            startingSummary,
            introduction,
            nodes: nodes.map(n => ({
                id: n.id,
                content: n.content,
                transitionTurns: n.transitionTurns,
                contentTurns: n.contentTurns
            }))
        };

        await onSubmit(story);
    };

    const addNode = () => {
        setNodes([...nodes, { content: "", transitionTurns: 0, contentTurns: 0 }]);
    };

    const removeNode = (index: number) => {
        setNodes(nodes.filter((_, i) => i !== index));
    };

    const updateNode = (
        index: number,
        field: "content" | "transitionTurns" | "contentTurns",
        value: string
    ) => {
        const updated = [...nodes];
        updated[index] = {
            ...updated[index],
            [field]: field === "content" ? value : Number(value)
        };
        setNodes(updated);
    };

    return (
        <form className="form" onSubmit={submit}>
            <label> Name </label>
            <input value={name} onChange={e => setName(e.target.value)} />
            <label> Structure </label>
            <textarea value={structure} onChange={e => setStructure(e.target.value)} />
            <label> Introduction </label>
            <textarea value={introduction} onChange={e => setIntroduction(e.target.value)} />
            <label> Starting Summary </label>
            <textarea value={startingSummary} onChange={e => setStartingSummary(e.target.value)} />

            <h3>Nodes</h3>
            <div className={"tips"}>
                <i>Node Writing Tips</i>
                <ul>
                    <li>The transition turns are the number of turns it takes for the character to transition to/ foreshadow the content of the node.</li>
                    <li>The content turns is how many turns the game stays on the content of the node.</li>
                    <li>Try to keep the transition turns and content turns as low as possible. (eg. 1 or 2)</li>
                    <li>Avoid introducing objects without being clear about what they are. Eg. prefer "Bashar finds a very valuable pearl" to "Bashar finds a very valuable object"</li>
                </ul>
            </div>
            <div className="nodes">
                {nodes.map((node, i) => (
                    <div className="node" key={i}>
                        <textarea
                            value={node.content}
                            placeholder="Content"
                            onChange={e => updateNode(i, "content", e.target.value)}
                        />
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
            </div>

            <button type="button" onClick={addNode}>Add Node</button>
            <button type="submit">Submit</button>
        </form>
    );
};