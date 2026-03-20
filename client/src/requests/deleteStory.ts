export const deleteStory = async (id: number) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories/${id}`, {
        method: 'DELETE',
    });
}