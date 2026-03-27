<template>
    <div class="dashboard">
        <div class="main-bg"></div>
        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header title="People" />

                <div class="cards">
                    <div class="tabs">
                    <span 
                        :class="{ active: activeTab === 'members' }"
                        @click="activeTab = 'members'"
                    >
                        Members
                    </span>

                    <span 
                        :class="{ active: activeTab === 'teams' }"
                        @click="activeTab = 'teams'"
                    >
                        Teams
                    </span>

                </div>
            <div class="search-row">
                <div class="search-box">
                    <Search v-model="search" />
                </div>

                <button class="add-btn">
                    + Add Member
                </button>
            </div>

            <div class="table">
                <div class="table-header people">

                <span>
                    <input type="checkbox" v-model="selectAll" @change="toggleAll" />
                </span>

                <span>{{ filteredUsers.length }} Members</span>
                <span>Email</span>
                <span>Team</span>
                <span>Last Active</span>
                <span></span>

            </div>

            <div class="row people" v-for="user in filteredUsers" :key="user.email">

                <span>
                <input type="checkbox" v-model="selectedUsers" :value="user.email" />
            </span>

            <div class="user">
                <img :src="sampleIMG" class="avatar" />
                <span>{{ user.name }}</span>
                </div>
                    <span>{{ user.email }}</span>
                    <span>{{ user.team }}</span>
                    <span>{{ user.lastActive }}</span>
                    <span class="dots">•••</span>
                </div>
            </div>
            </div>
            </div>
        </div>
    </div>
</template>   

<script setup>
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue';
    import Header from '../components/Header.vue';
    import { computed } from 'vue'
    import sampleIMG from '../assets/sample_img.jpg'
    import SearchBar from '../components/Search.vue'
    import { useSearch } from '../services/search.js'
    import Search from '../components/Search.vue'
    import { 
        allUsers
    } from '../services/summaryData.js'
    
    const activeTab = ref('members')
    const isOpen = ref(true)
    const { search, filtered: filteredUsers } = useSearch(allUsers, ['name', 'email', 'team'])

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const selectedUsers = ref([])
    const selectAll = ref(false)

    function toggleAll() {
        if (selectAll.value) {
            selectedUsers.value = filteredUsers.value.map(user => user.email)
        } else {
            selectedUsers.value = []
        }
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
        color: white;
    }

    .app.collapsed .main-content {
        margin-left: var(--sidebar-collapsed-width);
    }

    .cards {        
        border-radius: 5px;
        width: 100%;
        min-height: 85vh; 
        padding: 20px; 
        display: flex;
        flex-direction: column;
        box-sizing: border-box;
        gap: 25px;
        margin-bottom: 30px;
    }

    .table {
        backdrop-filter: blur(6px);
        border-radius: 8px;
        margin-left: -20px;
        margin-right: -20px;
    }

    .table-header.people {
        background-color: #75889A4D;
        display: flex;
        align-items: center;
        gap: 10px;
        width: 100%;
        justify-content: flex-start;
        padding: 10px 20px;
    }

    .table-header input {
        padding: 10px 15px;
        border-radius: 10px;
        border: 2px solid rgba(255,255,255,0.1);
        background-color: rgba(0, 19, 36, 0.2);
        color: white;
        outline: none;
    }

    .row.people {
        display: grid;
        grid-template-columns: 250px 1fr;
        align-items: center;
        padding: 15px 20px;
        border-top: 1px solid rgba(255,255,255,0.1);
        gap: 10px;
    }

    .row.people span {
        font-size: 13px;
    }

    .user {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .avatar {
        width: 30px;
        height: 30px;
        border-radius: 50%;
        object-fit: cover;
    }

  
    .dots {
        text-align: center;
        cursor: pointer;
        opacity: 0.7;
    }

    .dots:hover {
        opacity: 1;
    }

    .tabs {
        display: flex;
        gap: 20px;
        margin-bottom: 15px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .tabs span {
        padding: 8px 0;
        cursor: pointer;
        opacity: 0.6;
        position: relative;
    }

    .tabs span.active {
        opacity: 1;
        font-weight: 600;
    }

    .tabs span.active::after {
        content: "";
        position: absolute;
        bottom: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background: white;
    }

    .search-row {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
    }

    .search-box {
        width: 30%;
    }

    .add-btn {
        background: none;
        border: none;
        border-radius: 10px;
        padding: 8px 15px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
        outline: none;
        font-size: 13px;
    }

    .add-btn:hover {
        opacity: 0.8;
    }

    .table-header.people,
    .row.people {
        display: grid;
        grid-template-columns: 40px 250px 1.5fr 1.5fr 2fr 50px;
        align-items: center;
    }

    .table-header.people span:nth-child(4),
    .table-header.people span:nth-child(5) {
        text-align: center;
    }

    .row.people span:nth-child(4),
    .row.people span:nth-child(5) {
        text-align: center;
    }

</style>