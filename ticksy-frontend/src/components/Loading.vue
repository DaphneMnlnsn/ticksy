<template>
  <div class="loading-wrapper">
    <div class="spinner"></div>
    <span class="text">Loading...</span>
  </div>
</template>

<script setup>
defineProps({
  text: {
    type: String,
    default: 'Loading...'
  }
})
</script>

<style scoped>
    .loading-wrapper {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 10px;
        padding: 40px;
        color: rgba(255, 255, 255, 0.8);
    }

    .spinner {
        width: 35px;
        height: 35px;
        border: 4px solid rgba(255, 255, 255, 0.2);
        border-top: 4px solid #ffffff;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
    }

    .text {
        font-size: 14px;
    }

    @keyframes spin {
        to {
            transform: rotate(360deg);
        }
    }
</style>