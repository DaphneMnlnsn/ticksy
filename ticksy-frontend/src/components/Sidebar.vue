<template>
    <div :class="['sidebar', { 'sidebar-collapsed': !props.isOpen }]">

        <div class="sidebar-top">
            <button class="toggle-btn" @click="$emit('toggle')">
                <ArrowLeftFromLine v-if="props.isOpen" />
                <ArrowRightFromLine v-else />
            </button>

            <img :src="logo" alt="Logo" class="logo" />
        </div>

        <nav class="sidebar-nav">
            <ul>
                <li v-for="item in menuItems" :key="item.name">

                    <div 
                        v-if="item.children" 
                        class="menu-link" 
                        @click="toggleDropdown(item)"
                    >
                        <component 
                            :is="CircleChevronDown"
                            class="icon arrow"
                            :class="{ 'rotated': manuallyOpened.includes(item.name) }"
                        />
                        <component :is="item.icon" class="icon" />
                        <span :class="{ 'text-hidden': !props.isOpen }">
                            {{ item.name }}
                        </span>
                    </div>

                    <ul 
                        v-if="item.children && props.isOpen"
                        :class="['submenu', { open: manuallyOpened.includes(item.name) }]"
                    >
                        <li v-for="child in item.children" :key="child.name">
                            <router-link :to="child.path" class="submenu-link" :class="{ active: isActive(child.path) }">
                            <component :is="child.icon" class="icon submenu-icon" />
                            <span :class="{ 'text-hidden': !props.isOpen }">{{ child.name }}</span>
                            </router-link>
                        </li>
                    </ul>

                    <router-link 
                        v-else-if="!item.children" 
                        :to="item.path" 
                        class="menu-link"
                        :class="{ active: isActive(item.path) }"
                    >
                        <component :is="item.icon" class="icon" />
                        <span :class="{ 'text-hidden': !props.isOpen }">
                            {{ item.name }}
                        </span>
                    </router-link>

                </li>
            </ul>
        </nav>

        <div class="sidebar-footer">
            <div class="menu-link profile-link" @click="openProfileModal">
                <img :src="user.avatar || sampleIMG" class="avatar" @error="handleImageError" />

                <span :class="{ 'text-hidden': !props.isOpen }">
                    {{ user.firstName }}
                </span>

                <component 
                    :is="ChevronRight" 
                    class="icon chevron"
                    :class="{ 'hidden': !props.isOpen }"
                />
            </div>
        </div>

        <Teleport to="body">
            <ProfilePanel
                :isOpen="isProfileOpen"
                @close="isProfileOpen = false"
                :name="user.name"
                :firstName="user.firstName"
                :middleName="user.middleName"
                :lastName="user.lastName"
                :email="user.email"
                :role="user.role"
                :avatar="user.avatar || sampleIMG"
                :phone="user.phone"
                :timeZone="user.timezone"
                :isSidebarCollapsed="!props.isOpen"
            />
        </Teleport>
    </div>
</template>

<script setup>
    import { onMounted, ref, defineProps, watchEffect } from 'vue'
    import logo from "../assets/ticksy_logo_white.png"
    import sampleIMG from '../assets/sample_img.jpg'
    import { LayoutDashboard, ClipboardClock, FileChartLine, 
        CircleChevronDown, Users, SquareChartGantt, 
        ChevronRight, ArrowRightFromLine, ArrowLeftFromLine 
    } from 'lucide-vue-next';
    import { useRoute } from 'vue-router'
    import ProfilePanel from './ProfilePanel.vue'
    import { getUserProfile } from '../services/userService'

    const route = useRoute()
    const manuallyOpened = ref([])
    const isProfileOpen = ref(false)

    const props = defineProps({
        menuItems: {
            type: Array,
            default: () => [
                { name: 'Dashboard', path: '/dashboard', icon: LayoutDashboard },
                { name: 'Timesheets', path: '/timesheets', icon: ClipboardClock },
                { name: 'Reports and Analytics', path: '/reports-and-analytics', icon: FileChartLine },
                { name: 'Management',
                    children: [
                        {name: 'People', icon: Users, path: '/management/people' },
                        {name: 'Work Schedules', icon: SquareChartGantt, path: '/management/work-schedules' }
                    ]    
                }
            ]
        },
        isOpen: {
            type: Boolean,
            default: true
        }
    })

    const user = ref({
        name: '',
        email: '',
        role: '',
        avatar: ''
    })

    function toggleDropdown(item) {
        const index = manuallyOpened.value.indexOf(item.name)
        if (index === -1) manuallyOpened.value.push(item.name)
        else manuallyOpened.value.splice(index, 1)
    }

    function isDropdownOpen(item) {
        return props.isOpen || manuallyOpened.value.includes(item.name)
    }

    function openProfileModal() {
        isProfileOpen.value = !isProfileOpen.value
    }

    function isActive(path) {
        return route.path === path
    }

    function handleImageError(e) {
        e.target.src = sampleIMG
    }

    onMounted(async () => {
        try {
            const userId = localStorage.getItem('userId')

            const res = await getUserProfile(userId)
            const data = res.data

            user.value = {
                firstName: data.firstName,
                middleName: data.middleName,
                lastName: data.lastName,
                name: [data.firstName, data.middleName, data.lastName].filter(Boolean).join(' '),
                email: data.email,
                role: data.role,
                avatar: data.avatarUrl,
                phone: data.phone,           
                timezone: data.timeZone
            }

        } catch (err) {
            console.error('Failed to fetch user:', err)
        }
    })

    onMounted(() => {
        props.menuItems.forEach(item => {
            if (item.children) {
                const childMatch = item.children.some(child => child.path === route.path)
                if (childMatch && !manuallyOpened.value.includes(item.name)) {
                    manuallyOpened.value.push(item.name)
                }
            }
        })
    })

</script>

<style scoped>
    .sidebar {
        width: var(--sidebar-width);
        background: linear-gradient(
            to bottom,
            #001B31 47%,
            #003867 93%
        );
        color: #F0F0F0;
        transition: width 0.4s ease-in-out, padding 0.4s ease-in-out;
        padding: 16px;
        height: 100dvh;
        min-height: 100dvh;

        position: fixed;
        top: 0;
        left: 0;
        border-top-right-radius: 20px;
        border-bottom-right-radius: 20px;
        display: flex;
        flex-direction: column;
    }

    .sidebar-collapsed {
        width: var(--sidebar-collapsed-width);
        padding: 1rem 0.5rem;
    }

    .sidebar-top {
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .sidebar-nav {
        flex: 1;
        min-height: 0;
    }

    .sidebar-collapsed .menu-link {
        justify-content: center;
        gap: 0;
    }

    .sidebar:not(.sidebar-collapsed) .menu-link {
        justify-content: flex-start; 
    }

    .toggle-btn {
        position: absolute;
        top: 0.5rem;
        right: 0.3rem;
        background: none;
        border: none;
        outline: none;
        cursor: pointer;
        color: white;
        padding: 0.2rem 0.5rem;
        border-radius: 4px;
        transition: all 0.3s;
    }

    .toggle-btn svg {
        width: 20px;
        height: 20px;
    }

    .toggle-btn:hover {
        opacity: 0.8;
    }

    .logo {
        width: 90px;
        height: 90px;
        display: block;
        margin: 40px auto 100px auto;
        transition: all 0.3s ease-in-out;
    }

    .sidebar-nav {
        flex-grow: 1;
        min-height: 0;
    }

    .sidebar-footer {
        width: 100%; 
        padding: 20px 0;
        flex-shrink: 0;
    }

    .sidebar-collapsed .sidebar-footer {
        display: flex;
        justify-content: center;
    }

    .sidebar-collapsed .profile-link {
        width: auto;
        justify-content: center;
    }
    
    nav ul {
        list-style: none;
        padding: 0 10px;
    }

    nav li {
        margin: 8px 0;
        padding: 0;
        list-style: none;
        position: relative;
    }

    nav a {
        color: white;
        text-decoration: none;
    }

    nav a:hover {
        opacity: 0.8;
    }

    span {
        font-size: 14px;
        transition: opacity 0.2s ease-in-out, transform 0.2s ease-in-out, width 0.3s ease-in-out;
        white-space: nowrap;
    }

    .text-hidden {
        opacity: 0;
        transform: translateX(-10px);
        max-width: 0;
        overflow: hidden;
        white-space: nowrap;
        transition:
            opacity 0.2s ease 0.2s,
            transform 0.2s ease 0.2s;
    }

    .menu-link {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 10px 12px; 
        background-color: transparent;
        transition: background-color 0.2s ease;
        margin: 0 !important; 
        width: 100%;
        box-sizing: border-box;
    }

    .menu-link:hover {
        opacity: 0.8;
    }

    .menu-link span {
        display: inline-block;
        width: 250px;
        overflow: hidden;
        white-space: nowrap;

        transition:
            max-width 0.3s ease,
            opacity 0.3s ease,
            transform 0.3s ease;
    }

    .sidebar-collapsed .menu-link span {
        max-width: 0;
        opacity: 0;
        transform: translateX(-10px);
    }

    .icon {
        width: 23px;
        height: 23px;
        min-width: 23px;
        min-height: 23px; 
        flex-shrink: 0;
    }

    .submenu {
        list-style: none;
        padding-left: 40px;
        max-height: 0;
        opacity: 0;
        overflow: hidden;
        transform: translateY(-5px);
        transition: 
            max-height 0.3s ease,
            opacity 0.2s ease 0.15s,
            transform 0.2s ease 0.15s;
    }

    .submenu.open {
        max-height: 120px;
        opacity: 1;
        transform: translateY(0);
    }

    .submenu li {
        margin: 4px 0;
    }

    .submenu-link span {
        white-space: normal;  
        overflow: visible;   
        word-break: break-word;   
    }

    .submenu-link {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 10px 12px;
        border-radius: 12px;
        background-color: transparent; 
        transition: background-color 0.2s ease; 
        margin: 0 !important; 
        width: 100%;
        box-sizing: border-box;
    }

    .submenu-link:hover {
        opacity: 0.8;
    }

    .arrow {
        transition: transform 0.3s ease;
    }

    .rotated {
        transform: rotate(180deg);
    }

    .submenu-link {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .profile-link {
        display: flex;
        align-items: center;
        gap: 15px;
        width: 100%;
        border-top: 2px solid white;
        padding-top: 25px;
    }

    .profile-link span {
        font-weight: 600; 
    }

    .avatar {
        width: 35px;
        height: 35px;
        border-radius: 50%;
        object-fit: cover;
        flex-shrink: 0;
    }

    .chevron {
        margin-left: auto;
        transition: opacity 0.3s;
    }

    .hidden {
        display: none;
    }
    
    .menu-link.active,
    .submenu-link.active {
        background-color: #083A73;
        border-radius: 15px;
        padding: 10px 7px;
        margin: 5px 0;
        opacity: 1;
    }
</style>