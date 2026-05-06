<template>
    <DimmedBg :show="isOpen" @close="emit ('close')"></DimmedBg>

    <transition name="slide-calendar">
        <div v-if="isOpen" 
            class="calendar-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="calendar-header">
                <h3>Add New Calendar</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="calendar-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Calendar name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input type="text" v-model="calendarName" placeholder="Calendar name" />
                    <div class="radio-group">
                        <input type="checkbox" v-model="form.isDefault" /> 
                        <div class="form-label">Make default?</div>
                    </div>
                </div>

                <div class="input-group">
                    <div class="input-header">
                        <label class="form-label">Holidays</label>
                        <select v-model="form.region" class="form-select">
                            <option value="" disabled selected>Select a region</option>
                            <option v-for="c in countries" :key="c.countryCode" :value="c.countryCode">
                                {{ c.name }}
                            </option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-save" @click="saveCalendar">Save</button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref, onMounted } from "vue";
    import { X, CheckCircle } from "lucide-vue-next";
    import { createCalendar } from "../services/calendars"; 
    import { getSupportedCountries } from "../services/calendars";
    import DimmedBg from "./DimmedBg.vue";

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });

    const emit = defineEmits(['close', 'setup-finished']);

    const countries = ref([]);
    const calendarName = ref('');
    const autoClockIn = ref(false);
    const showToast = ref(false);
    const form = ref({
        region: ''
    });
    const errors = ref({ name: false });

    onMounted(async () => {
        try {
            countries.value = await getSupportedCountries();
        } catch (err) {
            console.error("Error loading countries:", err);
        }
    });

    async function saveCalendar() {
        if (!calendarName.value.trim()) {
            errors.value.name = true;
            return;
        }

        try {
            const payload = {
                name: calendarName.value,
                source: form.value.region ? 2 : 1,
                isDefault: form.value.isDefault
            };

            await createCalendar(payload, form.value.region);

            showToast.value = true;
            
            setTimeout(() => {
                showToast.value = false;
                emit('setup-finished');
                emit('close');
                calendarName.value = '';
                form.value.region = '';
            }, 2000);

        } catch (err) {
            console.error("Save Error:", err);
            const errorMsg = err.response?.data || "Failed to save calendar";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    }
</script>

<style scoped>

    .calendar-panel {
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

    .calendar-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .calendar-header h3 {
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

    .calendar-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .calendar-content::-webkit-scrollbar {
        width: 8px;
    }

    .calendar-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .calendar-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .calendar-content::-webkit-scrollbar-track {
        background: transparent; 
    }

    .input-group {
        margin-bottom: 24px;
    }
    .radio-group {
        display: flex;
        align-items: center;
        gap: 10px;
        margin: 10px 0;
    }

    .radio-group input {
        width: 5%;
        accent-color: #003867;
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

    .calendar-type, .day-picker {
        display: flex;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 6px;
        overflow: hidden;
        gap: 0;
        transition: background 0.2s ease;
    }

    .calendar-type button, .day-btn {
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

    .time-grid {
        margin-top: 20px;
    }

    .time-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 12px 0;
        border-bottom: 1px solid transparent;
    }

    .day-label {
        font-size: 0.9rem;
        color: #e2e8f0;
        width: 100px;
    }

    .time-inputs {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .time-box {
        background: #001a33;
        border: 1px solid rgba(255, 255, 255, 0.1);
        color: white;
        padding: 6px 8px;
        border-radius: 4px;
        font-size: 0.85rem;
        outline: none;
    }

    .arrow-icon {
        margin: 0 5px -5px 5px;
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

    .form-select {
        background-color: #00386780;
        padding: 12px;
        font-size: 12px;
        border-radius: 5px;
        width: 100%;
        margin-bottom: 5px;

        border: none;
        outline: none;
        box-shadow: none;
    }

    .form-select option {
        background: #001527;
        color: white;
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

    .btn-invite, .btn-save {
        padding: 10px 20px;
        min-width: 120px;
        height: fit-content;
        border-radius: 6px;
        border: none;
        cursor: pointer;
        font-weight: 500;
    }

    .btn-invite {
        background: #003867;
        color: white;
        outline: none;
    }

    .btn-invite:hover {
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

    .slide-calendar-enter-active,
    .slide-calendar-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-calendar-enter-from,
    .slide-calendar-leave-to {
        transform: translateX(100%);
    }

    .rest-day-label {
        display: inline-block;
        font-size: 0.80rem;
        color: rgba(255, 255, 255, 0.3);
        transform: skewX(-15deg);
        width: 130px; 
        text-align: center;
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

    input::-webkit-outer-spin-button,
    input::-webkit-inner-spin-button {
        -webkit-appearance: none;
        margin: 0;
    }

    .weekly-container, .form-label {
        display: flex;
        flex-direction: column;
        align-items: left; 
        border-radius: 12px;
        margin-top: 10px;
    }

    .same-day-toggle {
        display: flex;
        align-items: center;
        gap: 5px;
        font-size: 0.85rem;
        color: var(--text-secondary);
        cursor: pointer;
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
        z-index: 2000;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }
</style>