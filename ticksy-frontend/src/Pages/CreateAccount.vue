<template>
	<div class="page-wrapper">
		<div class="page-bg"></div>
		<div class="root">

		<div class="header-section">
			<img :src="logo" alt="Logo" class="logo" />
			<h1>Create an Account</h1>
			<h5>Sign up to track time with ease.</h5>
			<h5>By IDA</h5>
		</div>

		<div class="signup-container">
			<div class="avatar-selection-section">
				<p class="upload-label">Choose your Avatar</p>
				<div class="avatar-grid">
					<div 
						v-for="seed in robotSeeds" 
						:key="seed"
						class="avatar-item"
						:class="{ active: selectedAvatar === seed }"
						@click="selectAvatar(seed)"
					>
						<img :src="`https://api.dicebear.com/9.x/${selectedStyle}/svg?seed=${seed}&options[backgroundType]=none&options[scale]=120`" alt="avatar" />
					</div>
				</div>
			</div>
			<form @submit.prevent="handleSubmit">
				<div class="input-row name-group">
					<div class="input-group">
						<input type="text" v-model="firstName" placeholder="First Name" required />
					</div>
					<div class="input-group">
						<input type="text" v-model="middleName" placeholder="M.I." />
					</div>
					<div class="input-group">
						<input type="text" v-model="lastName" placeholder="Last Name" required />
					</div>
				</div>

				<div class="input-group">
					<Mail class="icon" />
					<input type="email" v-model="email" placeholder="Email Address" required />
				</div>
				<div class="input-group">
					<Phone class="icon" /> <input type="tel" v-model="phone" placeholder="Phone Number" required />
				</div>

				<div class="input-group">
					<Lock class="icon" />
					<input :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="Password" required />
				</div>
				<div class="input-group">
					<Lock class="icon" />
					<input :type="showPassword ? 'text' : 'password'" v-model="confirmPassword" placeholder="Confirm Password" required />
				</div>

				<div class="input-group full-span">
					<MapPin class="icon" /> <input type="text" v-model="address" placeholder="Full Street Address" required />
				</div>

				<button type="submit">Create Account</button>
				<p class="login-link">
					Already have an account?
					<router-link to="/login">Login</router-link>
				</p>

			</form>
		</div>
		</div>
	</div>
	<Toast 
		v-if="toastMessage" 
		:message="toastMessage" 
		:type="toastType" 
		:icon="toastIcon"
		@finished="toastMessage = ''"
	/>
</template>

<script setup>
	import { ref, onMounted, nextTick } from "vue"
	import { Mail, Lock, Eye, EyeOff, Camera, Phone, MapPin, CheckCircle } from "lucide-vue-next"
	import { useRouter } from "vue-router"
	import logo from "../assets/ticksy_logo.png"
	import { registerUser } from "../services/userService"
	import Toast from "../components/Toast.vue"
	import { acceptTeamInvite } from "../services/people"

	const firstName = ref("")
	const middleName = ref("")
	const lastName = ref("")
	const phone = ref("")
	const address = ref("")
	const router = useRouter()
	const email = ref("")
	const password = ref("")
	const confirmPassword = ref("")
	const showPassword = ref(false)
	const avatarFile = ref(null)

	const selectedStyle = 'bottts-neutral'; 
    const robotSeeds = ['Amaya', 'Charlie', 'Eden', 'Grey', 'Harlow', 'Nova', 'Orion', 'Astra', 'Draco']; 
    const selectedAvatar = ref(robotSeeds[0]);
    const avatarPreview = ref("");

	const toastMessage = ref('')
	const toastType = ref('success')
	const toastIcon = ref(null)
	const emit = defineEmits(['finished']);

	function showToast(message, type = 'success', icon = null) {
		toastMessage.value = '' 
		nextTick(() => {      
			toastMessage.value = message
			toastType.value = type
			toastIcon.value = icon
		})
	}

    function selectAvatar(seed) {
        selectedAvatar.value = seed;
        avatarPreview.value = `https://api.dicebear.com/9.x/${selectedStyle}/svg?seed=${seed}`;
    }

    onMounted(() => {
        selectAvatar(robotSeeds[0]);
    });

	function togglePassword() {
		showPassword.value = !showPassword.value
	}

	function handleFileChange(event) {
		const file = event.target.files[0]
		if (file) {
			avatarFile.value = file
			avatarPreview.value = URL.createObjectURL(file)
		}
	}

	async function handleSubmit() {
		if (password.value !== confirmPassword.value) {
			alert("Passwords do not match!")
			return
		}

		const userData = {
			firstName: firstName.value,
			middleName: middleName.value,
			lastName: lastName.value,
			email: email.value,
			password: password.value,
			phone: phone.value,
			address: address.value,
			avatarUrl: avatarPreview.value,
			timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
			autoClockIn: true
		};

		try {
			await registerUser(userData)

			showToast("Account created successfully!", "success", CheckCircle)

			const inviteToken = localStorage.getItem("pendingInvite")

			if (inviteToken) {
				try {
					await acceptTeamInvite(inviteToken)
					localStorage.removeItem("pendingInvite")
				} catch (err) {
					console.error("Auto join failed:", err)
				}
			}

			setTimeout(() => {
				router.push("/dashboard")
			}, 800)

		} catch (err) {
			showToast("Registration failed", "error")
		}
	}
</script>

<style scoped>
	.page-wrapper {
		width: 100vw;
		min-height: 100vh;
		display: flex;
		justify-content: center;
		align-items: center;
		padding: 20px; 
		box-sizing: border-box;
		overflow-y: auto;
	}

	.root {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center; 
		text-align: center;
		z-index: 1; 
		width: 700px; 
		max-width: 95%;   
		padding: 30px; 
		border-radius: 15px;
	}

	.page-bg {
		position: absolute;
		top: 0;
		left: 0;
		width: 100%;
		height: 100%;
		background-image: url("../assets/login_background.jpg");
		background-size: cover;
		background-position: center;
		background-repeat: no-repeat;
		z-index: -1;
	}

	.header-section {
		text-align: center;
		margin-bottom: 15px;
		color: #fff;
	}

	.logo {
		display: block;
		margin: 0 auto 15px auto; 
		width: 130px;
		height: auto;
	}

	.header-section h1 {
		font-size: 30px;
		margin-bottom: 10px;
	}

	.header-section h5 {
		font-size: 14px;
		margin: 2px 0;
		font-weight: lighter;
	}

	.signup-container {
		width: 100%;
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	form {
		display: grid;
		grid-template-columns: 1fr 1fr; 
		gap: 15px;
		width: 100%;
		max-width: 600px;
		margin: 0 auto;
	}

	.name-group, .full-span {
		grid-column: span 2;
	}

	.name-group {
		display: grid;
		grid-template-columns: 2fr 1fr 2fr;
		gap: 15px;
		grid-column: span 2;
		width: 100%;
		box-sizing: border-box;
	}

	.input-group {
		background: rgba(255, 255, 255, 0.9); 
		border-radius: 8px;
		padding: 10px 15px;
		display: flex;
		align-items: center;
		transition: box-shadow 0.2s ease;
		min-width: 0;
	}

	.input-group:focus-within {
		box-shadow: 0 0 0 2px #0C365C;
		background: #fff;
	}

	.input-group input {
		flex: 1;
		border: none;
		outline: none;
		background: transparent;
		font-size: 14px;
		margin-left: 10px; 
		margin-right: 10px; 
		color: black;
		min-width: 0;
	}

	.icon {
		width: 18px;
		height: 20px;
		color: #1f2937;
		flex-shrink: 0;
	}

	.eye {
		width: 20px;
		height: 20px;
		cursor: pointer;
		color: #1f2937;
		flex-shrink: 0;
	}

	button {
		width: 300px;
		display: block;
		margin: 15px auto 0 auto; 
		padding: 12px;
		background: #0C365C;
		border: none;
		border-radius: 12px;
		color: #fff;
		font-weight: bold;
		cursor: pointer;
		transition: all 0.3s ease;
		box-shadow: 0 5px 20px rgba(0, 0, 0, 0.4);
	}

	button:hover {
		opacity: 0.8;
	}

	.login-link {
		margin-top: 2px;
		text-align: center;
		font-size: 0.9rem;
		color: #fff;
	}

	.login-link a {
		color: #fff;
		text-decoration: underline;
	}

	input:-webkit-autofill,
	input:-webkit-autofill:hover, 
	input:-webkit-autofill:focus, 
	input:-webkit-autofill:active {
		-webkit-text-fill-color: black !important;
		font-size: 1rem !important; 
		font-family: inherit !important;
		transition: background-color 5000s ease-in-out 0s;
	}

	input[type="password"]::-ms-reveal,
	input[type="password"]::-webkit-password-toggle-button {
		display: none;
	}

	button, .login-link {
		grid-column: span 2; 
		justify-self: center; 
	}

	.upload-label {
		font-size: 13px;
		font-style: italic;
		text-decoration: underline;
	}

	.avatar-grid {
		display: flex;
		gap: 12px;
		justify-content: center;
		margin-top: 7px;
		flex-wrap: wrap;
		margin-bottom: 15px;
	}

	.avatar-item {
		width: 60px;
		height: 60px;
		border-radius: 50%;
		background: rgba(255, 255, 255, 0.2);
		cursor: pointer;
		transition: all 0.3s ease;
		border: 2px solid transparent;
		padding: 5px;
		display: flex;
		align-items: center;
		justify-content: center;
		overflow: hidden;
	}

	.avatar-item img {
		width: 125%;
		height: 125%;
		object-fit: cover;
	}

	.avatar-item:hover {
		transform: translateY(-3px);
		background: rgba(255, 255, 255, 0.4);
	}

	.avatar-item.active {
		border-color: #00d2ff;
		background: white;
		box-shadow: 0 0 15px rgba(0, 210, 255, 0.5);
	}

	@media (max-width: 600px) {
		form {
			grid-template-columns: 1fr;
		}
		button, 
		.login-link {
			grid-column: span 1;
		}
	}
</style>