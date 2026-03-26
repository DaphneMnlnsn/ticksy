import { ref, computed } from 'vue'

export function useSearch(listRef, keys = []) {
    const search = ref("")

    const filtered = computed(() => {
        return listRef.value.filter(item =>
            keys.some(key =>
                item[key].toLowerCase().includes(search.value.toLowerCase())
            )
        )
    })

    return { search, filtered }
}