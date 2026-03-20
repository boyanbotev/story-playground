export const fetchStories = async () => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories`);
    return await response.json();
}