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
        <form @submit.prevent="handleSubmit">

          <div class="input-group">
            <Mail class="icon" />
            <input
              type="email"
              v-model="email"
              placeholder="Enter your email"
              required
            />
          </div>

          <div class="input-group">
            <Lock class="icon" />
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              placeholder="Create your password"
              required
            />
            <component
              :is="showPassword ? EyeOff : Eye"
              class="eye"
              @click="togglePassword"
            />
          </div>

          <div class="input-group">
            <Lock class="icon" />
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="confirmPassword"
              placeholder="Confirm your password"
              required
            />
            <component
              :is="showPassword ? EyeOff : Eye"
              class="eye"
              @click="togglePassword"
            />
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
</template>

<script setup>
  import { ref } from "vue"
  import { Mail, Lock, Eye, EyeOff } from "lucide-vue-next"
  import { useRouter } from "vue-router"
  import logo from "../assets/ticksy_logo.png"

  const router = useRouter()
  const email = ref("")
  const password = ref("")
  const confirmPassword = ref("")
  const showPassword = ref(false)

function handleSubmit() {
  if (password.value !== confirmPassword.value) {
    alert("Passwords do not match")
    return
  }

  const userData = { email: email.value, password: password.value }
  console.log("User Data:", userData)
  alert("Account created! Check console for details.")
  router.push('/dashboard')
}

function togglePassword() {
  showPassword.value = !showPassword.value
}
</script>

<style scoped>
.page-wrapper {
  width: 100vw;
  min-width: 500px;
  height: 100vh;
  display: flex;
  justify-content: center;  
  align-items: center;      
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

.root {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  z-index: 1; 
  width: 500px;       
  max-width: 90%;   
  padding: 40px;       
  background-color: transparent; 
  border-radius: 15px; 
  
}

.header-section {
  text-align: center;
  margin-bottom: 30px;
  color: #fff;
}

.logo {
  display: block;
  width: 200px;
  height: auto;
  margin-right: 80px;
  margin-left: 105px;

}

.header-section h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
  margin-left: 5px;
  margin-right: 5px;
}

.header-section h5 {
  font-size: 1rem;
  margin: 2px 0;
  font-weight: lighter;
  margin-left: 20px; 
}


.input-group {
  width: 100%;
  display: flex;
  align-items: center;
  background-color: #C3CCD5;
  border-radius: 10px;
  padding: 12px;
  margin-bottom: 15px;
  position: relative; 
  margin-right: 100px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.4);
}

.input-group input {
  flex: 1;
  border: none;
  outline: none;
  background: transparent;
  font-size: 1rem;
  margin-left: 10px; 
  color:black;
}

.input{
  color:black;
}

.icon {
  width: 18px;
  height: 20px;
  color: #1f2937;
}

.eye {
  width: 20px;
  height: 20px;
  position: absolute;
  right: 12px;
  cursor: pointer;
  color: #1f2937;
}

button {
  width: 300px;
  display: block;
  margin: 25px auto 0 auto;
  margin-left: 25px;
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
  margin-top: 15px;
  text-align: center;
  font-size: 0.9rem;
  color: #fff;
  margin-left: 10px;
}

.login-link a {
  color: #fff;
  text-decoration: underline;
}

input[type="password"]::-ms-reveal,
input[type="password"]::-webkit-password-toggle-button {
  display: none;
}

</style>