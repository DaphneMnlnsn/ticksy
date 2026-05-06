import { ref } from 'vue'

export function useMonthlyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(false)

    function setMonthly(data) {
        users.value = data
        hasData.value = data.length > 0
    }

    return {
        users,
        loading,
        hasData,
        setMonthly
    }
}