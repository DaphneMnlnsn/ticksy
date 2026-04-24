import axios from 'axios'

const API = import.meta.env.VITE_API_BASE_URL + '/dashboard'
const API_HOLIDAYS = import.meta.env.VITE_API_BASE_URL + '/Holidays'
const API_CALENDARS = import.meta.env.VITE_API_BASE_URL + '/Calendars'

export default {
  getMostAbsences(start, end, token) {
    return axios.get(`${API}/most-absences`, {
      params: { start, end },
      headers: { Authorization: `Bearer ${token}` }
    })
  },

  getLiveStatus(token) {
    return axios.get(`${API}/live-status`, {
      headers: { Authorization: `Bearer ${token}` }
    })
  },

  getTrackedHours(type, start, end, token) {
    return axios.get(`${API}/${type}-hours`, {
      params: { start, end },
      headers: { Authorization: `Bearer ${token}` }
    })
  },

  getCalendars(token) {
    return axios.get(`${API_CALENDARS}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
  },

  getHolidays(calendarId, year, token) {
    console.log('Fetching holidays with:', { calendarId, year, apiUrl: API_HOLIDAYS })
    return axios.get(`${API_HOLIDAYS}`, {
      params: { calendarId, year },
      headers: {
        Authorization: `Bearer ${token}`
      }
    }).then(res => {
      console.log('Holidays API response:', res.data)
      return res
    }).catch(err => {
      console.error('Holidays API error:', err.response?.data || err.message)
      throw err
    })
  }
}

