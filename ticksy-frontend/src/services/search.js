import { ref, computed } from 'vue'

export function useSearch(listRef, keys = []) {
    const search = ref("")

    const filtered = computed(() => {
        return listRef.value.filter(item =>
            keys.some(key => {
                const field = item[key];
                return String(field || '').toLowerCase().includes(search.value.toLowerCase());
            })
        )
    })

    return { search, filtered }
}