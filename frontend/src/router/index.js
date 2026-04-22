import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

import ClientLayout from '@/layouts/Client/ClientLayout.vue'
import AdminLayout from '@/layouts/Admin/AdminLayout.vue'

import HomeView from '@/views/client/HomeView.vue'
import LoginView from '@/views/client/LoginView.vue'
import RegisterView from '@/views/client/RegisterView.vue'
import ProductDetailView from '@/views/client/ProductDetailView.vue'
import CartView from '@/views/client/CartView.vue'
import CheckoutView from '@/views/client/CheckoutView.vue'
import ProductView from '@/views/client/ProductView.vue'
import OrderHistoryView from '@/views/client/OrderHistoryView.vue'
import Profile from '@/views/client/Profile.vue'

import DashboardView from '@/views/admin/DashboardView.vue'
import ProductManagementView from '@/views/admin/ProductManagementView.vue'
import ProductFormView from '@/views/admin/ProductFormView.vue'
import UserManagementView from '@/views/admin/UserManagementView.vue'
import BrandManagementView from '@/views/admin/BrandManagementView.vue'
import CategoryManagementView from '@/views/admin/CategoryManagementView.vue'
import AdminOrderManagementView from '@/views/admin/AdminOrderManagementView.vue'
import UserTrashView from '@/views/admin/UserTrashView.vue'

// Router
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: ClientLayout,
      children: [
        { path: '', name: 'home', component: HomeView },

        { path: 'product/:id', name: 'product-detail', component: ProductDetailView },

        {
          path: 'cart',
          name: 'cart',
          component: CartView,
          meta: { requiresAuth: true, roles: ['Admin', 'Customer'] }
        },

        {
          path: 'checkout',
          name: 'checkout',
          component: CheckoutView,
          meta: { requiresAuth: true, roles: ['Admin', 'Customer'] }
        },

        { path: 'products', name: 'products', component: ProductView },

        { path: 'login', name: 'login', component: LoginView },

        { path: 'register', name: 'register', component: RegisterView },

        { path: 'my/orders', name: 'my/orders', component: OrderHistoryView },
        { path: '/profile', name: '/profile', component: Profile}
      ]
    },

    {
      path: '/admin',
      component: AdminLayout,
      meta: { requiresAuth: true, role: 'Admin' }, 
      children: [
        { path: '', name: 'admin-dashboard', component: DashboardView },

        { path: 'products', name: 'admin-products', component: ProductManagementView },

        { path: 'products/new', name: 'admin-product-add', component: ProductFormView },

        { path: 'products/:id/edit', name: 'admin-product-edit', component: ProductFormView },

        { path: 'brands', name: 'admin-brands', component: BrandManagementView },

        { path: 'categories', name: 'admin-categories', component: CategoryManagementView },

        { path: 'users', name: 'admin-users', component: UserManagementView },

        { path: 'orders', name: 'admin-orders', component: AdminOrderManagementView },

        { path: 'users/trash', name: 'trash', component: UserTrashView },

        {
          path: 'products/trash',
          name: 'product-trash',
          component: () => import('@/views/admin/ProductTrashView.vue')
        }
      ]
    }
  ],
  scrollBehavior() {
    return { top: 0, behavior: 'smooth' }
  }
})

router.beforeEach((to, from) => {
  const authStore = useAuthStore()

  const isLoggedIn = authStore.isAuthenticated()
  const role = authStore.user?.role

  //chưa login mà vào trang cần auth
  if (to.meta.requiresAuth && !isLoggedIn) {
    return '/login'
  }

  //đã login mà vào login/register
  if ((to.name === 'login' || to.name === 'register') && isLoggedIn) {
    if (role === 'Admin') return '/admin'
    return '/'
  }

  //chỉ 1 role
  if (to.meta.role && role !== to.meta.role) {
    return '/'
  }

  //nhiều role
  if (to.meta.roles && !to.meta.roles.includes(role)) {
    return '/'
  }
  return true
})

export default router
