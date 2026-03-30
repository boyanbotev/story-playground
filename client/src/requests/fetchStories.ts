export const fetchStories = async (signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}/stories`, { signal });
    return await response.json();
}