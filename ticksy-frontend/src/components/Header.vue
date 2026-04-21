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
        <button v-if="isClockedIn" class="break-btn" @click="isOnBreak ? endBreakAction() : handleBreakAction()">
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
      :userAvatar="currentUser?.avatar"
      :isOpen="isPanelOpen"
      :recordedTime="currentTimeForPanel"
      :initialTab="activePanelTab"
      :isClockedIn="isClockedIn" 
      :isSidebarCollapsed="isSidebarCollapsed"
      @close="isPanelOpen = false"
      @save="confirmSave"
    />
  </div>
</template>

<script setup>
    import { ref, onMounted, onUnmounted, computed } from "vue";
    import { Coffee, Square, Timer } from "lucide-vue-next";
    import Swal from "sweetalert2";
    import ClockInOutPanel from "./ClockInOutPanel.vue";
    import { startBreak, endBreakApi, getTodayStatus } from "../services/attendanceService";
    import { getMySchedule } from "../services/schedule";

    const props = defineProps({
        title: { type: String, default: "Page Title" },
        showTimer: { type: Boolean, default: false }
    });

    const currentUser = ref(null);

    onMounted(() => {
        const storedUser = localStorage.getItem("user");

        if (storedUser) {
            currentUser.value = JSON.parse(storedUser);
        }
    });   
    
    const isClockedIn = ref(false);
    const isOnBreak = ref(false);
    const isPanelOpen = ref(false);
    const isOverBreakLimit = ref(false);
    const isSidebarCollapsed = ref(false);

    const timerValue = ref("0:00:00");
    const breakTimerValue = ref("0:00:00");
    const totalWorkedSeconds = ref(0);
    const breakCount = ref(0);

    const userBreaks = ref([]);
    const maxBreaks = ref(2);

    const activePanelTab = ref("in");
    const currentTimeForPanel = ref("");
    const lastOut = ref("");

    let timerInterval = null;
    let breakInterval = null;

    const swalConfig = {
        background: '#001527',
        color: '#ffffff',
        confirmButtonColor: '#003867',
        cancelButtonColor: 'rgba(0, 56, 103, 0.5)',
        backdrop: `rgba(0, 21, 39, 0.6)`
    };

    const formatSeconds = (totalSeconds) => {
        const h = Math.floor(totalSeconds / 3600);
        const m = Math.floor((totalSeconds % 3600) / 60);
        const s = totalSeconds % 60;
        return `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
    };

    const getCurrentFormattedTime = () => new Date().toLocaleTimeString([], {
        hour: 'numeric', minute: '2-digit', hour12: true
    });

    const updateWelcomeMessage = () => {
        const hour = new Date().getHours();
        if (hour < 12) return "Good morning! Ready to clock in?";
        if (hour < 18) return "Good afternoon! Time to start?";
        return "Working late? Start your session here.";
    };

    const fetchUserSchedule = async () => {
        try {
            const response = await getMySchedule();
            console.log("Schedule API Response:", response.data);

            const schedule = response.data;
            const todayName = new Date().toLocaleDateString('en-US', { weekday: 'long' });
            
            const days = schedule.days || schedule.Days || [];
            const todayConfig = days.find(d => d.day === todayName || d.Day === todayName);
            
            if (todayConfig) {
                const breaks = todayConfig.breaks || todayConfig.Breaks || [];
                userBreaks.value = breaks;
                maxBreaks.value = breaks.length;
                console.log("Breaks loaded:", userBreaks.value);
            }
        } catch (e) {
            console.error("Schedule load failed", e);
        }
    };

    const startWorkTimer = (initialSeconds = 0) => {
        totalWorkedSeconds.value = initialSeconds;
        clearInterval(timerInterval);
        timerInterval = setInterval(() => {
            totalWorkedSeconds.value++;
            timerValue.value = formatSeconds(totalWorkedSeconds.value);
        }, 1000);
    };

    const startBreakTimer = (initialSeconds = 0, durationMinutes = 0) => {
        isOverBreakLimit.value = false;
        const limitInSeconds = durationMinutes * 60;
        
        let elapsed = initialSeconds;

        clearInterval(breakInterval);
        
        breakTimerValue.value = formatSeconds(elapsed);
        if (limitInSeconds > 0 && elapsed >= limitInSeconds) {
            isOverBreakLimit.value = true;
        }

        breakInterval = setInterval(() => {
            elapsed++;
            breakTimerValue.value = formatSeconds(elapsed);
            
            if (limitInSeconds > 0 && elapsed >= limitInSeconds) {
                isOverBreakLimit.value = true;
            }
        }, 1000);
    };

    const handleButtonClick = () => {
        currentTimeForPanel.value = getCurrentFormattedTime();
        activePanelTab.value = isClockedIn.value ? 'out' : 'in';
        isPanelOpen.value = true;
    };

    const confirmSave = (data) => {
        isPanelOpen.value = false;
        if (data.type === 'in') {
            isClockedIn.value = true;
            startWorkTimer();
        } else {
            stopAllTimers(true);
        }
    };

    const stopAllTimers = (isClockOut = false) => {
        clearInterval(timerInterval);
        clearInterval(breakInterval);
        
        if (isClockOut) {
            const now = new Date();
            lastOut.value = `Last out at ${getCurrentFormattedTime()}, ${now.toLocaleDateString([], {weekday: 'long'})}`;
            isClockedIn.value = false;
            isOnBreak.value = false;
            totalWorkedSeconds.value = 0;
            breakCount.value = 0;
        }
    };

    const handleBreakAction = async () => {
        if (breakCount.value >= maxBreaks.value) {
            return Swal.fire({ ...swalConfig, title: 'Limit Reached', text: 'No breaks left!', icon: 'error' });
        }

        const inputOptions = {};
        userBreaks.value.forEach((b, i) => {
            const rawDuration = b.breakDuration || b.BreakDuration; // "01:00:00"
            const name = b.breakName || b.BreakName;

            let totalMinutes = 0;

            if (typeof rawDuration === 'string' && rawDuration.includes(':')) {
                const parts = rawDuration.split(':');
                const hours = parseInt(parts[0]) || 0;
                const minutes = parseInt(parts[1]) || 0;
                totalMinutes = (hours * 60) + minutes;
            } else {
                totalMinutes = parseInt(rawDuration) || 0;
            }

            inputOptions[i] = `${name} (${totalMinutes}m)`; 
        });

        const { value: breakIdx, isDismissed } = await Swal.fire({
            ...swalConfig,
            title: 'Select Break',
            input: 'radio',
            inputOptions,
            showCancelButton: true,
            customClass: {
                popup: 'swal-custom-dark',
            },
            inputValidator: (v) => {
                if (v === null || v === undefined) return 'Please select a break type';
            }
        });

        if (isDismissed || breakIdx === undefined) return;

        try {
            await startBreak();
            const selected = userBreaks.value[breakIdx];
            
            stopAllTimers(false);
            isOnBreak.value = true;
            breakCount.value++;
            startBreakTimer(0, selected.breakDuration);
        } catch (e) {
            Swal.fire('Error', 'Failed to start break', 'error');
        }
    };

    const endBreakAction = async () => {
        const { isConfirmed } = await Swal.fire({
            ...swalConfig,
            title: 'End Break?',
            text: "Resume work?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Resume'
        });

        if (!isConfirmed) return;

        try {
            await endBreakApi();
            clearInterval(breakInterval);
            isOnBreak.value = false;
            isOverBreakLimit.value = false;
            startWorkTimer(totalWorkedSeconds.value);
        } catch (e) {
            Swal.fire('Error', 'Failed to end break', 'error');
        }
    };

    onMounted(async () => {
        lastOut.value = updateWelcomeMessage();
        await fetchUserSchedule();
        
        try {
            const { data } = await getTodayStatus();
            if (data?.isClockedIn) {
                isClockedIn.value = true;

                const timeIn = new Date(data.timeIn);
                const now = new Date();
                
                const workSecs = !isNaN(timeIn) ? Math.floor((now - timeIn) / 1000) : 0;
                totalWorkedSeconds.value = workSecs;

                if (data.isOnBreak) {
                    isOnBreak.value = true;
                    const breakStart = new Date(data.breakStart);
                    const breakSecs = !isNaN(breakStart) ? Math.floor((now - breakStart) / 1000) : 0;

                    const currentBreakInfo = userBreaks.value.find(b => 
                        (b.breakName || b.BreakName) === data.currentBreakName
                    );
                    
                    let durationMins = 0;
                    if (currentBreakInfo) {
                        const raw = currentBreakInfo.breakDuration || currentBreakInfo.BreakDuration;
                        durationMins = typeof raw === 'string' && raw.includes(':') 
                            ? (parseInt(raw.split(':')[0]) * 60) + parseInt(raw.split(':')[1])
                            : parseInt(raw) || 0;
                    }

                    startBreakTimer(breakSecs, durationMins); 
                } else {
                    startWorkTimer(workSecs);
                }
            }
        } catch (e) {
            console.error("Initialization failed", e);
        }
    });

    onUnmounted(() => stopAllTimers(false));
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
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.8);
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

    :deep(.swal-custom-dark) {
    border: 1px solid rgba(255, 255, 255, 0.1);
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