<template>
    <div class="table">
        <div class="table-header">
            <SearchBar v-model="search" />
            <div class="days">
                <span>M</span>
                <span>T</span>
                <span>W</span>
                <span>TH</span>
                <span>F</span>
                <span>S</span>
                <span>S</span>
                <span>Total Hours</span>
            </div>
        </div>

        <div v-if="loading" class="empty-state">
            Loading timesheets...
        </div>

        <div v-else-if="!hasData" class="empty-state">
            <Calendar class="empty-icon" />
            <p>No attendance records found for this date range</p>
            <span>Try selecting another week</span>
        </div>

        <div v-else>
            <div class="row" v-for="user in filteredUsers" :key="user.name">
                <div class="user">
                    <img :src="user.avatar || defaultAvatar" class="avatar" />
                    <span>{{ user.name }}</span>
                </div>

                <div class="days">
                    <span v-for="(day, index) in user.days" :key="index">
                        <span v-if="day === 'rest'" class="rest">Rest day</span>
                        <span v-else>{{ day }}</span>
                    </span>
                    <span>{{ user.total }}</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, computed } from 'vue'
    import SearchBar from '../../components/Search.vue'
    import { Calendar } from 'lucide-vue-next';

    const props = defineProps({
        users: Array,
        loading: Boolean,
        hasData: Boolean,
        defaultAvatar: String
    })

    const search = ref('')

    const filteredUsers = computed(() => {
        if (!props.users) return []

        return props.users.filter(u =>
            u.name.toLowerCase().includes(search.value.toLowerCase())
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

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>