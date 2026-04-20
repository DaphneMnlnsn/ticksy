import { ChevronsRight } from "lucide-vue-next";
import api from "./api";

export async function getUsers() {
    const res = await api.get('/Users');
    return res.data || [];
}

export async function archiveUser(id) {
    return await api.delete(`/Users/${id}`);
}

export async function getTeams() {
    const res = await api.get('/Teams');
    
    if (!res.data) return [];

    return res.data.map((item, index) => ({
        id: item.id,
        name: item.teamName,
        label: item.memberCount + " member/s",
        icon: ChevronsRight
    }));
}

export async function getTeam(id) { 
    const res = await api.get(`/Teams/${id}`)
    return res.data
}

export async function createTeam(payload) {
    return await api.post('/Teams/create', payload);
}

export async function updateTeam(id, payload) {
    const res = await api.put(`/Teams/${id}`, payload);
    return res.data;
}

export async function updateUser(id, payload) {
    const res = await api.put(`/Users/${id}`, payload)
    return res.data
}

export async function unassignMember(teamId, memberId) {
    return await api.delete(`/Teams/${teamId}/members/${memberId}`);
}