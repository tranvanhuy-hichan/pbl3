```vue
<template>
  <header class="bg-blue-50 border-b border-blue-200 py-4 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 flex justify-between items-center gap-4 lg:gap-8 flex-wrap lg:flex-nowrap">
      
      <!-- Logo -->
      <div class="text-2xl font-extrabold text-blue-700 tracking-tight">
        <router-link to="/">Cửa hàng điện tử</router-link>
      </div>

      <!-- Menu -->
      <nav class="hidden md:flex gap-6">
        <router-link 
          to="/"
          :class="[
            'font-medium transition-colors',
            $route.path === '/'
              ? 'text-blue-800 font-bold underline'
              : 'text-blue-600 hover:text-blue-800'
          ]"
        >
          Trang chủ
        </router-link>

        <router-link 
          to="/products"
          :class="[
            'font-medium transition-colors',
            $route.path.startsWith('/products')
              ? 'text-blue-800 font-bold underline'
              : 'text-blue-600 hover:text-blue-800'
          ]"
        >
          Sản phẩm
        </router-link>

        <router-link 
          to="/categories"
          :class="[
            'font-medium transition-colors',
            $route.path.startsWith('/categories')
              ? 'text-blue-800 font-bold underline'
              : 'text-blue-600 hover:text-blue-800'
          ]"
        >
          Danh mục
        </router-link>

        <router-link 
          to="/brands"
          :class="[
            'font-medium transition-colors',
            $route.path.startsWith('/brands')
              ? 'text-blue-800 font-bold underline'
              : 'text-blue-600 hover:text-blue-800'
          ]"
        >
          Nhãn hàng
        </router-link>

        <router-link
          v-if="authStore.user?.role === 'Customer'"
          to="/my/orders"
          :class="[
            'font-medium transition-colors',
            $route.path.startsWith('/my/orders')
              ? 'text-blue-800 font-bold underline'
              : 'text-blue-600 hover:text-blue-800'
          ]"
        >
          Đơn hàng
        </router-link>
      </nav>

      <!-- Cart & Auth -->
      <div class="flex items-center gap-4">

        <!-- Cart -->
        <router-link 
          v-if="authStore.user?.role === 'Customer'"
          to="/cart" class="relative p-2 text-blue-700 hover:text-blue-900 transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>

          <span class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold text-white transform translate-x-1/2 -translate-y-1/2 bg-blue-600 rounded-full border-2 border-white">
            2
          </span>
        </router-link>

        <!-- User Info -->
        <div v-if="authStore.isAuthenticated()" class="flex items-center gap-3">
          <div class="hidden sm:flex flex-col items-end">
            <span class="text-sm font-bold text-slate-800">
              {{ authStore.user?.fullName || 'User' }}
            </span>
            <span class="text-xs text-brand-600 font-medium">
              {{ authStore.user?.role }}
            </span>
          </div>

          <router-link 
            v-if="authStore.user?.role === 'Admin'"
            to="/admin"
            title="Trang quản trị"
            class="w-9 h-9 flex items-center justify-center 
                  text-indigo-500 hover:text-white 
                  hover:bg-indigo-500 rounded-full transition">

            <svg xmlns="http://www.w3.org/2000/svg" 
                fill="none" 
                viewBox="0 0 24 24" 
                stroke-width="2" 
                stroke="currentColor" 
                class="w-5 h-5">
              <path stroke-linecap="round" stroke-linejoin="round" 
                d="M3 3h7v7H3V3zm11 0h7v4h-7V3zM14 10h7v11h-7V10zM3 14h7v7H3v-7z" />
            </svg>
          </router-link>

          <!-- 🚪 LOGOUT ICON -->
          <button 
            @click="handleLogout" 
            title="Đăng xuất"
            class="w-9 h-9 flex items-center justify-center 
                  text-red-500 hover:text-white 
                  hover:bg-red-500 rounded-full transition">

            <svg xmlns="http://www.w3.org/2000/svg" 
                fill="none" 
                viewBox="0 0 24 24" 
                stroke-width="2" 
                stroke="currentColor" 
                class="w-5 h-5">
              <path stroke-linecap="round" stroke-linejoin="round" 
                d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-7.5A2.25 2.25 0 003.75 5.25v13.5A2.25 2.25 0 006 21h7.5a2.25 2.25 0 002.25-2.25V15" />
              <path stroke-linecap="round" stroke-linejoin="round" 
                d="M18 12h-9m0 0l3-3m-3 3l3 3" />
            </svg>
          </button>
        </div>

        <!-- Login -->
        <router-link 
          v-else 
          to="/login" 
          class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2 rounded-full font-semibold transition-colors">
          Log In
        </router-link>

      </div>

    </div>
  </header>
</template>

<script setup>
import { useAuthStore } from '@/stores/authStore'
import { useAuth } from '@/composables/useAuth'

const authStore = useAuthStore()
const { logout } = useAuth()

const handleLogout = () => {
  logout()
}
</script>
