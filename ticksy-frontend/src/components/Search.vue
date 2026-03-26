<template>
    <div class="search-container">
        <Search class="search-icon"/>

        <input
            type="text"
            v-model="internalValue"
            @input="emit('update:modelValue', internalValue)"
            placeholder="Search..."
        />
    </div>
</template>

<script setup>
    import { ref, watch } from 'vue'
    import { Search } from 'lucide-vue-next'

    const props = defineProps({
        modelValue: String
    })

    const emit = defineEmits(['update:modelValue'])

    const internalValue = ref(props.modelValue || "")

    watch(() => props.modelValue, (val) => {
        internalValue.value = val
    })

    function emitSearch() {
        emit('update:modelValue', internalValue.value)
    }

</script>

<style scoped>
    .search-container {
        display: flex;
        border-radius: 10px;
        border: 2px solid #75889A80;
        width: 100%;
        padding: 5px 15px;
        gap: 10px;
        align-items: center;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
    }

    .search-icon {
        width: 16px;
        height: 16px;
    }

    span {
        font-size: 15px;
        color: #F0F0F0AD;
    }

    .search-container input {
        border: none;
        outline: none;
        background: transparent;
        color: #F0F0F0B3;
        width: 100%;
        font-family: inter;
        font-weight: 600;
    }
</style>
