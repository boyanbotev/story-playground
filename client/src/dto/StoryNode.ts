export class StoryNode {
    content: string;
    transitionTurns: number;
    contentTurns: number;

    constructor(content: string, transitionTurns: number, contentTurns: number) {
        this.content = content;
        this.transitionTurns = transitionTurns;
        this.contentTurns = contentTurns;
    }
}