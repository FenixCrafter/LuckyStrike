import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import ReservationDashboard from "@/views/ReserveringDashboard.vue";
import RegisterEmployeeView from '../views/RegisterEmployeeView.vue';
import ReserveringDetailView from "@/views/ReserveringDetailView.vue";
import Home from "@/views/Home.vue";
import Invoice from "@/views/Invoice.vue";
import EditReserveringView from "@/views/EditReserveringView.vue";
import BezettingsDashboard from "@/views/BezettingsDashboard.vue";
import ConfirmView from "@/views/ConfirmView.vue";
import PasswordResetView from '@/views/PasswordResetView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/registreer',
      name: 'register',
      component: LoginView
    },
    {
      path: '/reserveringen',
      name: 'reserveringen',
      component: ReservationDashboard
    },
    {
      path: '/reservering/:id',
      name: 'reservering',
      component: ReserveringDetailView
    },
    {
      path:'/reservering/:id/factuur',
      name: 'factuur',
      component: Invoice
    },
    {
      path:'/reservering/:id/bewerk',
      name: 'editReservering',
      component: EditReserveringView
    },
    {
      path: '/registerEmployee',
      name: "registerEmployee",
      component: RegisterEmployeeView
    },
    {
      path: '/bezettingsDashboard',
        name: 'bezetting',
        component: BezettingsDashboard
    },
    {
      path: '/confirm/:token/:id',
        name: 'confirm',
        component: ConfirmView
    },
    {
      path: '/passwordReset/:token',
        name: 'passwordReset',
        component: PasswordResetView
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'notFound',
      component: Home
    },
  ]
})

export default router
