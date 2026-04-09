import api from "./api";
import { getUserProfile } from "./userService";

export async function loginUser(email, password) {
    const res = await api.post("/Users/login", { email, password });

    const { token, userId, role } = res.data;

    localStorage.setItem("token", token);
    localStorage.setItem("userId", userId);
    localStorage.setItem("role", role);

    try {
        const profileRes = await getUserProfile(userId);
        const data = profileRes.data;

        const user = {
            firstName: data.firstName || '',
            middleName: data.middleName || '',
            lastName: data.lastName || '',
            name: [data.firstName, data.middleName, data.lastName].filter(Boolean).join(' '),
            email: data.email || '',
            role: data.role || role,       
            avatar: data.avatarUrl || '',
            phone: data.phone || '',
            timezone: data.timeZone || ''
        };

        localStorage.setItem("user", JSON.stringify(user));

        return { token, userId, role, user };
    } catch (err) {
        console.error("Failed to fetch user profile:", err);
        return { token, userId, role }; 
    }
}

export function isAuthenticated() {
    return !!localStorage.getItem('token'); 
}