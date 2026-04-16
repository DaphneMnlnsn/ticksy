<template>
    <transition name="fade">
        <DimmedBg
            v-if="modelValue"
            @click="$emit('update:modelValue', false)"
        />
    </transition>

    <div v-if="modelValue" class="side-panel">
        <div class="modal-header divider">
            <h2>Assign Members</h2>
            <button class="close-btn" @click="$emit('update:modelValue', false)">✕</button>
        </div>

        <div class="section">
            <p class="section-title">Invite by Link</p>
            <p class="desc">
                We recommend letting members create their profile so they can log from their devices.
                Send them this invite link to get started!
            </p>

            <div class="link-box">
                <input type="text" value="ticksy.com/join/sample" readonly />
                <button class="copy-btn" @click="copyLink">
                <Copy size="14" class="icon-transparent" />
                Copy Link
                </button>
        </div>

            <div class="generate-link" @click="generateLink">
            Generate new link
            <RefreshCw size="14" class="icon-transparent" />
        </div>
    </div>

        <div class="section">
        <p class="section-title">Manually Add Members</p>
        <div class="modal-search-wrapper">
            <Search v-model="searchQuery" />
        </div>

        <div class="table">
        <div class="table-header people">
        <span class="header-with-icon"><User size="14" /> Name</span>
        <span class="header-with-icon"><Mail size="14" /> Email</span>
        <span class="header-with-icon"><Phone size="14" /> Contact</span>
        </div>

        <div class="row people" v-for="user in filteredUsers" :key="user.email">
                <span>
                    <input type="checkbox" :value="user.id" v-model="selectedUsers" />
                </span>
                <span>{{ user.firstName }}</span>
                <span>{{ user.email }}</span>
                <span>{{user.phone}}</span>
                </div>
            </div>
        </div>

        <div class="modal-footer">
        <button class="cancel" @click="$emit('update:modelValue', false)">Cancel</button>
        <button class="save" @click="saveMembers">Save</button>
        </div>
    </div>
</template>

<script setup>
    import { ref, computed, watch } from 'vue'
    import Search from './Search.vue'
    import { User, Mail, Phone } from 'lucide-vue-next'
    import { Copy } from 'lucide-vue-next'
    import { RefreshCw } from 'lucide-vue-next'
    import DimmedBg from './DimmedBg.vue'
    import { assignMembers } from '../services/schedule'

    const props = defineProps({
        modelValue: Boolean,
        users: { type: Array, default: () => [] },
        inviteLink: { type: String, default: 'ticksy.com/join/sample' },
        scheduleId: [String, Number]
    })

    const emit = defineEmits(['update:modelValue', 'save'])

    const searchQuery = ref('')
    const selectedUsers = ref([])

    const filteredUsers = computed(() => {
        if (!props.users) return [] 
        if (!searchQuery.value) return props.users
        
        const query = searchQuery.value.toLowerCase()
        
        return props.users.filter(u => {
            // Combine names for a full-name search
            const fullName = `${u.firstName} ${u.lastName}`.toLowerCase()
            return fullName.includes(query) || u.email.toLowerCase().includes(query)
        })
    })

    function copyLink() {
    navigator.clipboard.writeText(props.inviteLink)
    }

    function generateNewLink() {

        alert('New invite link generated!')
    }

    async function saveMembers() {
        try {
            await assignMembers(props.scheduleId, selectedUsers.value);

            emit('save', selectedUsers.value);
            emit('update:modelValue', false);

        } catch (err) {
            console.error("Assign members failed:", err);
            alert("Failed to assign members");
        }
    }

    const selectAll = ref(false)

    watch(selectAll, (val) => {
        if (val) {
        selectedUsers.value = filteredUsers.value.map(u => u.id)
        } else {
        selectedUsers.value = []
        }
    })

    function toggleAll() {
        selectAll.value = !selectAll.value
    }

    function generateLink() {
        alert('New link generated!');
    }
</script>

<style scoped>
    .side-panel {
    position: fixed;
    top: 0;
    right: 0;
    height: 100vh;
    background: #001527;
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

    .side-panel {
    width: 800px !important;
    }

    @keyframes slideIn {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
    }

    .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
    font-family: Righteous;
    }

    .close-btn {
    background: none;
    border: none;
    color: white;
    cursor: pointer;
    outline: none;
    }

    .section-title {
    font-weight: 600;
    margin-bottom: 8px;
    }

    .desc {
    font-size: 12px;
    opacity: 0.7;
    }

    .link-box {
    display: flex;
    border: 1px solid rgba(255,255,255,0.2);
    border-radius: 6px;
    overflow: hidden;
    margin-top: 10px;
    }

    .link-box input {
    flex: 1;
    background: transparent;
    border: none;
    color: white;
    padding: 10px;
    outline: none;
    }

    .copy-btn {
    display: flex;
    align-items: center;
    gap: 6px; 
    color: white;
    padding: 8px 12px;
    background: transparent; 
    border: none;
    cursor: pointer;
    outline: none;
    font-size: 13px;
    transition: opacity 0.2s;

    }

    .copy-btn:hover {
    opacity: 0.85;
    }

    .icon-transparent {
    opacity: 0.5;
    }


    .generate-link {
    display: inline-flex;
    align-items: center;
    gap: 5px;        
    margin-top: 8px;
    font-size: 12px;
    color: #4db8ff;
    cursor: pointer;
    }

    .generate-link:hover {
        opacity: 0.7;
    }

    .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 25px;
    }

    .cancel, .save {
        padding: 10px 20px;
        min-width: 120px;
        height: fit-content;
        border-radius: 6px;
        border: none;
        cursor: pointer;
        font-weight: 500;
    }

    .cancel {
    background: #003867;
    border: none;
    color: white;
    cursor: pointer;
    outline: none;
    }

    .save {
    background: rgb(0, 56, 103, 50%);
    border: none;
    padding: 8px 15px;
    border-radius: 6px;
    color: white;
    cursor: pointer;
    outline: none;
    }

    .table {
    backdrop-filter: blur(6px);
    border-radius: 8px;
    overflow: hidden;
    margin-top: 10px;
    }

    .table-header.people,
    .row.people {
    display: grid;
    grid-template-columns: 40px 1.5fr 2fr 1.5fr;
    align-items: center;
    padding: 10px 15px;
    gap: 10px;
    }

    .table-header.people {
    display: grid;
    grid-template-columns: 1.5fr 2fr 1.5fr;
    align-items: center;
    padding: 12px 15px;
    gap: 10px;
    font-weight: 600;
    font-size: 13px;
    margin-left: 45px;
    }

    .row.people {
    display: grid;
    grid-template-columns: 40px 1.5fr 2fr 1.5fr;
    align-items: center;
    padding: 12px 15px;
    gap: 10px;
    border-top: 1px solid rgba(255,255,255,0.08);
    font-size: 13px;
    border-radius: 8px;
    cursor: pointer;
    transition: background 0.2s, transform 0.1s;
    }

    .divider {
    border-bottom: 1px solid rgba(255, 255, 255, 0.1); 
    margin-bottom: 1px; 
    }

    .section {
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        padding-bottom: 15px;
    }

    .section-title {
    font-weight: 600;
    margin-bottom: 20px;
    position: relative;
    padding-bottom: 10px;
    }

    .section-title::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 0;
    width: 100%;
    height: 1px;
    background-color: rgba(255, 255, 255, 0.1); 
    }

    .fade-enter-active,
    .fade-leave-active {
    transition: opacity 0.25s ease;
    }

    .fade-enter-from,
    .fade-leave-to {
    opacity: 0;
    }

    .slide-enter-active,
    .slide-leave-active {
    transition: transform 0.3s ease, opacity 0.3s ease;
    }

    .slide-enter-from,
    .slide-leave-to {
    transform: translateX(100%);
    opacity: 0;
    }

    .side-panel::-webkit-scrollbar {
        width: 8px;
    }

    .side-panel::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .side-panel::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }

    .side-panel::-webkit-scrollbar-track {
        background: transparent; 
    }
</style>