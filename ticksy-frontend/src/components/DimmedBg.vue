<template>
    <transition name="dimmed">
        <div v-if="show" class="backdrop-dimmer" @click="$emit('close')"></div>
    </transition>
</template>

<script setup>
    defineProps({
        show: Boolean
    });
    defineEmits(['close']);
</script>

<style scoped>
    .backdrop-dimmer {
        position: fixed;
        top: 0;
        left: 0;
        width: 100vw;
        height: 100vh;
        background-color: rgba(0, 10, 20, 0.4); 
        backdrop-filter: blur(2px);
        -webkit-backdrop-filter: blur(2px);
        z-index: 900;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
    }
</style>