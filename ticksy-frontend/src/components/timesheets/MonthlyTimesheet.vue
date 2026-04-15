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

        <div v-if="!hasData" class="empty-state">
            <Calendar class="empty-icon" />
            <p>No attendance records found for this month</p>
            <span>Try selecting a different month</span>
        </div>

        <div class="rows-wrapper">
            <TransitionGroup name="row-fade" tag="div">
                <div class="row" v-for="user in filteredUsers" :key="user.userId" :style="{ '--i': i }" >
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
                            @mouseenter="showTooltip($event, user.days?.[index])"
                            @mousemove="moveTooltip"
                            @mouseleave="hideTooltip"
                        >
                            <template v-if="user.days?.[index] === 'REST'">
                                <TreePalm class="rest-icon" />
                            </template>

                            <template v-else-if="user.days?.[index] === '0h'">
                            </template>
                        </span>

                        <span>{{ user.totalHours }}</span>
                    </div>
                </div>
            </TransitionGroup>
        </div>
    </div>
    <div
        v-if="tooltip.show"
        class="tooltip"
        :style="{
            left: tooltip.x + 'px',
            top: tooltip.y + 'px',
            transform: 'translate(-50%, -100%)'
        }"
    >
        {{ tooltip.text }}
    </div>
</template>

<script setup>
    import { ref, computed } from 'vue'
    import SearchBar from '../../components/Search.vue'
    import { Calendar, TreePalm } from 'lucide-vue-next'
    import Loading from '../Loading.vue'

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

    const tooltip = ref({
        show: false,
        text: '',
        x: 0,
        y: 0
    })

    function getBoxClass(value) {
        if (!value) return ''

        if (value === 'REST') return 'rest-day'
        if (value === '0h') return 'zero-day'

        const hours = parseFloat(value)

        if (hours === 0) return 'zero-day'
        if (hours >= 1 && hours <= 5) return 'low-hours'
        if (hours >= 6 && hours <= 8) return 'full-day'
        if (hours >= 9) return 'overtime'

        return ''
    }

    function showTooltip(event, value) {
        tooltip.value.show = true
        updateTooltipPosition(event)
        tooltip.value.text = getTooltip(value)
    }

    function moveTooltip(event) {
        updateTooltipPosition(event)
    }

    function updateTooltipPosition(event) {
        const offset = 10 

        tooltip.value.x = event.clientX
        tooltip.value.y = event.clientY - offset 
    }

    function hideTooltip() {
        tooltip.value.show = false
    }

    function getTooltip(value) {
        if (!value) return 'No record'

        if (value === 'REST') return 'Rest Day'
        if (value === '0h') return '0 hours (No work)'

        const hours = parseFloat(value)

        if (hours >= 1 && hours <= 5) return `${hours} hours (Underworked)`
        if (hours >= 6 && hours <= 8) return `${hours} hours (Full Day)`
        if (hours >= 9) return `${hours} hours (Overtime)`

        return `${value} hours`
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

    .zero-day {
        background-color: #D9D9D9;
        color: #666;
    }

    .low-hours {
        background-color: #FFC300;
        color: black;
    }

    .full-day {
        background-color: #00DC28;
        color: white;
    }

    .overtime {
        background-color: #E53935; 
        color: white;
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

    .tooltip {
        pointer-events: none;
        position: fixed;
        background: rgba(0, 21, 39, 0.95);
        color: white;
        padding: 6px 10px;
        border-radius: 6px;
        font-size: 12px;
        white-space: nowrap;
        box-shadow: 0 6px 18px rgba(0,0,0,0.3);
        backdrop-filter: blur(6px);
        z-index: 9999;
        transition: transform 0.05s ease;
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