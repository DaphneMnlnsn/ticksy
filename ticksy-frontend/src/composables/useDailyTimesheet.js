import { ref } from 'vue'
import dayjs from 'dayjs'
import { getDailyAttendance } from '../services/attendanceService'

export function useDailyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(true)

    async function fetchDaily(date) {
        try {
            loading.value = true

            const res = await getDailyAttendance(date)
            users.value = res.data
            hasData.value = users.value.length > 0

        } catch (err) {
            console.error("Daily fetch error:", err)
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
        fetchDaily
    }
}