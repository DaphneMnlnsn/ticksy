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
                        {{ new Date(startDate).toDateString() }}
                    </span>

                    <span v-else>
                        {{ new Date(startDate).toLocaleString('default', { month: 'long', year: 'numeric' }) }}
                    </span>

                    <div class="icon-btn" @click="nextWeek">
                        <ChevronRight />
                    </div>

                    <div class="icon-btn" @click="toggleDatePicker">
                        <Calendar/>
                    </div>

                    <div v-if="showDatePicker" class="date-picker">
                        <input type="date" @change="setStartDate($event.target.value)" />
                        <input type="date" @change="setEndDate($event.target.value)" />
                    </div>

                </div>
                <button class="export-btn">
                <Download />
                Export
                </button>
            </div>
            <div class="header-line"></div>

                        <!-- ================= WEEKLY ================= -->
            <div v-if="selectedTimesheet === 'weekly'" class="table">

                <div class="table-header">
                    <input type="text" placeholder="Search..." v-model="search" />
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
                        <img :src="user.avatar" />
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
                    <input type="text" placeholder="Search..." v-model="search" />
                    <div class="days daily">
                        <span>First In</span>
                        <span>Last Out</span>
                        <span>Total Hours</span>
                    </div>
                </div>

                <div class="row" v-for="user in filteredUsers" :key="user.name">
                    <div class="user">
                        <img :src="user.avatar" />
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
                    <input type="text" placeholder="Search..." v-model="search" />
                    <div class="days monthly">
                        <span v-for="day in 31" :key="day">{{ day }}</span>
                        <span>Total</span>
                    </div>
                </div>

                <div class="row" v-for="user in filteredUsers" :key="user.name">
                    <div class="user">
                        <img :src="user.avatar" />
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
    
    import { ref, computed } from 'vue'
    import Sidebar from '../components/Sidebar.vue'
    import { ChevronDown, ChevronLeft, ChevronRight, Calendar, Download, TreePalm} from "lucide-vue-next"
    import { watch } from 'vue'

    const isOpen = ref(true)
    
    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const showDropdown = ref(false)
    const selectedTimesheet = ref('weekly')

    function toggleDropdown() {
        showDropdown.value = !showDropdown.value
    }

    const showDatePicker = ref(false)
    const startDate = ref(new Date())
    const endDate = ref(new Date())
    endDate.value.setDate(startDate.value.getDate() + 6)

    function selectTimesheet(type) {
        selectedTimesheet.value = type
        showDropdown.value = false
    }
    
    function toggleDatePicker() {
        showDatePicker.value = !showDatePicker.value
    }

    const formattedRange = computed(() => {
        const options = { month: 'short', day: 'numeric' }

        const start = new Date(startDate.value).toLocaleDateString('en-US', options)
        const end = new Date(endDate.value).toLocaleDateString('en-US', options)

        return `${start} - ${end}`
    })

    function nextWeek() {
    if (selectedTimesheet.value === 'weekly') {
        startDate.value.setDate(startDate.value.getDate() + 7)
        endDate.value.setDate(endDate.value.getDate() + 7)
    } 
    else if (selectedTimesheet.value === 'daily') {
        startDate.value.setDate(startDate.value.getDate() + 1)
    } 
    else if (selectedTimesheet.value === 'monthly') {
        startDate.value.setMonth(startDate.value.getMonth() + 1)
    }

    startDate.value = new Date(startDate.value)
    endDate.value = new Date(endDate.value)
}
    function prevWeek() {
        startDate.value.setDate(startDate.value.getDate() - 7)
        endDate.value.setDate(endDate.value.getDate() - 7)

        startDate.value = new Date(startDate.value)
        endDate.value = new Date(endDate.value)
    }

    function setStartDate(value) {
        startDate.value = new Date(value)
    }

    function setEndDate(value) {
        endDate.value = new Date(value)
    }
    
    const search = ref("")
        const users = ref([
        {
            name: "Kiana Martin",
            avatar: "https://i.pravatar.cc/40?img=1",
            days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
            total: "40 hrs"
        },
        {
            name: "Daphne Manalansan",
            avatar: "https://i.pravatar.cc/40?img=2",
            days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
            total: "40 hrs"
        },
        {
            name: "Lei Anysson Marquez",
            avatar: "https://i.pravatar.cc/40?img=3",
            days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
            total: "40 hrs"
        },
        {
            name: "Quiana Domingo",
            avatar: "https://i.pravatar.cc/40?img=4",
            days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
            total: "40 hrs"
        }
    ])

     const filteredUsers = computed(() => {
    return users.value.filter(user =>
        user.name.toLowerCase().includes(search.value.toLowerCase())
    )
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
        margin-left: var(--sidebar-width); /*DO NOT CHANGE!*/
        flex: 1;
        padding: 1rem;
        transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
        color: white; 
       
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
        background-color: rgba(0, 19, 36, 0.2);
        border-radius: 15px;
        padding: 20px;
        
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
        background: #052947;
        border: none;
        border-radius: 10px;
        padding: 8px 15px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
        outline: none;
        box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
    }
    .export-btn:hover {
        background: #083A73;
        transition: 0.2s ease;
    }
   .h2{
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
        padding: 15px 0;
        border-top: 1px solid rgba(255,255,255,0.1);
    }

    .user {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .user img {
        border-radius: 50%;
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

</style>