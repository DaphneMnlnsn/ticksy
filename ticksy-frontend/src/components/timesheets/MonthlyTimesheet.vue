<template>
    <div class="table">
        <div class="table-header">
            <SearchBar v-model="search" />

            <div class="days monthly">
                <span v-for="(day, i) in daysInMonth" :key="i">
                    {{ i + 1 }}
                </span>
                <span>Total</span>
            </div>
        </div>

        <div v-if="loading">Loading...</div>
        <div v-else-if="!hasData" class="empty-state">
            <Calendar class="empty-icon" />
            <p>No attendance records found for this month</p>
            <span>Try selecting a different month</span>
        </div>

        <div class="row" v-for="user in filteredUsers" :key="user.userId">
            <div class="user">
                <img :src="user.avatar || defaultAvatar" class="avatar" />
                <span>{{ user.name }}</span>
            </div>

            <div class="days monthly">
                <span
                    v-for="(day, index) in 31"
                    :key="index"
                    class="box"
                    :class="getBoxClass(user.days?.[index])"
                >
                    <template v-if="user.days?.[index] === 'REST'">
                        <TreePalm class="rest-icon" />
                    </template>

                    <template v-else-if="user.days?.[index] === '0h'">
                    </template>

                    <template v-else>
                        {{ user.days?.[index] || '' }}
                    </template>
                </span>

                <span>{{ user.totalHours }}</span>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, computed } from 'vue'
    import SearchBar from '../../components/Search.vue'
    import { Calendar, TreePalm } from 'lucide-vue-next'

    const props = defineProps({
        users: Array,
        search: String,
        loading: Boolean,
        hasData: Boolean,
        defaultAvatar: String
    })

    const search = ref('')
    const daysInMonth = Array.from({ length: 31 })

    const filteredUsers = computed(() => {
        if (!props.search) return props.users

        return props.users.filter(u =>
            u.name.toLowerCase().includes(props.search.toLowerCase())
        )
    })

    function getBoxClass(value) {
        if (!value) return ''

        if (value === 'REST') return 'rest-day'
        if (value === '0h') return 'zero-day'

        const hours = parseInt(value)

        if (hours >= 8) return 'full-day'
        if (hours >= 4) return 'half-day'
        if (hours > 0) return 'partial-day'

        return ''
    }
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
        color: #D9D9D9;
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
        font-size: 13px;
    }

    .days.monthly {
        grid-template-columns: repeat(32, 1fr);
    }

    .box {
        width: 20px;
        height: 20px;
        border-radius: 1px;
        margin: auto;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 12px;
    }

    .full-day {
        background-color: #00DC28;
        color: white;
    }

    .half-day {
        background-color: #ffc107;
        color: black;
    }

    .partial-day {
        background-color: #64b5f6;
        color: white;
    }

    .zero-day {
        background-color: #D9D9D9;
        color: #666;
    }

    .rest-day {
        background: transparent;
        font-size: 14px;
    }

    .rest-icon {
        width: 19px;
        height: 19px;
        color: #D9D9D9;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>