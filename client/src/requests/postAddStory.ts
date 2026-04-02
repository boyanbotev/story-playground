import type { Story } from "../dto/Story";

export const postAddStory = async (story: Story, token: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    await fetch(`${baseUrl}/stories`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(story),
        signal,
    });
}