<template>
    <DimmedBg :show="isOpen" @close="emit ('close')"></DimmedBg>

    <transition name="slide-addBreak">
        <div v-if="isOpen" 
            class="addBreak-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="addBreak-header">
                <h3>Add Break Time</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="addBreak-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Day of the Week</label>
                        <span v-if="errors.days" class="error-msg">*Select at least one day</span>
                    </div>
                    <div class="day-picker">
                        <button v-for="(initial, index) in dayInitials"
                            :key="index" class="day-btn"
                            :class="{ active: selectedDays.includes(dayMap[index])}"
                            @click="toggleDay(dayMap[index])"
                        > {{ initial }} </button>
                    </div>
                </div>

                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Break Name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input type="text" v-model="breakName"/> 
                </div>
                
                <div class="duration-container">
                    <label class="form-label">Duration</label>
                    <div class="duration-input large">
                        <input type="number" v-model="duration.hours" min="0" max="23" class="time-box small" />
                        <span class="unit-label"> h</span>
                        <input type="number" v-model="duration.minutes" min="0" max="59" class="time-box small" />
                        <span class="unit-label"> m</span>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-cancel" @click="emit('close')">Cancel</button>
                <button class="btn-save" @click="handleSave">Save</button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref, watch } from "vue";
    import { X, CheckCircle, Clock } from "lucide-vue-next";
    import DimmedBg from "./DimmedBg.vue";

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });
    const emit = defineEmits(['close', 'save']);

    const showToast = ref(false);
    const selectedDays = ref([])
    const breakName = ref('')
    const breakDuration = ref('00:30') //default
    const isSaving = ref(false);

    const duration = ref({
        hours: 0,
        minutes: 30
    })

    const dayInitials = ['M', 'T', 'W', 'TH', 'F', 'S', 'S'];
    const dayMap = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    const errors = ref({
        name: false,
        days: false
    })

    watch(breakName, (val) => {
        if (val.trim()) errors.value.name = false;
    });

    const toggleDay = (day) => {
        const index = selectedDays.value.indexOf(day);
        if (index > -1) {
            selectedDays.value.splice(index, 1);
        } else {
            selectedDays.value.push(day);
            errors.value.days = false;
        }
    }

    const handleSave = async () => {
        if (isSaving.value) return;

        errors.value.name = !breakName.value.trim();
        errors.value.days = selectedDays.value.length === 0;

        if (errors.value.name || errors.value.days) {
            return;
        }

        isSaving.value = true;

        if (duration.value.hours === 0 && duration.value.minutes === 0) {
            // You could add an error for this if you like
            return;
        }

        const formattedHours = String(duration.value.hours).padStart(2, '0');
        const formattedMinutes = String(duration.value.minutes).padStart(2, '0');
        const durationString = `${formattedHours}:${formattedMinutes}`;

        const payload = {
            name: breakName.value,
            days: selectedDays.value,
            duration: durationString,
        };

        showToast.value = true;
        emit('save', payload);

        setTimeout(() => {
            showToast.value = false;
            breakName.value = "";
            selectedDays.value = [];
            isSaving.value = false;
            emit('close');
        }, 2000);
    };
    </script>

<style scoped>

    .addBreak-panel {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        width: 420px;
        background-color: #001527;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        color: white;
        box-shadow: -10px 0 30px rgba(0, 0, 0, 0.5);
    }

    .addBreak-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .addBreak-header h3 {
        font-size: 1.2rem;
        font-weight: 500;
        margin-left: 30px;
        font-family: Righteous;
        font-size: 20px;
    }

    .icon-btn {
        background: transparent;
        border: none;
        color: white;
        cursor: pointer;
        opacity: 0.7;
        outline: none;
    }

    .addBreak-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .addBreak-content::-webkit-scrollbar {
        width: 8px;
    }

    .addBreak-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .addBreak-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .addBreak-content::-webkit-scrollbar-track {
        background: transparent; 
    }

    .input-group {
        margin-bottom: 24px;
    }

    .form-label {
        display: block;
        font-size: 0.85rem;
        color: #ffffff;
        margin-bottom: 8px;
    }

    input[type="text"] {
        width: 100%;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 10px 12px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    .day-picker {
        display: flex;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 6px;
        overflow: hidden;
        gap: 0;
        transition: background 0.2s ease;
    }

    .day-btn {
        flex: 1;
        background: transparent;
        border: none;
        outline: none;
        color: white;
        padding: 10px 0;
        cursor: pointer;
        font-size: 0.9rem;
        transition: background 0.2s ease;
        margin: 2px;  
        border-radius: 7px;
    }

    .active {
        background: #003366 !important;
    }

    .label-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 8px;
    }

    .label-header .form-label {
        margin-bottom: 0;
    }

    .error-msg {
        color: rgb(255, 99, 99);
        font-size: 0.65rem;
        font-weight: 500;
        letter-spacing: 0.3px;
        animation: errorShake 0.4s ease-in-out;
    }

    @keyframes errorShake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(4px); }
        75% { transform: translateX(-4px); }
    }

    .footer-actions {
        padding: 16px 24px;
        display: flex;
        gap: 12px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
        justify-content: flex-end;
    }

    .btn-cancel, .btn-save {
        padding: 10px 20px;
        min-width: 120px;
        height: fit-content;
        border-radius: 6px;
        border: none;
        cursor: pointer;
        font-weight: 500;
    }

    .btn-cancel {
        background: #003867;
        color: white;
        outline: none;
    }

    .btn-cancel:hover {
        opacity: 0.8;
    }

    .btn-save {
        background: rgb(0, 56, 103, 50%);
        color: white;
        outline: none;
    }

    .btn-save:hover {
        opacity: 0.8;
    }

    .slide-addBreak-enter-active,
    .slide-addBreak-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-addBreak-enter-from,
    .slide-addBreak-leave-to {
        transform: translateX(100%);
    }

    input::-webkit-outer-spin-button,
    input::-webkit-inner-spin-button {
        -webkit-appearance: none;
        margin: 0;
    }

    .success-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #06d6a0;
        color: #001324;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(6, 214, 160, 0.3);
        z-index: 9999;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }

    .duration-container {
        display: flex;
        flex-direction: column;
        align-items: left; 
        border-radius: 12px;
        margin-top: 10px;
    }

    .duration-inputs {
        display: flex;
        align-items: center;
        gap: 4px;
        color: white;
    }

    .time-box.small {
        width: 50px;
        text-align: center;
        background: #001a33;
        border: 1px solid rgba(255, 255, 255, 0.1);
        color: white;
    }

    .unit-label {
        font-size: 0.8rem;
        color: white;;
        margin-right: 8px;
    }

    .duration-input.large {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .duration-input.large .time-box {
        width: 65px;
        height: 40px;
        font-size: 0.9rem;
        font-weight: 600;
        text-align: center;
        background: #001e36; 
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 8px;
        color: white;
    }

    .unit-label {
        font-size: 0.9rem;
        color: white;
        font-weight: 400;
    }

    .duration-input.large .time-box:focus {
        outline: none;
        border-color: #3b82f6;
        background: #002a4d;
    }
</style>