<template>
    <transition name="slide-calendar">
        <div v-if="isOpen" 
            class="team-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="team-header">
                <h3>Edit Team</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="team-content">
                <div class="input-group">
                    <div class="label-header">
                        <label class="form-label">Team name</label>
                        <span v-if="errors.name" class="error-msg">*Required</span>
                    </div>
                    <input 
                        type="text" 
                        v-model="form.teamName" 
                        placeholder="Team name" 
                        @input="errors.name = false" 
                    />
                </div>

                <div class="input-group">
                    <div class="input-header">
                        <label class="form-label">Team Members</label>
                        <div class="user-box multi" @click="showAssignMembersToTeamModal = true">
                            <UserPlus class="add-icon" />
                        </div>
                    </div>
                    
                    <div v-if="selectedMembers.length > 0" class="members-display">
                        <div v-for="member in selectedMembers" :key="member.id" class="member-chip">
                            <img :src="member.avatar" :alt="member.name" class="member-avatar" />
                            <span class="member-name">{{ member.name }}</span>
                            <button class="remove-btn" @click="removeMember(member.id)">✕</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-save" @click="handleSaveTeam">Save</button>
            </div>
        </div>
    </transition>

    <AssignMembersToTeam
        v-model="showAssignMembersToTeamModal"
        :users="allUsers"
        :initial-selected-ids="form.memberIds"
        :teamId="form.id"
        :showInvite="true"
        @save="handleAddMembers"
    />
</template>

<script setup>
    import { ref, watch } from "vue";
    import { X, CheckCircle, UserPlus } from "lucide-vue-next";
    import { updateTeam } from "../services/people";
    import AssignMembersToTeam from "./AssignMembersToTeam.vue";

    const props = defineProps({
        isOpen: Boolean,
        team: Object,
        isSidebarCollapsed: Boolean,
        allUsers: { type: Array, default: () => [] }
    });

    const emit = defineEmits(['close', 'setup-finished']);

    const showToast = ref(false);
    const errors = ref({ name: false });
    const showAssignMembersToTeamModal = ref(false);
    const selectedMembers = ref([]);

    const form = ref({
        id: null,
        teamName: '',
        memberIds: []
    })

    watch(() => props.team, (newVal) => {
        if (newVal) {
            let rawIds = newVal.memberIds || [];
            if (newVal.members && rawIds.length === 0) {
                rawIds = newVal.members.map(m => m.id || m.userId);
            }

            const normalizedIds = rawIds.map(id => Number(id));

            form.value = {
                id: newVal.id || null,
                teamName: newVal.name || newVal.teamName || '',
                memberIds: normalizedIds
            }

            selectedMembers.value = props.allUsers.filter(u => 
                normalizedIds.includes(Number(u.id))
            );
        }
    }, { immediate: true });

    function handleAddMembers(memberIds) {
        const normalizedIds = memberIds.map(id => Number(id));
        
        form.value.memberIds = normalizedIds;

        selectedMembers.value = props.allUsers.filter(u => 
            normalizedIds.includes(Number(u.id))
        );

        showAssignMembersToTeamModal.value = false;
    }

    function removeMember(memberId) {
        selectedMembers.value = selectedMembers.value.filter(m => m.id !== memberId);
        form.value.memberIds = selectedMembers.value.map(m => m.id);
    }

    async function handleSaveTeam() {
        if (!form.value.teamName.trim()) {
            errors.value.name = true;
            return;
        }

        try {
            await updateTeam(form.value.id, {
                teamName: form.value.teamName,
                memberIds: form.value.memberIds
            });

            showToast.value = true;
            
            setTimeout(() => {
                showToast.value = false;
                emit('setup-finished');
                emit('close');
            }, 2000);

        } catch (err) {
            console.error("Save Error:", err);
            alert("Failed to save team.");
        }
    }
</script>

<style scoped>

    .team-panel {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        width: 420px;
        background-color: #001527;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        color: white;
        box-shadow: -10px 0 30px rgba(0, 0, 0, 0.5);
    }

    .team-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .team-header h3 {
        font-size: 1.2rem;
        font-weight: 500;
        margin-left: 30px;
        font-family: Righteous;
        font-size: 20px;
    }

    .icon-btn {
        background: transparent;
        border: none;
        color: white;
        cursor: pointer;
        opacity: 0.7;
        outline: none;
    }

    .team-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .input-group {
        margin-bottom: 24px;
    }

    .form-label {
        display: block;
        font-size: 0.85rem;
        color: #ffffff;
        margin-bottom: 8px;
    }

    input[type="text"] {
        width: 100%;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 10px 12px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    .active {
        background: #003366 !important;
    }

    .arrow-icon {
        margin: 0 5px -5px 5px;
    }

    .label-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 8px;
    }

    .label-header .form-label {
        margin-bottom: 0;
    }

    .error-msg {
        color: rgb(255, 99, 99);
        font-size: 0.65rem;
        font-weight: 500;
        letter-spacing: 0.3px;
        animation: errorShake 0.4s ease-in-out;
    }

    .form-select {
        background-color: #00386780;
        padding: 12px;
        font-size: 12px;
        border-radius: 5px;
        width: 100%;
        margin-bottom: 5px;

        border: none;
        outline: none;
        box-shadow: none;
    }

    .form-select option {
        background: #001527;
        color: white;
    }

    @keyframes errorShake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(4px); }
        75% { transform: translateX(-4px); }
    }

    .footer-actions {
        padding: 16px 24px;
        display: flex;
        gap: 12px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
        justify-content: flex-end;
    }

    .btn-invite, .btn-save {
        padding: 10px 20px;
        min-width: 120px;
        height: fit-content;
        border-radius: 6px;
        border: none;
        cursor: pointer;
        font-weight: 500;
    }

    .btn-save {
        background: rgb(0, 56, 103, 50%);
        color: white;
        outline: none;
    }

    .btn-save:hover {
        opacity: 0.8;
    }

    .input-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 12px;
    }

    .user-box {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 32px;
        height: 32px;
        background: rgba(59, 130, 246, 0.2);
        border-radius: 6px;
        cursor: pointer;
        transition: all 0.2s ease;
    }

    .user-box:hover {
        background: rgba(59, 130, 246, 0.3);
    }

    .add-icon {
        color: #3b82f6;
    }

    .members-display {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
        margin-top: 12px;
    }

    .member-chip {
        display: flex;
        align-items: center;
        gap: 8px;
        background: rgba(59, 130, 246, 0.1);
        border: 1px solid rgba(59, 130, 246, 0.3);
        padding: 6px 10px;
        border-radius: 20px;
        font-size: 13px;
    }

    .member-avatar {
        width: 24px;
        height: 24px;
        border-radius: 50%;
        object-fit: cover;
    }

    .member-name {
        max-width: 100px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

    .remove-btn {
        background: none;
        border: none;
        color: rgba(255, 255, 255, 0.6);
        cursor: pointer;
        padding: 0 4px;
        font-size: 14px;
        outline: none;
        transition: color 0.2s;
    }

    .remove-btn:hover {
        color: #ff6b6b;
    }

    .slide-calendar-enter-active,
    .slide-calendar-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-calendar-enter-from,
    .slide-calendar-leave-to {
        transform: translateX(100%);
    }

    .unit-label {
        font-size: 0.8rem;
        color: white;;
        margin-right: 8px;
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
        z-index: 2000;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }
</style>