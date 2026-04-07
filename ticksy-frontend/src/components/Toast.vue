<template>
    <transition name="fade">
        <div v-if="show" :class="['base-toast', type]">
        <component v-if="icon" :is="icon" :size="18" />
        <span>{{ message }}</span>
        </div>
    </transition>
</template>

<script setup>
    import { ref, watch } from 'vue';

    const props = defineProps({
        message: { type: String, required: true },
        type: { type: String, default: 'success' },
        duration: { type: Number, default: 2000 },
        icon: { type: [Object, Function], default: null }
    });
    const emit = defineEmits(['finished']);
    const show = ref(false);

    watch(
        () => props.message,
        (newVal) => {
            if (newVal) {
                show.value = true;
                setTimeout(() => {
                    show.value = false;
                    emit('finished');
                }, props.duration);
            }
        },
        { immediate: true }
    );
</script>

<style scoped>
    .base-toast {
        position: fixed;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        z-index: 2000;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
    }

    .base-toast.success {
        background-color: #06d6a0;
        color: #001324;
    }

    .base-toast.error {
        background-color: #ef476f;
        color: white;
    }

    .base-toast.info {
        background-color: #ffd166;
        color: #001324;
    }

    .fade-enter-active, .fade-leave-active {
        
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }
</style>