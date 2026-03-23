<template>
    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header title="People" />
                <div class="cards">
                    <!--ADD CONTENT HERE-->
                </div>
            </div>
        </div>
    </div>
</template>   

<script setup>
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue';
    import Header from '../components/Header.vue';

    const isOpen = ref(true)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
</script>

<style scoped>

:global(html), 
:global(body), 
:global(#app) {
    width: 100% !important;
    margin: 0;
    padding: 0;
    overflow: hidden;
}

.main-bg {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-image: url("../assets/main_bg.jpg");
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
}

.dashboard {
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100vh;
}

.app {
    display: flex;
    width: 100%;
    flex: 1;
    min-height: 100vh;
    overflow: hidden;
}

.main-content {
    margin-left: var(--sidebar-width); 
    flex: 1;
    padding: 20px 20px 60px 20px;
    transition: margin-left 0.5s cubic-bezier(0.4, 0, 0.2, 1);
    overflow-x: hidden;
    min-width: 0;
    z-index: 1;
}

.app.collapsed .main-content {
    margin-left: var(--sidebar-collapsed-width);
}

.cards {
    background-color: rgba(0, 19, 36, 0.2);
    border-radius: 5px;
    width: 100%;
    min-height: 85vh; 
    padding: 20px 20px 45px 20px; 
    display: flex;
    flex-direction: column;
    box-sizing: border-box;
    margin-bottom: 30px;
}
</style>