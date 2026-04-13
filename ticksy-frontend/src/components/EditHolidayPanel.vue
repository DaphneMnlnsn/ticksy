<template>
    <DimmedBg :show="isOpen" @close="emit('close')"></DimmedBg>

    <transition name="slide-addHoliday">
        <div v-if="isOpen" 
            class="addHoliday-panel"
            :style="{ borderRight: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="addHoliday-header">
                <h3>Edit Holiday</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="addHoliday-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Holiday Name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input type="text" v-model="holidayName" placeholder="Enter holiday name"/> 
                </div>

                <div class="input-group">
                    <label class="form-label">Date</label>
                    <div class="time-row">
                        <input 
                            type="date" 
                            v-model="holidayDate" 
                            class="time-box full-width" 
                        />
                    </div>
                </div>

                <div class="input-group">
                    <label class="form-label">Type</label>
                    <div class="holiday-type">
                        <button 
                            v-for="type in ['Public', 'Company-Specific']"
                            :key="type" 
                            type="button"
                            :class="{ active: holidayType === type }"
                            @click="holidayType = type"
                        > 
                            {{ type }} 
                        </button>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-cancel" @click="emit('close')">Cancel</button>
                <button class="btn-save" @click="handleSave" :disabled="isSaving">
                    {{ isSaving ? 'Saving...' : 'Save' }}
                </button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref, watch } from "vue";
    import { X, CheckCircle } from "lucide-vue-next";
    import DimmedBg from "./DimmedBg.vue";
    import { updateHoliday } from "../services/holidays";

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean,
        holiday: Object,
        calendarId: Number
    });

    const handleSave = async () => {
        if (isSaving.value) return;
        if (!holidayName.value.trim()) {
            errors.value.name = true;
            return;
        }

        isSaving.value = true;

        const typeMapping = {
            'Public': 'Public',
            'Company-Specific': 'CompanySpecific'
        };

        const payload = {
            Name: holidayName.value.trim(),
            Date: holidayDate.value,
            Type: typeMapping[holidayType.value],
        };

        try {
            const result = await updateHoliday(props.holiday.id, payload);
            emit('save', result);

            showToast.value = true;
            setTimeout(() => {
                showToast.value = false;
                resetForm();
                emit('close');
            }, 1500);

        } catch (error) {
            console.error("Failed to update holiday:", error.response?.data || error);
            alert("Could not save holiday. Check console for details.");
        } finally {
            isSaving.value = false;
        }
    };
    
    const emit = defineEmits(['close', 'save']);

    const holidayName = ref('');
    const holidayDate = ref(new Date().toISOString().substr(0, 10));
    const holidayType = ref('Public');
    
    const showToast = ref(false);
    const isSaving = ref(false);

    const errors = ref({
        name: false
    });

    watch(holidayName, (val) => {
        if (val.trim()) errors.value.name = false;
    });

    const resetForm = () => {
        holidayName.value = "";
        holidayDate.value = new Date().toISOString().substr(0, 10);
        holidayType.value = "Public";
        isSaving.value = false;
        errors.value.name = false;
    };

    watch(() => props.isOpen, (newVal) => {
        if (newVal && props.holiday) {
            holidayName.value = props.holiday.name || "";
            
            if (props.holiday.date) {
                holidayDate.value = new Date(props.holiday.date).toISOString().split('T')[0];
            }
            
            const reverseMapping = {
                'Public': 'Public',
                'CompanySpecific': 'Company-Specific'
            };
            holidayType.value = reverseMapping[props.holiday.type] || 'Public';
        } else if (!newVal) {
            resetForm();
        }
    }, { immediate: true });
</script>

<style scoped>

    .addHoliday-panel {
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

    .addHoliday-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .addHoliday-header h3 {
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

    .addHoliday-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .addHoliday-content::-webkit-scrollbar {
        width: 8px;
    }

    .addHoliday-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .addHoliday-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .addHoliday-content::-webkit-scrollbar-track {
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

    .slide-addHoliday-enter-active,
    .slide-addHoliday-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-addHoliday-enter-from,
    .slide-addHoliday-leave-to {
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

    .holiday-type, .day-picker {
        display: flex;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 6px;
        overflow: hidden;
        gap: 0;
        transition: background 0.2s ease;
    }

    .holiday-type button, .day-btn {
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

    .duration-input.large {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .duration-input {
        display: flex;
        align-items: center;
        gap: 10px;
        width: 100%;
    }

    .time-box.full-width {
        flex: 1;
    }

    .same-day-toggle {
        display: flex;
        align-items: center;
        gap: 5px;
        font-size: 0.85rem;
        color: var(--text-secondary);
        cursor: pointer;
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

    .duration-input.large .time-box:focus {
        outline: none;
        border-color: #3b82f6;
        background: #002a4d;
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

    .time-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 12px 0;
        border-bottom: 1px solid transparent;
    }
</style>