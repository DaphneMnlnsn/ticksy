import { ref } from 'vue'

export function useDailyTimesheet() {
    const users = ref([])
    const loading = ref(false)
    const hasData = ref(false)

    function setDaily(data) {
        users.value = data
        hasData.value = data.length > 0
    }

    return {
        users,
        loading,
        hasData,
        setDaily
    }
}