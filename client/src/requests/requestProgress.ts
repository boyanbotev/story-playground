export const requestProgress = async (storyId: number, userAction: string, summarySoFar: string, turnsRemaining: number, nextNode: string, storyStructure: string) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\progress`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            storyId,
            userAction,
            summarySoFar,
            turnsRemaining,
            nextNode,
            storyStructure,
        }),
    });
    return await response.json();
}