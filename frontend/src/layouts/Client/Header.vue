<template>
  <header class="bg-blue-50 border-b border-blue-200 py-4 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 flex justify-between items-center gap-4 lg:gap-8 flex-wrap lg:flex-nowrap">
      
      <!-- Logo -->
      <div class="text-2xl font-extrabold text-blue-700 tracking-tight">
        <router-link to="/">Cửa hàng điện tử</router-link>
      </div>

      <!-- Menu -->
      <nav class="hidden md:flex gap-6">
        <router-link to="/" class="text-blue-600 font-medium hover:text-blue-800 transition-colors">Trang chủ</router-link>
        <router-link to="/products" class="text-blue-600 font-medium hover:text-blue-800 transition-colors">Sản phẩm</router-link>
        <router-link to="/categories" class="text-blue-600 font-medium hover:text-blue-800 transition-colors">Danh mục</router-link>
        <router-link to="/brands" class="text-blue-600 font-medium hover:text-blue-800 transition-colors">Nhãn hàng</router-link>
      </nav>

      <!-- Cart & Login -->
      <div class="flex items-center gap-4">
        <!-- Cart -->
        <router-link to="/cart" class="relative p-2 text-blue-700 hover:text-blue-900 transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>

          <span class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-blue-600 rounded-full border-2 border-white">2</span>
        </router-link>

        <!-- Login Button -->
        <router-link to="/login" class="bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white px-5 py-2 rounded-full font-semibold transition-all">
          Đăng nhập
        </router-link>

        <div v-if="authStore.isAuthenticated()" class="flex items-center gap-3">
          <div class="hidden sm:flex flex-col items-end">
            <span class="text-sm font-bold text-slate-800">{{ authStore.user?.fullName || 'User' }}</span>
            <span class="text-xs text-brand-600 font-medium">{{ authStore.user?.role }}</span>
          </div>
          <button @click="handleLogout" class="text-sm text-slate-500 hover:text-red-500 font-medium transition-colors">
            Log out
          </button>
        </div>
        <router-link v-else to="/login" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2 rounded-full font-semibold transition-colors">Log In</router-link>

      </div>

    </div>
  </header>
</template>


<script setup>
import { useAuthStore } from '@/stores/authStore'
import { useAuth } from '@/composables/useAuth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const { logout } = useAuth()
const router = useRouter()

const handleLogout = () => {
  logout()
  router.push('/login')
}
</script>

