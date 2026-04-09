<template>
    <transition name="slide-profile">
        <div 
            v-if="isOpen" 
            class="profile-panel"
            :style="{ left: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <div class="profile-header">
                <h3 v-if="!isEditing">Profile</h3>
                <h3 v-else>Edit Profile</h3>
                <div class="profile-actions">
                    <button class="icon-btn" @click="isEditing = true" v-if="!isEditing"><Pencil /></button>
                    <button class="icon-btn" @click="emit('close')"><X /></button>
                </div>
            </div>
            <div class="profile-content">
                <div class="profile-info">
                    <img :src="avatar" class="avatar" />
                    <div class="text-info">
                        <h3 :title="name">{{ fullName }}</h3>
                        <p class="role">{{ role }}</p>
                    </div>
                </div>
            </div>
            <div class="profile-details">
                <div class="form">
                    <label>Account Information</label>
                    
                    <form>
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
                            <div class="form-label">Email</div>
                            <input type="email" v-model="form.email" :disabled="!isEditing" />
                        </div>
                        <div class="input-group">
                            <div class="form-label">Phone Number</div>
                            <input type="text" v-model="form.phone" :disabled="!isEditing" />
                        </div>
                        <div class="input-group">
                            <div class="form-label">Password</div>
                            <button class="pass-btn" type="button" @click="showChangePassModal = true" >Change Password</button>
                        </div>
                    </form>
                    <ChangePasswordModal
                        :show="showChangePassModal"
                        @close="showChangePassModal = false"
                    />
                </div>
                <div class="preferences">
                    <label>Preferences</label>
                    <div class="input-group">
                        <form>
                            <div class="form-label">Timezone</div>
                            <select v-model="form.timezone" class="form-select" :disabled="!isEditing" >
                                <option v-for="tz in timezones" :key="tz" :value="tz">
                                    {{ tz }}
                                </option>
                            </select>

                            <div class="radio-group">
                                <input type="checkbox" v-model="autoClockIn" /> 
                                <div class="form-label" >Clock in automatically upon login?</div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
            <div v-if="isEditing" class="save-footer">
                <button class="cancel-btn" v-if="isEditing" @click="cancelEdit">
                    Cancel
                </button>
                <button class="save-btn" @click="saveChanges">
                    Save Changes
                </button>
            </div>
            <div class="logout-footer">
                <button class="logout-btn" @click="handleLogout">Logout</button>
            </div>
        </div>
    </transition>

    <Teleport to="body">
        <DimmedBg :show="isOpen" @close="$emit('close')" />
    </Teleport>
</template>

<script setup>
    import { ref, watch, computed } from 'vue'
    import { X, Pencil, TextSelect } from 'lucide-vue-next'
    import { useRouter } from "vue-router"
    import DimmedBg from './DimmedBg.vue'
    import { getUserProfile, updateUserProfile } from '../services/userService'
    import ChangePasswordModal from './ChangePasswordModal.vue'

    const router = useRouter()

    const autoClockIn = ref(false)
    const passwordType = ref('password')
    const isEditing = ref(false)
    const emit = defineEmits(['close'])

    const props = defineProps({
        isOpen: Boolean,
        name: String,
        firstName: String,
        middleName: String,
        lastName: String,
        email: String,
        role: String,
        avatar: String,
        phone: String,
        timeZone: String,
        isSidebarCollapsed: Boolean
    })

    const timezones = Intl.supportedValuesOf('timeZone');

    const form = ref({
        name: '',
        firstName: '',
        middleName: '',
        lastName: '',
        email: '',
        phone: '',
        password: '',
        timezone: 'Asia/Manila'
    })
    
    const fullName = computed(() => {
        return `${form.value.firstName} ${form.value.middleName} ${form.value.lastName}`.trim()
    })

    const showChangePassModal = ref(false)

    async function saveChanges() {
        try {
            const userId = localStorage.getItem("userId");
            await updateUserProfile(userId, form.value);

            alert('Profile saved successfully!');
            isEditing.value = false;
            refreshProfile();
        } catch (err) {
            console.error(err);
            alert('Failed to save profile');
        }
    }

    async function refreshProfile() {
        try {
            const userId = localStorage.getItem("userId");
            const res = await getUserProfile(userId);

            form.value = {
                firstName: res.data.firstName,
                middleName: res.data.middleName,
                lastName: res.data.lastName,
                email: res.data.email,
                phone: res.data.phone,
                password: '',
                timezone: res.data.timeZone || 'Asia/Manila'
            };
        } catch (err) {
            console.error(err);
        }
    }

    function cancelEdit() {
        form.value = {
            firstName: props.firstName || '',
            middleName: props.middleName || '',
            lastName: props.lastName || '',
            email: props.email || '',
            phone: props.phone || '',
            password: '',
            timezone: props.timeZone || 'Asia/Manila'
        };
        isEditing.value = false;
    }

    function handleLogout () {
        if (confirm("Are you sure you want to logout?")) {
            localStorage.removeItem("token");
            localStorage.removeItem("userId");
            localStorage.removeItem("role");
            localStorage.clear();

            router.push('/login');
        }
    }

    watch(() => props.isOpen, (open) => {
        if (open) {
            form.value = {
                firstName: props.firstName || '',
                middleName: props.middleName || '',
                lastName: props.lastName || '',
                email: props.email || '',
                phone: props.phone || '',
                password: '',
                timezone: props.timeZone || 'Asia/Manila'
            }
        }
    }, { immediate: true })

    watch(() => props.show, (val) => {
        if (!val) {
            form.currentPassword = ''
            form.newPassword = ''
            form.confirmPassword = ''
        }
    })
</script>

<style scoped>
    .profile-panel {
        position: fixed;
        top: 0;
        left: var(--sidebar-width);
        width: 300px;
        height: 100dvh;
        display: flex;
        flex-direction: column;

        background: #001527;
        color: #D3D3D3;

        border-top-right-radius: 20px;
        border-bottom-right-radius: 20px;
        z-index: 999;
        box-shadow: 4px 0 15px rgba(0,0,0,0.1);
        transition: left 0.4s ease-in-out;

        z-index: 99999;
        overflow-y: auto;
    }

    .profile-panel::-webkit-scrollbar {
        width: 8px;
    }

    .profile-panel::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .profile-panel::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .profile-panel::-webkit-scrollbar-track {
        background: transparent; 
    }

    :host(.collapsed) .profile-panel {
        left: var(--sidebar-collapsed-width);
    }

    .slide-profile-enter-from {
        transform: translateX(-100%);
        opacity: 0;
    }
    .slide-profile-enter-to {
        transform: translateX(0);
        opacity: 1;
    }
    .slide-profile-enter-active {
        transition: all 0.3s ease;
    }
    .slide-profile-leave-from {
        transform: translateX(0);
        opacity: 1;
    }
    .slide-profile-leave-to {
        transform: translateX(-100%);
        opacity: 0;
    }
    .slide-profile-leave-active {
        transition: all 0.25s ease;
    }

    .profile-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: 0 20px;
    }

    .profile-actions {
        display: flex;
    }

    .profile-actions .icon-btn {
        background: none;
        border: none;
        cursor: pointer;
        margin: 5px;
        padding: 0;
        outline: none;
        box-shadow: none;
        color: #F0F0F0;
        width: 20px;
        height: 20px;
    }

    .icon-btn:hover {
        opacity: 0.6;
    }

    .profile-content {
        display: flex;
        flex-direction: column;
        gap: 10px;
        border-bottom: 1px solid #d3d3d33c;
        border-top: 1px solid #d3d3d33c;
    }

    .profile-info {
        display: flex;
        align-items: center; 
        gap: 20px; 
        margin: 20px; 
        min-width: 0;
    }

    .text-info {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: flex-start;
        min-width: 0;
        flex: 1;
    }

    .text-info h3 {
        margin: 0;
        font-size: 18px;
        font-family: 'Righteous', sans-serif;
        color: white;

        width: 100%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .avatar {
        width: 70px;
        height: 70px;
        border-radius: 50%;
        object-fit: cover;
        flex-shrink: 0;
        border: 2px solid rgba(255, 255, 255, 0.1);
    }

    .role {
        font-size: 11px;
        opacity: 0.7;
        margin: 0;
    }

    .email {
        font-size: 14px;
        opacity: 0.8;
    }

    .profile-details {
        padding: 20px;
        flex: 1;
    }

    label {
        font-size: 14px;
        font-weight: 500;
    }

    form {
        margin: 20px 5px 5px 5px;
    }

    .form-label {
        font-size: 12px;
        font-weight: 500;
        margin: 5px 0;
    }

    .input-group input, .form-select {
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

    .radio-group {
        display: flex;
        align-items: center;
        gap: 10px;
        margin: 10px 0;
    }

    .radio-group input {
        width: 5%;
        accent-color: #003867;
    }

    .save-footer {
        margin-bottom: 10px;
        padding: 0 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
    }

    .save-btn {
        font-size: 13px;
        width: auto;
        padding: 10px;
        border-radius: 10px;
        background: #00386780;
        color: white;
        border: none;
        cursor: pointer;

        border: none;
        outline: none;
        box-shadow: none;
    }

    .cancel-btn {
        font-size: 13px;
        width: auto;
        padding: 10px;
        background-color: transparent;
        color: white;
        border: none;
        cursor: pointer;

        border: none;
        outline: none;
        box-shadow: none;
    }

    .pass-btn {
        font-size: 12px;
        width: 100%;
        padding: 10px;
        border-radius: 5px;
        background: #00386780;
        color: white;
        border: none;
        cursor: pointer;

        border: none;
        outline: none;
        box-shadow: none;
    }

    .save-btn:hover, .cancel-btn:hover, .logout-btn:hover, .pass-btn:hover {
        opacity: 0.8;
    }

    .logout-btn {
        margin-top: auto;
        border-top-left-radius: 20px;
        border-top-right-radius: 20px;
        width: 100%;
        background-color: #00386780;
        color: #EB3223;
        font-size: 14px;
        font-weight: 600;
        padding: 10px;

        -webkit-tap-highlight-color: transparent;
        border: none;
        outline: none;
        box-shadow: none;
    }

</style>