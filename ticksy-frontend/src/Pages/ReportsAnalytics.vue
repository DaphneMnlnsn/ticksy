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
                                            <ChevronsUpDownIcon 
                                                :class="['sort-icon', { active: sortConfig.key === 'employeeName' }]" 
                                                @click="handleSort('employeeName')"
                                            />
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <ClockCheck class="icon" />
                                            <span>Clock-in & out</span>
                                            <ChevronsUpDownIcon 
                                            :class="['sort-icon', { active: sortConfig.key === 'clockIn' }]" 
                                            @click="handleSort('clockIn')"/>
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <ClockPlus class="icon" />
                                            <span>Overtime</span>
                                            <ChevronsUpDownIcon 
                                            :class="['sort-icon', { active: sortConfig.key === 'overtimeHours' }]" 
                                            @click="handleSort('overtimeHours')"/>
                                        </div>
                                    </th>
                                    <th>
                                        <div class="table-header">
                                            <FileText class="icon" />
                                            <span>Note</span>
                                            <ChevronsUpDownIcon
                                            :class="['sort-icon', { active: sortConfig.key === 'notes' }]" 
                                            @click="handleSort('notes')"/>
                                        </div>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(report, index) in sortedAndFilteredReports" :key="index">
                                    <td>
                                        <div class="user-group">
                                            <img :src="report.avatar || sampleIMG" alt="profile" class="avatar" /> 
                                            <span>{{ report.employeeName }}</span>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="time-group">
                                            <span class="span-time-in">{{ report.clockIn }}</span>
                                            <img :src="timeStyle" alt="style" class="time-style-left" />
                                            
                                            <span class="span-total">{{ report.totalHours }}h</span>
                                            
                                            <img :src="timeStyle" alt="style" class="time-style-right" /> 
                                            <span class="span-time-out">{{ report.clockOut }}</span>
                                        </div>
                                    </td>
                                    <td>{{ report.overtimeHours }}h</td>
                                    <td>{{ report.notes || '-' }}</td>
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
    import Sidebar from '../components/Sidebar.vue';
    import Header from '../components/Header.vue';
    import SummaryCard from '../components/SummaryCard.vue'
    import Search from '../components/Search.vue'
    import { Users, ClockCheck, ClockPlus, FileText, 
        ChevronsUpDownIcon, MoveRight, Download 
    } from 'lucide-vue-next'
    import timeStyle from '../assets/time_style.png'
    import sampleIMG from '../assets/sample_img.jpg'
    import DatePicker from '../components/DatePicker.vue'
    import { computed } from 'vue'
    import { ref, onMounted, watch } from 'vue'
    import { useSearch } from '../services/search.js'
    import { getAttendanceReport, getAttendanceSummary } from '../services/reports.js'

    const startDate = ref(new Date())
    const endDate = ref(new Date())
    const isOpen = ref(true)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const reports = ref([])
    const reportsRef = reports
    const loading = ref(false)

    async function loadAttendanceData() {
        loading.value = true
        try {
            const reportResponse = await getAttendanceReport(startDate.value, endDate.value);
            reports.value = reportResponse.data;

            const summaryResponse = await getAttendanceSummary(startDate.value, endDate.value);
            updateSummaryCards(summaryResponse.data);
            
        } catch (error) {
            console.error("Error loading reports:", error);
        } finally {
            loading.value = false
        }
    }

    watch([startDate, endDate], () => {
        if (startDate.value && endDate.value) {
            loadAttendanceData();
        }
    }, { immediate: false });

    onMounted(loadAttendanceData);

    const presentSummary = ref([]);
    const notPresentSummary = ref([]);
    const awaySummary = ref([]);

    function updateSummaryCards(data) {
        presentSummary.value = [
            { label: 'Present', value: data.present, color: '#4CAF50' },
            { label: 'Late', value: data.late, color: '#FF9800' },
            { label: 'Half Day', value: data.halfDay, color: '#2196F3' }
        ];

        notPresentSummary.value = [
            { label: 'Absent', value: data.absent, color: '#F44336' },
            { label: 'No Clock-In', value: data.noClockIn, color: '#9C27B0' }
        ];

        awaySummary.value = [
            { label: 'On Leave', value: data.onLeave, color: '#795548' },
            { label: 'Day Off', value: data.dayOff, color: '#607D8B' },
            { label: 'No Clock-Out', value: data.noClockOut, color: '#FF5722' }
        ];
    }

    const sortConfig = ref({ key: null, direction: 'asc' });

    function handleSort(key) {
        let direction = 'asc';
        if (sortConfig.value.key === key && sortConfig.value.direction === 'asc') {
            direction = 'desc';
        }
        sortConfig.value = { key, direction };
    }

    const sortedAndFilteredReports = computed(() => {
        let items = [...filteredReports.value];

        if (sortConfig.value.key) {
            items.sort((a, b) => {
                const varA = a[sortConfig.value.key];
                const varB = b[sortConfig.value.key];

                if (varA < varB) return sortConfig.value.direction === 'asc' ? -1 : 1;
                if (varA > varB) return sortConfig.value.direction === 'asc' ? 1 : -1;
                return 0;
            });
        }
        return items;
    });

    const { search, filtered: filteredReports } = useSearch(reports, ['employeeName'])

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

    .report-table {
        animation: fadeIn 0.4s ease-in-out;
        display: flex;
        flex-direction: column;
        width: 100%;
    }

    .sort-icon {
        cursor: pointer;
        opacity: 0.5;
        transition: opacity 0.2s;
    }
    .sort-icon:hover, .sort-icon.active {
        opacity: 1;
        color: #4379a9;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }

</style>