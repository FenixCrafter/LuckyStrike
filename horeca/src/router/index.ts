import HomeView from '@/views/HomeView.vue'
import OverviewView from '@/views/OverviewView.vue'
import ManagementView from '@/views/ManagementView.vue'
import ManagementOrderView from '@/views/ManagementOrderView.vue'
import { createRouter, createWebHistory } from 'vue-router'
import ProductenBeheerView from "@/views/ProductenBeheerView.vue";
import Invoice from "@/views/Invoice.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { 
      path: "", 
      name: "home", 
      component: HomeView
    },
    { 
      path: "/management",
      name: "management",
      component: ManagementView
    },
    {
      path: "/managementOrder/:id",
      name: "managementOrder",
      component: ManagementOrderView
    },
    {
      path: "/overview",
      name: "baanOverview", 
      component: OverviewView
    },
    { 
      path: "/:catchAll(.*)", 
      name: "notFound", 
      component: HomeView
    },
    {
      path: "/producten-beheer",
      name: "productenBeheer",
      component: ProductenBeheerView
    },
    {
      path: "/rekening/:id",
        name: "rekening",
        component: Invoice
    }
  ]
})

export default router
