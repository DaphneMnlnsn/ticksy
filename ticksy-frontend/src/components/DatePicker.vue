<template>
    <div class="ticksy-datepicker">
        <input 
            type="date" 
            v-model="formattedValue" 
            class="custom-input"
        />
        <Calendar class="calendar-icon" :size="18" />
    </div>
</template>

<script setup>
    import { computed } from 'vue'
    import dayjs from 'dayjs'
    import { Calendar } from 'lucide-vue-next'

    const props = defineProps({
    modelValue: {
        type: [String, Date, null],
        default: null
    }
    })

    const emit = defineEmits(['update:modelValue'])

    const formattedValue = computed({
        get: () => (props.modelValue ? dayjs(props.modelValue).format('YYYY-MM-DD') : ''),
        set: (val) => emit('update:modelValue', val)
    })
</script>

<style scoped>
    .ticksy-datepicker {
        position: relative;
        width: 100%;
        max-width: 160px; 
    }

    .custom-input {
        width: 100%;
        padding: 10px 12px;
        padding-right: 35px;
        border-radius: 10px;
        border: 1px solid #75889A80;
        background-color: transparent;
        color: #F0F0F0;
        font-family: inherit;
        outline: none;
    }

    .custom-input::-webkit-calendar-picker-indicator {
        position: absolute;
        right: 10px;
        cursor: pointer;
        opacity: 0; 
        z-index: 2;
    }

    .calendar-icon {
        position: absolute;
        right: 12px;
        top: 50%;
        transform: translateY(-50%);
        pointer-events: none;
        color: #F0F0F0;
        z-index: 1;
    }

    .custom-input::-webkit-datetime-edit { color: #F0F0F0; }
    .custom-input::-webkit-datetime-edit-fields-wrapper { padding: 0; }
    
</style>