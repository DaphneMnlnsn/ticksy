<template>
    <DimmedBg :show="isScheduleOpen || isRequestOpen" @close="closeAllPanels" />

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
                                        <Search v-model="scheduleSearch" />
                                    </template>

                                    <template #body>
                                        <RowItem 
                                            v-for="item in filteredSchedules" 
                                            :key="item.id" 
                                            :item="item"
                                            :icon="item.icon"
                                            :class="{ active: selectedSchedule?.id === item.id }"
                                            @click="selectSchedule(item.id)" 
                                        />
                                    </template>
                                </SidePanel> 
                            </div>
                            <div class="right-side" v-if="selectedSchedule">
                                <div class="container">
                                    <div class="top-row">
                                        <div class="mini-card">
                                            <div class="card-title">Weekly Summary</div>
                                            
                                            <div class="card-row">
                                                <span>Total Weekly Hours</span>
                                                <span>{{ totalWeeklyHours }} Hrs</span>
                                            </div>   
                                            <div class="card-row">
                                                <span>Average Shift</span>
                                                <span>{{ averageShift }} Hrs</span>
                                            </div>  
                                            <div class="card-row">
                                                <span>Scheduled Days</span>
                                                <span>{{ scheduledDaysCount }}</span>
                                            </div>   
                                        </div>
                                        <div class="mini-card">
                                            <div class="card-title">Status</div>

                                            <div class="card-row">
                                                <span>Work Arrangement</span>
                                                <span style="color:#00DC28">{{ selectedSchedule.workArrangement }}</span>
                                            </div>
                                            <div class="card-row">
                                                <span>Total Staff Assigned</span>
                                                <span>
                                                    {{ selectedSchedule ? (selectedSchedule.userWorkSchedules?.length || 0) : '—' }}
                                                </span>
                                            </div>
                                            <div class="card-row">
                                                <span>Last Modified</span>
                                                <span>{{ formatFullDateTime(selectedSchedule.updatedAt) }}</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mini-card">
                                        <div class="divider">
                                            <div class="divider-left">
                                                <div class="card-title2">
                                                    <span>SCHEDULE</span>
                                                    <SquarePen class="icon"/>
                                                </div>

                                                <div class="schedule">
                                                    <div v-if="selectedSchedule.workArrangement === 'Weekly'" class="row">
                                                        <span>Required Weekly</span>
                                                        <span class="total-highlight">{{ displayWeeklyHours }} Hours</span>
                                                    </div>

                                                    <template v-else>
                                                        <div v-for="day in selectedSchedule.days" :key="day.day" class="row">
                                                            <span>{{ day.day }}</span>

                                                            <span v-if="day.isRestDay" class="rest-day">Rest Day</span>
                                                            
                                                            <span v-else-if="selectedSchedule.workArrangement === 'Flexible'">
                                                                {{ formatDuration(day.duration) || 0 }} hrs
                                                            </span>

                                                            <span v-else>
                                                                {{ formatTime(day.startTime) }} - {{ formatTime(day.endTime) }}
                                                            </span>
                                                        </div>
                                                    </template>
                                                </div>
                                            </div>
                                            <div class="divider-right">
                                                <div class="card-title2">BREAKS</div>
                                                <div class="schedule">
                                                    <div v-for="breakItem in selectedSchedule.breaks" :key="breakItem.id" class="row">
                                                        <span>{{ breakItem.name }}</span>
                                                        <span>{{ breakItem.duration }} minutes</span>
                                                    </div>
                                                    <div class="btn-row">
                                                        <button class="add-btn" @click="isAddBreakOpen = true">+ Add Break</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mini-card">
                                        <div class="card-title">ASSIGNED MEMBERS</div>
                                        <div class="row2">
                                            <div class="avatar-group">
                                                <img v-for="assignment in selectedSchedule?.userWorkSchedules" 
                                                    :key="assignment.userId"
                                                    :src="assignment.profilePicture || sampleIMG" 
                                                    :title="assignment.fullName"
                                                    class="avatar"
                                                />
                                                
                                                <span v-if="!selectedSchedule?.userWorkSchedules?.length" class="no-members">
                                                    None
                                                </span>
                                            </div>
                                            <div>
                                                <button class="assign-btn" @click="handleAssignMember">Assign</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div v-else-if="activeTab === 'Time Off'" class="tab-content">
                        <div class="two-cards">
                            <div class="container">
                                <div class="search-row">
                                    <div class="search-box">
                                        <Search v-model="requestSearch" />
                                    </div>

                                    <button class="create-btn" @click="handleAddRequest">
                                        <FilePlus class="create-icon"/>
                                        <span>Create Request</span>
                                    </button>
                                </div>
                                <div class="holiday-table-wrapper">
                                    <table class="timeoff-table">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <div class="table-header">
                                                        <input type="checkbox" v-model="selectAllRequests" @change="toggleAllRequests" />
                                                    </div>
                                                </th>
                                                <th><div class="table-header">Name</div></th>
                                                <th><div class="table-header">Type</div></th>
                                                <th><div class="table-header">Reason</div></th>
                                                <th><div class="table-header">Requested Dates</div></th>
                                                <th><div class="table-header">Status</div></th>
                                                <th><div class="table-header"></div></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="request in filteredRequests" :key="request.id">
                                                <td>
                                                    <input type="checkbox" v-model="selectedRequests" :value="request.id" />
                                                </td>
                                                <td>{{ request.name }}</td>
                                                <td>
                                                    <span :class="['leave-badge', getLeaveTypeClass(request.type)]">
                                                        {{ request.type }}
                                                    </span>
                                                </td>
                                                <td>{{ request.reason }}</td>
                                                <td>{{ formatRange(request.r_date) }}</td>
                                                <td>
                                                    <span :class="['status-badge', getStatusClass(request.status)]">
                                                        {{ request.status }}
                                                    </span>
                                                </td>
                                                <td><span class="dots" @click="toggleMenu(user.email)">•••</span></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
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
                                        <Search v-model="calendarSearch"/>
                                    </template>

                                    <template #body>
                                        <RowItem
                                            v-for="item in filteredCalendars" 
                                            :key="item.id"
                                            :item="item"
                                            :icon="item.icon"
                                            :class="{ active: activeCalendarName === item.name }"
                                            @click="selectCalendar(item)"
                                        />
                                    </template>

                                </SidePanel> 
                            </div>
                            <div class="right-side">
                                <div class="holiday-container">
                                    <div class="title-group">
                                        <div>
                                            <span>{{ activeCalendarName }}</span>
                                            <span class="year-bubble">{{ new Date().getFullYear().toString() }}</span>
                                        </div>
                                        
                                        <div class="btn-group">
                                            <SquarePen class="icon" />
                                            <SquarePlus class="icon" />
                                        </div>
                                    </div>
                                    <div class="holiday-table-wrapper">
                                        <table class="holiday-table">
                                            <colgroup>
                                                <col style="width: 55%;" />
                                                <col style="width: 23%;" />
                                                <col style="width: 22%;" />
                                            </colgroup>
                                            <thead>
                                                <tr>
                                                    <th><div class="table-header">Name</div></th>
                                                    <th><div class="table-header"><span>Date</span></div></th>
                                                    <th><div class="table-header"><span>Day</span></div></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(holiday, index) in selectedCalendar" :key="index">
                                                    <td>{{ holiday.name }}</td>
                                                    <td>{{ formatDate(holiday.date) }}</td>
                                                    <td>{{ holiday.day }}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <SchedulePanel
        :isOpen="isScheduleOpen"
        :isSidebarCollapsed="!isOpen"
        @close="isScheduleOpen = false"
    />
    <RequestTimeOffPanel
        :isOpen="isRequestOpen"
        :isSidebarCollapsed="!isOpen"
        @close="isRequestOpen = false"
    />
    <AddBreak
        :isOpen="isAddBreakOpen"
        :isSidebarCollapsed="!isospin"
        @close="isAddBreakOpen = false"
        @save="handleSaveBreak"
    />

</template>   

<script setup lang="ts">
    import Header from '../components/Header.vue'
    import { ref, onMounted } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import SidePanel from '../components/SidePanel.vue'
    import SchedulePanel from '../components/SchedulePanel.vue'
    import DimmedBg from '../components/DimmedBg.vue'
    import Search from '../components/Search.vue'
    import RowItem from '../components/RowItem.vue'
    import { calendars } from '../services/summaryData'
    import sampleIMG from '../assets/sample_img.jpg'
    import { SquarePen, SquarePlus, FilePlus, ChevronsRight } from 'lucide-vue-next'
    import { computed } from 'vue';
    import { useSearch } from '../services/search'
    import { getScheduleById, getSchedules } from '../services/schedule'
    import { getRequests } from '../services/timeOff'
    import { getCalendars } from '../services/calendars'
    import { getHolidays } from '../services/holidays'
    import { formatDate, formatDuration, formatFullDateTime, formatRange, formatTime } from '../services/formatting'
    import RequestTimeOffPanel from '../components/RequestTimeOffPanel.vue'
    import AddBreak from '../components/AddBreak.vue'

    const activeTab = ref ('Schedules')
    const isScheduleOpen = ref(false)
    const isRequestOpen = ref(false)
    const isOpen = ref(true)
    const isAddBreakOpen = ref(false)

    const selectedRequests = ref([]);
    const selectAllRequests = ref(false);

    const handleSaveBreak = (breakData) => {
        console.log("New break received:", breakData);
    };

    const toggleAllRequests = () => {
        if (selectAllRequests.value) {
            selectedRequests.value = filteredRequests.value.map(r => r.id); 
        } else {
            selectedRequests.value = []; 
        }
    };

    const displayWeeklyHours = computed(() => {
        const input = selectedSchedule.value?.weeklyDuration || "00:00:00";
        
        const match = input.match(/(?:(\d+)\.)?(\d+):(\d+)/);
        
        if (!match) return '0';

        const days = parseInt(match[1] || '0');
        const hours = parseInt(match[2] || '0');
        const minutes = parseInt(match[3] || '0');

        const totalHours = (days * 24) + hours;
        
        return minutes > 0 
            ? `${totalHours}:${String(minutes).padStart(2, '0')}` 
            : totalHours;
    });

    const getLeaveTypeClass = (type) => {
        switch (type) {
            case 'Sick Leave':
                return 'leave-sick';
            case 'Vacation Leave':
                return 'leave-vacation';
            case 'Emergency Leave':
                return 'leave-emergency';
            case 'Unpaid Leave':
                return 'leave-unpaid';
            default:
                return '';
        }
    };

    const getStatusClass = (status) => {
        switch (status) {
            case 'Approved':
                return 'status-approved';
            case 'Pending':
                return 'status-pending';
            case 'Rejected':
                return 'status-rejected';
            default:
                return '';
        }
    };

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
    function handleAddSchedule() {
        isScheduleOpen.value = true;
    }
    function handleAddRequest() {
        isRequestOpen.value = true;
    }

    const schedules = ref<{ id: number, name: string, label?: string, icon: any }[]>([]);
    const selectedSchedule = ref<any>(null);
    const isLoadingDetails = ref(false);

    async function loadSchedules() {
        try {
            schedules.value = await getSchedules();
            
            if (schedules.value.length > 0) {
                await selectSchedule(schedules.value[0].id);
            }
        } catch (error) {
            console.error("Failed to load schedules:", error);
        }
    }

    async function selectSchedule(id: number) {
        isLoadingDetails.value = true;
        try {
            selectedSchedule.value = await getScheduleById(id);
        } catch (error) {
            console.error("Error fetching schedule details:", error);
        } finally {
            isLoadingDetails.value = false;
        }
    }

    const requests = ref([]);

    async function loadTimeOffRequests() {
        try {
            requests.value = await getRequests();
        } catch (error) {
            console.error("Failed to load requests:", error);
        }
    }

    const calendars = ref([]);
    const selectedCalendar = ref<any>([]);
    const activeCalendarName = ref("");

    async function loadCalendars() {
        try {
            calendars.value = await getCalendars();

            if (calendars.value.length > 0) {
                await selectCalendar(calendars.value[0]);
            }
        } catch (error) {
            console.error("Failed to load calendars:", error);
        }
    }

    async function selectCalendar(item: any) {
        try {
            console.log("Calendar clicked!", item);
            activeCalendarName.value = item.name;
            
            try {
                selectedCalendar.value = await getHolidays(item.id, new Date().getFullYear().toString());
            } catch (error) {
                console.error("Error fetching calendar details:", error);
            }
        } catch (error) {
            console.error("Error fetching calendar details:", error);
        }
    }

    onMounted(() => {
        loadSchedules();
        loadTimeOffRequests();
        loadCalendars();
    });

    const totalWeeklyHours = computed(() => {
        if (!selectedSchedule.value?.days) return 0;
        
        return selectedSchedule.value.days.reduce((acc, day) => {
            if (day.isRestDay) return acc;

            if (day.duration) {
                return acc + parseFloat(day.duration);
            }

            if (day.startTime && day.endTime) {
                const [startH, startM] = day.startTime.split(':').map(Number);
                const [endH, endM] = day.endTime.split(':').map(Number);

                const startInMinutes = startH * 60 + startM;
                const endInMinutes = endH * 60 + endM;

                let diffInMinutes = endInMinutes - startInMinutes;
                if (diffInMinutes < 0) {
                    diffInMinutes += 24 * 60;
                }

                return acc + (diffInMinutes / 60);
            }

            return acc;
        }, 0);
    });

    const averageShift = computed(() => {
        const activeDays = selectedSchedule.value?.days.filter(d => !d.isRestDay) || [];
        if (activeDays.length === 0) return 0;
        return (totalWeeklyHours.value / activeDays.length).toFixed(1);
    });

    const scheduledDaysCount = computed(() => {
        return selectedSchedule.value?.days.filter(d => !d.isRestDay).length || 0;
    });

    const { search: scheduleSearch, filtered: filteredSchedules } = useSearch(schedules, ['name']);
    const { search: requestSearch, filtered: filteredRequests } = useSearch(requests, ['name', 'reason', 'type']);
    const { search: calendarSearch, filtered: filteredCalendars } = useSearch(calendars, ['name']);

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

    .tabs {
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
        padding: 5px 0 0 0; 
        display: flex;
        flex-direction: column;
        box-sizing: border-box;
        margin-bottom: 30px;
    }

    .two-cards {
        display: flex;
        width: 100%;
        align-items: stretch;
    }

    .left-side {
        width: 260px;
        min-width: 260px;
        max-width: 260px;
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .left-side > * {
        flex: 1;
    }

    .left-side :deep(.side-panel-root) {
        flex: 1;
    }

    .right-side {
        width: 100%;
        flex: 1;
        min-height: 0;
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
    }

    .container {
        padding: 20px 40px;
        display: flex;
        flex-direction: column;
        gap: 10px;
        width: 100%;
    }

    .holiday-container {
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

    .card-title, .card-title2 {
        font-size: 15px;
        font-weight: 500;
        color: #F0F0F0;
        padding-bottom: 10px;
    }

    .card-title2 {
        padding-bottom: 20px;
        margin-left: -15px;
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
        border-right: 1px solid #75889A80;
        padding: 10px 20px;
        flex: 1;
    }

    .divider-right {
        padding: 10px 20px;
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
        gap: 10px;
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

    .add-btn, .assign-btn {
        background-color: #003867;
        padding: 8px 25px;
        border: none;
        outline: none;
        font-size: 14px;
    }

    .btn-row {
        display: flex;
        justify-content: flex-end;
    }

    .row2 {
        display: flex;
        justify-content: space-between;
    }

    .avatar-group {
        display: flex;
        margin-left: 15px;
    }

    .avatar {
        width: 30px;
        height: 30px;
        border-radius: 50%;
        box-shadow: 4px 0 6px rgba(0, 0, 0, 0.15); 
        margin-left: -8px; 
    }

    .avatar:first-child {
        margin-left: 0;
    }

    .holiday-table-wrapper {
        max-height: 100vh;
        min-height: 65vh;
        overflow-y: auto;
    }

    .holiday-table-wrapper table {
        width: 100%;
        border-collapse: collapse;
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
        font-size: 13px;
        font-weight: 400;
    }

    thead {
        background-color: #75889A4D;
    }
    
    td {
        padding: 10px 20px;
        font-size: 12px;
    }

    .title-group, .card-title2 {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

    .btn-group {
        display: flex;
        flex-direction: row;
        gap: 10px;
    }

    .icon, .create-icon {
        width: 20px;
        height: 20px;
        cursor: pointer;
    }

    .icon:hover, .create-btn:hover, .add-btn:hover, .assign-btn:hover {
        opacity: 0.8;
    }

    .year-bubble {
        font-size: 11px;
        color: #F0F0F0;
        border-radius: 10px;
        background-color:#003867;
        padding: 5px 15px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
        margin-left: 10px;
    }

    .search-row {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
    }

    .search-box {
        width: 30%;
    }
    
    .create-btn {
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 14px;
        gap: 10px;
        background: none;
        outline: none;
        border: none;
        cursor: pointer;
    }

    .leave-badge {
        padding: 5px 15px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 500;
    }

    .leave-sick {
        background-color: #EB322380;
        color: #F0F0F0;
    }

    .leave-vacation {
        background-color: #083A73;
        color: #F0F0F0;
    }

    .leave-emergency {
        background-color: #FF7B0080;
        color: #F0F0F0;
    }

    .leave-unpaid {
        background-color: #7E7E7E80;
        color: #F0F0F0;
    }

    .status-badge {
        padding: 5px 10px;
        border-radius: 999px; 
        font-size: 12px;
        font-weight: 500;
    }

    .status-approved {
        color: #00DC28;
        background-color: #00DC2880;
    }

    .status-pending {
        color: #F0F0F0;
        background-color: #7E7E7E99;
    }

    .status-rejected {
        color: #F3857C;
        background-color: #EB322380;
    }

    .table-header input {
        padding: 10px 15px;
        border-radius: 10px;
        border: 2px solid rgba(255,255,255,0.1);
        background-color: rgba(0, 19, 36, 0.2);
        color: white;
        outline: none;
    }

    .total-highlight {
        color: #88b6ff;
        font-weight: 600;
        font-size: 12px !important;
    }

    .row span:last-child {
        margin-left: auto;
        text-align: right;
    }

    .schedule-item {
        padding: 12px 20px;
        cursor: pointer;
        transition: all 0.2s ease-in-out;
        border-left: 4px solid transparent;
        color: rgba(255, 255, 255, 0.7);
    }

    .left-side .row-item.active {
        background-color: rgba(255, 255, 255, 0.04);
        border-left: 4px solid #88b6ffc8;
        color: #ffffff;
        opacity: 1;
    }

    .schedule-item:hover:not(.active) {
        background: rgba(255, 255, 255, 0.01);
        color: #ffffff;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>