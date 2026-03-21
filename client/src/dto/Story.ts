import { StoryNode } from "./StoryNode";

export class Story {
    name: string;
    structure: string;
    startingSummary: string;
    introduction: string;
    nodes: StoryNode[];

    constructor(name: string, structure: string, startingSummary: string, introduction: string, nodes: StoryNode[]) {
        this.introduction = introduction;
        this.startingSummary = startingSummary;
        this.name = name;
        this.structure = structure;
        this.nodes = nodes;
    }
}