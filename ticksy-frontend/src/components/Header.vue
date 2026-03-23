<template>
    <div class="header">
        <h1>Dashboard</h1>
        <span class="out-time">{{ lastOut }}</span>
        <Timer :size="23" color="#00DC28" stroke-width="2" class="header-icon" />
    </div>
</template>

<script setup>
    const lastOut = "Last out an hour ago"
    import { Timer } from "lucide-vue-next"; 
</script>

<style scoped>
.header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    padding: 0 0 0 10px;  
}

.out-time {
    font-size: 14px;
    opacity: 0.8;
    color: white;
    padding: 10px;
    margin-left: auto;
}

.header-icon {
    opacity: 0.8;
    cursor: pointer;
    transition: transform 0.2s ease;
}

.header-icon:hover {
    transform: scale(1.1);
    opacity: 1;
}

h1 {
    font-size: 30px;
    font-family: Righteous;
}
</style>
