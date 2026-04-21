<template>
    <div class="table">
        <div class="table-header">
            <SearchBar v-model="search" />

            <div class="days daily">
                <span>First In</span>
                <span>Last Out</span>
                <span>Total Hours</span>
            </div>
        </div>

        <div v-if="!hasData" class="empty-state">
            <Calendar class="empty-icon" />
            <p>No attendance records found for this date</p>
            <span>Try selecting another date</span>
        </div>

        <div v-else class="rows-wrapper">
            <TransitionGroup name="row-fade" tag="div">
                <div class="row" v-for="(user, i) in filteredUsers" :key="user.userId" :style="{ '--i': i }" >
                    <div class="user">
                        <img :src="user.avatar || defaultAvatar" class="avatar" />
                        <span>{{ user.name }}</span>
                    </div>

                    <div class="days daily">
                        <span>{{ user.firstIn }}</span>
                        <span>{{ user.lastOut }}</span>
                        <span>{{ user.totalHours }}</span>
                    </div>
                </div>
            </TransitionGroup>
        </div>
    </div>
</template>

<script setup>
    import { ref, computed } from 'vue'
    import SearchBar from '../../components/Search.vue'
    import { Calendar } from 'lucide-vue-next'

    const props = defineProps({
        users: Array,
        search: String,
        loading: Boolean,
        hasData: Boolean,
        defaultAvatar: String
    })

    const search = ref('')

    const filteredUsers = computed(() => {
        if (!props.search) return props.users

        return props.users.filter(user =>
            user.name.toLowerCase().includes(props.search.toLowerCase())
        )
    })
</script>

<style scoped>
    .table {
        backdrop-filter: blur(6px);
        border-radius: 5px;
        display: flex;
        flex-direction: column;
        width: 100%;
        animation: fadeIn 0.4s ease-in-out;
    }

    .table-header {
        display: grid;
        grid-template-columns: 250px 1fr;
        margin-bottom: 15px;
    }

    .days {
        display: grid;
        grid-template-columns: repeat(8, 1fr);
        text-align: center;
        gap: 5px;
    }

    .row {
        display: grid;
        grid-template-columns: 250px 1fr;
        align-items: center;
        padding: 12px 0;
        border-top: 1px solid rgba(255,255,255,0.1);
    }

    .user {
        display: flex;
        align-items: center;
        gap: 15px;
    }

    .avatar {
        width: 35px;
        height: 35px;
        border-radius: 50%;
    }

    .rest {
        background: #16a34a;
        padding: 4px 10px;
        border-radius: 999px;
        font-size: 12px;
    }

    .empty-state {
        text-align: center;
        padding: 40px;
        color: #F0F0F0AD;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 10px;
    }

    .empty-icon {
        width: 40px;
        height: 40px;
    }

    span {
        font-size: 14px;
    }

    .days.daily {
        grid-template-columns: repeat(3, 1fr);
    }

    .rows-wrapper {
        display: flex;
        flex-direction: column;
    }

    .row-fade-enter-active,
    .row-fade-leave-active {
        transition: all 0.2s ease;
    }

    .row-fade-enter-from {
        opacity: 0;
        transform: translateY(8px);
    }

    .row-fade-leave-to {
        opacity: 0;
        transform: translateY(-8px);
    }

    .row-fade-enter-active {
        transition-delay: calc(var(--i) * 40ms);
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>