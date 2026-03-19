export const fetchStory = async (storyId: number) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories/${storyId}`);
    const txt = await response.text();
    return await txt;
}