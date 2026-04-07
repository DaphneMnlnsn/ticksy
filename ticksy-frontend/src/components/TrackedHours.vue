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
                    <p class="value">---</p>
                    </div>
                </div>

                <div class="legend-item">
                    <span class="dot red"></span>
                    <div>
                    <p class="label">OVERTIME HOURS</p>
                    <p class="value">---</p>
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
                <div class="bar" :style="barStyle"></div>
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
        viewMode: String
    })
    const displayTitle = computed(() => {
        if (props.viewMode === 'Day') return "Today's Activity"
        if (props.viewMode === 'Week') return "Weekly Progress"
        return "Monthly Summary"
    })
    const Hours = computed(() => {
        if (props.viewMode === 'Day') return "8.5 hrs"
        if (props.viewMode === 'Week') return "42.0 hrs"
        return "168.5 hrs"
    })
    const barStyle = computed(() => {
    if (props.viewMode === 'Day') return { height: '40%', background: '#00DC28' }
    if (props.viewMode === 'Week') return { height: '75%', background: '#00DC28' }
    return { height: '90%', background: '#00DC28' }
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
        justify-content: space-between; /* Pushes content to opposite ends */
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

    .green { background: #00d12f; }
    .orange { background: #ff8c00; }
    .red { background: #ff3b30; }

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
        margin-left: 50px;
    }

    .grid {
        position: absolute;
        width: 100%;
        height: 100%;
    }

    .line {
        height: 1px;
        background: rgba(255,255,255,0.2);
        margin: 25px 0;
    }

    .bar {
        position: absolute;
        bottom: 8px;
        left: 200px;
        width: 60%;
        height: 75%;
        background: #00d12f;
        border-radius: 4px;
        transition: height 0.5s cubic-bezier(0.4, 0, 0.2, 1), background 0.5s ease;
    }

    .y-axis {
        position: absolute;
        left: -65px;
        top: 15px;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        height: 90%;
        font-size: 12px;
        opacity: 0.6;
    }
</style>