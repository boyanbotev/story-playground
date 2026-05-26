export const login = async (username: string, password: string, signal?: AbortSignal) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}/auth/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            userName: username,
            password: password,
        }),
        signal,
    });
    if (!response.ok) return { error: "Invalid username or password" };
    return await response.json();
}