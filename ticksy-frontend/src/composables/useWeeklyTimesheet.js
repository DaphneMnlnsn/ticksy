import { ref } from 'vue'
import { getWeeklyAttendance } from '../services/attendanceService'

export function useWeeklyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(false)

    const fetchWeekly = async (range) => {
        loading.value = true

        try {
            const [start, end] = range
            const res = await getWeeklyAttendance(start, end)

            users.value = Array.isArray(res.data) ? res.data : []
            hasData.value = users.value.length > 0
        } catch (err) {
            console.error(err)
            users.value = []
            hasData.value = false
        } finally {
            loading.value = false
        }
    }
    return {
        users,
        loading,
        hasData,
        fetchWeekly
    }
}