import api from "./api";
import { ChevronsRight } from 'lucide-vue-next'
import { ref } from "vue";

export async function createHoliday(payload) {
    const res = await api.post("/Holidays/create", payload);
    return res.data;
}

export async function getHolidays(id, year) {
    const res = await api.get(`/Holidays?calendarId=${id}&year=${year}`);
    
    if (!res.data) return [];

    return res.data.map((item) => {
        const dateObj = new Date(item.date);
        
        return {
            id: item.id,
            name: item.name,
            date: item.date,
            day: dateObj.toLocaleDateString('en-US', { weekday: 'long' }),
        };
    });
}

export async function updateHoliday(id, payload) {
    const res = await api.put(`/Holidays/${id}`, payload);
    return res.data;
}

export async function deleteHoliday(id) {
    const res = await api.delete(`/Holidays/${id}`);
    return res.data;
}