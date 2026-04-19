import { Card, FormControl, FormLabel, TextField } from "@mui/material";
import type { StoryNode } from "../dto/StoryNode";
import { StyledButton } from "./StyledButton";

type Props = {
    node: StoryNode;
    onChange: (node: StoryNode) => void;
    onRemove: () => void;
    onMoveUp: () => void;
    onMoveDown: () => void;
};

export const NodeEditor = ({ node, onChange, onRemove, onMoveUp, onMoveDown }: Props) => {
    const update = (field: keyof StoryNode, value: any) => {
        onChange({ ...node, [field]: value });
    };

    return (
        <Card className="node">
            <select
                value={node.type}
                onChange={e => update("type", e.target.value)}
            >
                <option value="story">Story Node</option>
                <option value="quest">Quest Node</option>
            </select>

            {node.type === "story" && (
                <>
                    <FormControl>
                        <TextField
                            multiline
                            value={node.content}
                            placeholder="Content"
                            onChange={e => update("content", e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel>
                            Transition Turns:
                        </FormLabel>
                        <TextField
                            type="number"
                            value={node.transitionTurns ?? 0}
                            onChange={e => update("transitionTurns", e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel>
                            Content Turns:
                        </FormLabel>
                        <TextField
                            type="number"
                            value={node.contentTurns ?? 0}
                            onChange={e => update("contentTurns", e.target.value)}
                        />
                    </FormControl>
                </>
            )}

            {node.type === "quest" && (
                <>
                    <label>
                        User Goal:
                        <TextField
                            value={node.userGoal ?? ""}
                            onChange={e => update("userGoal", e.target.value)}
                        />
                    </label>

                    <label>
                        Difficulty:
                        <TextField
                            value={node.difficulty ?? ""}
                            onChange={e => update("difficulty", e.target.value)}
                        />
                    </label>
                </>
            )}
            <div className="node-buttons">
                <StyledButton type="button" onClick={() => onMoveUp()}>
                    ↑
                </StyledButton>
                <StyledButton type="button" onClick={() => onMoveDown()}>
                    ↓
                </StyledButton>
                <StyledButton type="button" onClick={() => onRemove()}>Remove</StyledButton>
            </div>
        </Card>
    );
};