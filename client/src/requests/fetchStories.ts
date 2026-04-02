export const fetchStories = async (token: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}/stories`, { 
        signal,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
    });
    return await response.json();
}