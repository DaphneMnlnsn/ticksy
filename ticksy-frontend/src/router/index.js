import { createRouter, createWebHistory } from 'vue-router'
import CreateAccount from '../Pages/CreateAccount.vue'
import Dashboard from '../Pages/Dashboard.vue'
import LoginPage from '../Pages/LoginPage.vue'
import ForgotPassword from '../Pages/ForgotPassword.vue'
import ReportsAnalytics from '../Pages/ReportsAnalytics.vue'
import Timesheets from '../Pages/Timesheets.vue'
import People from '../Pages/People.vue'
import WorkSchedules from '../Pages/WorkSchedules.vue'
import ResetPassword from '../Pages/ResetPassword.vue'
import { isAuthenticated } from '../services/auth'
import JoinTeamPage from '../Pages/JoinTeamPage.vue'

const routes = [
    {
        path: '/create-account',
        component: CreateAccount
    },
    {
        path: '/login',
        component: LoginPage
    },
    {
        path: '/forgot-password', 
        component: ForgotPassword
    },
    {
        path: "/reset-password",
        component: ResetPassword
    },

    {
        path: '/dashboard',
        component: Dashboard,
        meta: { requiresAuth: true, roles: ['Admin', 'User'] }
    },
    {
        path: '/timesheets', 
        component: Timesheets,
        meta: { requiresAuth: true, roles: ['Admin', 'User'] }
    },
    {
        path: '/management/work-schedules', 
        component: WorkSchedules,
        meta: { requiresAuth: true, roles: ['Admin', 'User'] }
    },
    {
        path: '/join/:token',
        component: JoinTeamPage
    },

    // Admin-only !!!
    {
        path: '/reports-and-analytics', 
        component: ReportsAnalytics,
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/management/people', 
        component: People,
        meta: { requiresAuth: true, roles: ['Admin'] }
    },

    // Redirect
    {
        path: '/',
        redirect: '/login'
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to) => {
    const userRole = localStorage.getItem('role') || '';

    if (to.meta.requiresAuth && !isAuthenticated()) {
        return '/login';
    }

    if ((to.path === '/login' || to.path === '/create-account') && isAuthenticated()) {
        return '/dashboard';
    }

    if (to.meta.roles && !to.meta.roles.includes(userRole)) {
        return '/dashboard';
    }

    return true;
});

export default router