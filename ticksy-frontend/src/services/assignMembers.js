import api from "./api";

export async function getUsers() {
    const res = await api.get("/Users");
    return res.data;
}

export async function assignMembers(scheduleId, memberIds) {
    const res = await api.post(
        `/Schedules/${scheduleId}/assign-members`,
        {
            memberIds
        }
    );

    return res.data;
}