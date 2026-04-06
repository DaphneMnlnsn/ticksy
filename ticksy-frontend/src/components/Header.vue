<template>
  <div class="header">
    <h1>{{ title }}</h1>

    <div v-if="showTimer" class="timer-wrapper">
        <template v-if="isClockedIn">
            <span v-if="isOnBreak" class="timer-text break-mode-color" :class="{ 'flashing-red': isOverBreakLimit }">
                {{ breakTimerValue }} (On Break)
            </span>
            <span v-else class="timer-text">
                {{ timerValue }}
            </span>
        </template>
        
        <span v-else class="out-time">{{ lastOut }}</span>

        <div class="controls">
        <button v-if="isClockedIn" class="break-btn" @click="isOnBreak ? endBreak() : handleBreak()"
        >
            <Coffee :size="19" :color="isOnBreak ? '#ffffff' : '#052348'" />
        </button>
        
        <button 
            @click="handleButtonClick" 
            class="circle-btn" 
            :class="isClockedIn ? 'stop-btn' : 'start-btn'"
        >
            <Square v-if="isClockedIn" :size="17" color="#052348" fill="#052348" />
            <Timer v-else :size="19" color="#052348"/>
        </button>
      </div>
    </div>

    <ClockInOutPanel
        :userName="currentUser?.name"
        :userRole="currentUser?.role"
        :lastInStatus="lastOut"
        :isOpen="isPanelOpen"
        :recordedTime = "currentTimeForPanel"
        :initialTab = "activePanelTab"
        :isSidebarCollapsed="isSidebarCollapsed"
        @close="isPanelOpen = false"
        @save="confirmSave"
    />
  </div>
</template>

<script setup>
    import {Coffee, Square, Timer } from "lucide-vue-next";
    import { ref } from "vue";
    import ClockInOutPanel from "./ClockInOutPanel.vue";
    import Swal from "sweetalert2";

    const currentUser = ref({
        name: "IDA Admin",
        role: "admin"
    });

    const getCurrentFormattedTime = () => {
        const now = new Date();
        return now.toLocaleTimeString([], {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });
    };

    const getWelcomeMessage = () => {
        const hour = new Date().getHours();
        if (hour < 12) return "Good morning! Ready to clock in?";
        if (hour < 18) return "Good afternoon! Time to start?";
        return "Working late? Start your session here.";
    };

    const confirmSave = () => {
        if(activePanelTab.value == 'in'){
            isPanelOpen.value = false;
            isClockedIn.value = true;
            startTimer();
        } else {
            isPanelOpen.value = false;
            stopTimer();
        }
    }

    const props = defineProps({
    title: { type: String, default: "Page Title" },
    showTimer: { type: Boolean, default: false }
    });

    const isClockedIn = ref(false)
    const isPanelOpen = ref(false)
    const isOnBreak = ref(false)
    const isOverBreakLimit= ref(false)
    const isSidebarCollapsed = ref(false)

    const timerValue = ref("0:00:00")
    const breakTimerValue = ref("0:00:00")
    const currentTimeForPanel = ref("")
    const activePanelTab = ref("in")
    const lastOut = ref (getWelcomeMessage())
    const totalWorkedTime = ref(0)
    const breakCount = ref(0)
    const MAX_BREAKS = 2

    let timerInterval = null
    let breakInterval = null

    //connect to API 
    const breakTime = { 
        lunchDuration: 60, 
        shortBreakDuration: 1 //change to how many minutes for short break time
    };

    const handleButtonClick = () => {
        if (isClockedIn.value) {
            currentTimeForPanel.value = getCurrentFormattedTime();
            activePanelTab.value = 'out';
            isPanelOpen.value = true;
        } else {
            currentTimeForPanel.value = getCurrentFormattedTime();
            activePanelTab.value = 'in';
            isPanelOpen.value = true; 
        }
    };

    const startTimer = () => {
        if (timerInterval) clearInterval(timerInterval);
        timerInterval = setInterval(() => {
            totalWorkedTime.value++;

            const h = Math.floor(totalWorkedTime.value / 3600);
            const m = Math.floor((totalWorkedTime.value % 3600) / 60);
            const s = totalWorkedTime.value % 60;
            timerValue.value = `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        }, 1000);
    };

    const stopTimer = () => {
        const now = new Date();
        const day = now.toLocaleDateString([], {weekday: 'long'});
        const time = now.toLocaleTimeString([], {hour: 'numeric', minute: '2-digit', hour12: true});

        lastOut.value = `Last out at ${time}, ${day}`;

        clearInterval(timerInterval);
        clearInterval(breakInterval);
        timerInterval = null; 
        breakInterval = null;

        totalWorkedTime.value = 0;
        breakCount.value = 0;
        timerValue.value = "0:00:00";      
        isClockedIn.value = false;
        isOnBreak.value = false;
        isOverBreakLimit.value = false;
    }; 

    const handleBreak = async () => {
        const { value: breakType } = await Swal.fire({
            title: 'Select Break Type',
            background: '#001527', 
            color: '#ffffff',
            input: 'radio',
            inputOptions: {
                'lunch': `Lunch Break (${breakTime.lunchDuration} minutes)`,
                'short': `Short Break (${breakTime.shortBreakDuration} minutes)`
            },
            footer: `<i style="color: #88b6ff">Remaining breaks: ${MAX_BREAKS - breakCount.value}</i>`,
            inputValidator: (value) => {
                if (!value) return 'You need to choose something!';
            },
            confirmButtonColor: 'rgb(0, 56, 103, 50%)',
            showCancelButton: true,
            cancelButtonColor: '#003867',
            customClass: {popup: 'swal-custom-dark'}
        });

        if(breakCount.value >= MAX_BREAKS){
            Swal.fire({
                title: 'Limit Reached',
                text: 'You have used up all your breaks for today.',
                icon: 'error',
                background: '#001527',
                color: '#ffffff',
                confirmButtonColor: '#003867'
            });
            return;
        }

        if(breakType) {
            const minutes = breakType === 'lunch' 
            ? breakTime.lunchDuration 
            : breakTime.shortBreakDuration;
            startBreakMode(minutes);
        }
    };

    const startBreakMode = (durationInMinutes) => {
        clearInterval(timerInterval);
        timerInterval = null;

        isOnBreak.value = true;
        breakCount.value++;
        let seconds = 0;
        const limitInSeconds = 5;
        
        if (breakInterval) clearInterval(breakInterval);
        breakInterval = setInterval(() => {
            seconds++;

            if (seconds === limitInSeconds) {
                isOverBreakLimit.value = true;
            }

            const h = Math.floor(seconds / 3600);
            const m = Math.floor((seconds % 3600) / 60);
            const s = seconds % 60;
            breakTimerValue.value = `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        }, 1000);
    };

    const endBreak = async () => {
        const { isConfirmed } = await Swal.fire({
            title: 'End Break?',
            text: "Are you ready to go back to work?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Resume Work',
            background: '#001527',
            color: '#fff',
            confirmButtonColor: '#003867',
            cancelButtonColor: 'rgb(0, 56, 103, 50%)'
        });

        if (isConfirmed) {
            clearInterval(breakInterval);
            breakInterval = null;
            isOnBreak.value = false;
            isOverBreakLimit.value = false;
            breakTimerValue.value = "0:00:00";
            startTimer();
        }
    };
</script>

<style scoped>
    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        width: 100%;
        padding: 10px 20px;
    }

    .timer-wrapper {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .controls {
        display: flex;
        gap: 10px;
        align-items: center;
    }

    .circle-btn {
        width: 31px;
        height: 30px;
        border-radius: 50%;
        display: flex;
        align-items: center;   
        justify-content: center; 
        padding: 0;
        border: none;
        line-height: 0; 
        transition: transform 0.2s ease, background-color 0.3s ease;    
        outline: none;
    }

    .circle-btn svg {
        margin-top: -1px;
    }

    .break-btn svg {
        margin: -1px 0 0 2px;;
        
    }

    .break-btn {
        width: 31px;
        height: 31px;       
        border-radius: 50%;  
        border: none;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: transform 0.2s ease, background-color 0.3s ease;
        padding: 0;    
        outline: none;
    }

    .circle-btn:hover, .break-btn:hover {
        transform: scale(1.1);
    }

    .start-btn {
        background-color: #00DC28;
    }

    .stop-btn {
        background-color: #EB3223; 
    }

    .break-btn {
        background-color: #FF7B00; 
    }

    .timer-text {
        font-size: 16px;
        color: white; 
    }

    .out-time {
        font-size: 14px;
        opacity: 0.8;
        color: white;
    }

    h1 {
        font-size: 30px;
        font-family: 'Righteous', sans-serif;
        color: white;
        margin: 0;
    }

    :deep(.swal-custom-dark) {
        border: 1px solid rgba(255, 255, 255, 0.1);
    }

    :deep(.swal-custom-dark .swal2-radio) {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
        padding: 10px 40px;
    }

    :deep(.swal-custom-dark .swal2-radio input) {
        accent-color: #88b6ff; 
    }

    :deep(.swal-custom-dark .swal2-label) {
        color: white !important;
    }

    .break-mode-color {
        color: #ff9f43 !important;
        font-weight: 700;
    }

    .break-btn.active-break {
        background: rgba(255, 159, 67, 0.2);
        border: 1px solid #ff9f43;
    }

    .flashing-red {
        color: #ff4d4d !important;
        animation: pulse-red 1s infinite;
        font-weight: bold;
    }

    @keyframes pulse-red {
        0% { opacity: 1; }
        50% { opacity: 0.4; }
        100% { opacity: 1; }
    }
</style>