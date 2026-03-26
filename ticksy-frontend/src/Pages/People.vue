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
                    <input type="text" placeholder="Search..." v-model="search" />

                <button class="add-btn">
                    + Add Member
                </button>
            </div>

            <div class="header-line"></div>
                <div class="table">
                    <div class="table-header people">

                <span>
                    <input type="checkbox" v-model="selectAll" @change="toggleAll" />
                </span>

                <span>{{ users.length }} Members</span>
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
    
    const activeTab = ref('members')
    const isOpen = ref(true)

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }

    const search = ref("")
    const users = ref([
        {
            name: "Kiana",
            email: "kianamartinxiv@gmail.com",
            team: "-",
            lastActive: "9 minutes ago"
        },
        {
            name: "Daphne",
            email: "daphnemenalansan123@gmail.com",
            team: "-",
            lastActive: "19 minutes ago"
        },
        {
            name: "Lei",
            email: "anyssonleim.it@gmail.com",
            team: "-",
            lastActive: "10 minutes ago"
        },
        {
            name: "Quiana",
            email: "quianadomingo004@gmail.com",
            team: "-",
            lastActive: "8 minutes ago"
        }
    ])

    const filteredUsers = computed(() => {
        return users.value.filter(user =>
            user.name.toLowerCase().includes(search.value.toLowerCase()) ||
            user.email.toLowerCase().includes(search.value.toLowerCase())
        )
    })

    const selectedUsers = ref([])
    const selectAll = ref(false)

    function toggleAll() {
        if (selectAll.value) {
            selectedUsers.value = users.value.map(user => user.email)
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

    .table {
        backdrop-filter: blur(6px);
        border-radius: 8px;
        padding: 20px;
    }

    .table-header.people {
        display: grid;
        grid-template-columns: 250px 1.5fr 1fr 1fr 40px;
        align-items: center;
        padding: 10px 0;
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
        padding: 15px 0;
        border-top: 1px solid rgba(255,255,255,0.1);
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
        justify-content: space-between;
        align-items: center;
        margin: 10px 0;
    }

    .search-row input {
        width: 250px;
        padding: 10px 15px;
        border-radius: 10px;
        border: 2px solid rgba(255,255,255,0.1);
        background-color: rgba(0, 19, 36, 0.2);
        color: white;
        outline: none;
    }

    .header-line {
        width: 100%;
        height: 1px;
        background: rgba(255,255,255,0.1);
        margin: 10px 0;
    }

    .add-btn {
        background: #052947;
        border: none;
        border-radius: 10px;
        padding: 8px 15px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
        outline: none;
        box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
    }

    .add-btn:hover {
        opacity: 0.8;
    }

    .table-header.people,
    .row.people {
        display: grid;
        grid-template-columns: 40px 250px 1.5fr 1fr 1fr 50px;
        align-items: center;
    }

</style>