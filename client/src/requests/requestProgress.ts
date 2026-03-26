export type ProgressRequestProps = {
    storyId: number;
    nodeIndex: number;
    userAction: string;
    summarySoFar: string;
    transitionTurnsRemaining?: number;
    contentTurnsRemaining?: number;
}

export const requestProgress = async (props: ProgressRequestProps) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\progress`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            ...props,
        }),
    });
    return await response.json();
}