<template>
    <div class="row-item">
        <div class="text">
            <span class="name">{{ item.name }}</span>
            <span v-if="item.label" class="sub">{{ item.label }}</span>
        </div>
        <component :is="icon" size="18" class="icon" />
    </div>
</template>

<script setup>
    import { ChevronsRight } from 'lucide-vue-next'
    
    defineProps({
        item: {
            type: Object,
            required: true
        },
        icon: {
            type: [Object, Function], 
            default: ChevronsRight
        }
    })
</script>

<style scoped>
    .row-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 14px 16px;
        cursor: pointer;
    }

    .text {
        display: flex;
        flex-direction: column;
    }

    .name {
        font-size: 14px;
        font-weight: 500;
    }

    .sub {
        font-size: 12px;
        opacity: 0.6;
    }

    .icon {
        font-size: 16px;
    }

    .icon:hover {
        opacity: 0.8;
    }
</style>