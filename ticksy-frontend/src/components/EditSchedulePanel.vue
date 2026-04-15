<template>
    <DimmedBg :show="isOpen" @close="emit ('close')"></DimmedBg>
    
    <transition name="slide-workschedule">
        <div v-if="isOpen" 
            class="schedule-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="schedule-header">
                <h3>Edit Schedule</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="schedule-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Schedule name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input type="text" v-model="scheduleName" placeholder="Schedule name" />
                </div>

                <div class="input-group">
                    <label class="form-label">Schedule Type</label>
                    <div class="schedule-type">
                        <button v-for="type in ['Fixed', 'Flexible', 'Weekly']"
                            :key="type" :class="{ active: scheduleType === type }"
                            @click="scheduleType = type"
                        > {{ type }} </button>
                    </div>
                </div>

                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Time of the Week</label>
                        <span v-if="errors.days && scheduleType !== 'Weekly'" class="error-msg">*Select at least one day</span>
                    </div>
                    <div class="day-picker">
                        <button v-for="(initial, index) in dayInitials"
                            :key="index" class="day-btn"
                            :class="{ active: scheduleData[dayMap[index]]?.selected }"
                            @click="toggleDay(index)"
                        > {{ initial }} </button>
                    </div>
                </div>

                <div class="time-grid">
                    <template v-if="scheduleType !== 'Weekly'">
                        <div v-for="(data, day) in scheduleData" :key="day" class="time-row">
                            <span class="day-label">{{ day }}:</span>
                            <div class="time-inputs">
                                <template v-if="data?.selected">
                                    <div v-if="scheduleType === 'Fixed'" class="duration-input">
                                        <input type="time" v-model="data.start" class="time-box" />
                                        <MoveRight :size="18" class="arrow-icon" />
                                        <input type="time" v-model="data.end" class="time-box" />
                                    </div>
                                    <div v-else class="duration-input">
                                        <input type="number" v-model="data.hours" class="time-box small" />
                                        <span class="unit-label"> h</span>
                                        <input type="number" v-model="data.minutes" class="time-box small" />
                                        <span class="unit-label"> m</span>
                                    </div>
                                </template>
                                <span v-else class="rest-day-label">Rest Day</span>
                            </div>
                        </div>
                    </template>

                    <div v-else class="weekly-container">
                        <label class="form-label">Hours</label>
                        <div class="duration-input large">
                            <input type="number" v-model="weeklyTotal.hours" class="time-box" placeholder="0" />
                            <span class="unit-label"> h</span>
                            <input type="number" v-model="weeklyTotal.minutes" class="time-box" placeholder="0" />
                            <span class="unit-label"> m</span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-invite">Invite</button>
                <button class="btn-save" @click="saveSchedule">Save</button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref, watch, computed } from "vue";
    import { MoveRight, X, CheckCircle } from "lucide-vue-next";
    import { createSchedule, updateSchedule } from "../services/schedule";
    import DimmedBg from "./DimmedBg.vue";

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean,
        scheduleEdit: Object
    });
    const emit = defineEmits(['close', 'setup-finished']);

    const scheduleName = ref('');
    const scheduleType = ref('Fixed');

    const errors = ref({ name: false, days: false });
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
    const isEditing = computed(() => !!props.scheduleEdit?.id);
    const formatTime = (t) => t ? `${t}:00` : null;

    function toggleDay(index) {
        const day = dayMap[index];
        scheduleData.value[day].selected = !scheduleData.value[day].selected;
    }

    function resetForm() {
      scheduleName.value = '';
      scheduleType.value = 'Fixed';
      Object.keys(scheduleData.value).forEach(day => {
          scheduleData.value[day] = {
              selected:false,start:'09:00',end:'17:00',hours:0,minutes:0
          };
      });
    }

    watch(() => props.scheduleEdit, (data) => {
        if (!data) {
            resetForm();
            return;
        }

        scheduleName.value = data.scheduleName || '';
        scheduleType.value = data.workArrangement || 'Fixed';

        if (data.weeklyDuration) {
            const parts = data.weeklyDuration.split(':');
            weeklyTotal.value = { 
                hours: parseInt(parts[0]) || 0, 
                minutes: parseInt(parts[1]) || 0 
            };
        }

        if (data.days && Array.isArray(data.days)) {
            data.days.forEach(d => {
                const normalizedDay = d.day.charAt(0).toUpperCase() + d.day.slice(1).toLowerCase();
                const day = scheduleData.value[normalizedDay];
                
                if (!day) return;

                day.selected = !d.isRestDay;

                if (d.startTime && d.endTime) {
                    day.start = d.startTime.slice(0, 5);
                    day.end = d.endTime.slice(0, 5);
                }

                if (d.duration) {
                    const [h, m] = d.duration.split(':');
                    day.hours = parseInt(h) || 0;
                    day.minutes = parseInt(m) || 0;
                }
            });
        } else {
            console.warn("Schedule data received without a valid 'days' array.");
        }
    }, { immediate: true });

    async function saveSchedule() {
        errors.value = { name: false, days: false };

        if (!scheduleName.value.trim()) {
            errors.value.name = true;
        }
        if (scheduleType.value !== 'Weekly') {
            const hasSelected = Object.values(scheduleData.value).some(d => d.selected);
            if (!hasSelected) {
            errors.value.days = true;
            }
        }
        if (errors.value.name || errors.value.days) return;

        const days = dayMap.map(day => {
            const d = scheduleData.value[day];

            if (scheduleType.value === 'Weekly') {
                return { day, isRestDay: !d.selected };
            }

            if (!d.selected) return { day, isRestDay: true };

            if (scheduleType.value === 'Fixed') {
                return {
                    day,
                    isRestDay: false,
                    startTime: formatTime(d.start),
                    endTime: formatTime(d.end)
                };
            }

            return {
                day,
                isRestDay: false,
                duration: `${String(d.hours).padStart(2, '0')}:${String(d.minutes).padStart(2, '0')}:00`
            };
        });

        const payload = {
            scheduleName: scheduleName.value,
            workArrangement: scheduleType.value,
            weeklyDuration: scheduleType.value === 'Weekly'
            ? `${String(weeklyTotal.value.hours).padStart(2,'0')}:${String(weeklyTotal.value.minutes).padStart(2,'0')}:00`
            : null,
            days
        };

        try {
            let result;
            if (isEditing.value) {
                result = await updateSchedule(props.scheduleEdit.id, payload);
                } else {
                result = await createSchedule(payload);
            }

            showToast.value = true;

            setTimeout(() => {
            showToast.value = false;
            emit('setup-finished', result); 
            // Close the panel
            emit('close');
            }, 2000);

        } catch (err) {
            console.error("Save Error:", err);
            alert('Failed to save schedule. Check the console for details.');
        }
    }
</script>

<style scoped>

    .schedule-panel {
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

    .schedule-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .schedule-header h3 {
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

    .schedule-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .schedule-content::-webkit-scrollbar {
        width: 8px;
    }

    .schedule-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .schedule-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .schedule-content::-webkit-scrollbar-track {
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

    .schedule-type, .day-picker {
        display: flex;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 6px;
        overflow: hidden;
        gap: 0;
        transition: background 0.2s ease;
    }

    .schedule-type button, .day-btn {
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