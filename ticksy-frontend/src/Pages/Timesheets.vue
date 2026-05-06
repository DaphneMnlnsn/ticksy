<template>
    <div class="timesheet-bg"></div>
    <div :class="['app', { collapsed: !isOpen }]">
        <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

        <div class="main-content"> <!--DO NOT change class!-->
            <Header title="Timesheets" />

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

                    <div v-show="showDatePicker" class="date-picker-wrapper" :style="pickerStyle">
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
                <div class="button-group">
                    <button v-if="selectedTimesheet === 'monthly'" 
                        class="export-btn" 
                        ref="legendBtn" 
                        @click="toggleLegend"
                    >
                        <Clock3 class="export-icon" />
                        <span>Legends</span>
                    </button>
                    <button class="export-btn" @click="handleExport">
                        <Download class="export-icon" />
                        <span>Export</span>                       
                    </button>
                </div>
            </div>
            <div class="header-line"></div>

            <!-- ================= WEEKLY ================= -->
            <WeeklyTimesheet
                v-if="selectedTimesheet === 'weekly'"
                :users="tableData"
                :loading="loading"
                :hasData="hasData"
                :defaultAvatar="defaultAvatar"
            />

            <!-- ================= DAILY ================= -->
            <DailyTimesheet
                v-if="selectedTimesheet === 'daily'"
                :users="tableData"
                :loading="loading"
                :hasData="hasData"
                :defaultAvatar="defaultAvatar"
            />

            <!-- ================= MONTHLY ================= -->
            <MonthlyTimesheet
                v-if="selectedTimesheet === 'monthly'"
                :users="tableData"
                :loading="loading"
                :hasData="hasData"
                :defaultAvatar="defaultAvatar"
                :selectedMonth="selectedMonth ? selectedMonth.getMonth() + 1 : 1"
                :selectedYear="selectedMonth ? selectedMonth.getFullYear() : 2026"
            />
        </div>
    </div>
    <LegendModal 
        :show="showLegend" 
        :style="legendStyle"
    />
    </div>
</template>

<script setup>
    import { ref, computed, nextTick, onMounted, watch } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import SearchBar from '../components/Search.vue'
    import Header from '../components/Header.vue'
    import { ChevronDown, ChevronLeft, ChevronRight, Calendar, Download, TreePalm,
        Clock3, 
    } from "lucide-vue-next"
    import { useSearch } from '../services/search.js'
    import { VueDatePicker } from '@vuepic/vue-datepicker'
    import '@vuepic/vue-datepicker/dist/main.css'
    import dayjs from 'dayjs'
    import defaultAvatar from '../assets/sample_img.jpg'
    import WeeklyTimesheet from '../components/timesheets/WeeklyTimesheet.vue'
    import DailyTimesheet from '../components/timesheets/DailyTimesheet.vue'
    import MonthlyTimesheet from '../components/timesheets/MonthlyTimesheet.vue'
    import { fetchAttendance } from "../services/attendanceManager"

    import LegendModal from '../components/LegendModal.vue'
    import { buildExportPayload } from '../services/exportBuilder'
    import { formatCSV } from '../services/exportFormatter'
    import { useExportService } from '../services/exportService.js'
    import { formatRange } from '../services/formatting.js'
    import Swal from 'sweetalert2'

    const calendarBtn = ref(null)
    const pickerStyle = ref({})
    const isOpen = ref(true)
    const selectedMonth = ref(new Date())

    const showDropdown = ref(false)
    const selectedTimesheet = ref('weekly')
    const showDatePicker = ref(false)
    const dateRange = ref([
        dayjs().toDate(),
        dayjs().add(6, 'day').toDate()
    ])
    const startDate = ref(dateRange.value[0])
    const endDate = ref(dateRange.value[1])
    const formattedRange = computed(() => {
        const options = { month: 'short', day: 'numeric' }

        const start = new Date(startDate.value).toLocaleDateString('en-US', options)
        const end = new Date(endDate.value).toLocaleDateString('en-US', options)

        return `${start} - ${end}`
    })

    const search = ref('')

    const legendBtn = ref(null)
    const showLegend = ref(false)
    const legendStyle = ref({})

    const { exportCSV, exportPDF } = useExportService()

    const tableData = ref([])
    const loading = ref(false)
    const hasData = ref(false)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    function toggleDropdown() {
        showDropdown.value = !showDropdown.value
    }

    function selectTimesheet(type) {
        selectedTimesheet.value = type
        showDropdown.value = false

        tableData.value = []
        hasData.value = false
        loading.value = true

        loadData()
    }

    function nextWeek() {
        if (selectedTimesheet.value === 'weekly') {
            const start = dayjs(startDate.value).add(7, 'day').toDate()
            const end = dayjs(endDate.value).add(7, 'day').toDate()

            dateRange.value = [start, end]
            startDate.value = start
            endDate.value = end
        } 
        else if (selectedTimesheet.value === 'daily') {
            startDate.value = dayjs(startDate.value).add(1, 'day').toDate()
            endDate.value = startDate.value
        } 
        else {
            const newDate = dayjs(selectedMonth.value).add(1, 'month').toDate()

            selectedMonth.value = newDate
            startDate.value = dayjs(newDate).startOf('month').toDate()
            endDate.value = dayjs(newDate).endOf('month').toDate()
        }
    }

    function prevWeek() {
        if (selectedTimesheet.value === 'weekly') {
            const start = dayjs(startDate.value).subtract(7, 'day').toDate()
            const end = dayjs(endDate.value).subtract(7, 'day').toDate()

            dateRange.value = [start, end]
            startDate.value = start
            endDate.value = end
        } 
        else if (selectedTimesheet.value === 'daily') {
            startDate.value = dayjs(startDate.value).subtract(1, 'day').toDate()
            endDate.value = startDate.value
        } 
        else {
            const newDate = dayjs(selectedMonth.value).subtract(1, 'month').toDate()

            selectedMonth.value = newDate
            startDate.value = dayjs(newDate).startOf('month').toDate()
            endDate.value = dayjs(newDate).endOf('month').toDate()
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
        if (!showDatePicker.value) {
            const rect = calendarBtn.value.getBoundingClientRect();
            pickerStyle.value = {
                position: 'absolute',
                top: `${rect.bottom + window.scrollY + 5}px`,
                left: `${rect.left + window.scrollX}px`,
                zIndex: 999
            };
        }
        showDatePicker.value = !showDatePicker.value;
    }

    function toggleLegend() {
        showLegend.value = !showLegend.value

        if (showLegend.value) {
            nextTick(() => {
                const rect = legendBtn.value.getBoundingClientRect()

                legendStyle.value = {
                    top: `${rect.bottom + 5}px`,
                    left: `${rect.left}px`
                }
            })
        }
    }

    const exportMap = {
        weekly: buildWeeklyPayload,
        daily: buildDailyPayload,
        monthly: buildMonthlyPayload
    }

    function getExportPayload() {
        return exportMap[selectedTimesheet.value]?.() || buildWeeklyPayload()
    }

    async function handleExport() {
        const result = await Swal.fire({
            title: 'Export Timesheet',
            text: 'Choose export format',
            icon: 'question',
            showCancelButton: true,
            showDenyButton: true,
            confirmButtonText: 'CSV',
            denyButtonText: 'PDF',
            confirmButtonColor: "#083A73",
            denyButtonColor: "#052348",
            cancelButtonText: 'Cancel',
        })

        if (result.isConfirmed) {
            exportCSVHandler()
        } 
        else if (result.isDenied) {
            exportPDFHandler()
        }
    }

    function exportCSVHandler() {
        const payload = getExportPayload()

        const formatted = formatCSV(payload)

        exportCSV(
            `timesheet-${selectedTimesheet.value}`,
            formatted
        )
    }

    function exportPDFHandler() {
        const payload = getExportPayload()

        const pdfPayload = payload

        exportPDF(
            `timesheet-${selectedTimesheet.value}`,
            pdfPayload,
            {
                landscape: selectedTimesheet.value === 'monthly'
            }
        )
    }

    function buildWeeklyPayload() {
        return buildExportPayload({
            title: 'WEEKLY TIMESHEET REPORT',
            subtitle: 'Employee Attendance Summary',
            dateRange: formatRange(
                `${startDate.value.toISOString()} - ${endDate.value.toISOString()}`
            ),

            headers: ['Name','Mon','Tue','Wed','Thu','Fri','Sat','Sun','Total Hours'],

            rows: tableData.value.map(user => ({
                Name: user.name,
                Mon: user.days[0],
                Tue: user.days[1],
                Wed: user.days[2],
                Thu: user.days[3],
                Fri: user.days[4],
                Sat: user.days[5],
                Sun: user.days[6],
                'Total Hours': user.total ?? 0
            }))
        })
    }

    function buildDailyPayload() {
        return buildExportPayload({
            title: 'DAILY TIMESHEET REPORT',
            subtitle: 'Employee Attendance Summary',
            dateRange: startDate.value.toDateString(),

            headers: [
                'Name',
                'First In',
                'Last Out',
                'Total Hours'
            ],

            rows: tableData.value.map(user => ({
                'Name': user.name,
                'First In': user.firstIn || '-',
                'Last Out': user.lastOut || '-',
                'Total Hours': user.total || 0
            }))
        })
    }

    function buildMonthlyPayload() {
        const monthDate = selectedMonth.value
        const year = monthDate.getFullYear()
        const month = monthDate.getMonth()

        const daysInMonth = new Date(year, month + 1, 0).getDate()

        function parseHours(val) {
            if (typeof val === 'number') return val
            if (typeof val === 'string') {
                const match = val.match(/(\d+)h\s*(\d+)m/)
                if (match) {
                    return parseInt(match[1]) + parseInt(match[2]) / 60
                }
                const hMatch = val.match(/(\d+)h/)
                if (hMatch) return parseInt(hMatch[1])
                const mMatch = val.match(/(\d+)m/)
                if (mMatch) return parseInt(mMatch[1]) / 60
            }
            return 0
        }

        const rows = tableData.value.map(user => {
            const days = user.days || []

            let present = 0
            let absent = 0
            let late = 0

            const weekTotals = [0, 0, 0, 0]

            for (let i = 0; i < daysInMonth; i++) {
                const val = days[i]

                if (val && val !== '0h' && val !== '0' && val !== 'REST' && val !== 'LEAVE' && val !== '—') {
                    present++
                    weekTotals[Math.floor(i / 7)] += parseHours(val)
                } else if (val === '0h' || val === '0' || !val || val === '—') {
                    absent++
                }

                if (val === 'late') late++
            }

            return {
                'Name': user.name,
                'Month': monthDate.toLocaleString('default', { month: 'long', year: 'numeric' }),
                'Total Hours': user.total || 0,
                'Days Present': present,
                'Days Absent': absent,
                'Late Count': user.lateCount ?? late,
                'Week 1 Hours': weekTotals[0],
                'Week 2 Hours': weekTotals[1],
                'Week 3 Hours': weekTotals[2],
                'Week 4 Hours': weekTotals[3],
                days: user.days
            }
        })

        return buildExportPayload({
            title: 'MONTHLY TIMESHEET REPORT',
            subtitle: 'Employee Attendance Summary',
            dateRange: monthDate.toLocaleString('default', {
                month: 'long',
                year: 'numeric'
            }),

            headers: [
                'Name',
                'Month',
                'Total Hours',
                'Days Present',
                'Days Absent',
                'Late Count',
                'Week 1 Hours',
                'Week 2 Hours',
                'Week 3 Hours',
                'Week 4 Hours'
            ],

            rows
        })
    }

    async function loadData() {
        loading.value = true

        const role = localStorage.getItem("role")
        const userId = localStorage.getItem("userId")

        const params = {
            start: startDate.value,
            end: endDate.value,
            date: startDate.value
        }

        const res = await fetchAttendance(
            selectedTimesheet.value,
            params,
            role,
            userId
        )

        tableData.value = res.data ?? []
        hasData.value = tableData.value.length > 0

        loading.value = false
    }

    onMounted(() => {
        loadData()
    })

    watch(
        [selectedTimesheet, startDate, endDate, selectedMonth],
        () => {
            loadData()
        }
    )

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
        gap: 10px;
        outline: none;
        box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
    }

    .export-btn:hover {
        opacity: 0.8;    }

    .h2 {
        font-size: 30px;
        margin-bottom: 20px;
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
        backdrop-filter: blur(6px);
        border-radius: 5px;
        animation: fadeIn 0.4s ease-in-out;
        display: flex;
        flex-direction: column;
        width: 100%;
    }

    .empty-state {
        text-align: center;
        padding: 40px;
        color: #F0F0F0AD;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 10px;
    }

    .empty-icon {
        width: 40px;
        height: 40px;
    }

    .button-group {
        display: flex;
        flex-direction: row;
        gap: 10px;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }

</style>