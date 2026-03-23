import type { StoryNode } from "./StoryNode";

export type Story = {
    id?: number,
    name: string,
    structure: string,
    startingSummary: string,
    introduction: string,
    nodes: StoryNode[]
}