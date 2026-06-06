<template>
  <header class="bg-blue-100 border-b border-blue-200 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 h-16 flex items-center">

      <div class="flex items-center gap-2 text-2xl font-extrabold text-blue-700">
      <router-link to="/" class="flex items-center gap-2">
        
        <!-- Logo -->
        <img src="/icon.ico" alt="logo" class="w-10 h-10 object-contain" />

        <!-- Tên shop -->
        <span>TechStore</span>
      </router-link>
    </div>


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
          v-if="authStore.user?.role === 'Customer'"
          to="/my/orders"
          class="nav-link"
          active-class="nav-active">
          Đơn hàng
        </router-link>
      </nav>

      <div class="flex items-center gap-5 ml-auto">

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

        <div v-if="authStore.isAuthenticated()" 
             class="relative flex items-center gap-3 border-l border-blue-200 pl-4">

          <div
            id="user-menu-btn"
            @click.stop="toggleDropdown"
            class="hidden sm:flex items-center gap-3 hover:opacity-80 cursor-pointer select-none"
          >
            <div class="flex flex-col text-right">
              <span class="text-sm font-semibold text-slate-800">
                {{ authStore.user?.fullName || 'User' }}
              </span>

              <span class="text-xs text-blue-600 font-medium">
                {{ authStore.user?.role || '' }}
              </span>
            </div>

            <div class="w-9 h-9 rounded-full bg-blue-600 text-white 
                        flex items-center justify-center font-bold shadow ml-2 relative overflow-hidden group">
              {{ authStore.user?.fullName?.charAt(0) || 'U' }}
              <!-- Subtle hover ring overlay -->
              <div class="absolute inset-0 bg-black/10 opacity-0 group-hover:opacity-100 transition duration-150"></div>
            </div>
            
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2.5" stroke="currentColor" 
                 class="w-3.5 h-3.5 text-slate-500 transition-transform duration-200"
                 :class="{ 'rotate-180': isDropdownOpen }">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
            </svg>
          </div>

          <!-- Beautiful Premium Dropdown Menu -->
          <transition name="dropdown-slide">
            <div v-if="isDropdownOpen"
                 id="user-dropdown"
                 class="absolute right-0 top-full mt-2 w-60 bg-white/95 backdrop-blur-md rounded-2xl shadow-xl border border-blue-100/50 py-1.5 z-50 overflow-hidden transform origin-top-right transition-all duration-200">
              
              <!-- User header info -->
              <div class="px-4 py-3 border-b border-slate-100/80 bg-slate-50/50">
                <p class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Tài khoản</p>
                <p class="text-sm font-bold text-slate-800 truncate mt-0.5">{{ authStore.user?.fullName }}</p>
                <p class="text-[11px] font-semibold text-blue-600 mt-0.5">{{ authStore.user?.role }}</p>
              </div>

              <!-- Menu options -->
              <div class="p-1 space-y-0.5">
                <router-link
                  to="/profile"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-blue-600 hover:bg-blue-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-slate-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                  </svg>
                  Xem hồ sơ cá nhân
                </router-link>

                <router-link
                  to="/profile?changePassword=true"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-blue-600 hover:bg-blue-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-slate-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 5.25a3 3 0 013 3m3 0a6 6 0 01-7.029 5.912c-.563-.097-1.159.026-1.563.43L10.5 17.25H8.25v2.25H6v2.25H2.25v-2.818c0-.597.237-1.17.659-1.591l6.499-6.499c.404-.404.527-1 .43-1.563A6 6 0 1121.75 8.25z" />
                  </svg>
                  Đổi mật khẩu
                </router-link>

                <router-link
                  v-if="authStore.user?.role === 'Admin'"
                  to="/admin"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-indigo-600 hover:bg-indigo-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-indigo-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M10.5 6h9.75M10.5 6a1.5 1.5 0 11-3 0m3 0a1.5 1.5 0 10-3 0M3.75 6H7.5m3 12h9.75m-9.75 0a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m-3.75 0H7.5m9-6h3.75m-3.75 0a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m-9.75 0h9.75" />
                  </svg>
                  Trang quản trị
                </router-link>
              </div>

              <!-- Logout action -->
              <div class="border-t border-slate-100 mt-1 p-1">
                <button
                  @click="handleLogout"
                  class="w-full flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50/50 rounded-xl transition duration-150 text-left cursor-pointer"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-red-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l-3 3m0 0l3 3m-3-3h12.75" />
                  </svg>
                  Đăng xuất
                </button>
              </div>
            </div>
          </transition>
        </div>

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
import { ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useAuth } from '@/composables/useAuth'

const authStore = useAuthStore()
const { logout } = useAuth()

const isDropdownOpen = ref(false)

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value
}

const closeDropdown = () => {
  isDropdownOpen.value = false
}

const handleLogout = () => {
  closeDropdown()
  logout()
}

const handleClickOutside = (event) => {
  const dropdown = document.getElementById('user-dropdown')
  const userBtn = document.getElementById('user-menu-btn')
  if (dropdown && !dropdown.contains(event.target) && userBtn && !userBtn.contains(event.target)) {
    closeDropdown()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
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

/* Premium Dropdown animation */
.dropdown-slide-enter-active,
.dropdown-slide-leave-active {
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

.dropdown-slide-enter-from,
.dropdown-slide-leave-to {
  opacity: 0;
  transform: translateY(8px) scale(0.95);
}
</style>
