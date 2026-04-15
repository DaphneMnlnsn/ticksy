<template>
    <div v-if="show" class="legend-popup" :style="style">
    <div class="legend-grid">
        
        <div class="legend-column">
            <div class="legend-item">
                <span class="box" style="background:#D9D9D9"></span>
                <span>0 hours</span>
            </div>

            <div class="legend-item">
                <span class="box" style="background:#FFC300"></span>
                <span>1–5 hours</span>
            </div>

            <div class="legend-item">
                <span class="box" style="background:#00DC28"></span>
                <span>6–8 hours</span>
            </div>

            <div class="legend-item">
                <span class="box" style="background:#E53935"></span>
                <span>9+ hours</span>
            </div>
        </div>

        <div class="legend-column">
            <div class="legend-item">
                <Calendar class="icon" />
                <span>Public holiday</span>
            </div>

            <div class="legend-item">
                <TreePalm class="icon" />
                <span>Rest day</span>
            </div>

            <div class="legend-item">
                <Luggage class="icon" />
                <span>Time off</span>
            </div>
        </div>

    </div>
    </div>
</template>

<script setup>
    import { Calendar, TreePalm, Luggage, ClockFading, Moon } from 'lucide-vue-next'
    defineProps({
        show: Boolean,
        style: Object
    });
</script>

<style scoped>
    .legend-popup {
        position: absolute;
        background: #001527;
        color: white;
        padding: 15px;
        border-radius: 10px;
        width: 270px;
        box-shadow: 0 10px 25px rgba(0,0,0,0.4);
        z-index: 999;
    }

    .legend-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
    }

    .legend-column {
        display: flex;
        flex-direction: column;
    }

    .legend-item {
        display: flex;
        align-items: center;
        margin: 5px 0;
        font-size: 13px;
    }

    .box {
        width: 16px;
        height: 16px;
        margin-right: 10px;
        border-radius: 1px;
    }

    .icon {
        width: 18px;
        margin-right: 10px;
        text-align: center;
    }
</style>