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
                        <h3>{{ name }}</h3>
                        <p class="role">{{ role }}</p>
                    </div>
                </div>
            </div>
            <div class="profile-details">
                <div class="form">
                    <label>Login & Security</label>
                    <form>
                        <div class="input-group">
                            <div class="form-label">Email</div>
                            <input
                                type="email"
                                v-model="email"
                                :disabled="!isEditing"
                            />
                        </div>

                        <div class="input-group">
                            <div class="form-label">Phone Number</div>
                            <input
                                :type="tel"
                                v-model="password"
                                :disabled="!isEditing"
                            />
                        </div>

                        <div class="input-group">
                            <div class="form-label">Password</div>
                            <input
                                :type="password"
                                v-model="Password"
                                :disabled="!isEditing"
                            />
                        </div>
                    </form>
                </div>
                <div class="preferences">
                    <label>Preferences</label>
                    <div class="input-group">
                        <form>
                            <div class="form-label">Timezone</div>
                            <select v-model="timezone" class="form-select" >
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
                <button class="cancel-btn" v-if="isEditing" @click="isEditing = false">
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
</template>

<script setup>
    import { ref } from 'vue'
    import { X, Pencil, TextSelect } from 'lucide-vue-next';
    import { useRouter } from "vue-router"

    const router = useRouter()

    const email = ref("")
    const password = ref("")
    const phoneNumber = ref("")
    const isEditing = ref(false)
    const emit = defineEmits(['close'])
    
    const props = defineProps({
        isOpen: Boolean,
        name: String,
        email: String,
        role: String,
        avatar: String,
        isSidebarCollapsed: Boolean
    })

    const timezone = ref('Asia/Manila')

    const timezones = [
        'Asia/Manila',
        'UTC',
        'America/New_York'
    ]

    function saveChanges() {
        alert('Profile Saved successfully!')

        isEditing.value = false
    }

    function handleLogout () {
        if (confirm("Are you sure you want to logout?")) {
            router.push('/')
        }
    }
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
        justify-content: center;
        gap: 30px;
        margin: 30px;
    }

    .avatar {
        width: 85px;
        height: 85px;
        border-radius: 50%;
        object-fit: cover;
    }

    .text-info {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
    }

    h3 {
        font-size: 20px;
        font-family: righteous;
    }

    .text-info h3 {
        margin: 0;
        font-size: 20px;
        font-family: righteous;
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

    .save-btn:hover, .cancel-btn:hover, .logout-btn:hover {
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