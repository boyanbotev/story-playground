import type { StoryNodeData } from "../pages/Stories"

export const NodeCard = ({ node }: { node: StoryNodeData }) => {
    return (
        <div className="node">
            <p>Content: {node.content}</p>
            <p>Transition Turns: {node.transitionTurns}</p>
            <p>Content Turns: {node.contentTurns}</p>
        </div>
    )
}