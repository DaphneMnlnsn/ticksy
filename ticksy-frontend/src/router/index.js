import { createRouter, createWebHistory } from 'vue-router'
import CreateAccount from '../Pages/CreateAccount.vue'
import Dashboard from '../Pages/Dashboard.vue'

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
        path: '/',
        redirect: '/create-account'
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router