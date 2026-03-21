export const requestProgress = async (storyId: number, nodeIndex: number, userAction: string, summarySoFar: string, turnsRemaining: number) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\progress`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            storyId,
            nodeIndex,
            userAction,
            summarySoFar,
            turnsRemaining,
        }),
    });
    return await response.json();
}