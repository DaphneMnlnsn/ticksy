import api from "./api";
import { ChevronsRight, Download } from 'lucide-vue-next'
import { ref } from "vue";

export async function createCalendar(payload) {
    const res = await api.post("/Calendars/create", payload);
    return res.data;
}

export async function getCalendars() {
    const res = await api.get('/Calendars');
    
    if (!res.data) return [];

    return res.data.map((item, index) => ({
        id: item.id,
        name: item.name,
        label: item.source,
        icon: ChevronsRight
    }));
}

export async function importHolidays() {
    const res = await api.get(`/Calendars/${id}/import`);
    return res.data; 
}

export async function deleteCalendar(id) {
    const res = await api.delete(`/Calendars/${id}`);
    return res.data;
}