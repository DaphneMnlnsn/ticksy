<template>
    <div :class="['app', { collapsed: !isOpen }]">
        <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

        <div class="main-content"> <!--DO NOT change class!-->
            <p>Timesheets</p>
        </div>
    </div>
</template>

<script setup>
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue'

    const isOpen = ref(true)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
</script>

<style scoped>
    .app {
        display: flex;
    }

    .main-content {
        margin-left: var(--sidebar-width); /*DO NOT CHANGE!*/
        flex: 1;
        padding: 1rem;
        transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .app.collapsed .main-content {
        margin-left: var(--sidebar-collapsed-width); /*DO NOT CHANGE!*/
    }
    
</style>