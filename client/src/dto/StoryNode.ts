export type StoryNode = {
    id?: number;
    type: "story" | "quest";
    content: string;

    transitionTurns?: number;
    contentTurns?: number;

    userGoal?: string;
    difficulty?: string;
}