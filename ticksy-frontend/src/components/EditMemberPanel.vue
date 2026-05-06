<template>
    <div v-if="modelValue" class="overlay">
        <div class="overlay-bg" @click="$emit('update:modelValue', false)"></div>

        <div class="side-panel">
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="modal-header">
                <h2>Edit Member</h2>
                <button class="close-btn" @click="$emit('update:modelValue', false)">✕</button>
            </div>

            <div class="edit-profile">
                <img :src="avatar" class="edit-avatar" />
                <div>
                    <div class="edit-name">{{ form.firstName }} {{ form.lastName }}</div>
                    <div class="edit-sub">Member since Dec 1994</div>
                </div>
            </div>

            <div class="section">
                <p class="section-title">Member Details</p>
                <div class="form">
                    <div class="input-group">
                        <div class="form-label">First Name</div>
                        <input type="text" v-model="form.firstName" :disabled="!isEditing" />
                    </div>

                    <div class="input-group">
                        <div class="form-label">Middle Initial</div>
                        <input type="text" v-model="form.middleName" :disabled="!isEditing" />
                    </div>

                    <div class="input-group">
                        <div class="form-label">Last Name</div>
                        <input type="text" v-model="form.lastName" :disabled="!isEditing" />
                    </div>

                    <div class="input-group">
                        <div class="form-label">Phone Number</div>
                        <input type="text" v-model="form.contact" :disabled="!isEditing" />
                    </div>
                </div>
            </div>

            <div class="section">
                <p class="section-title">Assignments</p>
                <div class="form-group">
                    <label>Assign to Team</label>
                    <select class="input" v-model="form.teamId">
                        <option :value="null">No Team Assigned</option>
                        <option v-for="team in allTeams" :key="team.id" :value="team.id">
                        {{ team.name }}
                        </option>
                    </select>
                </div>

                <div class="form-group">
                    <label>Assign to Schedule</label>
                    <select class="input" v-model="form.scheduleId">
                        <option :value="null">No Schedule Assigned</option>
                        <option v-for="sched in allSchedules" :key="sched.id" :value="sched.id">
                        {{ sched.name }}
                        </option>
                    </select>
                </div>
            </div>

            <div class="modal-footer">
                <button class="cancel" @click="$emit('update:modelValue', false)">Cancel</button>
                <button class="save" @click="onSave" :disabled="isSaving">
                    {{ isSaving ? 'Saving...' : 'Save' }}
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, watch, onMounted } from 'vue'
    import { CheckCircle } from 'lucide-vue-next'
    import { getTeams, updateUser } from '../services/people'
    import { getSchedules } from '../services/schedule'

    const props = defineProps({
        modelValue: Boolean,
        user: Object,
        avatar: String
    })

    const emit = defineEmits(['update:modelValue', 'saved'])

    const allTeams = ref([])
    const allSchedules = ref([])
    const isEditing = ref(true)
    const isSaving = ref(false)
    const showToast = ref(false)

    const form = ref({
        id: null,
        firstName: '',
        middleName: '',
        lastName: '',
        contact: '',
        teamId: null,
        scheduleId: null
    })

    onMounted(async () => {
        try {
            const [teamsData, schedulesData] = await Promise.all([
                getTeams(),
                getSchedules()
            ])
            allTeams.value = teamsData
            allSchedules.value = schedulesData
        } catch (err) {
        console.error("Failed to load options:", err)
        }
    })

    watch(() => props.user, (val) => {
        if (!val) return
        form.value = {
            id: val.id,
            firstName: val.firstName || '',
            middleName: val.middleName || '',
            lastName: val.lastName || '',
            contact: val.phone || '',
            teamId: val.teamId || null,
            scheduleId: val.scheduleId || null
        }
    }, { immediate: true })

    async function onSave() {
        if (!form.value.id) return

        isSaving.value = true
        try {
            await updateUser(form.value.id, {
            firstName: form.value.firstName,
            middleName: form.value.middleName,
            lastName: form.value.lastName,
            phone: form.value.contact,
            teamId: form.value.teamId,
            scheduleId: form.value.scheduleId
        })

        showToast.value = true

        emit('saved')

        setTimeout(() => {
            showToast.value = false
            emit('update:modelValue', false)
        }, 1500)

        } catch (err) {
            console.error("Save failed:", err)

            setTimeout(() => {
            showToast.value = false
            }, 3000)
        } finally {
            isSaving.value = false
        }
    }
</script>

<style scoped>
    .overlay {
        position: fixed;
        inset: 0;
        z-index: 9999;
        display: flex;
        justify-content: flex-end;
    }

    .overlay-bg {
        position: absolute;
        inset: 0;
        background: rgba(0, 19, 36, 0.45);
        backdrop-filter: blur(2px);
        z-index: 1;
    }

    .side-panel {
        position: relative;
        z-index: 2;
        width: 340px;
        height: 100dvh;
        display: flex;
        flex-direction: column;
        background: #001527;
        color: #D3D3D3;
        border-top-left-radius: 20px;
        border-bottom-left-radius: 20px;
        box-shadow: -4px 0 15px rgba(0,0,0,0.3);
        animation: slideProfile 0.35s ease;
        overflow-y: auto;
    }

    .side-panel::-webkit-scrollbar {
        width: 8px;
    }

    .side-panel::-webkit-scrollbar-thumb {
        background: rgba(255,255,255,0.1);
        border-radius: 10px;
        border: 2px solid transparent;
        background-clip: content-box;
    }

    .side-panel::-webkit-scrollbar-thumb:hover {
        background: rgba(255,255,255,0.25);
    }

    @keyframes slideProfile {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    .modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 20px;
    }

    .modal-header h2 {
        font-size: 18px;
        margin: 0;
    }

    .close-btn {
        all: unset;
        cursor: pointer;
        font-size: 18px;
        opacity: .8;
    }

    .close-btn:hover {
        opacity: .5;
    }

    .edit-profile {
        display: flex;
        align-items: center;
        gap: 18px;
        padding: 20px;
        border-top: 1px solid #ffffff1a;
        border-bottom: 1px solid #ffffff1a;
    }

    .edit-avatar {
        width: 70px;
        height: 70px;
        border-radius: 50%;
        object-fit: cover;
        border: 2px solid rgba(255,255,255,0.1);
    }

    .edit-name {
        font-size: 18px;
        color: white;
        font-family: 'Righteous', sans-serif;
    }

    .edit-sub {
        font-size: 11px;
        opacity: .7;
    }

    .section {
        padding: 20px;
    }

    .section-title {
        font-size: 14px;
        font-weight: 500;
        margin-bottom: 10px;
    }

    .form-label {
        font-size: 12px;
        font-weight: 500;
        margin: 5px 0;
    }

    .input-group input,
    .input,
    select {
        background: #00386780;
        padding: 12px;
        font-size: 12px;
        border-radius: 5px;
        width: 100%;
        margin-bottom: 8px;
        border: none;
        outline: none;
        box-shadow: none;
        color: white;
    }

    select option {
        background: #001527;
        color: white;
    }

    .modal-footer {
        margin-top: auto;
        padding: 20px;
        display: flex;
        justify-content: center;
        gap: 10px;
        border-top: 1px solid #ffffff1a;
    }

    .save {
        font-size: 13px;
        padding: 10px 16px;
        border-radius: 10px;
        background: #00386780;
        color: white;
        border: none;
        cursor: pointer;
    }

    .cancel {
        font-size: 13px;
        background: transparent;
        color: white;
        border: none;
        cursor: pointer;
    }

    .save:hover,
    .cancel:hover {
        opacity: .8;
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