import type { Story } from "../dto/Story";

export const updateStory = async (storyId: number, story: Story) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    await fetch(`${baseUrl}\stories/${storyId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(story),
    });
}