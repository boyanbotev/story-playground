export const fetchStories = async (token: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    try {
        const response = await fetch(`${baseUrl}/stories`, { 
            signal,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        });
        if (!response.ok) {
            throw new Error('Failed to fetch stories');
        }
        return await response.json();
    } catch (error) {
        console.error(error);
        throw error;
    }
}