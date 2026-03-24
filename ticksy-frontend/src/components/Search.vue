<template>
    <div class="search-container">
        <Search class="search-icon"/>
        <span>Search...</span>
    </div>
</template>

<script setup>
    import { Search } from 'lucide-vue-next';


</script>

<style scoped>
    .search-container {
        display: flex;
        border-radius: 10px;
        border: 2px solid #75889A80;
        width: 100%;
        padding: 5px 15px;
        gap: 10px;
        align-items: center;
    }

    .search-icon {
        width: 16px;
        height: 16px;
    }

    span {
        font-size: 15px;
        color: #F0F0F0AD;
    }
</style>