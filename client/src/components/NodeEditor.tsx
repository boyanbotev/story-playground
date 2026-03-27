import type { StoryNode } from "../dto/StoryNode";


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
        <div className="node">
            <select
                value={node.type}
                onChange={e => update("type", e.target.value)}
            >
                <option value="story">Story Node</option>
                <option value="quest">Quest Node</option>
            </select>

            {node.type === "story" && (
                <>
                    <textarea
                        value={node.content}
                        placeholder="Content"
                        onChange={e => update("content", e.target.value)}
                    />
                    <label>
                        Transition Turns:
                        <input
                            type="number"
                            value={node.transitionTurns ?? 0}
                            onChange={e => update("transitionTurns", e.target.value)}
                        />
                    </label>

                    <label>
                        Content Turns:
                        <input
                            type="number"
                            value={node.contentTurns ?? 0}
                            onChange={e => update("contentTurns", e.target.value)}
                        />
                    </label>
                </>
            )}

            {node.type === "quest" && (
                <>
                    <label>
                        User Goal:
                        <input
                            value={node.userGoal ?? ""}
                            onChange={e => update("userGoal", e.target.value)}
                        />
                    </label>

                    <label>
                        Difficulty:
                        <input
                            value={node.difficulty ?? ""}
                            onChange={e => update("difficulty", e.target.value)}
                        />
                    </label>
                </>
            )}
            <div className="node-buttons">
                <button type="button" onClick={() => onMoveUp()}>
                    ↑
                </button>
                <button type="button" onClick={() => onMoveDown()}>
                    ↓
                </button>
                <button type="button" onClick={() => onRemove()}>Remove</button>
            </div>
        </div>
    );
};