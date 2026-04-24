<template>
    <div class="dashboard">
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header 
                    title="Dashboard"
                    :showTimer="true"
                    lastOut="Last out an hour ago"
                />
                    <div class="tabs">
                        <button v-for="tab in ['Day','Week','Month']"
                            :key="tab"
                            :class="{ active: activeTab === tab }"
                            @click="activeTab = tab">
                            {{ tab }}
                        </button>
                    </div>
a
                <div class="cards" :key="activeTab">

                     <div class="top-grid" :class="{ 'admin-grid': isAdmin, 'user-grid': !isAdmin }" :key="activeTab">
                        <WelcomeCard :viewMode="activeTab" :welcomeImg="welcomeImg"/>

                        <div class="card absences" v-if="isAdmin">
                            <div class="absence-header">
                                <h3>Most Absences</h3>
                            </div>
                            
                            <div class="chart">
                                <div class="y-axis">
                                    <span v-for="val in yAxisValues" :key="val">
                                        {{ val }}
                                    </span>
                                </div>

                                <div class="bars-wrapper">
                                <div class="bars">
                                    <div v-for="item in absencesData" :key="item.month" class="bar-group">
                                       <div class="stack">
                                    <div class="bar-wrapper">
                                        <div 
                                            class="red bar"
                                            :style="{ height: (item.absent / axisMax) * 100 + '%' }"
                                        >
                                            <span class="tooltip">
                                                {{ item.absent }} absences
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                    </div>
                                </div>
                                    <div class="months">
                                        <span v-for="item in absencesData" :key="item.month">
                                            {{ item.name }}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <HolidayCard :holidays="holidays" />
                    </div>

                    <div class="bottom-grid" :key="activeTab">
                        <TrackedHours 
                            :viewMode="activeTab"
                            :data="trackedHours"
                        />

                        <div class="card who">
                        <div class="who-header">
                            <h3>Who's In/Out</h3>
                            <p class="count">
                                {{ peopleByStatus[activeStatusTab]?.length || 0 }} members
                            </p>
                        </div>
 
                            <div class="who-tabs">
                                <button 
                                    v-for="tab in ['In','Break','Out']"
                                    :key="tab"
                                    :class="{ active: activeStatusTab === tab }"
                                    @click="activeStatusTab = tab"
                                >
                                    {{ tab }}
                                </button>
                            </div>

                            <div class="who-list">
                                <div 
                                    class="user" 
                                    v-for="p in peopleByStatus[activeStatusTab]" 
                                    :key="p.name"
                                >
                                    <img
                                        :src="p.avatar"
                                        class="avatar-small"
                                        @error="e => e.target.src = sampleImg"
                                    />

                                    <div class="user-info">
                                        <p class="name" :title="p.name">{{ p.name }}</p>
                                        <small class="time">{{ p.label }} {{ p.time }}</small>
                                    </div>
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
    import Header from '../components/Header.vue';
    import { ref, onMounted, computed, watch } from 'vue'
    import welcomeImg from "/welcome-img.png";
    import WelcomeCard from '../components/WelcomeCard.vue';
    import HolidayCard from '../components/HolidayCard.vue';
    import TrackedHours from '../components/TrackedHours.vue';
    import Sidebar from '../components/Sidebar.vue';
    import sampleImg from '../assets/sample_img.jpg'
    import dashboardService from '../services/dashboard.js';
    import { formatFullDateTime } from '../services/formatting.js'

    const activeTab = ref('Week')
    const holidays = ref([])
    const absencesData = ref([])
    const trackedHours = ref(null)
    const activeStatusTab = ref('In')
    const peopleByStatus = ref({ In: [], Break: [], Out: [] })
    const isOpen = ref(true)

    const token = localStorage.getItem('token')

    const role = localStorage.getItem('role') || 'User'
    const isAdmin = role.toLowerCase() === 'admin'

    const maxAbsence = computed(() => {
    const values = absencesData.value.map(x => x.absent || 0)
        return Math.max(...values, 1) 
    })

    function getDateRange(tab) {
        const now = new Date();
        if (tab === 'Day') {
            const today = now.toISOString().split('T')[0];
            return { start: today, end: today };
        } else if (tab === 'Week') {
            const startOfWeek = new Date(now);
            startOfWeek.setDate(now.getDate() - now.getDay());
            const endOfWeek = new Date(startOfWeek);
            endOfWeek.setDate(startOfWeek.getDate() + 6);
            return {
                start: startOfWeek.toISOString().split('T')[0],
                end: endOfWeek.toISOString().split('T')[0]
            };
        } else if (tab === 'Month') {
            const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
            const endOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0);
            return {
                start: startOfMonth.toISOString().split('T')[0],
                end: endOfMonth.toISOString().split('T')[0]
            };
        }
        return { start: now.toISOString().split('T')[0], end: now.toISOString().split('T')[0] }; // fallback to today
    }

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const formatDateTime = (date, time) => {
        if (!date || !time) return '--'
        const iso = `${date}T${time}`
        return formatFullDateTime(iso)
    }
    
    const loadDashboard = async () => {
        try {
            const { start, end } = getDateRange(activeTab.value)
            const type = activeTab.value === 'Day' ? 'daily' : 
                         activeTab.value === 'Week' ? 'weekly' : 'monthly'

            let defaultCalendarId = 2 
            try {
                const calendarsRes = await dashboardService.getCalendars(token)
                const defaultCalendar = calendarsRes.data.find(cal => cal.isDefault === true)
                if (defaultCalendar) {
                    defaultCalendarId = defaultCalendar.id
                    console.log('Using default calendar ID:', defaultCalendarId)
                } else {
                    console.warn('No default calendar found, using fallback ID:', defaultCalendarId)
                }
            } catch (calErr) {
                console.error('Failed to fetch calendars, using fallback ID:', defaultCalendarId, calErr)
            }

            let absencesRes = null
            let liveRes = null
            let hoursRes = null
            let holidaysRes = null

            const requests = [
                dashboardService.getLiveStatus(token),
                dashboardService.getTrackedHours(type, start, end, token),
                dashboardService.getHolidays(defaultCalendarId, new Date().getFullYear(), token).catch(err => {
                    console.warn('Failed to fetch holidays:', err)
                    return { data: [] }
                })
            ]

            if (isAdmin) {
                requests.unshift(dashboardService.getMostAbsences(start, end, token))
            }

            const responses = await Promise.all(requests)

            if (isAdmin) {
                [absencesRes, liveRes, hoursRes, holidaysRes] = responses
            } else {
                [liveRes, hoursRes, holidaysRes] = responses
            }

        try {
            const resHolidays = holidaysRes.data
            const listHolidays = Array.isArray(resHolidays) ? resHolidays : (resHolidays.data || [])

            const today = new Date()

            const upcoming = listHolidays
                .map(h => {
                    const dateStr = h.date || h.Date
                    const date = new Date(dateStr)

                    return {
                        rawDate: date,
                        month: !isNaN(date)
                            ? date.toLocaleString('en-US', { month: 'short' }).toUpperCase()
                            : 'N/A',
                        day: !isNaN(date)
                            ? String(date.getDate()).padStart(2, '0')
                            : '--',
                        name: h.name || h.Name || 'Holiday'
                    }
                })
                .filter(h => h.rawDate >= today)
                .sort((a, b) => a.rawDate - b.rawDate) 

            holidays.value = upcoming.slice(0, 2)

            } catch (holidayErr) {
                console.error('Error processing holidays:', holidayErr)
                holidays.value = []
            }

            if (isAdmin && absencesRes && absencesRes.data) {
                absencesData.value = absencesRes.data
                .map(x => {
                    const fullName = x.fullName || 'User'

                    const parts = fullName.trim().split(' ')

                    const firstName = parts[0] || ''
                    const lastName = parts.length > 1 ? parts[parts.length - 1] : ''

                    const displayName = lastName
                        ? `${firstName.charAt(0).toUpperCase()}. ${lastName}`
                        : firstName

                    return {
                        name: displayName,
                        fullName,
                        absent: x.absenceCount || 0
                    }
                })
                .sort((a, b) => b.absent - a.absent)
                .slice(0, 5)
            
            }

            if (hoursRes && hoursRes.data) {
                if (Array.isArray(hoursRes.data)) {
                    trackedHours.value = {
                        workHours: hoursRes.data.reduce((sum, d) => sum + (d.workHours || 0), 0),
                        breakHours: hoursRes.data.reduce((sum, d) => sum + (d.breakHours || 0), 0),
                        overtimeHours: hoursRes.data.reduce((sum, d) => sum + (d.overtimeHours || 0), 0)
                    }
                } else {
                    trackedHours.value = hoursRes.data
                }
            }

            const grouped = { In: [], Break: [], Out: [] }
            if (liveRes && liveRes.data) {
                liveRes.data.forEach(u => {
                    const key = u.status === 'On Break' ? 'Break' : u.status === 'In' ? 'In' : 'Out'
                    const label = key === 'In' ? 'Last in' : key === 'Break' ? 'Last break' : 'Last out'

                    grouped[key].push({
                        name: u.fullName,
                        time: formatDateTime(u.lastActionDate, u.lastActionTime),
                        label,
                        avatar: u.avatarUrl || '/default.png'
                    })
                })
            }
            peopleByStatus.value = grouped

        } catch (err) {
            console.error('Dashboard error details:', err)
        }
    }

    onMounted(() => {
        loadDashboard()
    })

    watch(activeTab, () => {
        loadDashboard()
    })

    const yAxisValues = computed(() => {
    const max = maxAbsence.value

    const steps = 5
    const stepSize = Math.ceil(max / steps)

    const values = []
    for (let i = steps; i >= 0; i--) {
        values.push(i * stepSize)
    }

    return values
    })

    const axisMax = computed(() => {
        const max = maxAbsence.value
        const steps = 5
        const stepSize = Math.ceil(max / steps)
        return stepSize * steps
    })
    
</script>
 
<style scoped>
    .dashboard {
        height: 100vh;
        background: linear-gradient(135deg, #071a2f, #0d2c4a);
        color: #fff;
        overflow-y: auto;
    }

    .app { 
        display: flex;
        width: 100%;
        flex: 1;
        min-height: 100vh;
        overflow: hidden;
        align-items: stretch;
    }

    .app.collapsed .main-content {
        margin-left: var(--sidebar-collapsed-width);
    }

    .main-content {
        margin-left: var(--sidebar-width);
        flex: 1;
        padding: 1rem;
        height: 100vh;         
        overflow-y: auto;     
        min-height: 0;   
        transition: margin-left 0.5s ease;   
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
        margin-bottom: 15px;    
    }

    .tabs button {
        background: none;
        border: none;
        outline: none;
        color: #ffff;
        position: relative;
        font-size: 16px;
        margin-top: 15px;
        margin-left: 12px;
    }

    .tabs button.active {
        color: #88b6ff;
        font-weight: 600;
        transition: color 0.3s ease;
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

    .tabs button.active::after {
        width: 100%; 
        background: #88b6ff; 
        box-shadow: 0 0 8px rgba(59, 130, 246, 0.5);
    }

    .tabs button:hover:not(.active) {
        color: rgba(255, 255, 255, 0.8);
        cursor: pointer;
    }

    .top-grid {
        display: grid;
        grid-template-columns: 1.2fr 1fr 1fr;
        gap: 20px;
        margin-bottom: 20px;
    }

    .top-grid.user-grid {
        grid-template-columns: 1.2fr 1fr;
    }

    .top-grid {
        animation: fadeInUp 0.35s ease;
    }

    .bottom-grid {
        display: grid;
        grid-template-columns: 2fr 1fr;
        gap: 20px;
    }

    .absence-header {
        margin-top: -10px;
    }

    .card {
        background: #001527;
        border-radius: 10px;
        padding: 15px;
        backdrop-filter: blur(10px);
    }

    .cards {
        animation: fadeUp 0.9s cubic-bezier(0.22, 1, 0.36, 1);
    }

    .top-grid,
    .bottom-grid {
        animation: fadeUp 0.9s cubic-bezier(0.22, 1, 0.36, 1);
    }

    .top-grid > *,
    .bottom-grid > * {
        animation: fadeUp 0.8s cubic-bezier(0.22, 1, 0.36, 1);
    }

    @keyframes fadeUp {
        from {
            opacity: 0;
            transform: translateY(16px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .top-grid > .card {
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    .chart {
        display: flex;
        align-items: stretch;
        height: 155px;
        gap: 12px;
    }

    .y-axis {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        height: 140px; 
        font-size: 11px;
        color: rgba(255,255,255,0.4);
        
    }

    .bars::before {
        content: "";
        position: absolute;
        inset: 0;
        background: repeating-linear-gradient(
            to top,
            rgba(255,255,255,0.08),
            rgba(255,255,255,0.08) 1px,
            transparent 1px,
            transparent calc(140px / 5)
            
        );
    }

    .bars-wrapper {
        display: flex;
        flex-direction: column;
        flex: 1;
    }

    .bars {
        position: relative;
        display: flex;
        align-items: flex-end;
        justify-content: space-between;
        height: 140px;
        padding: 0 10px;
    }

    .months {
        display: flex;
        justify-content: space-between;
        padding: 6px 10px 0;
    }

    .months span {
        font-size: 8px;
        color: rgba(255,255,255,0.6);
        text-align: center;
        width: 18px;
    }

    .stack {
        width: 20px;
        height: 140px; 
        display: flex;
        flex-direction: column-reverse;
        justify-content: flex-start;
    }

    .green { background: #22c55e; }
    .red { background: #ef4444; }

    .month {
        font-size: 10px;
        color: rgba(255, 255, 255, 0.6);
        margin-top: 10px;
        text-align: center;
        line-height: 1.2;
        white-space: normal;
        width: 100%;
        display: block;
    }

    .users {
        display: flex;
        flex-direction: column;
        gap: 12px;
    }

    .user {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .user img {
        width: 36px;
        height: 36px;
        border-radius: 50%;
    }

    .user p { font-size: 13px; margin: 0; }
    .user small { font-size: 11px; color: rgba(255,255,255,0.6); }

    @media (max-width: 1000px) {
        .top-grid, .bottom-grid {
            grid-template-columns: 1fr;
        }
    }

    .who-tabs {
        display: flex;
        justify-content: space-between;
        margin: 10px 0 15px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .who-tabs button {
        flex: 1;
        background: none;
        border: none;
        color: rgba(255,255,255,0.5);
        padding: 8px 0;
        font-size: 12px;
        cursor: pointer;
        position: relative;
        outline: none;
    }

    .who-tabs button.active {
        color: #4da3ff;
    }

    .who-tabs button.active::after {
        content: "";
        position: absolute;
        bottom: -1px;
        left: 20%;
        width: 60%;
        height: 2px;
        background: #4da3ff;
        border-radius: 2px;
    }

    .who-header {
        width: 100%;
        display: flex;
        flex-direction: column;
        align-items: center; 
        justify-content: center;
        text-align: center;
        margin-bottom: 10px;
    }

    .who-header h3 {
        margin: 0;
        font-size: 16px;
        font-weight: 600;
    }

    .count {
        margin: 3px 0 0;
        font-size: 12px;
        color: rgba(255,255,255,0.6);
    }

    .who {
        display: flex;
        flex-direction: column;
        height: 350px;
    }

    .who-list {
        overflow-y: auto;
        max-height: 260px;
        padding-right: 5px;
    }

    .who-list::-webkit-scrollbar {
        width: 6px;
    }

    .who-list::-webkit-scrollbar-thumb {
        background: rgba(255,255,255,0.2);
        border-radius: 4px;
    }

    .bottom-grid > * {
        height: 330px;
    }

    .bar:hover {
    opacity: 0.85;
    }

    .tooltip {
        position: absolute;
        top: -30px;
        left: 50%;
        transform: translateX(-50%);
        
        background: rgba(0,0,0,0.8);
        color: #fff;
        font-size: 12px;
        padding: 4px 8px;
        border-radius: 6px;

        white-space: nowrap;
        
        opacity: 0;
        visibility: hidden;
        transition: 0.2s ease;
        pointer-events: none;
    }

    .bar:hover .tooltip {
        opacity: 1;
        visibility: visible;
    }

    .bar-wrapper {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: flex-end;
    }

    .bar {
        position: relative;
        width: 100%;
        transition: 0.2s ease;
        cursor: pointer;
    }

</style>