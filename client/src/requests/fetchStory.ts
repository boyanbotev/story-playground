export const fetchStory = async (storyId: number, token: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}/stories/${storyId}`, { 
        signal,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
    });
    return await response.json();
}