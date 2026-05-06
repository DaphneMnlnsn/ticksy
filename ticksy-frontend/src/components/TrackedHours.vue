<template>
    <div class="tracked-hours">
        <div class="title-group">
            <div class="main-titles">
                <h3 class="main-title">TRACKED HOURS</h3>
                <p class="subtitle">({{ displayTitle }})</p>
            </div>
            
            <span @click="goToTimesheets" class="view-link">View Timesheets</span>
        </div>

        <div class="tracked-content">
            <div class="legend">
                <div class="legend-item">
                    <span class="dot green"></span>
                    <div>
                    <p class="label">WORK HOURS</p>
                    <p class="value">{{ Hours }}</p>
                    </div>
                </div>

                <div class="legend-item">
                    <span class="dot orange"></span>
                    <div>
                    <p class="label">BREAKS</p>
                    <p class="value">{{ breakHours }}</p>
                    </div>
                </div>

                <div class="legend-item">
                    <span class="dot red"></span>
                    <div>
                    <p class="label">OVERTIME HOURS</p>
                    <p class="value">{{ overtimeHours }}</p>
                    </div>
                </div>
                </div>

                <div class="graph">
                <div class="grid">
                    <div class="line" v-for="n in 6" :key="n"></div>
                    
                    <div class="y-axis">
                        <span v-for="label in yAxisLabels" :key="label">{{ label }}</span>
                    </div>          
                </div>
                <div class="bars-wrapper">
                    <div class="bars">
                        <div class="stack">
                            <div class="bar red" :style="{ height: overtimePercent + '%' }"></div>
                            <div class="bar orange" :style="{ height: breakPercent + '%' }"></div>
                            <div class="bar green" :style="{ height: workPercent + '%' }"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import {computed} from 'vue'
    import { useRouter } from 'vue-router'

    const router = useRouter()
    const goToTimesheets = () => {
        router.push('/Timesheets');
    }

    const props = defineProps({
    viewMode: String,
    data: Object   // galing backend
})
    const displayTitle = computed(() => {
        if (props.viewMode === 'Day') return "Today's Activity"
        if (props.viewMode === 'Week') return "Weekly Progress"
        return "Monthly Summary"
    })
    const Hours = computed(() => {
    if (!props.data) return '0 hrs'
    return `${props.data.workHours?.toFixed(1) ?? 0} hrs`
    })

    const breakHours = computed(() => {
        if (!props.data) return '0 hrs'
        return `${props.data.breakHours?.toFixed(1) ?? 0} hrs`
    })

    const overtimeHours = computed(() => {
        if (!props.data) return '0 hrs'
        return `${props.data.overtimeHours?.toFixed(1) ?? 0} hrs`
    })

    const barStyle = computed(() => {
        if (!props.data) return { height: '5%', background: '#00DC28' }
    
    const max =
        props.viewMode === 'Day' ? 10 :
        props.viewMode === 'Week' ? 50 : 200

    const percent = Math.min((props.data.workHours / max) * 100, 100)
        return {
            height: percent + '%',
            background: '#00DC28'
        }
    })

    const workPercent = computed(() => {
        if (!props.data || !props.data.workHours) return 0
        const max = props.viewMode === 'Day' ? 10 : props.viewMode === 'Week' ? 50 : 200
        return Math.min((props.data.workHours / max) * 100, 100)
    })

    const breakPercent = computed(() => {
        if (!props.data || !props.data.breakHours) return 0
        const max = props.viewMode === 'Day' ? 10 : props.viewMode === 'Week' ? 50 : 200
        return Math.min((props.data.breakHours / max) * 100, 100)
    })

    const overtimePercent = computed(() => {
        if (!props.data || !props.data.overtimeHours) return 0
        const max = props.viewMode === 'Day' ? 10 : props.viewMode === 'Week' ? 50 : 200
        return Math.min((props.data.overtimeHours / max) * 100, 100)
    })
    const yAxisLabels = computed(() => {
    if (props.viewMode === 'Day') return ['10h', '8h', '6h', '4h', '2h', '0s']
    if (props.viewMode === 'Week') return ['50h', '40h', '30h', '20h', '10h', '0s']
    return ['200h', '160h', '120h', '80h', '40h', '0s']
    })
</script>

<style scoped>
    .tracked-hours {
        background: #001527;
        width: 100%; 
        padding: 20px 25px 35px;
        border-radius: 5px;
        box-sizing: border-box;
        color: #E6EDF3;
    }

    

    .title-group {
        display: flex;
        justify-content: space-between;
        align-items: center; 
        margin-bottom: 10px;
        width: 100%;
    }

    .main-titles {
        display: flex;
        align-items: baseline;
        gap: 12px;
    }

    .view-link {
        color: #4FB4FF; 
        text-decoration: none;
        opacity: 0.8;
        font-size: 0.85rem;
        font-weight: 500;
        transition: transform 0.2s ease, opacity 0.2s ease;
        cursor: pointer;
    }

    .view-link:hover {
        text-decoration: none;
        transform: scale(1.1);
        opacity: 1;
    }

    .subtitle {
        margin: 0;             
        font-size: 14px;
        opacity: 0.6;          
        font-weight: 400;
    }

    .tracked-content {
        display: flex;
        margin-top: 30px;
    }

    .legend {
        width: 220px;
        display: flex;
        flex-direction: column;
        gap: 20px;
    }

    .legend-item {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .dot {
        margin-left: 15px;
        width: 10px;
        height: 10px;
        border-radius: 50%;
    }

    .green { background: #22c55e; }
    .orange { background: #f97316; }
    .red { background: #ef4444; }

    .label {
        font-size: 12px;
        opacity: 0.7;
        margin: 0;
    }

    .value {
        font-size: 14px;
        margin: 2px 0 0 0;
    }

    .graph {
        flex: 1;
        position: relative;
        display: flex;
        align-items: stretch;
        height: 155px;
        gap: 12px;
    }

    .grid {
        display: flex;
        align-items: stretch;
    }

    .line {
        position: absolute;
        width: 100%;
        height: 1px;
        background: rgba(255,255,255,0.08);
    }

    .line:nth-child(1) { top: 0; }
    .line:nth-child(2) { top: 20%; }
    .line:nth-child(3) { top: 40%; }
    .line:nth-child(4) { top: 60%; }
    .line:nth-child(5) { top: 80%; }
    .line:nth-child(6) { top: 100%; }

    .bars-wrapper {
        display: flex;
        flex-direction: column;
        flex: 1;
    }

    .bars {
        position: relative;
        display: flex;
        align-items: flex-end;
        justify-content: center;
        height: 140px;
        padding: 0 10px;
    }

    .stack {
        width: 50px;
        height: 140px; 
        display: flex;
        flex-direction: column-reverse;
        justify-content: flex-start;
    }

    .bar {
        width: 100%;
        transition: 0.2s ease;
        cursor: pointer;
    }

    .bar:hover {
        opacity: 0.85;
    }

    .green { background: #22c55e; }
    .orange { background: #f97316; }
    .red { background: #ef4444; }

    .y-axis {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        height: 140px; 
        font-size: 11px;
        color: rgba(255,255,255,0.4);
    }
    
</style>