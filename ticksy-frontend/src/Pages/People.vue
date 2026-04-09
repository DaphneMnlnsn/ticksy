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
                        <div v-if="activeTab === 'members'" class="search-box">
                            <Search v-model="search" />
                        </div>

                        <button 
                        v-if="activeTab === 'members'" 
                        class="add-btn"
                        @click="showAddMemberModal = true"
                    >
                        + Add Member

                            
                        </button>
                    </div>
                        
                    <div :key="activeTab" class="tab-content">
                        
                        <div v-if="activeTab === 'members'" class="table">
                            <div class="table-header people">
                                <span>
                                    <input 
                                        type="checkbox" 
                                        v-model="selectAll" 
                                        @change="toggleAll" 
                                    />
                                </span>
                                <span>{{ filteredUsers.length }} Members</span>
                                <span>Email</span>
                                <span>Team</span>
                                <span>Last Active</span>
                                <span></span>
                            </div>

                            <div class="row people" v-for="user in filteredUsers" :key="user.email">
                                <span>
                                    <input 
                                        type="checkbox"
                                        v-model="selectedUsers" 
                                        :value="user.email"
                                    />
                                </span>

                                <div class="user">
                                    <img :src="sampleIMG" class="avatar" />
                                    <span>{{ user.name }}</span>
                                </div>

                                <span>{{ user.email }}</span>
                                <span>{{ user.team }}</span>
                                <span>{{ user.lastActive }}</span>

                                <div class="actions" @click.stop>
                                    <span class="dots" @click="toggleMenu(user.email)">•••</span>

                                    <div v-if="activeMenu === user.email" class="dropdown">
                                        <div class="dropdown-item" @click="openEdit(user)">
                                    <Edit size="14" />
                                    Edit
                                </div>

                                        <div class="dropdown-item archive">
                                            <Archive size="14" />
                                            Archive
                                        </div>  
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div v-else class="teams-layout">

                            <div class="teams-left">
                                <SidePanel buttonText="+ Add Team">
                                    
                                    <template #header>
                                        <Search v-model="teamSearch" />
                                    </template>

                                    <template #body>
                                     <div 
                                            v-for="team in filteredTeams"
                                            :key="team.id"
                                            class="team-item"
                                            :class="{ active: selectedTeam === team.id }"
                                            @click="selectedTeam = team.id"
                                            
                                        >
                                            <div class="team-info">
                                                <div class="team-name">{{ team.name }}</div>
                                                <div class="team-sub">{{ team.users.length }} members</div>
                                            </div>

                                            <div class="team-arrow">››</div>
                                        </div>
                                    </template>

                                </SidePanel>
                            </div>

                            <div class="teams-right" @click="closeMenu">
                                <div class="table-header people">
                                    <span>
                                        <input 
                                            type="checkbox" 
                                            v-model="selectAllTeams" 
                                            @change="toggleAllTeams" 
                                        />
                                    </span>
                                    <span>{{ selectedTeamMembers.length }} Members</span>
                                    <span>Email</span>
                                    <span>Last Active</span>
                                    <span></span>
                                </div>

                                <div 
                                    class="row people"
                                    v-for="user in selectedTeamMembers"
                                    :key="user.email"
                                >
                                    <span>
                                        <input 
                                            type="checkbox" 
                                            v-model="selectedTeamUsers" 
                                            :value="user.email"
                                        />
                                    </span>

                                    <div class="user">
                                        <img :src="sampleIMG" class="avatar" />
                                        <span>{{ user.name }}</span>
                                    </div>

                                    <span>{{ user.email }}</span>
                                    <span>{{ user.lastActive }}</span>

                                    <div class="actions" @click.stop>
                                        <span class="dots" @click="toggleMenu(user.email)">•••</span>

                                        <div v-if="activeMenu === user.email" class="dropdown">
                                            <div class="dropdown-item" @click="openEditTeam(user); activeMenu = null">
                                            <Edit size="14" />
                                            Edit
                                        </div>

                                            <div class="dropdown-item archive">
                                                <Archive size="14" />
                                                Archive
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

        <EditMemberPanel
            v-model="showEditModal"
            :user="selectedUserData"
            :avatar="sampleIMG"
            @save="handleSaveEdit"
        />

        <AddMemberPanel
            v-model="showAddMemberModal"
            :users="allUsers"
            @save="handleAddMembers"
        />

       <EditTeamsPanel
            v-model="showEditTeamModal"
            :team="selectedTeamData"
            :avatar="sampleIMG"
            @save="handleSaveTeam"
        />
   
</template>   

<script setup>
    import { ref } from 'vue'
    import Sidebar from '../components/Sidebar.vue';
    import Header from '../components/Header.vue';
    import { computed } from 'vue'
    import sampleIMG from '../assets/sample_img.jpg'
    import { watch } from 'vue'
    import { useSearch } from '../services/search.js'
    import Search from '../components/Search.vue'
    import { allUsers } from '../services/summaryData.js'
    import { Edit, Archive, Trash2 } from 'lucide-vue-next'
    import { User, Mail, Phone } from 'lucide-vue-next'
    import SidePanel from '../components/SidePanel.vue'
    import EditMemberPanel from '../components/EditMemberPanel.vue'
    import EditTeamsPanel from '../components/EditTeamsPanel.vue'

   
    
    const activeTab = ref('members')
    const isOpen = ref(true)
    const { search, filtered: filteredUsers } = useSearch(allUsers, ['name', 'email', 'team'])

    function toggleSidebar() {
        isOpen.value = !isOpen.value
    }
    import { onMounted, onUnmounted } from 'vue'

    const activeMenu = ref(null)

    function toggleMenu(email) {
        activeMenu.value = activeMenu.value === email ? null : email
    }

    function closeMenu() {
        activeMenu.value = null
    }

    function handleClickOutside() {
        activeMenu.value = null
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })

    const selectedTeam = ref(1)  
    const selectedUsers = ref([])
    const selectAll = ref(false)
    const selectedTeamUsers = ref([])
    const selectAllTeams = ref(false)

    watch(selectedTeam, () => {
        selectedTeamUsers.value = []
        selectAllTeams.value = false
    })

    function toggleAll() {
        if (selectAll.value) {
            selectedUsers.value = filteredUsers.value.map(user => user.email)
        } else {
            selectedUsers.value = []
        }
    }

    function toggleAllTeams() {
        if (selectAllTeams.value) {
            selectedTeamUsers.value = selectedTeamMembers.value.map(user => user.email)
        } else {
            selectedTeamUsers.value = []
        }
    }

    const teamSearch = ref('')

    const teams = ref([
    {
        id: 1,
        name: 'Interns',
        members: 9,
        users: [
            { name: 'Kiana', email: 'kianamartinxxiv@gmail.com', lastActive: '9 minutes ago' },
            { name: 'Daphne', email: 'daphnemanalansan1213@gmail.com', lastActive: '19 minutes ago' },
            { name: 'Lei', email: 'anyssonleim.it@gmail.com', lastActive: '10 minutes ago' },
            { name: 'Quiana', email: 'quianadomingo004@gmail.com', lastActive: '8 minutes ago' }
        ]
    },
    {
        id: 2,
        name: 'Dev Team',
        members: 4,
        users: [
            { name: 'Mark', email: 'markdev@gmail.com', lastActive: '5 minutes ago' },
            { name: 'John', email: 'johncode@gmail.com', lastActive: '12 minutes ago' },
            { name: 'Lisa', email: 'lisa.dev@gmail.com', lastActive: '1 hour ago' },
            { name: 'Anne', email: 'anne.dev@gmail.com', lastActive: '30 minutes ago' }
        ]
    },
    {
        id: 3,
        name: 'QA Team',
        members: 4,
        users: [
            { name: 'Paul', email: 'paul.qa@gmail.com', lastActive: '15 minutes ago' },
            { name: 'Jane', email: 'jane.qa@gmail.com', lastActive: '22 minutes ago' },
            { name: 'Kyle', email: 'kyle.qa@gmail.com', lastActive: '40 minutes ago' },
            { name: 'Mia', email: 'mia.qa@gmail.com', lastActive: '1 hour ago' }
        ]
    }
    ])

    const filteredTeams = computed(() => {
        return teams.value.filter(t =>
            t.name.toLowerCase().includes(teamSearch.value.toLowerCase())
        )
    })

    const selectedTeamMembers = computed(() => {
        const team = teams.value.find(t => t.id === selectedTeam.value)
        return team ? team.users : []
    })

    const showAddMemberModal = ref(false)

    const showEditModal = ref(false)
    const selectedUserData = ref(null)

    function openEdit(user) {
        selectedUserData.value = user
        showEditModal.value = true
    }
    
    function handleSaveEdit(updatedUser) {
        console.log('Updated:', updatedUser)
    }

    import AddMemberPanel from '../components/AddMemberPanel.vue'
        function handleAddMembers(selected) {
            console.log('Members to add:', selected)
    }

    const props = defineProps(['modelValue', 'team'])
    const emit = defineEmits(['update:modelValue', 'save'])
    const showEditTeamModal = ref(false)
    const selectedTeamData = ref(null)

    function openEditTeam(user) {
        selectedTeamData.value = { ...user }
        showEditTeamModal.value = true
    }

    function handleSaveTeam(updatedUser) {

    const team = teams.value.find(t => t.id === selectedTeam.value)

    if (team) {
        const index = team.users.findIndex(u => u.email === updatedUser.email)

        if (index !== -1) {
        team.users[index] = updatedUser
        }
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
        margin-left: 5px;
        margin-right: 10px;
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

    .tab-content {
        animation: fadeInUp 0.35s ease;
    }

    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .teams-layout {
        display: flex;
        height: 100%;

    }

    .teams-left {
        width: 260px;
        height: 550px;
        background: rgba(0, 19, 36, 0.6);
        display: flex;
        flex-direction: column;
        margin-top: -65px;
        margin-left: -20px;
    }

    .panel-search {
        padding: 10px;
    }

    .panel-search input {
        width: 100%;
        padding: 8px;
        border-radius: 8px;
        border: 1px solid rgba(255,255,255,0.2);
        background: transparent;
        color: white;
    }

    .team-item {
        padding: 12px;
        display: flex;
        justify-content: space-between;
        cursor: pointer;
        border-left: 3px solid transparent;
    }

    .team-item:hover {
        background-color: rgba(0, 19, 36, 0.2);

    }

    .team-item.active {
        background: rgba(0, 56, 103, 0.6);
        border-left: 3px solid #3b82f6;
    }

    .team-name {
        font-size: 13px;
        font-weight: 500;
    }

    .team-sub {
        font-size: 11px;
        opacity: 0.6;
    }

    .team-arrow {
        opacity: 0.6;
    }

    .add-team {
        margin-top: auto;
        padding: 10px;
        text-align: center;
        background: #003867;
        cursor: pointer;
    }

    .teams-right {
        flex: 1;
    }

    .teams-right {
        margin-left: 20px;
        margin-top: -40px;
    }


    .teams-right .table-header.people,
    .teams-right .row.people {
        display: grid;
        grid-template-columns: 40px 1.5fr 2fr 1.5fr 2fr 50px;
        align-items: center;
        
    }

    .teams-right .row.people span {
        font-size: 13px;
    }

    .teams-right .dots {
        text-align: center;
    }

    .actions {
        position: relative;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .dots {
        cursor: pointer;
        font-size: 18px;
        opacity: 0.7;
    }

    .dots:hover {
        opacity: 1;
    }

    .dropdown {
        position: absolute;
        right: 0;
        top: 22px;
        width: 140px;
        background: #061a2b; 
        border-radius: 12px;
        padding: 6px;
        z-index: 100;
        overflow: hidden;
        box-shadow: 0 10px 25px rgba(0,0,0,0.3);
    }

    .dropdown-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 10px;
        cursor: pointer;
        font-size: 12px;
    }

    .dropdown-item:hover {
        background: rgba(255,255,255,0.08);
    }

    .dropdown-item.archive {
        color: #ff4d4d;
    }
    
    .dropdown {
        animation: fadeIn 0.12s ease;
    }

    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(-5px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    

    .side-panel {
    position: fixed;
    top: 0;
    right: 0;
    height: 100vh;
    width: 400px;
    background: #031e2f;
    box-shadow: -5px 0 20px rgba(0,0,0,0.5);
    z-index: 999;
    display: flex;
    flex-direction: column;
    padding: 20px;
    overflow-y: auto;
    animation: slideIn 0.3s ease forwards;
    color: white;
    border-radius: 15px;
    }

    @keyframes slideIn {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
    }

    .edit-modal {
        width: 100%;        
        padding: 0;          
        background: none;    
        position: relative;
        margin-top: 50px;
    }

    .modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
         padding: 10px 20px;
        margin: 0;  
        margin-top: 10px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .modal-header{
        margin: 35px -20px 0 -20px;
    }
    .edit-profile {
        display: flex;
        align-items: center;
        gap: 12px;
        margin: 15px 0;
        padding: 8px 20px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .edit-profile{
        margin: 35px -20px 0 -20px;
    }

    .edit-avatar {
        width: 50px;
        height: 50px;
        border-radius: 50%;
    }

    .edit-name {
        font-weight: 600;
    }

    .edit-sub {
        font-size: 12px;
        opacity: 0.6;
    }

    .input {
        width: 100%;
        margin-top: 5px;
        padding: 10px;
        border-radius: 8px;
        border: none;
        background: #0b2a3c;
        color: white;
        outline: none;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }

    .form-group {
        display: flex;
        flex-direction: column;
        margin-top: 10px;
    }

    .form-group label {
        font-size: 13px;
        margin-bottom: 2px;
        opacity: 0.7;
        
    }

    .edit-modal .close-btn {
        position: absolute;
        top: 15px;
        right: 15px;
        z-index: 10;
        background: none;
        border: none;
        color: white;
        cursor: pointer;
        
    }

    .modal-footer {
        display: flex;
        justify-content: flex-end;
        gap: 10px;
        margin-top: 20px;
        
    }

    .cancel {
        background: none;
        border: none;
        color: white;
        outline: none;
    }

    .save {
        background: #003867;
        border: none;
        padding: 8px 15px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    .save:hover {
        opacity: 0.85;
        cursor: pointer;
    }

    .footer-divider {
        border-bottom: 1px solid rgba(255,255,255,0.1);
        margin: 35px -20px 0 -20px;
    }

</style>