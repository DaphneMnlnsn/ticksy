import { createRouter, createWebHistory } from 'vue-router'
import CreateAccount from '../Pages/CreateAccount.vue'
import Dashboard from '../Pages/Dashboard.vue'
import LoginPage from '../Pages/LoginPage.vue'
import ForgotPassword from '../Pages/ForgotPassword.vue'
import ReportsAnalytics from '../Pages/ReportsAnalytics.vue'
import Timesheets from '../Pages/Timesheets.vue'
import People from '../Pages/People.vue'
import WorkSchedules from '../Pages/WorkSchedules.vue'

const routes = [
    {
        path: '/create-account',
        component: CreateAccount
    },
    {
        path: '/dashboard',
        component: Dashboard
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
        path: '/reports-and-analytics', 
        component: ReportsAnalytics
    },
    {
        path: '/timesheets', 
        component: Timesheets
    },
    {
        path: '/management/people', 
        component: People
    },
    {
        path: '/management/work-schedules', 
        component: WorkSchedules
    },
    {
        path: '/',
        redirect: '/login'
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router