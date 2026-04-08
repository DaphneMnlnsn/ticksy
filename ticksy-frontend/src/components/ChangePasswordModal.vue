<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay">
            <div class="modal">

            <div class="modal-header">
                <h2>Change Password</h2>
            </div>

            <div class="section">
                <div class="input-group-pass">
                    <div class="form-label">Current Password</div>
                    <input type="password" v-model="form.currentPassword" />
                </div>

                <div class="input-group-pass">
                    <div class="form-label">New Password</div>
                    <input type="password" v-model="form.newPassword" />
                </div>

                <div class="input-group-pass">
                    <div class="form-label">Confirm Password</div>
                    <input type="password" v-model="form.confirmPassword" />
                </div>
            </div>

            <div class="modal-footer">
                <button class="cancel-pass-btn" @click="$emit('close')">Cancel</button>
                <button class="update-pass-btn" @click="handleSubmit">Update Password</button>
            </div>

            </div>
        </div>
    </Teleport>
    <Toast
                v-if="toastMessage"
                :message="toastMessage"
                :type="toastType"
                @finished="toastMessage = ''"
            />
</template>

<script setup>
    import { reactive, ref } from 'vue'
    import { changePassword } from '../services/userService'
    import Toast from '../components/Toast.vue'

    const toastMessage = ref('')
    const toastType = ref('success')
    const props = defineProps({
        show: Boolean
    })

    const emit = defineEmits(['close'])

    const form = reactive({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
    })

    const closeModal = () => {
        form.currentPassword = ''
        form.newPassword = ''
        form.confirmPassword = ''
        emit('close')
    }

    const handleSubmit = async () => {
        if (!form.currentPassword || !form.newPassword || !form.confirmPassword) {
            toastMessage.value = "Please fill in all fields"
            toastType.value = "error"
            return
        }

        if (form.newPassword !== form.confirmPassword) {
            toastMessage.value = "Passwords do not match"
            toastType.value = "error"
            return
        }

        try {
            toastMessage.value = "Password updated successfully"
            toastType.value = "success"

            setTimeout(() => closeModal(), 50)

            await changePassword({
                currentPassword: form.currentPassword,
                newPassword: form.newPassword
            })
        } catch (err) {
            console.error(err)
            toastMessage.value = err.response?.data?.message || "Failed to update password"
            toastType.value = "error"
        }
    }
</script>

<style scoped>
    .modal-overlay {
        position: fixed;
        inset: 0;
        background-color: rgba(0, 19, 36, 0.4);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 99999; 
    }

    .modal {
        width: 400px;
        background: #031e2f;
        border-radius: 12px;
        padding: 20px;
        color: white;
    }

    .modal-footer {
        display: flex;
        justify-content: flex-end;
        gap: 10px;
        margin-top: 20px;
    }

    .input-group-pass {
        margin-top: 15px;
    }

    ::v-deep(.input-group-pass input) {
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

    .form-label {
        font-size: 14px;
        margin-bottom: 5px;
    }

    .cancel-pass-btn, .update-pass-btn {
        background-color: #00386780;
        color: #F0F0F0;
        font-size: 13px;
        padding: 10px;

        border: none;
        outline: none;
    }

    .cancel-pass-btn {
        background-color: transparent;
    }

    .cancel-pass-btn:hover, .update-pass-btn:hover {
        opacity: 0.8;
    }
</style>