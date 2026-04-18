import { useState } from "react";
import type { Story } from "../dto/Story";
import type { StoryNode } from "../dto/StoryNode";
import { NodeEditor } from "./NodeEditor";
import { TextField, Button, FormControl, FormLabel, Container, Box } from "@mui/material";

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
    const [isSubmitting, setIsSubmitting] = useState(false);

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

        if (isSubmitting) return;
        
        setIsSubmitting(true);
        await onSubmit(story);
        setIsSubmitting(false);
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
        <Container maxWidth="sm">
            <Box component="form" className="form" onSubmit={submit} noValidate sx={{ mt: 1 }}>
                <FormControl>
                    <FormLabel> Name </FormLabel>
                    <TextField value={name} onChange={e => setName(e.target.value)} />
                </FormControl>
                <FormControl>
                    <FormLabel> Structure </FormLabel>
                    <TextField multiline value={structure} onChange={e => setStructure(e.target.value)} />
                </FormControl>
                <FormControl>
                    <FormLabel> Introduction </FormLabel>
                    <TextField multiline value={introduction} onChange={e => setIntroduction(e.target.value)} />
                </FormControl>
                <FormControl>
                    <FormLabel> Main Character Name </FormLabel>
                    <TextField value={mainCharacterName} onChange={e => setMainCharacterName(e.target.value)} />
                </FormControl>
                <FormControl>
                    <FormLabel> Starting Summary </FormLabel>
                    <TextField multiline value={startingSummary} onChange={e => setStartingSummary(e.target.value)} />
                </FormControl>

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

                <Button type="button" onClick={addNode}>Add Node</Button>
                <Button type="submit">Submit</Button>
            </Box>
        </Container>

    );
};