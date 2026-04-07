import api from "./api";

export async function registerUser(userData) {
    const res = await api.post("/Users/register", userData);
    return res.data;
}

export function getUserProfile(id) {
    return api.get(`/Users/${id}`)
}

export function updateUserProfile(id, updatedData) {
    return api.put(`/Users/${id}`, updatedData);
}
