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
                                    <span>10</span>
                                    <span>8</span>
                                    <span>6</span>
                                    <span>4</span>
                                    <span>2</span>
                                    <span>0</span>
                                   
                                  
                                </div>

                                <div class="bars-wrapper">
                                <div class="bars">
                                    <div v-for="item in absencesData" :key="item.month" class="bar-group">
                                        <div class="stack">
                                            <div class="green" :style="{ height: item.present * 10 + '%' }"></div>
                                            <div class="red" :style="{ height: item.absent * 10 + '%' }"></div>
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
                        <TrackedHours :viewMode="activeTab" />

                        <div class="card who">
                        <div class="who-header">
                            <h3>Who's In/Out</h3>
                            <p class="count">
                                {{ peopleByStatus[activeStatusTab].length }} members
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

                            <div class="users">
                                <div 
                                    class="user" 
                                    v-for="p in peopleByStatus[activeStatusTab]" 
                                    :key="p.name"
                                >
                                    <img :src="p.avatar || sampleImg" class="avatar-small" />
                                    <div>
                                        <p>{{ p.name }}</p>
                                        <small>Last in {{ p.time }}</small>
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
    import { ref } from 'vue'
    import welcomeImg from "/welcome-img.png";
    import WelcomeCard from '../components/WelcomeCard.vue';
    import HolidayCard from '../components/HolidayCard.vue';
    import TrackedHours from '../components/TrackedHours.vue';
    import Sidebar from '../components/Sidebar.vue';
    import sampleImg from '../assets/sample_img.jpg'

    const activeTab = ref('Week')

    const holidays = [
        { month: 'MAY', day: '25', name: 'Memorial Day' },
        { month: 'JUN', day: '19', name: 'Juneteenth' },
        { month: 'JUL', day: '03', name: 'Memorial Independence Day' }
    ]

    const absencesData = [
        { month: 'Mar', present: 1, absent: 1 },
        { month: 'Apr', present: 3, absent: 2 },
        { month: 'May', present: 7, absent: 0 },
        { month: 'Jun', present: 2, absent: 0 },
        { month: 'Jul', present: 0, absent: 0 },
        { month: 'Aug', present: 0, absent: 0 }
    ]

    const activeStatusTab = ref('In')

    const peopleByStatus = {
        In: [
            {
                name: 'Kristen Quiambao Domingo',
                time: '11/13/2025, 2:00 PM',
                avatar: sampleImg
            },

            {
                name: 'Lei Anysson Marquez',
                time: '11/13/2025, 2:00 PM',
                avatar: sampleImg
            }
        ],
        Break: [
            {
                name: 'Daphne Manalansan',
                time: '11/13/2025, 2:00 PM',
                avatar: sampleImg
            }
        ],
        Out: [
            {
                name: 'Kiana Martin',
                time: '11/13/2025, 2:00 PM',
                avatar: sampleImg
            }
        ]
    }

    const isOpen = ref(true)
    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
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

    .chart-container {
    padding-bottom: 20px;
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
    margin-top: 6px;   /* 🔥 add spacing down */
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
</style>