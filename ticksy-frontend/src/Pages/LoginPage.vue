<template>
  <div class="page-wrapper">
    <div class="page-bg"></div>

    <div class="root">
  
      <div class="header-section">
        <img :src="logo" alt="Logo" class="logo" />
        <h1>Welcome Back!</h1>
        <h5>Log in to track time with ease.</h5>
      </div>

      <div class="login-container">
        <form @submit.prevent="handleLogin">
          
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
              placeholder="Enter your password"
              required
            />
            <component
              :is="showPassword ? EyeOff : Eye"
              class="eye"
              @click="togglePassword"
            />
          </div>

          <div class="links">
            <router-link to="/create-account">Create Account</router-link>
            <router-link to="/forgot-password">Forgot Password</router-link>
          </div>

          <button type="submit">Log In</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from "vue"
  import { Mail, Lock, Eye, EyeOff } from "lucide-vue-next"
  import logo from "../assets/ticksy_logo.png"
  import { useRouter } from 'vue-router'

  const router = useRouter()
  const email = ref("")
  const password = ref("")
  const showPassword = ref(false)

  function togglePassword() {
    showPassword.value = !showPassword.value
  }

  function handleLogin() {
    console.log("Login:", {
      email: email.value,
      password: password.value,
    })

    alert("Welcome User!")
    router.push('/dashboard')
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
  width: 700px;       
  max-width: 90%; 
  padding: 40px;
  border-radius: 15px;
}

.header-section {
  text-align: center;
  margin-bottom: 30px;
  color: #fff;
}

.logo {
  display: block;
  margin: 0 auto 30px auto;
  width: 200px;
  height: auto;
  margin-right: 25px;
 
}

.header-section h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
  margin-left: 20px;
}

.header-section h5 {
  font-size: 1rem;
  margin: 2px 0;
  font-weight: lighter;
  margin-left: 25px;
}

.input-group {
  width: 100%;
  display: flex;
  align-items: center;
  background-color: #C3CCD5;
  border-radius: 10px;
  padding: 12px;
  margin-bottom: 15px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.4);
}

.input-group input {
  border: none;
  outline: none;
  flex: 1;
  background: transparent;
  font-size: 1rem;
  margin-left: 10px;
  margin-right: 100px;
  color:black;
}

.input{
  color:black;
}

.icon {
  width: 18px;
  color: #1f2937;
}

.eye {
  width: 18px;
  height: 20px;
  color: #1f2937;
}

.links {
  display: flex;
  justify-content: space-between;
  width: 105%;
  margin: 5px ;
  font-size: 0.9rem;
  color: #fff;
}

.links a {
  color: #fff;
  font-weight: 400;
  text-decoration: underline;
}

button {
  width: 300px;
  display: block;
  margin: 25px auto 0 auto;
  margin-left: 45px;
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
  background: rgb(55, 146, 237);
}

input[type="password"]::-ms-reveal,
input[type="password"]::-webkit-password-toggle-button {
  display: none;
}

</style>