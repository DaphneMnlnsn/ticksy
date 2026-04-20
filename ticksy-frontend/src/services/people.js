import api from "./api";

export async function getUsers() {
  const res = await api.get('/Users');
  return res.data || [];
}

export async function archiveUser(id) {
  return await api.delete(`/Users/${id}`);
}

export async function getTeams() {
  return await api.get('/Teams');
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
  console.log('SENDING:', payload)

  const res = await api.put(`/Users/${id}`, payload)

  console.log('RESPONSE:', res.data)

  return res.data
}