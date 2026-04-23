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

                <div class="cards">
                    <div class="tabs">
                        <button v-for="tab in ['Day','Week','Month']"
                            :key="tab"
                            :class="{ active: activeTab === tab }"
                            @click="activeTab = tab">
                            {{ tab }}
                        </button>
                    </div>

                    <div class="top-grid">
                        <WelcomeCard :viewMode="activeTab" :welcomeImg="welcomeImg"/>

                        <div class="card absences">
                            <h3>Most Absences</h3>

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
                                            {{ item.month }}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <HolidayCard :holidays="holidays" />
                    </div>

                    <div class="bottom-grid">
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
    import { ref, onMounted, computed } from 'vue'
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
    const start = '2026-04-01'
    const end = '2026-04-30'

    const maxAbsence = computed(() => {
    const values = absencesData.value.map(x => x.absent || 0)
    return Math.max(...values, 1) // para iwas divide by 0
})

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

            const [absencesRes, liveRes, hoursRes, holidaysRes] = await Promise.all([
                dashboardService.getMostAbsences(start, end, token),
                dashboardService.getLiveStatus(token),
                dashboardService.getTrackedHours(type, start, end, token),
                dashboardService.getHolidays(defaultCalendarId, new Date().getFullYear(), token).catch(err => {
                    console.warn('Failed to fetch holidays:', err)
                    return { data: [] }
                })
            ])

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
        .filter(h => h.rawDate >= today) // 👉 future holidays lang
        .sort((a, b) => a.rawDate - b.rawDate) // 👉 nearest first

    // 👉 ONLY 1 UPCOMING HOLIDAY
    holidays.value = upcoming.slice(0, 1)

} catch (holidayErr) {
    console.error('Error processing holidays:', holidayErr)
    holidays.value = []
}

            if (absencesRes && absencesRes.data) {
                absencesData.value = absencesRes.data.map(x => ({
                    month: x.fullName ? x.fullName.split(' ')[0] : 'User',
                    present: 0,
                    absent: x.absenceCount || 0
                }))
            }

            if (hoursRes && hoursRes.data) {
                trackedHours.value = hoursRes.data
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

    .app { display: flex; }

    .main-content {
        margin-left: var(--sidebar-width);
        padding: 20px 25px 60px;
        width: 100%;
    }

    .tabs {
        display: flex;
        gap: 20px;
        border-bottom: 1px solid rgba(255,255,255,0.15);
        margin-bottom: 20px;
        
    }

    .tabs button {
        background: none;
        border: none;
        color: rgba(255,255,255,0.6);
        font-size: 15px;
        padding-bottom: 8px;
        cursor: pointer;
        position: relative;
        outline: none;
    }

    .tabs button.active {
        color: #4da3ff;
    }

    .tabs button.active::after {
        content: "";
        position: absolute;
        bottom: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background: #4da3ff;
    }

    .top-grid {
        display: grid;
        grid-template-columns: 1.2fr 1fr 1fr;
        gap: 20px;
        margin-bottom: 20px;
    }

    .bottom-grid {
        display: grid;
        grid-template-columns: 2fr 1fr;
        gap: 20px;
    }

    .card {
        background: #001527;
        border-radius: 12px;
        padding: 15px;
        backdrop-filter: blur(10px);
    }

    .chart {
        display: flex;
        height: 160px;
        gap: 12px;
    }

    .y-axis {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        height: 140px; 
        font-size: 11px;
        color: rgba(255,255,255,0.4);
        padding-top: 4%; 
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
            transparent 30px
            
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
        font-size: 11px;
        color: rgba(255,255,255,0.6);
        text-align: center;
        width: 18px;
    }

        .stack {
            width: 18px;
            height: 140px; 
            display: flex;
            flex-direction: column-reverse;
            justify-content: flex-start;
        }

        .green { background: #22c55e; }
        .red { background: #ef4444; }

        .month {
        font-size: 11px;
        color: rgba(255,255,255,0.6);
        margin-top: 6px;
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