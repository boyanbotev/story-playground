import type { Story } from "../dto/Story";

export const updateStory = async (storyId: number, token: string, story: Story, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    await fetch(`${baseUrl}/stories/${storyId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(story),
        signal,
    });
}