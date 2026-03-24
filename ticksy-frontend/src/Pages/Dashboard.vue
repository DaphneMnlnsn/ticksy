<script setup>
    import Header from '../components/Header.vue';
    import { ref } from 'vue'
    import welcomeImg from "/welcome-img.png";
    import WelcomeCard from '../components/WelcomeCard.vue';
    import HolidayCard from '../components/HolidayCard.vue';
    import TrackedHours from '../components/TrackedHours.vue';
    import Sidebar from '../components/Sidebar.vue';

    const activeTab = ref ('Day')
    const holidays = [
    { month: 'MAY', day: '25', name: 'Memorial Day' },
    { month: 'JUN', day: '19', name: 'Juneteenth' },
    { month: 'JUL', day: '03', name: 'Memorial Independence Day (substitute)' }
    ]
    const isOpen = ref(true)
    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
</script>

<template>
    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header 
                    title="Dashboard"
                    :showTimer="true"
                    lastOut="Last out an hour ago"
                />

                <div class="cards">
                    <div class="tabs">
                        <button  v-for="tab in ['Day', 'Week', 'Month']" 
                            :key="tab"
                            :class="{ active: activeTab === tab}"
                            @click="activeTab = tab"
                        > {{ tab }}
                        </button>
                    </div>

                    <div v-if="activeTab === 'Day'" class="tab-content">
                        <div class="two-cards">
                            <WelcomeCard :viewMode="activeTab" :welcomeImg="welcomeImg"/>
                            <HolidayCard :holidays="holidays" />
                        </div>
                        <TrackedHours :viewMode="activeTab" />
                    </div>

                    <div v-else-if="activeTab === 'Week'" class="tab-content">
                        <div class="two-cards">
                            <WelcomeCard :viewMode="activeTab" :welcomeImg="welcomeImg"/>
                            <HolidayCard :holidays="holidays"/>
                        </div>
                        <TrackedHours :viewMode="activeTab" />
                    </div>

                    <div v-else-if="activeTab === 'Month'" class="tab-content">
                        <div class="two-cards">
                            <WelcomeCard :viewMode="activeTab" :welcomeImg="welcomeImg"/>
                            <HolidayCard :holidays="holidays"/>
                        </div>
                        <TrackedHours :viewMode="activeTab" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>   

<style scoped>

:global(html), 
:global(body), 
:global(#app) {
    width: 100% !important;
    margin: 0;
    padding: 0;
    overflow: hidden;
}

.main-bg {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-image: url("../assets/main_bg.jpg");
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
}

.dashboard {
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100vh;
}

.app {
    display: flex;
    width: 100%;
    flex: 1;
    min-height: 100vh;
    overflow: hidden;
}

.main-content {
    margin-left: var(--sidebar-width); 
    flex: 1;
    padding: 20px 20px 60px 20px;
    transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
    overflow-x: hidden;
    min-width: 0;
    z-index: 1;
}

.main-content::-webkit-scrollbar {
    width: 8px;
}

.main-content::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.1); 
    border-radius: 10px; 
    border: 2px solid transparent; 
    background-clip: content-box; 
}

.main-content::-webkit-scrollbar-thumb:hover {
    background: rgba(255, 255, 255, 0.25); 
}


.main-content::-webkit-scrollbar-track {
    background: transparent; 
}

.app.collapsed .main-content {
    margin-left: var(--sidebar-collapsed-width);
}

.tabs{
    display: flex;
    gap: 10px;
    padding: 0;
    border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.tabs button{
    background: none;
    border: none;
    outline: none;
    color: #ffff;
    position: relative;
    font-size: 16px;
    margin-top: 15px;
    margin-left: 12px;
}

.tabs button::after {
    content: "";
    position: absolute;
    bottom: -1px;
    left: 0;
    width: 0;
    height: 3.1px;
    border-radius: 3px;
    background: rgb(117, 136, 154, 50%);
    transition: width 0.3s ease-in-out;
}

.tabs button.active {
    color: #88b6ff;
    font-weight: 600;
    transition: color 0.3s ease;
}

.tabs button.active::after {
    width: 100%; 
    background: #88b6ff; 
    box-shadow: 0 0 8px rgba(59, 130, 246, 0.5);
}

.tabs button:hover:not(.active) {
    color: rgba(255, 255, 255, 0.8);
    cursor: pointer;
}

.cards {
    background-color: rgba(0, 19, 36, 0.2);
    border-radius: 5px;
    width: 100%;
    min-height: 85vh; 
    padding: 5px 20px 45px 20px; 
    display: flex;
    flex-direction: column;
    box-sizing: border-box;
    margin-bottom: 30px;
}

.two-cards {
    display: flex;
    gap: 20px;
    padding: 25px 0;
    width: 100%;
    box-sizing: border-box;
}

.welcome-card,
.holiday-card {
    flex: 1 1 0; 
    min-width: 0; 
    height: 247px;
    border-radius: 5px;
}

h3{
    font-size: 18px;
    font-weight: 600;
    margin-top: 5px;
    margin-left: 10px;
}

.tab-content {
    animation: fadeIn 0.4s ease-in-out;
    display: flex;
    flex-direction: column;
    width: 100%;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}
</style>