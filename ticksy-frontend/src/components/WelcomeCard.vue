<script setup>
const props = defineProps({
    holidays: Array,
    welcomeImg: String
})
</script>

<template>
    <div class="welcome-card">
        <div class="text">
            <h2>Hello IDA Admin</h2>
            <p>Here's your dashboard for today.</p>
        </div>
        <div class="welcome-img">
            <img :src="welcomeImg" alt="Welcome Image" />
        </div>
    </div>
</template>

<style scoped>
.welcome-card {
    position: relative;
    background-color: #003867;
    height: 250px;
    border-radius: 10px;
    padding: 20px;
    color: white;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
}

.text {
    display: flex;
    flex-direction: column;
    gap: 8px; 
}

.text h2 {
    font-size: 35px;
    font-weight: 600;
    margin: 0; 
}

.text p {
    font-size: 14px;
    font-weight: lighter;
    margin: 0;
}

.welcome-img {
    flex-shrink: 0;
    display: flex;
    align-items: flex-end;
    justify-content: flex-end;
}

.welcome-img img {
    margin: 55px 0 0 25px ;
    padding: 15px;
    width: 180px;
    height: auto;
    object-fit: contain;
}
</style>