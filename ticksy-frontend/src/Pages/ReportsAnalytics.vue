<template>
    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header title="Reports" />
                <div class="cards">
                    <div class="summary-wrapper">
                        <SummaryCard title="Present Summary" :items="presentSummary" />
                        <SummaryCard title="Not Present Summary" :items="notPresentSummary" />
                        <SummaryCard title="Away Summary" :items="awaySummary" />
                    </div>
                    <div class="search-wrapper">
                        <div class="search-box">
                            <Search v-model="search" />
                        </div>
                        <div class="range-container">
                            <DatePicker v-model="startDate" placeholder="DD/MM/YYYY" />
                            <MoveRight class="icon" :size="18" />
                            <DatePicker v-model="endDate" placeholder="DD/MM/YYYY" />
                        </div>
                        <button class="export-btn">
                            <Download class="export-icon" />
                            <span>Export</span>                       
                        </button>
                    </div>
                    <div class="report-table">
                        <table>
                            <colgroup>
                                <col style="width: 27%;" />  <!-- Employee -->
                                <col style="width: 23%;" />  <!-- Clock-in/out -->
                                <col style="width: 10%;" />  <!-- Overtime -->
                                <col style="width: 40%;" />  <!-- Note -->
                            </colgroup>
                            <thead>
                                <tr>
                                    <th>
                                        <div class="table-header">
                                            <Users class="icon" />
                                            <span>Employee Name</span>
                                            <ChevronsUpDownIcon class="sort-icon" @click=""/>
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <ClockCheck class="icon" />
                                            <span>Clock-in & out</span>
                                            <ChevronsUpDownIcon class="sort-icon" @click=""/>
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <ClockPlus class="icon" />
                                            <span>Overtime</span>
                                            <ChevronsUpDownIcon class="sort-icon" @click=""/>
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <FileText class="icon" />
                                            <span>Note</span>
                                            <ChevronsUpDownIcon class="sort-icon" @click=""/>
                                        </div>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(report, index) in filteredReports" :key="index">
                                    <td>
                                        <div class="user-group">
                                            <img :src="sampleIMG" alt="profile" class="avatar" /> 
                                            <span>{{ report.name }}</span>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="time-group">
                                            <span class="span-time-in" >
                                                {{ report.clockIn }} 
                                            </span>
                                            <img :src="timeStyle" alt="style" class="time-style-left" />
                                            <span class="span-total"> {{report.totalHours}} </span>
                                            <img :src="timeStyle" alt="style" class="time-style-right" /> 
                                            <span class="span-time-out" >
                                                {{ report.clockOut }}
                                            </span>
                                        </div>
                                    </td>
                                    <td>{{ report.overtime }}</td>
                                    <td>{{ report.note }}</td>
                                </tr>
                            </tbody>
                            
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>   

<script setup>
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue';
    import Header from '../components/Header.vue';
    import SummaryCard from '../components/SummaryCard.vue'
    import { 
        presentSummary, 
        notPresentSummary, 
        awaySummary, 
        reports
    } from '../services/summaryData.js'
    import { useSearch } from '../services/search.js';
    import Search from '../components/Search.vue'
    import { Users, ClockCheck, ClockPlus, FileText, 
        ChevronsUpDownIcon, MoveRight, Download 
    } from 'lucide-vue-next'
    import timeStyle from '../assets/time_style.png'
    import sampleIMG from '../assets/sample_img.jpg'
    import DatePicker from '../components/DatePicker.vue'
    import { computed } from 'vue'

    const reportsRef = reports
    const { search, filtered: filteredReports } = useSearch(reportsRef, ['name'])

    const startDate = ref(new Date())
    const endDate = ref(new Date())
    const isOpen = ref(true)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
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

    .main-content {
        margin-left: var(--sidebar-width); 
        flex: 1;
        padding: 20px 20px 60px 20px;
        transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
        overflow-x: hidden;
        min-width: 0;
        z-index: 1;
        color: #F0F0F0;
    }

    .app.collapsed .main-content {
        margin-left: var(--sidebar-collapsed-width);
    }

    .cards {
        border-radius: 5px;
        width: 100%;
        min-height: 85vh; 
        padding: 40px 20px; 
        display: flex;
        flex-direction: column;
        box-sizing: border-box;
        gap: 25px;
        margin-bottom: 30px;
    }

    .summary-wrapper {
        display: flex;
        flex-direction: row;
        gap: 15px;
        flex-wrap: wrap; 
    }

    .search-wrapper {
        display: flex;
        align-items: center;
        gap: 15px;
        width: 100%;
    }

    .search-box {
        flex: 3;
    }

    .range-container {
        flex: 3;
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .export-btn {
        flex: 1;
        max-width: 120px;
        padding: 8px 12px;
        border-radius: 8px;
        border: none;
        background-color: #6366f1;
        color: white;
        cursor: pointer;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
    }

    .report-table {
        margin-left: -20px;
        margin-right: -20px;
    }

    table {
        width: 100%;
        height: 100%;
        table-layout: auto;
        border-collapse: collapse;
    }

    span {
        font-size: 14px;
    }

    .icon {
        width: 16px;
        height: 16px;
    }

    .sort-icon {
        width: 20px;
        height: 20px;
        cursor: pointer;
    }

    .sort-icon:hover {
        opacity: 0.5;
    }

    .table-header {
        display: flex;
        align-items: center;
        gap: 10px;
        width: 100%;
        justify-content: flex-start;
        padding: 10px 20px;
    }

    th {
        text-align: left;
        white-space: nowrap;
    }

    thead {
        background-color: #75889A4D;
    }
    
    td {
        padding: 10px 20px;
        font-size: 13px;
        border-right: 1px solid #243D53;
        border-bottom: 1px solid #243D53;
    }

    span {
        font-size: 13px;
    }

    td:last-child {
        border-right: none;
    }

    tbody tr:last-child td {
        border-bottom: none;
    }

    .avatar {
        width: 30px;
        height: 30px;
        border-radius: 50%;
    }

    .user-group, .time-group {
        display: flex;
        align-items: center;
        gap: 15px;
    }

    .time-group {
        justify-content: center;
    }

    .time-style-right {
        transform: scaleX(-1);
    }

    .span-time-in {
        color: #00DC28;
    }

    .span-time-out {
        color: #F3857C;
    }

    .span-total {
        font-size: 12px;
    }

    .export-btn {
        background-color: #234C70;
        gap: 10px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
        display: flex;
        justify-content: center;
        align-items: center;

        outline: none;
    }

    .export-btn:hover {
        opacity: 0.8;
    }

    .export-icon {
        width: 16px;
        height: 16px;
    }

</style>