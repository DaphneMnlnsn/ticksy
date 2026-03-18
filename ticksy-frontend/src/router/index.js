import { createRouter, createWebHistory } from 'vue-router'
import CreateAccount from '../Pages/CreateAccount.vue'
import Dashboard from '../Pages/Dashboard.vue'
import LoginPage from '../Pages/LoginPage.vue'

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
        path: '/',
        redirect: '/login'
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router