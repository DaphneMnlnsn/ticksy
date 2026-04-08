<template>
    <transition name="slide-workschedule">
        <div v-if="isOpen" 
            class="request-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="request-header">
                <h3>Request Time Off</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="request-content">

                <div class="input-group">
                    <label class="form-label">Type</label>
                    <select v-model="selectedPolicy" class="form-select">
                        <option disabled value="">Select a leave type</option>
                        
                        <option v-for="policy in policies" :key="policy.id" :value="policy.name">
                            {{ policy.name }}
                        </option>
                    </select>
                </div>

                <div class="input-group">
                    <label class="form-label">Reason</label>
                    <div class="textarea-field">
                        <textarea placeholder="Add a Note" v-model="note"></textarea>
                    </div>
                    <span v-if="errors.name" class="error-msg">*Required</span>
                </div>

                <div class="date-section">
                    <div class="date-header">
                        <label class="form-label">Date</label>
                        <div class="same-day-toggle">
                            <input type="checkbox" id="sameDay" v-model="isSameDay" />
                            <label for="sameDay">same day</label>
                        </div>
                    </div>

                    <div class="time-row">
                        <div v-if="scheduleType === 'Fixed'" class="duration-input">
                            <input 
                                v-if="isSameDay" 
                                type="date" 
                                v-model="startDate" 
                                @change="endDate = startDate"
                                class="time-box full-width" 
                            />

                            <template v-else>
                                <input type="date" v-model="startDate" class="time-box" />
                                <MoveRight :size="18" class="arrow-icon" />
                                <input type="date" v-model="endDate" class="time-box" />
                            </template>
                        </div>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-cancel" @click="emit('close')">Cancel</button>
                <button class="btn-save" @click="saveSchedule">Save</button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref, watch, onMounted } from "vue";
    import { MoveRight, X, CheckCircle } from "lucide-vue-next";
    import { createRequest, getPolicies } from "../services/timeOff";

    const policies = ref([]);
    const selectedPolicy = ref("");

    onMounted(async () => {
        try {
            const data = await getPolicies();
            policies.value = data;
            
            if (data.length > 0) {
                selectedPolicy.value = data[0].name;
            }
        } catch (err) {
            console.error("Failed to fetch policies:", err);
        }
    });

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });
    const emit = defineEmits(['close', 'setup-finished']);
    const formatTime = (time) => time ? `${time}:00` : null;

    const scheduleName = ref('');
    const scheduleType = ref('Fixed');
    const errors = ref({ name: false, days: false });
    const isTouched = ref(false);
    const showToast = ref(false);

    const dayInitials = ['M', 'T', 'W', 'TH', 'F', 'S', 'S'];
    const dayMap = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    const scheduleData = ref({
        'Monday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Tuesday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Wednesday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Thursday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Friday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Saturday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
        'Sunday': { selected: false, start: '09:00', end: '17:00', hours: 0, minutes: 0 },
    });

    const weeklyTotal = ref({ hours: 40, minutes: 0 });

    const isSameDay = ref(false);
    const startDate = ref('');
    const endDate = ref('');

    watch(isSameDay, (val) => {
        if (val) endDate.value = startDate.value;
    });

    watch(scheduleName, (newName) => {
        if (newName.trim().length === 0) {
            errors.value.name = true;
        } else {
            errors.value.name = false;
        }
    })

    function toggleDay(index) {
        const dayName = dayMap[index];
        scheduleData.value[dayName].selected = !scheduleData.value[dayName].selected;
    }

    const note = ref('');
    const timezone = ref('');

    async function saveSchedule() {
        errors.value.name = !startDate.value;
        if (!startDate.value || (!isSameDay.value && !endDate.value)) {
            alert("Please select dates.");
            return;
        }

        try {
            const payload = {
                leaveType: selectedPolicy.value,
                startDate: startDate.value,
                endDate: isSameDay.value ? startDate.value : endDate.value,
                reason: note.value
            };

            const result = await createRequest(payload);

            showToast.value = true;
            setTimeout(() => {
                showToast.value = false;
                emit('setup-finished');
                emit('close');
                startDate.value = '';
                endDate.value = '';
                note.value = '';
            }, 2000);

        } catch (err) {
            console.error("Save Error:", err);
            const errorMsg = err.response?.data || "Failed to save leave request";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    }
</script>

<style scoped>

    .request-panel {
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

    .request-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .request-header h3 {
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

    .request-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .request-content::-webkit-scrollbar {
        width: 8px;
    }

    .request-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .request-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .request-content::-webkit-scrollbar-track {
        background: transparent; 
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

    .request-type, .day-picker {
        display: flex;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 6px;
        overflow: hidden;
        gap: 0;
        transition: background 0.2s ease;
    }

    .request-type button, .day-btn {
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

    .textarea-field textarea {
        width: 100%;
        font-size: 14px;
        height: 140px;
        background: #00233d;
        border: none;
        border-radius: 6px;
        padding: 16px;
        color: white;
        resize: none;
        outline: none;
        font-family: inter;
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

    .slide-workschedule-enter-active,
    .slide-workschedule-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-workschedule-enter-from,
    .slide-workschedule-leave-to {
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

    .same-day-toggle {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.8rem;
        color: #cbd5e1;
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