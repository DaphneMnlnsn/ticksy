<template>

    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header 
                    title="Work Schedules"
                    :showTimer="false"
                />

                <transition name="fade">
                    <div v-if="showToast" class="success-toast">
                        <CheckCircle :size="18" />
                        <span>Saved successfully!</span>
                    </div>
                </transition>

                <transition name="fade">
                    <div v-if="showDeleteToast" class="delete-toast">
                        <CheckCircle :size="18" />
                        <span>Deleted successfully!</span>
                    </div>
                </transition>

                <transition name="fade">
                    <div v-if="showApproveToast" class="success-toast">
                        <CheckCircle :size="18" />
                        <span>Approved successfully!</span>
                    </div>
                </transition>

                <transition name="fade">
                    <div v-if="showRejectToast" class="delete-toast">
                        <CheckCircle :size="18" />
                        <span>Rejected successfully!</span>
                    </div>
                </transition>

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
                                                    <SquarePen class="icon" @click="handleEditSchedule" style="cursor: pointer;"/>
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
                                                    <div v-for="(info, name) in groupedBreaks" :key="name" class="break-group">
                                                        <div class="break-days">
                                                            {{ info.days.join(', ') }}
                                                        </div>
                                                        <div class="row">
                                                            <span class="break-name">{{ name }}</span>
                                                            <span>{{ formatDuration(info.duration) }} hr/s</span>
                                                        </div>
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

                                    <button class="policy-btn" @click="handleShowPolicies">
                                        <span class="bubble-policies">Show Time Off Policies/Types</span>
                                    </button>

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
                                                <th><div class="table-header">
                                                    <template v-if="selectedRequests.length > 0">
                                                        <div class="dropdown-item-multiple approve" @click="bulkApprove" title="Approve Selected">
                                                            <CircleCheck size="18" />
                                                        </div>

                                                        <div class="dropdown-item-multiple reject" @click="bulkReject" title="Reject Selected">
                                                            <CircleX size="18" />
                                                        </div>
                                                    </template>
                                                </div></th>
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
                                                <td>
                                                    <div class="actions" @click.stop>
                                                        <span class="dots" @click="toggleMenu(request.id)">•••</span>

                                                        <div v-if="activeMenu === request.id" class="dropdown">
                                                            <div class="dropdown-item approve" @click="handleApproveRequest(request.id)">
                                                                <CircleCheck size="16" />
                                                                Approve
                                                            </div>

                                                            <div class="dropdown-item reject" @click="handleRejectRequest(request.id)">
                                                                <CircleX size="16" />
                                                                Reject
                                                            </div>  
                                                        </div>
                                                    </div>
                                                </td>
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
                                            <template v-if="!isEditing">
                                                <span>{{ activeCalendarName }}</span>
                                            </template>
                                            <template v-else>
                                                <input v-model="activeCalendarName" type="text" class="edit-input" />
                                                <div class="edit-actions">
                                                    <div class="default-toggle" 
                                                        @click="activeCalendarIsDefault = !activeCalendarIsDefault"
                                                        :title="activeCalendarIsDefault ? 'System Default' : 'Set as Default'">
                                                        <Star :class="['icon', { 'star-active': activeCalendarIsDefault }]" 
                                                            :fill="activeCalendarIsDefault ? 'currentColor' : 'transparent'" />
                                                    </div>
                                                    <Check class="icon save-icon" @click="handleSaveCalendar" />
                                                    <Trash2 class="icon delete-icon" @click="handleDeleteCalendar" />
                                                </div>
                                            </template>
                                            
                                            <span class="year-bubble">{{ new Date().getFullYear().toString() }}</span>
                                        </div>
                                        
                                        <div class="btn-group">
                                            <SquarePen 
                                                class="icon" 
                                                :class="{ 'active-edit': isEditing }" 
                                                @click="toggleEdit" 
                                            />
                                            <SquarePlus class="icon" @click="handleAddHoliday" />
                                        </div>
                                    </div>

                                    <div class="holiday-table-wrapper">
                                        <table class="holiday-table">
                                            <colgroup>
                                                <col :style="{ width: isEditing ? '40%' : '55%' }" />
                                                <col style="width: 20%;" />
                                                <col style="width: 20%;" />
                                                <col v-if="isEditing" style="width: 15%;" />
                                            </colgroup>
                                            <thead>
                                                <tr>
                                                    <th><div class="table-header">Name</div></th>
                                                    <th><div class="table-header">Date</div></th>
                                                    <th><div class="table-header">Day</div></th>
                                                    <th v-if="isEditing"><div class="table-header">Actions</div></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(holiday, index) in selectedCalendar" :key="index">
                                                    <td>{{ holiday.name }}</td>
                                                    <td>{{ formatDate(holiday.date) }}</td>
                                                    <td>{{ holiday.day }}</td>
                                                    <td v-if="isEditing">
                                                        <div class="action-cell">
                                                            <Pencil class="small-icon" @click="handleEditHoliday(holiday)" />
                                                            <Trash2 class="small-icon delete-text" @click="handleDeleteHoliday(holiday.id)" />
                                                        </div>
                                                    </td>
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
    <EditSchedulePanel
        :isOpen="isEditScheduleOpen"
        :isSidebarCollapsed="!isOpen"
        :scheduleEdit="selectedSchedule"
        @setup-finished="loadSchedules"
        @close="isEditScheduleOpen = false"
    />
    <RequestTimeOffPanel
        :isOpen="isRequestOpen"
        :isSidebarCollapsed="!isOpen"
        @setup-finished="loadTimeOffRequests"
        @close="isRequestOpen = false"
    />
    <AddBreak
        :isOpen="isAddBreakOpen"
        :isSidebarCollapsed="!isOpen"
        :scheduleId=activeScheduleId
        @close="isAddBreakOpen = false"
    />
    <AddCalendarPanel
        :isOpen="isCalendarOpen"
        :isSidebarCollapsed="!isOpen"
        @close="isCalendarOpen = false"
    />
    <AddHolidayPanel
        :isOpen="isHolidayOpen"
        :isSidebarCollapsed="!isOpen"
        :calendarId="activeCalendarId"
        @close="isHolidayOpen = false"
    />
    <EditHolidayPanel
        :isOpen="isEditHolidayOpen"
        :isSidebarCollapsed="!isOpen"
        :holiday="holiday"
        :calendarId="activeCalendarId"
        @close="isEditHolidayOpen = false"
    />
    <AssignMembers
        v-model="isAssignOpen"
        :users="users"
        :scheduleId="activeScheduleId"
        @save="selectSchedule(activeScheduleId)"
    />
    <PolicyListPanel
        :isOpen="isPoliciesOpen"
        :isSidebarCollapsed="!isOpen"
        @close="isPoliciesOpen = false"
    />

</template>   

<script setup lang="ts">
    import Header from '../components/Header.vue'
    import { ref, onMounted, onUnmounted } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import SidePanel from '../components/SidePanel.vue'
    import SchedulePanel from '../components/SchedulePanel.vue'
    import Search from '../components/Search.vue'
    import RowItem from '../components/RowItem.vue'
    import { calendars } from '../services/summaryData'
    import sampleIMG from '../assets/sample_img.jpg'
    import { SquarePen, SquarePlus, FilePlus, ChevronsRight, DeleteIcon, Trash2, Pencil, Check, CheckCircle, Star, CircleCheck, CircleX } from 'lucide-vue-next'
    import { computed } from 'vue';
    import { useSearch } from '../services/search'
    import { getScheduleById, getSchedules } from '../services/schedule'
    import { approveRequest, createRequest, getRequests, rejectRequest } from '../services/timeOff'
    import { deleteCalendar, getCalendars, updateCalendar } from '../services/calendars'
    import { deleteHoliday, getHolidays } from '../services/holidays'
    import { formatDate, formatDuration, formatFullDateTime, formatRange, formatTime } from '../services/formatting'
    import RequestTimeOffPanel from '../components/RequestTimeOffPanel.vue'
    import AddCalendarPanel from '../components/AddCalendarPanel.vue'
    import AddBreak from '../components/AddBreak.vue'
    import AddHolidayPanel from '../components/AddHolidayPanel.vue'
    import EditHolidayPanel from '../components/EditHolidayPanel.vue'
    import EditSchedulePanel from '../components/EditSchedulePanel.vue'
    import AssignMembers from '../components/AssignMembers.vue'
    import { getUsers } from '../services/assignMembers'
    import PolicyListPanel from '../components/PolicyListPanel.vue'

    const activeTab = ref ('Schedules')
    const showToast = ref(false)
    const showDeleteToast = ref(false)
    const showApproveToast = ref(false)
    const showRejectToast = ref(false)
    const isScheduleOpen = ref(false)
    const isEditScheduleOpen = ref(false)
    const isRequestOpen = ref(false)
    const isCalendarOpen = ref(false)
    const isHolidayOpen = ref(false)
    const isOpen = ref(true)
    const isAddBreakOpen = ref(false)
    const isAssignOpen = ref (false)
    const isLoadingUsers = ref(false)
    const isPoliciesOpen = ref(false)

    const selectedRequests = ref([])
    const selectAllRequests = ref(false)
    const users = ref([])

    async function handleAssignMember() {
        try {
            if (!selectedSchedule.value) {
                console.error("No selected schedule")
                return
            }

            isLoadingUsers.value = true

            users.value = await getUsers()

            console.log("USERS:", users.value)

            isAssignOpen.value = true

        } catch (err) {
            console.error("Error fetching users:", err)
        } finally {
            isLoadingUsers.value = false
        }
    }

    function handleShowPolicies() {
        isPoliciesOpen.value = true
    }

    function handleEditSchedule() {
        if (!selectedSchedule.value) return
        isEditScheduleOpen.value = true
    }

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
    function handleAddCalendar() {
        isCalendarOpen.value = true;
    }
    function handleAddHoliday() {
        isHolidayOpen.value = true;
    }

    const schedules = ref<{ id: number, name: string, label?: string, icon: any }[]>([]);
    const selectedSchedule = ref<any>(null);
    const isLoadingDetails = ref(false);

    async function loadSchedules() {
        try {
            const data = await getSchedules();
            schedules.value = data;

            const currentId = activeScheduleId.value || selectedSchedule.value?.id;

            const stillExists = data.find(s => s.id === currentId);

            if (stillExists) {
                await selectSchedule(stillExists.id);

            } else if (data.length > 0) {
                await selectSchedule(data[0].id);
            }
        } catch (error) {
            console.error("Failed to load schedules:", error);
        }
    }

    const groupedBreaks = computed(() => {
        if (!selectedSchedule.value?.days) return {}; 
        
        const summary = {};
        
        selectedSchedule.value.days.forEach(day => {
            if (day.breaks) {
                day.breaks.forEach(b => {
                    if (!summary[b.breakName]) {
                        summary[b.breakName] = { 
                            duration: b.breakDuration, 
                            days: [] 
                        };
                    }
                    summary[b.breakName].days.push(day.day.substring(0, 3));
                });
            }
        });
        
        return summary;
    });

    const activeScheduleId = ref<number | null>(null);

    async function selectSchedule(id: number) {
        isLoadingDetails.value = true;
        activeScheduleId.value = id;
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

    const activeMenu = ref(null)

    function toggleMenu(email) {
        activeMenu.value = activeMenu.value === email ? null : email
    }
    
    function handleClickOutside() {
        activeMenu.value = null
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })

    async function bulkApprove() {
        if (selectedRequests.value.length === 0) return;

        if (!confirm(`Are you sure you want to approve ${selectedRequests.value.length} items?`)) return;

        try {
            await Promise.all(selectedRequests.value.map(id => approveRequest(id)));

            showApproveToast.value = true;
            setTimeout(() => showApproveToast.value = false, 2000);

            selectedRequests.value = [];
            selectAllRequests.value = false;
            
            await getRequests(); 

        } catch (err) {
            console.error("Bulk Approve Error:", err);
            alert("Some requests failed to approve.");
        }
    }

    async function bulkReject() {
        if (selectedRequests.value.length === 0) return;

        if (!confirm(`Reject ${selectedRequests.value.length} items?`)) return;

        try {
            await Promise.all(selectedRequests.value.map(id => rejectRequest(id)));

            showRejectToast.value = true;
            setTimeout(() => showRejectToast.value = false, 2000);

            selectedRequests.value = [];
            selectAllRequests.value = false;

            await getRequests();
        } catch (err) {
            console.error("Bulk Reject Error:", err);
        }
    }

    async function handleApproveRequest(id) {
        try {
            await approveRequest(id);

            showApproveToast.value = true;
            
            setTimeout(() => {
                showApproveToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Approve Error:", err);
            const errorMsg = err.response?.data || "Failed to approve request";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    }

    async function handleRejectRequest(id) {
        try {
            await rejectRequest(id);

            showRejectToast.value = true;
            
            setTimeout(() => {
                showRejectToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Approve Error:", err);
            const errorMsg = err.response?.data || "Failed to reject request";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    }

    const calendars = ref([]);
    const selectedCalendar = ref<any>([]);
    const activeCalendarName = ref("");
    const activeCalendarId = ref<number | null>(null);
    const activeCalendarIsDefault = ref(false);

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
            activeCalendarId.value = item.id;
            activeCalendarIsDefault.value = item.isDefault;
            
            try {
                selectedCalendar.value = await getHolidays(item.id, new Date().getFullYear().toString());
            } catch (error) {
                console.error("Error fetching calendar details:", error);
            }
        } catch (error) {
            console.error("Error fetching calendar details:", error);
        }
    }

    const isEditing = ref(false)

    const toggleEdit = () => {
        isEditing.value = !isEditing.value;
        if (isEditing.value) {
            activeCalendarIsDefault.value = activeCalendar.value.isDefault;
        }
    };

    async function handleSaveCalendar() {
        if (!activeCalendarName.value.trim()) {
            errors.value.name = true;
            return;
        }

        try {
            const payload = {
                name: activeCalendarName.value,
                isDefault: activeCalendarIsDefault.value
            };

            await updateCalendar(activeCalendarId.value, payload);

            isEditing.value = false;
            showToast.value = true;
            
            setTimeout(() => {
                showToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Save Error:", err);
            const errorMsg = err.response?.data || "Failed to save calendar";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    }

    async function handleDeleteCalendar() {
        try {

            await deleteCalendar(activeCalendarId.value);

            showDeleteToast.value = true;
            
            setTimeout(() => {
                showDeleteToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Delete Error:", err);
            const errorMsg = err.response?.data || "Failed to delete calendar";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    };
    
    const holiday = ref<any>(null);
    const isEditHolidayOpen = ref(false);

    const handleEditHoliday = (selectedHoliday: any) => {
        holiday.value = selectedHoliday;
        isEditHolidayOpen.value = true;
    };

    async function handleDeleteHoliday(index) {
        try {

            await deleteHoliday(index);

            selectedCalendar.value = selectedCalendar.value.filter(h => h.id !== id);

            showDeleteToast.value = true;
            
            setTimeout(() => {
                showDeleteToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Delete Error:", err);
            const errorMsg = err.response?.data || "Failed to delete holiday";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    };

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
        const activeDays = selectedSchedule.value?.days?.filter(d => !d.isRestDay) || [];
        if (activeDays.length === 0) return 0;
        return (totalWeeklyHours.value / activeDays.length).toFixed(1);
    });

    const scheduledDaysCount = computed(() => {
        return selectedSchedule.value?.days?.filter(d => !d.isRestDay).length || 0;
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
        height: 85%;
        position: relative;
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

    .policy-btn {
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

    .year-bubble {
        font-size: 11px;
        color: #F0F0F0;
        border-radius: 10px;
        background-color:#003867;
        padding: 5px 15px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
        margin-left: 10px;
    }

    .bubble-policies {
        background: #083A73;
        color: white;
        padding: 6px 12px;
        border-radius: 999px;
        display: inline-block;
        margin-right: -240%;
    }

    .bubble-policies:hover {
        opacity: 1px;
        transform: translateY(-1px);
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

    .break-group {
        margin-bottom: 16px;
        display: flex;
        flex-direction: column;
    }

    .break-name {
        font-weight: 600;
        color: var(--text-primary);
    }

    .break-days {
        margin-top: -1%;
        font-size: 11px;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        color: #9ca3af;
        padding: 2px 0px;
        border-radius: 4px;
        width: fit-content;
    }

    .btn-row {
        margin-top: auto;
        padding-top: 175px;
        background: transparent; 
    }

    input[type="text"] {
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 10px 12px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    .delete-icon {
        color: #ef4444;
        margin-left: 8px;
        margin-bottom: -1.9%;
        cursor: pointer;
    }

    .active-edit {
        color: #3b82f6;
    }

    .action-cell {
        display: flex;
        gap: 10px;
        align-items: center;
    }

    .edit-input {
        border: none;
        background: transparent;
        outline: none;
        font-size: 1rem;
        font-weight: 600;
        width: 200px;
    }

    .default-toggle {
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        margin-right: 10px;
        transition: transform 0.2s ease;
    }
    
    .star-active {
        color: #eab308;
    }

    .edit-actions {
        display: inline-flex;
        align-items: center;
        gap: 1px;
        margin-left: 10px;
        vertical-align: middle;
    }

    .save-icon {
        color: #22c55e;
        cursor: pointer;
    }

    .delete-icon {
        color: #ef4444;
        cursor: pointer;
    }

    .small-icon {
        width: 16px;
        height: 16px;
        cursor: pointer;
        opacity: 0.7;
    }

    .small-icon:hover {
        opacity: 1;
    }

    .delete-text {
        color: #ef4444;
    }

    .success-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #06d6a0;
        color: #001324;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(6, 214, 160, 0.3);
        z-index: 9999;
    }

    .delete-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #d60606;
        color: #d8dee3;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(214, 6, 6, 0.3);
        z-index: 9999;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }

    .actions {
        position: relative;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .dots {
        cursor: pointer;
        font-size: 10px;
        opacity: 0.7;
    }

    .dots:hover {
        opacity: 1;
    }

    .dropdown {
        position: absolute;
        right: 0;
        top: 22px;
        width: 140px;
        background: #061a2b; 
        border-radius: 12px;
        padding: 6px;
        z-index: 100;
        overflow: hidden;
        box-shadow: 0 10px 25px rgba(0,0,0,0.3);
    }

    .dropdown-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 10px;
        cursor: pointer;
        font-size: 12px;
    }

    .dropdown-item:hover {
        background: rgba(255,255,255,0.08);
    }

    .dropdown-item-multiple {
        display: flex;
        align-items: center;
        gap: 2px;
        padding: 1px;
        cursor: pointer;
        font-size: 12px;
    }

    .dropdown-item-multiple:hover {
        background: none;
    }

    .approve {
        color: #22c55e;
    }

    .reject {
        color: #ff4d4d;
    }
    
    .dropdown {
        animation: fadeIn 0.12s ease;
    }

    .timeoff-table th:nth-child(3),
    .timeoff-table td:nth-child(3) {
        width: 1%;
        white-space: nowrap;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>