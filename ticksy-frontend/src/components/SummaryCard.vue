<template>
    <div class="summary-card">
        <div class="card">
            <div class="card-header">
                <span>{{ title }}</span>
            </div>

            <div class="card-content">
                <div 
                    v-for="(item, index) in items" 
                    :key="index" 
                    class="summary-item"
                >
                    <div class="item-text">
                        <div class="item-label"><span>{{ item.label }}</span></div>
                        <div class="item-value"><span>{{ item.value }}</span></div>
                    </div>

                    <component 
                        v-if="item.icon" 
                        :is="item.icon" 
                        class="icon"
                    />

                    <div 
                        v-if="index !== items.length - 1" 
                        class="divider"
                    ></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    defineProps({
        title: String,
        items: Array
    })
</script>

<style scoped>
    .summary-card {
        margin-bottom: 15px;
        flex: 1;
    }

    .card {
        background-color: #75889A4D;
        border-radius: 10px;
        padding: 15px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
    }
    
    .card-content {
        display: flex;
        flex-direction: row;
        align-items: center;
    }

    .summary-item {
        display: flex;
        align-items: center;
        flex: 1; 
        justify-content: space-between;
        gap: 10px;
        padding: 10px 10px;
        max-height: 60px;
    }

    .summary-item:not(:last-child) {
        border-right: 2px solid rgba(255, 255, 255, 0.3);
    }

    .card-header {
        margin-bottom: 20px;
        font-size: 14px;
        font-weight: 500;
    }

    .item-text {
        font-size: 13px;
        padding: 10px 10px 10px 0;
        margin: 10px 30px 10px 0;
        white-space: nowrap;
    }

    .item-value {
        font-size: 22px;
        font-weight: 600;
    }

    .icon {
        width: 25px;
        height: 25px;
        color: #F0F0F0;
    }
</style>