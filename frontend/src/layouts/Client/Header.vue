<template>
  <header class="bg-blue-50 border-b border-blue-200 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 h-16 flex items-center">

      <!-- Logo -->
      <div class="text-2xl font-extrabold text-blue-700">
        <router-link to="/">Cửa hàng điện tử</router-link>
      </div>

      <!-- Menu -->
      <nav class="hidden md:flex items-center gap-6 ml-8">
        <router-link 
          to="/" 
          class="nav-link" 
          exact-active-class="nav-active">
          Trang chủ
        </router-link>

        <router-link 
          to="/products" 
          class="nav-link" 
          active-class="nav-active">
          Sản phẩm
        </router-link>

        <router-link 
          to="/categories" 
          class="nav-link" 
          active-class="nav-active">
          Danh mục
        </router-link>

        <router-link 
          to="/brands" 
          class="nav-link" 
          active-class="nav-active">
          Nhãn hàng
        </router-link>

        <router-link
          v-if="authStore.user?.role === 'Customer'"
          to="/my/orders"
          class="nav-link"
          active-class="nav-active">
          Đơn hàng
        </router-link>
      </nav>

      <!-- Right -->
      <div class="flex items-center gap-5 ml-auto">

        <!-- Cart -->
        <router-link 
          v-if="authStore.user?.role === 'Customer'"
          to="/cart"
          class="relative p-2 rounded-full hover:bg-blue-100 transition">

          <svg xmlns="http://www.w3.org/2000/svg" 
               class="h-6 w-6 text-blue-600" 
               fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>

          <span class="absolute -top-1 -right-1 
                       bg-blue-600 text-white text-xs 
                       px-1.5 py-0.5 rounded-full">
            2
          </span>
        </router-link>

        <!-- User -->
        <div v-if="authStore.isAuthenticated()" 
             class="flex items-center gap-3 border-l border-blue-200 pl-4">

          <!-- Info -->
          <div class="hidden sm:flex flex-col text-right">
            <span class="text-sm font-semibold text-slate-800">
              {{ authStore.user?.fullName || 'User' }}
            </span>
            <span class="text-xs text-blue-600">
              {{ authStore.user?.role }}
            </span>
          </div>

          <!-- Avatar -->
          <div class="w-9 h-9 rounded-full bg-blue-600 text-white 
                      flex items-center justify-center font-bold shadow">
            {{ authStore.user?.fullName?.charAt(0) || 'U' }}
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-1 ml-2">

            <!-- Admin -->
            <router-link 
              v-if="authStore.user?.role === 'Admin'"
              to="/admin"
              title="Trang quản trị"
              class="w-9 h-9 flex items-center justify-center 
                     text-indigo-500 hover:text-white 
                     hover:bg-indigo-500 rounded-full transition">

              <svg xmlns="http://www.w3.org/2000/svg" 
                   fill="none" viewBox="0 0 24 24" 
                   stroke-width="2" stroke="currentColor" 
                   class="w-5 h-5">
                <path stroke-linecap="round" stroke-linejoin="round" 
                  d="M3 3h7v7H3V3zm11 0h7v4h-7V3zM14 10h7v11h-7V10zM3 14h7v7H3v-7z" />
              </svg>
            </router-link>

            <!-- Logout -->
            <button 
              @click="handleLogout"
              title="Đăng xuất"
              class="w-9 h-9 flex items-center justify-center 
                     text-red-500 hover:text-white 
                     hover:bg-red-500 rounded-full transition">

              <svg xmlns="http://www.w3.org/2000/svg" 
                   fill="none" viewBox="0 0 24 24" 
                   stroke-width="2" stroke="currentColor" 
                   class="w-5 h-5">
                <path stroke-linecap="round" stroke-linejoin="round" 
                  d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-7.5A2.25 2.25 0 003.75 5.25v13.5A2.25 2.25 0 006 21h7.5a2.25 2.25 0 002.25-2.25V15" />
                <path stroke-linecap="round" stroke-linejoin="round" 
                  d="M18 12h-9m0 0l3-3m-3 3l3 3" />
              </svg>
            </button>

          </div>
        </div>

        <!-- Login -->
        <router-link 
          v-else 
          to="/login" 
          class="bg-blue-500 hover:bg-blue-600 text-white 
                 px-4 py-2 rounded-full font-medium transition">
          Đăng nhập
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

<style scoped>
.nav-link {
  font-weight: 500;
  color: #2563eb;
  transition: 0.2s;
}

.nav-link:hover {
  color: #1d4ed8;
}

.nav-active {
  color: #1e40af;
  font-weight: 700;
  border-bottom: 2px solid #1e40af;
}
</style>
