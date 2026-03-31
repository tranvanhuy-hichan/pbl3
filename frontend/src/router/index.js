import { createRouter, createWebHistory } from 'vue-router'
import ClientLayout from '../layouts/Client/ClientLayout.vue'
import AdminLayout from '../layouts/Admin/AdminLayout.vue'

import HomeView from '../views/client/HomeView.vue'
import LoginView from '../views/client/LoginView.vue'
import RegisterView from '../views/client/RegisterView.vue'
import ProductDetailView from '../views/client/ProductDetailView.vue'
import CartView from '../views/client/CartView.vue'
import CheckoutView from '../views/client/CheckoutView.vue'
import ProductView from '../views/client/ProductView.vue'

import DashboardView from '../views/admin/DashboardView.vue'
import ProductManagementView from '../views/admin/ProductManagementView.vue'
import ProductFormView from '../views/admin/ProductFormView.vue'
import UserManagementView from '../views/admin/UserManagementView.vue'
import BrandManagementView from '../views/admin/BrandManagementView.vue'
import CategoryManagementView from '../views/admin/CategoryManagementView.vue'
import AdminOrderManagementView from '../views/admin/AdminOrderManagementView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: ClientLayout,
      children: [
        {
          path: '',
          name: 'home',
          component: HomeView,
          meta: { title: 'Home' }
        },
        {
          path: 'product/:id',
          name: 'product-detail',
          component: ProductDetailView,
          meta: { title: 'Product Details' }
        },
        {
          path: 'cart',
          name: 'cart',
          component: CartView,
          meta: { title: 'Shopping Cart' }
        },
        {
          path: 'checkout',
          name: 'checkout',
          component: CheckoutView,
          meta: { title: 'Checkout' }
        },
        {
          path: 'login',
          name: 'login',
          component: LoginView,
          meta: { title: 'Login' }
        },
        {
          path: 'register',
          name: 'register',
          component: RegisterView,
          meta: { title: 'Register' }
        }, 
        {
          path: 'products',
          name: 'products',
          component: ProductView,
          meta: { title: 'Products' }
        }
    
      ]
    },
    {
      path: '/admin',
      component: AdminLayout,
      children: [
        {
          path: '',
          name: 'admin-dashboard',
          component: DashboardView,
          meta: { title: 'Dashboard' }
        },
        {
          path: 'products',
          name: 'admin-products',
          component: ProductManagementView,
          meta: { title: 'Product Management' }
        },
        {
          path: 'products/new',
          name: 'admin-product-add',
          component: ProductFormView,
          meta: { title: 'Add Product' }
        },
        {
          path: 'products/:id/edit',
          name: 'admin-product-edit',
          component: ProductFormView,
          meta: { title: 'Edit Product' }
        },
        {
          path: 'brands',
          name: 'admin-brands',
          component: BrandManagementView,
          meta: { title: 'Brand Management' }
        },
        {
          path: 'categories',
          name: 'admin-categories',
          component: CategoryManagementView,
          meta: { title: 'Category Management' }
        },
        {
          path: 'users',
          name: 'admin-users',
          component: UserManagementView,
          meta: { title: 'User Management' }
        },
        {
          path: 'orders',
          name: 'admin-orders',
          component: AdminOrderManagementView,
          meta: { title: 'Order Management' }
        },
        {
        path: '/admin/products/trash',
        name: 'ProductTrash',
        component: () => import('@/views/admin/ProductTrashView.vue')
      }
      ]
    }
  ],
  scrollBehavior() {
    return { top: 0, behavior: 'smooth' }
  }
})

export default router
