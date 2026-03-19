import { StoryNode } from "./StoryNode";

export class Story {
    name: string;
    structure: string;
    nodes: StoryNode[];

    constructor(name: string, structure: string, nodes: StoryNode[]) {
        this.name = name;
        this.structure = structure;
        this.nodes = nodes;
    }
}