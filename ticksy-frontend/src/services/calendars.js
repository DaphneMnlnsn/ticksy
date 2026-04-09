import api from "./api";
import { ChevronsRight, Download } from 'lucide-vue-next'
import { ref } from "vue";

export async function createCalendar(payload, country = null) {
    const config = country ? { params: { country } } : {};
    
    const res = await api.post("/Calendars/create", payload, config);
    return res.data;
}

export async function getSupportedCountries() {
    const response = await fetch('https://date.nager.at/api/v3/AvailableCountries');
    if (!response.ok) throw new Error('Failed to fetch countries');
    return await response.json();
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