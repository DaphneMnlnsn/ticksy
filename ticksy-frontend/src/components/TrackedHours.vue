<script setup>
</script>

<template>
    <div class="tracked-hours">
        <h3>TRACKED HOURS</h3>
        <div class="tracked-content">
            <div class="legend">
                <div class="legend-item">
                    <span class="dot green"></span>
                    <div>
                    <p class="label">WORK HOURS</p>
                    <p class="value">---</p>
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
                        <span>8h 20m</span>
                        <span>6h 25m</span>
                        <span>4h 1m</span>
                        <span>2h 25m</span>
                        <span>1h 23m</span>
                        <span>0s</span>
                    </div>          
                </div>
                <div class="bar"></div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.tracked-hours {
    background: #001527;
    width: 100%; 
    padding: 20px 25px 35px;
    border-radius: 5px;
    box-sizing: border-box;
    color: #E6EDF3;
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