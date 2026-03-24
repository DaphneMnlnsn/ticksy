<template>
    <div class="ticksy-datepicker">
        <VueDatePicker
            v-model="internalValue"
            :enable-time-picker="false"
            format="dd/MM/yyyy"
            :placeholder="placeholder"
        />
    </div>
</template>

<script setup>
    import { ref, watch } from 'vue'
    import { VueDatePicker } from '@vuepic/vue-datepicker'
    import '@vuepic/vue-datepicker/dist/main.css'
    import dayjs from 'dayjs'
    import customParseFormat from 'dayjs/plugin/customParseFormat'

    dayjs.extend(customParseFormat)

    const format = (date) => {
        return dayjs(date).format('DD/MM/YYYY')
    }

    const props = defineProps({
        modelValue: [Date, Array], 
        placeholder: {
            type: String,
            default: 'Select date'
        },
        range: {
            type: Boolean,
            default: false
        }
    })

    const emit = defineEmits(['update:modelValue'])

    const internalValue = ref(props.modelValue)

    watch(() => props.modelValue, (val) => {
        internalValue.value = val
    })

    watch(internalValue, (val) => {
        emit('update:modelValue', val)
    })
</script>

<style scoped>
    .ticksy-datepicker {
        position: relative;
        width: 100%;
    }

    :deep(.dp__input) {
        padding: 8px 40px 8px 12px;
        border-radius: 10px;
        border: 1px solid #ddd;
    }

    .ticksy-datepicker::after {
        content: "📅";
        position: absolute;
        right: 12px;
        top: 50%;
        transform: translateY(-50%);
        pointer-events: none;
    }

    :deep(.dp__input_wrap) {
        position: relative;
    }

    :deep(.dp__input_wrap::before) {
        content: "→";
        position: absolute;
        left: 50%;
        transform: translateX(-50%);
        color: #888;
    }
</style>