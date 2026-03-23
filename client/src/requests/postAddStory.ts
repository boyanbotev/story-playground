import type { Story } from "../dto/Story";

export const postAddStory = async (story: Story) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(story),
    });
    return await response.json();
}