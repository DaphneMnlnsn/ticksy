<template>
    <div class="header">
        <h1>{{ title }}</h1>

        <template v-if="showTimer">
            <span class="out-time">{{ lastOut }}</span>
            <Timer 
                :size="23" 
                color="#00DC28" 
                stroke-width="2" 
                class="header-icon" 
            />
        </template>
    </div>
</template>

<script setup>
    import { Timer, Coffee, Square } from "lucide-vue-next"; 
    import { ref } from "vue";

    const lastOut = "Last out an hour ago"
    const isClockedIn = ref(false)
    const timerValue = ref("0:00:00")
    let timerInterval = null

    const toggleClock = () => {
        isClockedIn.value = !isClockedIn.value;
    
        if  (isClockedIn.value) {
            startTimer();
        }
        else {
            stopTimer();
        }
    }

    const startTimer = () => {
        let seconds = 0;
        timerInterval = setInterval(() => {
            seconds++;
            const h = Math.floor(seconds / 3600);
            const m = Math.floor((seconds % 3600) / 60);
            const s = seconds % 60;
            timerValue.value = `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        }, 1000);
    };

    const stopTimer = () => {
        clearInterval(timerInterval);
        timerValue.value = "0:00:00";
    };


    defineProps({
        title: {
            type: String,
            default: "Page Title"
        },
        showTimer: {
            type: Boolean,
            default: false
        },
        lastOut: {
            type: String,
            default: ""
        }
    })
</script>

<style scoped>
    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        width: 100%;
        padding: 0 0 0 10px;  
    }

    .out-time {
        font-size: 14px;
        opacity: 0.8;
        color: white;
        padding: 10px;
        margin-left: auto;
    }

    .header-icon {
        opacity: 0.8;
        cursor: pointer;
        transition: transform 0.2s ease;
    }

    .header-icon:hover {
        transform: scale(1.1);
        opacity: 1;
    }

    h1 {
        font-size: 30px;
        font-family: Righteous;
    }
</style>
