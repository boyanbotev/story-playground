import type { StoryNodeData } from "../pages/Stories"

export const NodeCard = ({ node }: { node: StoryNodeData }) => {
    return (
        <div className="node">
            <p>Content: {node.content}</p>
            <p>Turns: {node.turns}</p>
        </div>
    )
}