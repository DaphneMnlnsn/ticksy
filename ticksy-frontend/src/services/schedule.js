import api from "./api";
import { ChevronsRight } from 'lucide-vue-next'
import { ref } from "vue";

export async function createSchedule(payload) {
    const res = await api.post("/Schedules/create", payload);
    return res.data;
}

export async function getSchedules() {
    const res = await api.get('/Schedules');
    
    if (!res.data) return [];

    return res.data.map((item, index) => ({
        id: item.id,
        name: item.scheduleName,
        label: index === 0 ? 'Default' : undefined,
        icon: ChevronsRight
    }));
}

export async function getScheduleById(id) {
    const res = await api.get(`/Schedules/${id}`);
    return res.data; 
}

export async function updateSchedule(id, payload) {
    const res = await api.put(`/Schedules/${id}`, payload);
    return res.data;
}

export async function assignMembers(scheduleId, memberIds) {
    const res = await api.post(`/Schedules/${scheduleId}/assign-members`, {
        memberIds: memberIds
    });
    return res.data;
}

export async function unassignMember(scheduleId, memberId) {
    const res = await api.delete(`/Schedules/${scheduleId}/members/${memberId}`);
    return res.data;
}

export async function deleteSchedule(id) {
    const res = await api.delete(`/Schedules/${id}`);
    return res.data;
}

export async function addBreak(payload) {
    const res = await api.post("/Schedules/add-break", payload);
    return res.data;
}