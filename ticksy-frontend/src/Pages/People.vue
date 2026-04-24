<template>
    <div class="dashboard">
        <div class="main-bg"></div>

        <div :class="['app', { collapsed: !isOpen }]">
            <Sidebar :isOpen="isOpen" @toggle="toggleSidebar" />

            <div class="main-content">
                <Header title="People" />

                <transition name="fade">
                    <div v-if="showToast" class="success-toast">
                        <CheckCircle :size="18" />
                        <span>Saved successfully!</span>
                    </div>
                </transition>

                <transition name="fade">
                    <div v-if="showDeleteToast" class="delete-toast">
                        <CheckCircle :size="18" />
                        <span>Unassigned successfully!</span>
                    </div>
                </transition>

                <transition name="fade">
                    <div v-if="showDeleteUserToast" class="delete-toast">
                        <CheckCircle :size="18" />
                        <span>Deleted successfully!</span>
                    </div>
                </transition>

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

                        <div class="search-actions">
                            <transition name="fade">
                                <button 
                                    v-if="activeTab === 'members' && selectedUsers.length > 0" 
                                    class="delete-btn"
                                    @click="bulkArchive"
                                >
                                    <Trash2 :size="14" />
                                    Delete Selected ({{ selectedUsers.length }})
                                </button>
                            </transition>

                        </div>
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

                            <div class="row people" v-for="user in filteredUsers" :key="user.id">
                                <span>
                                    <input 
                                        type="checkbox"
                                        v-model="selectedUsers" 
                                        :value="user.id"
                                    />
                                </span>

                                <div class="user">
                                    <img :src="user.avatarUrl" :alt="user.avatarUrl" class="avatar" />
                                    <span>{{ user.name }}</span>
                                </div>

                                <span>{{ user.email }}</span>
                                <span>{{ user.team }}</span>
                                <span>{{ formatFullDateTime(user.lastActive) }}</span>

                                <div class="actions" @click.stop>
                                    <span class="dots" @click="toggleMenu(user.email)">•••</span>

                                    <div v-if="activeMenu === user.email" class="dropdown">
                                        <div class="dropdown-item" @click="openEdit(user); activeMenu = null">
                                            <Edit size="14" />
                                            Edit
                                        </div>

                                        <div class="dropdown-item archive" @click="handleDeleteUser(user.id)">
                                            <CircleXIcon size="14" />
                                            Delete
                                        </div>  
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div v-else class="teams-layout">

                            <div class="teams-left">
                                <SidePanel 
                                    buttonText="+ Add Team"
                                    @actionClick="isAddTeamOpen = true"
                                >
                                    
                                    <template #header>
                                        <Search v-model="teamSearch" />
                                    </template>

                                    <template #body>
                                        <div 
                                            v-for="team in filteredTeams" 
                                            :key="team.id" 
                                            class="team-row-wrapper"
                                        >
                                            <RowItem
                                                :item="team"
                                                :icon="team.icon"
                                                :class="{ active: activeTeamName === team.name }"
                                                @click="selectTeam(team.id)"
                                            />
                                            
                                            <div class="team-actions-overlay">
                                                <Edit 
                                                    :size="16" 
                                                    class="action-icon" 
                                                    @click.stop="openEditTeam(team)" 
                                                />
                                                <Trash2 
                                                    :size="16" 
                                                    class="action-icon delete" 
                                                    @click.stop="handleDeleteTeam(team.id)" 
                                                />
                                            </div>
                                        </div>
                                    </template>

                                </SidePanel>
                            </div>

                            <div class="teams-right" @click="closeMenu">
                                <div class="table-header people">
                                    <span>{{ selectedTeamMembers.length }} Members</span>
                                    <span>Email</span>
                                    <span>Joined At</span>
                                    <span></span>
                                </div>

                                <div 
                                    class="row people"
                                    v-for="user in selectedTeamMembers"
                                    :key="user.email"
                                >
                                    <div class="user">
                                        <img :src="user.avatarUrl" :alt="user.avatarUrl" class="avatar" />
                                        <span>{{ user.name }}</span>
                                    </div>

                                    <span>{{ user.email }}</span>
                                    <span>{{ user.joinedAt }}</span>

                                    <div class="actions" @click.stop>

                                        <div class="dropdown-item archive" @click="handleUnassignUser(user.id);">
                                            <CircleXIcon size="14" />
                                            <b>Unassign</b>
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
        :allTeams="teamsList"       
        :allSchedules="schedules"   
        @saved="handleMemberSaved"
    />

    <AssignMembersToTeam
        v-model="showAddMemberModal"
        :users="users"
        @save="handleAddMembers"
    />

    <EditTeamPanel
        v-if="isEditTeamOpen"
        :key="selectedTeamData?.id"
        :isOpen="isEditTeamOpen"
        :isSidebarCollapsed="!isOpen"
        :team="selectedTeamData"
        :allUsers="users"
        @close="isEditTeamOpen = false"
        @setup-finished="handleEditTeam"  
    />

    <AddTeamPanel
        :isOpen="isAddTeamOpen"
        :isSidebarCollapsed="!isOpen"
        @close="isAddTeamOpen = false"
        :avatar="sampleIMG"
        :allUsers="users"
        @save="handleAddTeam"
    />
</template>   

<script setup>
    import { ref, computed, onMounted, watch } from 'vue'

    import Sidebar from '../components/Sidebar.vue'
    import Header from '../components/Header.vue'
    import Search from '../components/Search.vue'
    import SidePanel from '../components/SidePanel.vue'

    import EditMemberPanel from '../components/EditMemberPanel.vue'
    import AddMemberPanel from '../components/AddMemberPanel.vue'

    import { Edit, Archive, CircleXIcon, Trash2 } from 'lucide-vue-next'

    import sampleIMG from '../assets/sample_img.jpg'

    import { useSearch } from '../services/search.js'
    import { formatFullDateTime } from '../services/formatting.js'
    import { getUsers, archiveUser, getTeam, unassignMember, archiveTeam } from '../services/people.js'
    import { getTeams, createTeam, updateTeam } from '../services/people.js'

    import { nextTick } from 'vue'
    import AddTeamPanel from '../components/AddTeamPanel.vue'
    import RowItem from '../components/RowItem.vue'
    import AssignMembersToTeam from '../components/AssignMembersToTeam.vue'
    import EditTeamPanel from '../components/EditTeamPanel.vue'

    const users = ref([])
    const activeTab = ref('members')
    const isOpen = ref(true)
    const showToast = ref(false)
    const showDeleteToast = ref(false)
    const showDeleteUserToast = ref(false)

    const activeMenu = ref(null)
    const selectedUsers = ref([])
    const selectAll = ref(false)
    const showAddMemberModal = ref(false)
    const showEditModal = ref(false)
    const selectedUserData = ref(null)

    const teamsList = ref([])

    const selectedTeam = ref(null)
    const selectedTeamUsers = ref([])
    const selectAllTeams = ref(false)

    const isAddTeamOpen = ref(false)
    const isEditTeamOpen = ref(false)
    const selectedTeamData = ref(null)

    const { search, filtered: filteredUsers } =
    useSearch(users, ['name', 'email', 'team'])

    async function fetchUsers() {
        try {
            const data = await getUsers()

            users.value = []
            await nextTick()

            users.value = data.map(u => ({
                id: u.id,
                name: `${u.firstName} ${u.middleName ?? ''} ${u.lastName}`.trim(),
                email: u.email,
                contact: u.phone,
                team: u.teamName || 'No Team',
                schedule: u.scheduleName || 'No Schedule',
                lastActive: formatFullDateTime(u.lastActive),
                avatarUrl: u.avatarUrl || sampleIMG,
                raw: u
            }))
        } catch (err) {
            console.error('Failed to load users:', err)
        }
    }

    async function fetchTeams() {
        try {
            const data = await getTeams();
            teamsList.value = data; 

            if (teamsList.value.length > 0 && !selectedTeam.value) {
            selectTeam(teamsList.value[0].id);
            }
        } catch (err) {
            console.error('Failed to load teams:', err);
        }
    }

    const activeTeamName = computed(() => {
        const active = teams.value.find(t => t.id === selectedTeam.value);
        return active ? active.name : '';
    });

    async function selectTeam(id) {
        selectedTeam.value = id;
        try {
            const res = await getTeam(id);

            console.log("Team API Response:", res);

            const membersList = res.members || (res.data && res.data.members);

            if (membersList) {
                selectedTeamMembers.value = membersList.map(m => ({
                    id: m.id || m.userId,
                    name: m.fullName,
                    avatarUrl: m.avatarUrl || sampleIMG,
                    email: m.email,
                    joinedAt: formatFullDateTime(m.joinedAt)
                }));
            } else {
                console.warn("No members found in response for team:", id);
                selectedTeamMembers.value = [];
            }
        } catch (err) {
            console.error('Failed to fetch team members:', err);
        }
    }

    function toggleMenu(email) {
        activeMenu.value = activeMenu.value === email ? null : email
    }

    function closeMenu() {
        activeMenu.value = null
    }

    function handleClickOutside() {
        activeMenu.value = null
    }

    function toggleAll() {
        if (selectAll.value) {
            selectedUsers.value = filteredUsers.value.map(u => u.id)
        } else {
            selectedUsers.value = []
        }
    }

    function openEdit(user) {
        selectedUserData.value = {
            id: user.id,
            firstName: user.raw.firstName,
            middleName: user.raw.middleName,
            lastName: user.raw.lastName,
            phone: user.raw.phone,
            teamId: user.raw.teamId || null, 
            scheduleId: user.raw.scheduleId || null
        }

        showEditModal.value = true
    }

    async function handleDeleteUser(userId) {
        if (!confirm('Delete this user?')) return
        await archiveUser(userId)

        showDeleteUserToast.value = true;

        setTimeout(() => {
            showDeleteUserToast.value = false;
        }, 2000);

        await fetchUsers()
    }

    async function handleDeleteTeam(teamId) {
        if (!confirm('Delete this team?')) return
        await archiveTeam(teamId)

        showDeleteUserToast.value = true;

        setTimeout(() => {
            showDeleteUserToast.value = false;
        }, 2000);

        await fetchTeams()
    }

    const teams = computed(() => teamsList.value || [])

    const { search: teamSearch, filtered: filteredTeams } = useSearch(teams, ['name'])

    const selectedTeamMembers = ref([])

    function toggleAllTeams() {
        if (selectAllTeams.value) {
            selectedTeamUsers.value =
            selectedTeamMembers.value.map(u => u.email)
        } else {
            selectedTeamUsers.value = []
        }
    }

    async function openEditTeam(team) {
        try {
            const res = await getTeam(team.id)

            const fullTeam = res.data || res

            selectedTeamData.value = {
                ...team,
                members: fullTeam.members || []
            }

            isEditTeamOpen.value = true

        } catch (err) {
            console.error("Failed to load team details:", err)
        }
    }

    console.log("teamsList:", teamsList.value)
    console.log("teams:", teams.value)
    console.log("filteredTeams:", filteredTeams.value)

    async function handleEditTeam() {
        await fetchTeams()
    }

    async function handleAddTeam(team) {
        await fetchTeams()
    }

    async function handleUnassignUser(userId) {
        try {
            await unassignMember(selectedTeam.value, userId);

            await selectTeam(selectedTeam.value);

            showDeleteToast.value = true;

            setTimeout(() => {
                showDeleteToast.value = false;
            }, 2000);

        } catch (err) {
            console.error("Unassign Error:", err);
            const errorMsg = err.response?.data || "Failed to unassign user.";
            alert(typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg));
        }
    };

    onMounted(() => {
        fetchUsers()
        fetchTeams()
        document.addEventListener('click', handleClickOutside)
    })

    async function handleMemberSaved() {
        await fetchUsers()
    }

    async function handleAddMembers(memberIds) {
        showAddMemberModal.value = false
    }

    async function bulkArchive() {
        if (selectedUsers.value.length === 0) return;

        if (!confirm(`Are you sure you want to delete ${selectedUsers.value.length} users?`)) return;

        try {
            await Promise.all(selectedUsers.value.map(id => archiveUser(id)));

            showDeleteUserToast.value = true;
            setTimeout(() => (showDeleteUserToast.value = false), 2000);

            selectedUsers.value = [];
            selectAll.value = false;

            await fetchUsers();
        } catch (err) {
            console.error("Bulk Delete Error:", err);
            alert("Failed to delete some users.");
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
        grid-template-columns: 2.5fr 2.5fr 1.5fr 1fr 50px;
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

    .main-content::-webkit-scrollbar {
        width: 8px;
    }

    .main-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .main-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }

    .main-content::-webkit-scrollbar-track {
        background: transparent; 
    }

    .teams-left .row-item.active {
        background-color: rgba(255, 255, 255, 0.04);
        border-left: 4px solid #88b6ffc8;
        color: #ffffff;
        opacity: 1;
    }

    .success-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #06d6a0;
        color: #001324;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(6, 214, 160, 0.3);
        z-index: 9999;
    }

    .delete-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #d60606;
        color: #d8dee3;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(214, 6, 6, 0.3);
        z-index: 9999;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }

    .delete-btn {
        background: rgba(214, 6, 6, 0.2);
        border: 1px solid rgba(214, 6, 6, 0.4);
        border-radius: 10px;
        padding: 8px 15px 8px 8px;
        color: white;
        display: flex;
        align-items: center;
        gap: 8px;
        outline: none;
        font-size: 13px;
        cursor: pointer;
        transition: all 0.3s ease;
        margin-right: 10px;
    }

    .delete-btn:hover {
        background: rgba(214, 6, 6, 0.4);
        color: white;
        box-shadow: 0 0 15px rgba(214, 6, 6, 0.2);
    }
    
    .search-actions {
        display: flex;
        align-items: center;
    }

    .team-row-wrapper:hover :deep(.icon) {
        opacity: 0;
    }

    .team-row-wrapper {
        position: relative;
        display: flex;
        align-items: center;
    }

    .team-row-wrapper :deep(.row-item) { 
        width: 100%; 
    }

    .team-actions-overlay {
        position: absolute;
        right: 12px;
        display: flex;
        gap: 8px;
        opacity: 0;
        pointer-events: none;
        transition: opacity 0.2s ease;
        padding: 5%;
    }

    .team-row-wrapper:hover .team-actions-overlay {
        opacity: 1;
        pointer-events: auto;
    }

    .action-icon {
        cursor: pointer;
        color: #ffffff;
    }

    .action-icon:hover {
        color: #727272;
    }

    .action-icon.delete:hover {
        color: #ef4444;
    }
</style>