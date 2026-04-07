<template>
    <div class="holiday-card">
        <div class="holiday-header">
            <h3>UPCOMING HOLIDAYS AND TIME OFF</h3>
            <Calendar :size="26" color="white" stroke-width="2" opacity="0.9" class="calendar-icon clickable" @click="workSchedules"/>
        </div>

        <div class="holiday-list">
            <div class="holiday-item" v-for="holiday in holidays" :key="holiday.name">
                <div class="holiday-date">
                    <span class="month">{{ holiday.month }}</span>
                    <span class="day">{{ holiday.day }}</span>
                </div>

                <div class="divider"></div>

                <div class="holiday-info">
                    <strong>{{ holiday.name }}</strong>
                    <p>Imported Calendar (United States)</p>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { Calendar } from 'lucide-vue-next';
    import { useRouter } from 'vue-router';

    const router = useRouter();
    
    const workSchedules = () => {
        router.push('/management/work-schedules');
    }

    const props = defineProps({
        holidays: Array,
        welcomeImg: String
    })
</script>

<style scoped>
.holiday-card {
    height: 250px;
    background-color: #001527;
    border-radius: 10px;
    padding: 20px;
    color: white;
    display: flex;
    flex-direction: column;
}

.holiday-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}

.holiday-header h3 {
    margin: 3px 0 3px 10px;
}

.calendar-icon {
    opacity: 0.8;
    cursor: pointer;
    transition: transform 0.2s ease;
}

.calendar-icon:hover {
    transform: scale(1.1);
    opacity: 1;
}

.clickable {
    cursor: pointer;
    transition: transform 0.2s ease, opacity 0.2s ease;
}

.clickable:hover {
    opacity: 1 !important; 
    transform: scale(1.1);
}

.holiday-item {
    display: flex;
    align-items: center;
    margin-left: 15px;
    margin-bottom: 20px;
}

.holiday-item {
    transition: transform 0.2s ease, opacity 0.2s ease;
}

.holiday-item:hover {
    transform: translateX(4px);
    opacity: 1;
}

.holiday-list {
    flex: 1;
    max-height: 22vh;   
    overflow-y: auto;
    padding-right: 5px;
    scrollbar-width: thin;
    scrollbar-color:#001527 transparent;
    scroll-behavior: smooth;
}

.holiday-list::-webkit-scrollbar-track {
    background: transparent;
}

.holiday-list::-webkit-scrollbar-thumb {
    background: #003867;
    border-radius: 10px;
    transition: 0.3s;
}

.holiday-list::-webkit-scrollbar-thumb:hover {
    background: #003867;
}

.holiday-list::-webkit-scrollbar-thumb {
    background: transparent;
}

.holiday-date {
    width: 60px;
    text-align: center;
}

.month {
    display: block;
    font-size: 12px;
    opacity: 0.7;
}

.day {
    font-size: 20px;
    font-weight: bold;
}

.divider {
    width: 1.3px;
    height: 40px;
    background-color: rgba(255,255,255,0.3);
    margin: 0 15px;
}

.holiday-info strong {
    display: block;
    font-size: 14px;
}

.holiday-info p {
    font-size: 12px;
    opacity: 0.6;
    margin: 2px 0 0 0;
}
</style>