import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
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
          meta: { title: 'Trang chủ' }
        },
        {
          path: 'product/:id',
          name: 'product-detail',
          component: ProductDetailView,
          meta: { title: 'Chi tiết sản phẩm' }
        },
        {
          path: 'cart',
          name: 'cart',
          component: CartView,
          meta: { title: 'Giỏ hàng' }
        },
        {
          path: 'checkout',
          name: 'checkout',
          component: CheckoutView,
          meta: { title: 'Thanh toán' }
        },
        {
          path: 'login',
          name: 'login',
          component: LoginView,
          meta: { title: 'Đăng nhập' }
        },
        {
          path: 'register',
          name: 'register',
          component: RegisterView,
          meta: { title: 'Đăng ký' }
        }, 
        {
          path: 'products',
          name: 'products',
          component: ProductView,
          meta: { title: 'Sản phẩm' }
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
          meta: { title: 'Thống kê' }
        },
        {
          path: 'products',
          name: 'admin-products',
          component: ProductManagementView,
          meta: { title: 'Quản lý sản phẩm' }
        },
        {
          path: 'products/new',
          name: 'admin-product-add',
          component: ProductFormView,
          meta: { title: 'Thêm sản phẩm' }
        },
        {
          path: 'products/:id/edit',
          name: 'admin-product-edit',
          component: ProductFormView,
          meta: { title: 'Sửa thông tin sản phẩm' }
        },
        {
          path: 'brands',
          name: 'admin-brands',
          component: BrandManagementView,
          meta: { title: 'Quản lý nhãn hàng' }
        },
        {
          path: 'categories',
          name: 'admin-categories',
          component: CategoryManagementView,
          meta: { title: 'Quản lý danh mục' }
        },
        {
          path: 'users',
          name: 'admin-users',
          component: UserManagementView,
          meta: { title: 'Quản lý người dùng' }
        },
        {
          path: 'orders',
          name: 'admin-orders',
          component: AdminOrderManagementView,
          meta: { title: 'Quản lý đơn hàng' }
        },
        {
        path: 'products/trash',
        name: 'ProductTrash',
        component: () => import('@/views/admin/ProductTrashView.vue'),
        meta: { title: 'Sản phẩm đã xóa' }
      }
      ]
    }
  ],
  scrollBehavior() {
    return { top: 0, behavior: 'smooth' }
  }
})

router.beforeEach((to, from, next) => {
  // Gọi store bên trong guard
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated();

  // Chặn user đã đăng nhập không cho vào Login/Register
  if ((to.name === 'login' || to.name === 'register') && isAuthenticated) {
    if (authStore.user?.role === 'Admin') {
      next({ name: 'admin-dashboard' });
    } else {
      next({ name: 'home' });
    }
  } else {
    next();
  }
});

export default router
