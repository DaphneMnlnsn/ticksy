import { ref } from 'vue'

export function useWeeklyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(false)

    function setWeekly(data) {
        users.value = data
        hasData.value = data.length > 0
    }

    return {
        users,
        loading,
        hasData,
        setWeekly
    }
}