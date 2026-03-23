import type { StoryNode } from "../dto/StoryNode";

export const NodeCard = ({ node }: { node: StoryNode }) => {
    return (
        <div className="node">
            <p>Content: {node.content}</p>
            <p>Transition Turns: {node.transitionTurns}</p>
            <p>Content Turns: {node.contentTurns}</p>
        </div>
    )
}