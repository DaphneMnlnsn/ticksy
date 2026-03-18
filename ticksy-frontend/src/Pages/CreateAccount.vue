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

      <!-- Form container -->
      <div class="signup-container">
        <form @submit.prevent="handleSubmit">
          <input type="email" v-model="email" placeholder="Enter your email" required />
          <input type="password" v-model="password" placeholder="Create your password" required />
          <input type="password" v-model="confirmPassword" placeholder="Confirm your password" required />

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
  import logo from "../assets/ticksy_logo.png"
  import bgImage from "../assets/login_background.jpg"
  import { useRouter } from 'vue-router'

  const router = useRouter()
  const email = ref("")
  const password = ref("")
  const confirmPassword = ref("")

  function handleSubmit() {
    if (password.value !== confirmPassword.value) {
      alert("Passwords do not match")
      return
    }

    const userData = {
      email: email.value,
      password: password.value
    }

    console.log("User Data:", userData)
    alert("Account created! Check console for details.")
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
  margin: 0 auto 30px auto;
  width: 200px;
  height: auto;
}

.header-section h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
}

.header-section h5 {
  font-size: 1rem;
  margin: 2px 0;
  font-weight: lighter;
}

.form-group {
  width: 100%;
  display: flex;
  margin-bottom: 20px;
  max-width: 90%;
}

.root input {
  width: 90%;
  margin: 10px;
  padding: 15px;
  border: none;
  background-color: #C3CCD5;
  border-radius: 10px;
  outline: none;
  font-size: 1rem;
  color: black;
}

button {
  width: 250px;
  display: block;
  margin: 25px auto 0 auto;
  padding: 12px;
  background: #0C365C;
  border: none;
  border-radius: 12px;
  color: white;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
}

button:hover {
  background: #092845;
}

/* Login link */
.login-link {
  margin-top: 15px;
  text-align: center;
  font-size: 0.9rem;
}

.login-link a {
  color: #ffeb3b;
  text-decoration: underline;
}
</style>