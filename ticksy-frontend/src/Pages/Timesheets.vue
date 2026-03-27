<template>
    <div class="timesheet-bg"></div>
    <div :class="['app', { collapsed: !isOpen }]">
        <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

        <div class="main-content"> <!--DO NOT change class!-->
            <h2>Timesheets</h2>

            <div class="card">
            <div class="top-bar">
                <div class="left">
                    <div class="dropdown">
                        <div class="dropdown-trigger" @click="toggleDropdown">
                            <span class="title">
                            {{
                                selectedTimesheet === 'weekly'
                                ? 'Weekly Timesheets'
                                : selectedTimesheet === 'daily'
                                ? 'Daily Timesheets'
                                : 'Monthly Timesheets'
                            }}
                            </span>

                            <div class="icon-btn">
                                <ChevronDown />
                            </div>
                        </div>

                        <div v-if="showDropdown" class="dropdown-menu">
                            <div @click="selectTimesheet('weekly')">Weekly Timesheets</div>
                            <div @click="selectTimesheet('daily')">Daily Timesheets</div>
                            <div @click="selectTimesheet('monthly')">Monthly Timesheets</div>
                        </div>
                    </div>

                    <div class="icon-btn" @click="prevWeek">
                        <ChevronLeft />
                    </div>

                    <span v-if="selectedTimesheet === 'weekly'">
                        {{ formattedRange }}
                    </span>

                    <span v-else-if="selectedTimesheet === 'daily'">
                        {{ startDate.toDateString() }}
                    </span>

                    <span v-else>
                        {{ startDate.toLocaleString('default', { month: 'long', year: 'numeric' }) }}
                    </span>

                    <div class="icon-btn" @click="nextWeek">
                        <ChevronRight />
                    </div>

                    <div class="icon-btn" ref="calendarBtn" @click="toggleDatePicker">
                        <Calendar/>
                    </div>

                    <div v-if="showDatePicker" class="date-picker-wrapper" :style="pickerStyle">
                        <!-- WEEKLY -->
                        <VueDatePicker
                            v-if="selectedTimesheet === 'weekly' && showDatePicker"
                            v-model="dateRange"
                            range
                            inline
                            :enable-time-picker="false"
                            @update:model-value="onRangeSelect"
                            dark
                        />

                        <!-- DAILY -->
                        <VueDatePicker
                            v-else-if="selectedTimesheet === 'daily'"
                            v-model="startDate"
                            inline
                            auto-apply
                            :enable-time-picker="false"
                            @update:model-value="onDaySelect"
                            dark
                        />

                        <!-- MONTHLY -->
                        <VueDatePicker
                            v-else
                            v-model="selectedMonth"
                            inline
                            month-picker
                            auto-apply
                            @update:model-value="onMonthSelect"
                            dark
                        />
                    </div>

                </div>
                <button class="export-btn">
                    <Download class="export-icon" />
                    <span>Export</span>                       
                </button>
            </div>
            <div class="header-line"></div>

            <!-- ================= WEEKLY ================= -->
            <div v-if="selectedTimesheet === 'weekly'" class="table">

                <div class="table-header">
                    <SearchBar v-model="search" />
                    <div class="days">
                        <span>M</span>
                        <span>T</span>
                        <span>W</span>
                        <span>TH</span>
                        <span>F</span>
                        <span>S</span>
                        <span>S</span>
                        <span>Total Hours</span>
                    </div>
                </div>

                <div class="row" v-for="user in filteredUsers" :key="user.name">
                    <div class="user">
                        <img :src="user.avatar" class="avatar" />
                        <span>{{ user.name }}</span>
                    </div>

                    <div class="days">
                        <span v-for="(day, index) in user.days" :key="index">
                            <span v-if="day === 'rest'" class="rest">Rest day</span>
                            <span v-else>{{ day }}</span>
                        </span>
                        <span>{{ user.total }}</span>
                    </div>
                </div>
            </div>

            <!-- ================= DAILY ================= -->
            <div v-if="selectedTimesheet === 'daily'" class="table">

                <div class="table-header">
                    <SearchBar v-model="search" />
                    <div class="days daily">
                        <span>First In</span>
                        <span>Last Out</span>
                        <span>Total Hours</span>
                    </div>
                </div>

                <div class="row" v-for="user in filteredUsers" :key="user.name">
                    <div class="user">
                        <img :src="user.avatar" class="avatar" />
                        <span>{{ user.name }}</span>
                    </div>

                    <div class="days daily">
                        <span>8:00 AM</span>
                        <span>5:00 PM</span>
                        <span>8 hrs</span>
                    </div>
                </div>
            </div>

            <!-- ================= MONTHLY ================= -->
            <div v-if="selectedTimesheet === 'monthly'" class="table">

                <div class="table-header">
                    <SearchBar v-model="search" />
                    <div class="days monthly">
                        <span v-for="day in 31" :key="day">{{ day }}</span>
                        <span>Total</span>
                    </div>
                </div>

                <div class="row" v-for="user in filteredUsers" :key="user.name">
                    <div class="user">
                        <img :src="user.avatar" class="avatar" />
                        <span>{{ user.name }}</span>
                    </div>

                    <div class="days monthly">
                        <span v-for="day in 31" :key="day" class="box"></span>
                        <span>40 hrs</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </div>
</template>

<script setup>
    import { ref, computed, nextTick } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import SearchBar from '../components/Search.vue'
    import { ChevronDown, ChevronLeft, ChevronRight, Calendar, Download, TreePalm} from "lucide-vue-next"
    import { watch } from 'vue'
    import { users } from '../services/summaryData.js'
    import { useSearch } from '../services/search.js'
    import { VueDatePicker } from '@vuepic/vue-datepicker'
    import '@vuepic/vue-datepicker/dist/main.css'
    import dayjs from 'dayjs'

    const calendarBtn = ref(null)
    const pickerStyle = ref({})

    const isOpen = ref(true)
    const { search, filtered: filteredUsers } = useSearch(users, ['name'])
    const selectedMonth = ref(new Date())
    
    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const showDropdown = ref(false)
    const selectedTimesheet = ref('weekly')

    function toggleDropdown() {
        showDropdown.value = !showDropdown.value
    }

    const showDatePicker = ref(false)
    const dateRange = ref([
        dayjs().toDate(),
        dayjs().add(6, 'day').toDate()
    ])
    const startDate = ref(dateRange.value[0])
    const endDate = ref(dateRange.value[1])

    function selectTimesheet(type) {
        selectedTimesheet.value = type
        showDropdown.value = false
    }

    const formattedRange = computed(() => {
        const options = { month: 'short', day: 'numeric' }

        const start = new Date(startDate.value).toLocaleDateString('en-US', options)
        const end = new Date(endDate.value).toLocaleDateString('en-US', options)

        return `${start} - ${end}`
    })

    function nextWeek() {
        if (selectedTimesheet.value === 'weekly') {
            dateRange.value = [
                dayjs(startDate.value).add(7, 'day').toDate(),
                dayjs(endDate.value).add(7, 'day').toDate()
            ]
        } 
        else if (selectedTimesheet.value === 'daily') {
            startDate.value = dayjs(startDate.value).add(1, 'day').toDate()
            endDate.value = startDate.value
        } 
        else {
            dateRange.value = [
                dayjs(startDate.value).add(1, 'month').startOf('month').toDate(),
                dayjs(startDate.value).add(1, 'month').endOf('month').toDate()
            ]
        }
    }

    function prevWeek() {
        if (selectedTimesheet.value === 'weekly') {
            dateRange.value = [
                dayjs(startDate.value).subtract(7, 'day').toDate(),
                dayjs(endDate.value).subtract(7, 'day').toDate()
            ]
        } 
        else if (selectedTimesheet.value === 'daily') {
            startDate.value = dayjs(startDate.value).subtract(1, 'day').toDate()
            endDate.value = startDate.value
        } 
        else {
            dateRange.value = [
                dayjs(startDate.value).subtract(1, 'month').startOf('month').toDate(),
                dayjs(startDate.value).subtract(1, 'month').endOf('month').toDate()
            ]
        }
    }

    function onRangeSelect(val) {
        if (val && val.length === 2) {
            dateRange.value = val
            startDate.value = val[0]
            endDate.value = val[1]
            showDatePicker.value = false
        }
    }

    function onDaySelect(val) {
        startDate.value = val
        endDate.value = val

        dateRange.value = [val, val]

        showDatePicker.value = false
    }

    function onMonthSelect(val) {
        let date

        if (val instanceof Date) {
            date = val
        } else {
            date = new Date(val.year, val.month, 1)
        }

        selectedMonth.value = date

        const start = dayjs(date).startOf('month').toDate()
        const end = dayjs(date).endOf('month').toDate()

        dateRange.value = [start, end]
        startDate.value = start
        endDate.value = end

        showDatePicker.value = false
    }

    function toggleDatePicker() {
        showDatePicker.value = !showDatePicker.value

        if (showDatePicker.value) {
            nextTick(() => {
                const rect = calendarBtn.value.getBoundingClientRect()

                pickerStyle.value = {
                    position: 'absolute',
                    top: `${rect.bottom + 5}px`,
                    left: `${rect.left}px`,
                    zIndex: 999
                }
            })
        }
    }

    watch(dateRange, (val) => {
        if (val && val.length === 2) {
            startDate.value = val[0]
            endDate.value = val[1]
        }
    })

</script>

<style scoped>
    .app {
        width: 100%;
        flex: 1;
        min-height: 100vh;
        overflow: hidden;
    }

    .main-content {
        margin-left: var(--sidebar-width);
        flex: 1;
        padding: 1rem;
        transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
        color: white; 
       
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
        margin-left: var(--sidebar-collapsed-width); /*DO NOT CHANGE!*/
    }

    :global(html, body, #app) {
        width: 100% !important;
        height: 100%;
        margin: 0;
        padding: 0;
        overflow: hidden;
    }

    :global(*) {
        box-sizing: border-box;
    }

    .timesheet-page {
        position: relative;
        width: 100%;
        min-height: 100vh;
    }

    .timesheet-bg {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-image: url("../assets/main_bg.jpg");
        background-size: cover;
        background-position: center;
        background-repeat: no-repeat;
        z-index: -1;
    }

    .card {
        border-radius: 5px;
        width: 100%;
        min-height: 85vh; 
        padding: 40px 20px; 
        display: flex;
        flex-direction: column;
        box-sizing: border-box;
        gap: 5px;
        margin-bottom: 30px;
    }

    .top-bar {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .left {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .title {
        font-weight: 600;
        font-size: 18px;
    }

    .icon-btn {
        width: 28px;
        height: 28px;
        background: rgba(255,255,255,0.08);
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
    }

    .icon-btn svg {
        width: 14px;
        color: white;
    }

    .header-line {
        width: 100%;
        height: 1px;
        background: rgba(255,255,255,0.15);
        margin: 15px 0 20px;
    }

    .export-btn {
        background: #052348;
        border: none;
        border-radius: 10px;
        padding: 8px 25px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
        outline: none;
        box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
    }

    .export-btn:hover {
        opacity: 0.8;    }

    .h2 {
        font-size: 30px;
        margin-bottom: 20px;
    }

    .table {
        backdrop-filter: blur(6px);
        border-radius: 5px;
        padding: 20px;
        
    }

    .table-header {
        display: grid;
        grid-template-columns: 250px 1fr;
        margin-bottom: 15px;
    }

    .table-header input {
        padding: 10px 15px;
        border-radius: 10px;
        border: 2px solid rgba(255,255,255,0.1);
        background-color: rgba(0, 19, 36, 0.2);
        color: white;
        outline: none;
    }

    .days {
        display: grid;
        grid-template-columns: repeat(8, 1fr);
        text-align: center;
        gap: 5px;
    }

    .row {
        display: grid;
        grid-template-columns: 250px 1fr;
        align-items: center;
        padding: 12px 0;
        border-top: 1px solid rgba(255,255,255,0.1);
    }

    .user {
        display: flex;
        align-items: center;
        gap: 15px;
    }

    .rest {
        background: #16a34a;
        padding: 4px 10px;
        border-radius: 999px;
        font-size: 12px;
    }

    .dropdown {
        position: relative;
    }

    .dropdown-trigger {
        display: flex;
        align-items: center;
        gap: 8px;
        cursor: pointer;
    }

    .dropdown-menu {
        position: absolute;
        top: 40px;
        left: 0;
        background: rgba(0, 19, 36, 0.95);
        border-radius: 10px;
        min-width: 200px;
        overflow: hidden;
        z-index: 10;
    }

    .dropdown-menu div {
        padding: 10px;
        cursor: pointer;
    }

    .dropdown-menu div:hover {
        background: rgba(255,255,255,0.1);
    }

    .days.daily {
        grid-template-columns: repeat(3, 1fr);
    }

    .days.monthly {
        grid-template-columns: repeat(32, 1fr);
    }

    .box {
        width: 20px;
        height: 20px;
        background: #ccc;
        border-radius: 2px;
        margin: auto;
    }

    .export-btn:hover {
        opacity: 0.8;
    }

    .export-icon {
        width: 16px;
        height: 16px;
    }

    .avatar {
        width: 35px;
        height: 35px;
        border-radius: 50%;
    }

    span {
        font-size: 14px;
    }

    :global(.dp__theme_dark) {
        --dp-background-color: #052348;   
        --dp-text-color: #ffffff;          
        --dp-primary-color: #012033;     
        --dp-primary-text-color: #ffffff;
        --dp-hover-color: #083A73;     
        --dp-border-color: rgba(255,255,255,0.1);
        --dp-menu-border-color: rgba(255,255,255,0.1);
        --dp-border-radius: 10px;
    }

    .table {
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