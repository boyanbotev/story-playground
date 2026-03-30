export const fetchStory = async (storyId: number, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}/stories/${storyId}`, { signal });
    return await response.json();
}