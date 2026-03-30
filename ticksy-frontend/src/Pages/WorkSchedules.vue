<template>
    <DimmedBg :show="isScheduleOpen" @close="isScheduleOpen = false" /> 

    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header 
                    title="Work Schedules"
                    :showTimer="false"
                />

                <div class="cards">
                    <div class="tabs">
                        <button  v-for="tab in ['Schedules', 'Time Off', 'Holidays']" 
                            :key="tab"
                            :class="{ active: activeTab === tab}"
                            @click="activeTab = tab"
                        > {{ tab }}
                        </button>
                    </div>

                    <div v-if="activeTab === 'Schedules'" class="tab-content">
                        <div class="two-cards">
                            <div class="left-side">
                                <SidePanel 
                                    buttonText="+ Add Schedule"
                                    @actionClick="handleAddSchedule"
                                >
                                    <template #header>
                                        <Search />
                                    </template>

                                    <template #body>
                                        <RowItem 
                                            v-for="item in schedules" 
                                            :key="item.id" 
                                            :item="item"
                                            :icon="item.icon"
                                        />
                                    </template>
                                </SidePanel> 
                            </div>
                            <div class="right-side">
                                <div class="container">
                                    <div class="top-row">
                                        <div class="mini-card">
                                            <div class="card-title">Weekly Summary</div>
                                            
                                            <div class="card-row">
                                                <span>Total Weekly Hours</span>
                                                <span>48 Hrs</span>
                                            </div>   
                                            <div class="card-row">
                                                <span>Average Shift</span>
                                                <span>8 Hrs</span>
                                            </div>  
                                            <div class="card-row">
                                                <span>Scheduled Days</span>
                                                <span>6</span>
                                            </div>   
                                        </div>
                                        <div class="mini-card">
                                            <div class="card-title">Status</div>

                                            <div class="card-row">
                                                <span>Work Arrangement</span>
                                                <span style="color:#00DC28">Fixed</span>
                                            </div>   
                                            <div class="card-row">
                                                <span>Total Staff Assigned</span>
                                                <span>12</span>
                                            </div>  
                                            <div class="card-row">
                                                <span>Last Modified</span>
                                                <span>5 Mar 2026</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mini-card">
                                        <div class="divider">
                                            <div class="divider-left">
                                                <div class="card-title">SCHEDULE</div>

                                                <div class="schedule">
                                                    <div class="row">
                                                        <span>Monday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Tuesday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Wednesday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Thursday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Friday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Saturday</span>
                                                        <span>9:00am - 5:00pm</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Sunday</span>
                                                        <span class="rest-day">Rest Day</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="divider-right">
                                                <div class="card-title">BREAKS</div>

                                                <div class="schedule">
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <span>Lunch Time</span>
                                                        <span>60 minutes</span>
                                                    </div>
                                                    <div class="row">
                                                        <button class="add-btn">+ Add Break</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mini-card">
                                        <div class="card-title">ASSIGNED MEMBERS</div>
                                        <div class="row">
                                            <button class="add-btn">Assign</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div v-else-if="activeTab === 'Time Off'" class="tab-content">
                        <div class="two-cards">
                            <div class="container">
                                    
                            </div>
                        </div>
                    </div>

                    <div v-else-if="activeTab === 'Holidays'" class="tab-content">
                        <div class="two-cards">
                            <div class="left-side">
                                <SidePanel 
                                    buttonText="+ Add New Calendar"
                                    @actionClick="handleAddCalendar"
                                >
                                    <template #header>
                                        <Search />
                                    </template>

                                    <template #body>
                                        <RowItem 
                                            v-for="item in calendars" 
                                            :key="item.id" 
                                            :item="item"
                                            :icon="item.icon"
                                        />
                                    </template>

                                </SidePanel> 
                            </div>
                            <div class="right-side">
                                <div class="container">
                                    
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>   

<script setup>
    import Header from '../components/Header.vue'
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import SidePanel from '../components/SidePanel.vue'
    import { } from 'lucide-vue-next'
    import DimmedBg from '../components/DimmedBg.vue'
    import Search from '../components/Search.vue'
    import RowItem from '../components/RowItem.vue'
    import { schedules, calendars } from '../services/summaryData'

    const activeTab = ref ('Schedules')
    const isScheduleOpen = ref(false)

    const isOpen = ref(true)
    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    function handleAddSchedule() {
        console.log("Add Schedule clicked")
    }

</script>

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

    .setup-banner {
        display: flex;
        justify-content: space-between;
        align-items: center;
        background: rgba(255, 255, 255, 0.1);
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        padding: 12px 20px;
        margin: -20px -20px 20px -20px;
        color: #ffffff;
        font-size: 12px;
    }

    .main-content {
        margin-left: var(--sidebar-width); 
        flex: 1;
        padding: 20px 20px 60px 20px;
        transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
        color:#ffffff;
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
        height: 85vh; 
        padding: 5px 0 0 0; 
        display: flex;
        flex-direction: column;
        box-sizing: border-box;
        margin-bottom: 30px;
    }

    .two-cards {
        display: flex;
        width: 100%;
        height: 100%;
        box-sizing: border-box;
        align-items: stretch;
        flex: 1;
    }

    .left-side {
        width: 260px;
        min-width: 260px;
        max-width: 260px;
        height: 100%;
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .right-side {
        width: 100%;
        padding: 0 20px;
    }

    h3 {
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
        flex: 1;
    }

    .container {
        padding: 20px;
        display: flex;
        flex-direction: column;
        gap: 10px;
        width: 100%;
    }

    .top-row {
        display: flex;
        flex-direction: row;
        gap: 20px;
        width: 100%
    }

    .mini-card {
        padding: 15px;
        border: 2px solid #75889A80;;
        border-radius: 10px;
        width: 100%;
    }

    .card-title {
        font-size: 15px;
        font-weight: 500;
        color: #F0F0F0;
        padding-bottom: 10px;
    }

    .card-row {
        display: flex;
        justify-content: space-between;
        margin-bottom: 3px;
    }

    .card-row span {
        font-size: 13px;
    }

    .divider {
        display: flex;
        flex-direction: row;
        justify-content: center;
        gap: 20px;
    }

    .divider-left {
        border-right: 1px solid #F0F0F0;
        padding: 20px;
        flex: 1;
    }

    .divider-right {
        padding: 20px;
        flex: 1;
    }

    .schedule {
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .row {
        display: flex;
        justify-content: flex-start;
        gap: 40px;
        padding-bottom: 5px;
    }

    .row span:first-child {
        width: 120px;
    }

    .row span {
        font-size: 13px;
    }

    .rest-day {
        font-style: italic;
        color: #9ca3af;
    }

    .add-btn {
        background-color: #003867;
        border: none;
        outline: none;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>