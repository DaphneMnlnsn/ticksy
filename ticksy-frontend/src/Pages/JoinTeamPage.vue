<template>
    <div class="join-page">
        <div class="join-card">
            <h2>Join Team</h2>

            <p v-if="status === 'loading'">Loading invite...</p>

            <div v-else-if="status === 'ready'" class="ready">
                <p>You’ve been invited to join:</p>
                <h3>{{ teamName }}</h3>

                <button @click="joinTeam">Join Team</button>
            </div>

            <div v-else-if="status === 'error'" class="error">
                <CircleXIcon :size="40" />
                <p>{{ errorMessage }}</p>
                <button @click="goHome">Go Back</button>
            </div>
        </div>
    </div>
    <Toast 
		:message="toastMessage" 
		:type="toastType" 
		:icon="toastIcon"
		@finished="toastMessage = ''"
	/>
</template>

<script setup>
    import { onMounted, ref, nextTick } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import { CheckCircle, CircleXIcon } from 'lucide-vue-next'
    import { acceptTeamInvite, previewTeamInvite } from '../services/people'
    import Toast from '../components/Toast.vue'

    const route = useRoute()
    const router = useRouter()

    const toastMessage = ref('')
    const toastType = ref('success')
    const toastIcon = ref(null)

    const status = ref('loading')
    const errorMessage = ref('')
    const teamName = ref('')
    const token = route.params.token

    onMounted(async () => {
        if (!token) {
            status.value = 'error'
            errorMessage.value = 'Invalid invite link.'
            return
        }

        try {
            const data = await previewTeamInvite(token)
            teamName.value = data.teamName

            const authToken = localStorage.getItem('token')

            if (!authToken) {
                router.push(`/login?invite=${token}`)
                return
            }

            status.value = 'ready'
        } catch (err) {
            status.value = 'error'
            errorMessage.value = 'Invalid or expired invite.'
        }
    })

    async function joinTeam() {
        status.value = 'loading'

        try {
            await acceptTeamInvite(token)

            showToast("Join successful!", "success", CheckCircle)

            status.value = 'success'

            setTimeout(() => {
                router.push('/dashboard')
            }, 1500)

        } catch (err) {
            errorMessage.value = err.response?.data || 'Failed to join team.'
            status.value = 'error'
        }
    }

    function goToDashboard() {
        router.push('/dashboard')
    }

    function goHome() {
        router.push('/')
    }

    function showToast(message, type = 'success', icon = null) {
        toastType.value = type
        toastIcon.value = icon
        toastMessage.value = '' 
        nextTick(() => {
            toastMessage.value = message
        })
    }
</script>

<style scoped>
.join-page {
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background: linear-gradient(135deg, #071a2f, #0d2c4a);
    color: #fff;
}

.join-card {
    background: #001527;
    padding: 30px;
    border-radius: 14px;
    width: 360px;
    text-align: center;
    backdrop-filter: blur(10px);
}

.join-card h2 {
    margin-bottom: 15px;
}

.success {
    color: #22c55e;
}

.error {
    color: #ef4444;
}

button {
    margin-top: 15px;
    padding: 10px 16px;
    border: none;
    border-radius: 8px;
    background: #4da3ff;
    color: white;
    cursor: pointer;
}
</style>