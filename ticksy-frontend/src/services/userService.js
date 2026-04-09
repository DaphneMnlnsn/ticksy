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

export function changePassword(data) {
    return api.put("/Users/change-pass", data);
}

export async function forgotPassword(email) {  
    const res = await api.post("/Users/forgot-pass", { email });  
    return res.data;  
}

export async function resetPassword(data) {
    const res = await api.put("/Users/reset-pass", data)
    return res.data
}