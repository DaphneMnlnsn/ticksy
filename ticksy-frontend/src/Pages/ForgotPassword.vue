<template>
  <div class="page-wrapper">
    <div class="page-bg"></div>

    <div class="root">
      
      <div class="header-section">
        <img :src="logo" alt="Logo" class="logo" />
        <h1>Forgot Password</h1>
        <h5>Enter your email to reset your password.</h5>
      </div>

      <div class="login-container">
        <form @submit.prevent="handleReset">

          <div class="input-group">
            <Mail class="icon" />
            <input
              type="email"
              v-model="email"
              placeholder="Enter your email"
              required
            />
          </div>

          <button type="submit">Send Reset Link</button>

          <div class="links">
            <router-link to="/login">Back to Login</router-link>
          </div>

        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from "vue"
  import { Mail } from "lucide-vue-next"
  import { useRouter } from "vue-router"
  import logo from "../assets/ticksy_logo.png"

  const router = useRouter()
  const email = ref("")

function handleReset() {
  console.log("Reset link sent to:", email.value)
  alert("Reset link sent to your email!")
  router.push('/login')
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
  margin-right: 55px;
}

.header-section h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
  margin-left: 30px;
}

.header-section h5 {
  font-size: 1rem;
  margin: 2px 0;
  font-weight: lighter;
  margin-left: 30px;
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
  color: black;
}

.input{
  color:black;
}
.icon {
  width: 18px;
  color: #1f2937;
}

.links {
  margin-top: 10px;
}

.links a {
  color: #fff;
  font-size: 0.9rem;
  text-decoration: underline;
  margin-left: 55px;
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
</style>