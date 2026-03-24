<template>
    <transition name="slide-workschedule">
        <div 
            v-if="isOpen" 
            class="schedule-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <div class="schedule-header">
                <h3>Schedule</h3>
                <button class="icon-btn" @click="emit('close')"><X /></button>
            </div>
            
            <div class="schedule-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Schedule name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input 
                        type="text" 
                        v-model="scheduleName" 
                        placeholder="Schedule name"
                        :class="{ 'border-error': errors.name }" 
                    />
                </div>

                <div class="input-group">
                    <label class="form-label">Schedule Type</label>
                    <div class="schedule-type">
                        <button v-for="type in ['Fixed', 'Flexible', 'Weekly']"
                            :key="type" :class="{active: scheduleType === type}"
                            @click="scheduleType = type"
                        > {{ type }}
                        </button>
                    </div>
                </div>

                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Time of the Week</label>
                        <span v-if="errors.days" class="error-msg">*Select at least one day of the week</span>
                    </div>
                    
                    <div class="day-picker" :class="{ 'border-error': errors.days }">
                        <button v-for="(initial, index) in dayInitials"
                            :key="index" 
                            class="day-btn"
                            :class="{ active: scheduleData[dayMap[index]].selected }"
                            @click="toggleDay(index)"
                        > 
                            {{ initial }}
                        </button>
                    </div>
                </div>

                <div class="time-grid">
                    <template v-if="scheduleType !== 'Weekly'">
                        <div v-for="(data, day) in scheduleData" :key="day" class="time-row">
                            <span class="day-label">{{ day }}:</span>
                            
                            <div class="time-inputs">
                                <template v-if="data.selected">
                                    <input type="time" v-model="data.start" class="time-box" required />
                                    <MoveRight :size="19" color="white" class="arrow-icon" />
                                    <input type="time" v-model="data.end" class="time-box" required />
                                </template>
                                
                                <span v-else class="rest-day-label">Rest Day</span>
                            </div>
                        </div>
                    </template>

                    <div v-else class="input-group">
                        <label class="form-label">mwhihi</label>
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
    import { ref, watch } from "vue";
    import { MoveRight, X } from "lucide-vue-next";

    const emit = defineEmits(['close']);
    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });

    const scheduleName = ref('');
    const scheduleType = ref('Fixed');
    const errors = ref({ name: false, days: false });
    const isTouched = ref(false);

    const dayInitials = ['M', 'T', 'W', 'TH', 'F', 'S', 'S'];
    const dayMap = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    const scheduleData = ref({
        'Monday': { selected: false, start: '09:00', end: '17:00' },
        'Tuesday': { selected: false, start: '09:00', end: '17:00' },
        'Wednesday': { selected: false, start: '09:00', end: '17:00' },
        'Thursday': { selected: false, start: '09:00', end: '17:00' },
        'Friday': { selected: false, start: '09:00', end: '17:00' },
        'Saturday': { selected: false, start: '09:00', end: '17:00' },
        'Sunday': { selected: false, start: '09:00', end: '17:00' },
    });

    watch(
        () => scheduleData.value, 
        (newData) => {
            isTouched.value = true;
            const hasSelectedDays = Object.values(newData).some(d => d.selected);
        
            if (isTouched.value) {
                errors.value.days = !hasSelectedDays;
            }
        }, 
        { deep: true }
    );

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

    function saveSchedule() {
        const selectedDays = Object.values(scheduleData.value).filter(d => d.selected);
        
        if (selectedDays.length === 0) {
            errors.value.days = true;
            return;
        }

        if (!scheduleName.value) {
            errors.value.name = true;
            return;
        }

        alert("Schedule Saved!");
    }
</script>

<style scoped>

    .schedule-panel {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        width: 420px;
        background-color: #001324;
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
    }

    .schedule-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
        overflow-y: hidden;
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
    }

    .btn-save {
        background: rgb(0, 56, 103, 50%);
        color: white;
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
        font-size: 0.85rem;
        color: rgba(255, 255, 255, 0.3);
        transform: skewX(-15deg);
        width: 130px; 
        text-align: center;
    }
</style>