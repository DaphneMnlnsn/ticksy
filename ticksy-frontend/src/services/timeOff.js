import api from "./api";
import { ChevronsRight } from 'lucide-vue-next'
import { ref } from "vue";

export async function createRequest(payload) {
    const res = await api.post("/Requests/create", payload);
    return res.data;
}

export async function getRequests() {
    const res = await api.get('/Requests');
    
    if (!res.data) return [];

    return res.data.map((item, index) => ({
        id: item.id,
        name: item.fullName,
        type: item.leaveType,
        reason: item.reason,
        r_date: item.startDate + " - " + item.endDate,
        status: item.status
    }));
}

export async function getMyRequests() {
    const res = await api.get(`/Requests/me`);
    return res.data; 
}

export async function approveRequest(id) {
    const res = await api.put(`/Requests/${id}/approve`);
    return res.data;
}

export async function rejectRequest(id) {
    const res = await api.put(`/Requests/${id}/reject`);
    return res.data;
}

export async function cancelRequest(id) {
    const res = await api.put(`/Requests/${id}/cancel`);
    return res.data;
}

export async function createPolicy(payload) {
    const res = await api.post("/policies/create", payload);
    return res.data;
}

export async function getPolicies() {
    const res = await api.get('/policies');
    
    if (!res.data) return [];

    return res.data.map((item, index) => ({
        id: item.id,
        name: item.name,
    }));
}

export async function getPolicyDetails(id) {
    const res = await api.get(`/policies/${id}`);
    return res.data; 
}

export async function updatePolicy(id, payload) {
    const res = await api.put(`/policies/${id}`, payload);
    return res.data; 
}

export async function deletePolicy(id) {
    const res = await api.delete(`/policies/${id}`);
    return res.data; 
}