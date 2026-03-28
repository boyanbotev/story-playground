import { useState } from "react";
import type { Story } from "../dto/Story";
import type { StoryNode } from "../dto/StoryNode";
import { NodeEditor } from "./NodeEditor";

type StoryFormProps = {
    initialStory: Partial<Story>;
    onSubmit: (story: Story) => Promise<void>;
};

type StoryNodeWithId = StoryNode & { idString: string };

export const StoryForm = ({ initialStory, onSubmit }: StoryFormProps) => {
    const [name, setName] = useState(initialStory.name ?? "");
    const [structure, setStructure] = useState(initialStory.structure ?? "");
    const [startingSummary, setStartingSummary] = useState(initialStory.startingSummary ?? "");
    const [introduction, setIntroduction] = useState(initialStory.introduction ?? "");
    const [mainCharacterName, setMainCharacterName] = useState(initialStory.mainCharacterName ?? "");
    const [nodes, setNodes] = useState<StoryNodeWithId[]>(
        initialStory.nodes?.map(n => ({ ...n, idString: crypto.randomUUID(), type: n.type ?? (n.difficulty ? "quest" : "story") })) 
            ?? [{ idString: crypto.randomUUID(), type: "story", content: "", transitionTurns: 0, contentTurns: 0 }]
    );

    const submit = async (e: React.FormEvent) => {
        e.preventDefault();

        const story: Story = {
            ...initialStory, // keeps id if present
            name,
            structure,
            startingSummary,
            introduction,
            mainCharacterName,
            nodes: nodes.map(n => {
                if (n.type === "story") {
                    return {
                        type: "story",
                        content: n.content,
                        transitionTurns: n.transitionTurns,
                        contentTurns: n.contentTurns
                    };
                }

                if (n.type === "quest") {
                    return {
                        type: "quest",
                        userGoal: n.userGoal,
                        difficulty: n.difficulty
                    };
                }

                throw new Error("Unknown node type");
            })
        };
        await onSubmit(story);
    };

    const moveNode = (from: number, to: number) => {
        if (to < 0 || to >= nodes.length) return;

        const updated = [...nodes];
        const [moved] = updated.splice(from, 1);
        updated.splice(to, 0, moved);

        setNodes(updated);
    };

    const addNode = () => {
        setNodes([...nodes, { idString: crypto.randomUUID(), type: "story", content: "", transitionTurns: 0, contentTurns: 0 }]);
    };

    const removeNode = (index: number) => {
        setNodes(nodes.filter((_, i) => i !== index));
    };

    const updateNode = (
        index: number,
        updatedNode: StoryNodeWithId,
    ) => {
        const updated = [...nodes];

        updated[index] = updatedNode;

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
            <label> Main Character Name </label>
            <input value={mainCharacterName} onChange={e => setMainCharacterName(e.target.value)} />
            <label> Starting Summary </label>
            <textarea value={startingSummary} onChange={e => setStartingSummary(e.target.value)} />

            <h3>Nodes</h3>
            <div className={"tips"}>
                <i>Node Writing Tips</i>
                <ul>
                    <h4>StoryNodes</h4>
                    <li>The transition turns are the number of turns it takes for the character to transition to/ foreshadow the content of the node.</li>
                    <li>The content turns is how many turns the game stays on the content of the node.</li>
                    <li>Try to keep the transition turns and content turns as low as possible. (eg. 1 or 2)</li>
                    <li>Avoid introducing objects without being clear about what they are. Eg. prefer "Bashar finds a very valuable pearl" to "Bashar finds a very valuable object"</li>
                    <li>Use named characters or very specific descriptions (the one-eyed man) to avoid confusing the LLM</li>
                    <li>Finish with a Story node, not a Quest node.</li>
                </ul>
            </div>
            <div className="nodes">
                {nodes.map((node, i) => (
                    <NodeEditor
                        key={node.idString}
                        node={node}
                        onChange={(updated) => updateNode(i, { ...nodes[i], ...updated })}
                        onRemove={() => removeNode(i)}
                        onMoveUp={() => moveNode(i, i - 1)}
                        onMoveDown={() => moveNode(i, i + 1)}
                    />
                ))}
            </div>

            <button type="button" onClick={addNode}>Add Node</button>
            <button type="submit">Submit</button>
        </form>
    );
};