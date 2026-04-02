export const deleteStory = async (id: number, token: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    await fetch(`${baseUrl}/stories/${id}`, {
        method: 'DELETE',
        signal,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
    });
}