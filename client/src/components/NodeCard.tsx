import type { StoryNodeData } from "../pages/Stories"

export const NodeCard = ({ node }: { node: StoryNodeData }) => {
    return (
        <div>
            <h4>NodeCard</h4>
            <p>{node.content}</p>
            <p>{node.turns}</p>
        </div>
    )
}