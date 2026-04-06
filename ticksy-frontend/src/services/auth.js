import api from "./api";

export async function loginUser(email, password) {
    const res = await api.post("/Users/login", {
        email,
        password
    });

    const { token, userId, role } = res.data;

    localStorage.setItem("token", token);
    localStorage.setItem("userId", userId);
    localStorage.setItem("role", role);

    return res.data;
}