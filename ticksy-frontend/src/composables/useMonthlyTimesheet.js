import { ref } from 'vue'
import { getMonthlyAttendance } from '../services/attendanceService'

export function useMonthlyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(true)

    async function fetchMonthly(date) {
        try {
            loading.value = true

            const res = await getMonthlyAttendance(date)

            users.value = res.data ?? []
            hasData.value = users.value.length > 0

        } catch (err) {
            console.error("Monthly fetch error:", err)
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
        fetchMonthly
    }
}