<template>
  <div class="h-screen flex bg-slate-50 text-slate-800">
    <aside class="w-48 bg-slate-800 text-white flex flex-col shadow-xl z-20">
      <div class="h-16 flex items-center justify-center border-b border-slate-700">
        <div class="text-xl font-bold flex items-center gap-2">
          Admin<span class="text-sky-500">Panel</span>
        </div>
      </div>

      <nav class="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
        <router-link to="/admin" class="sidebar-nav-item" exact-active-class="sidebar-nav-active">
          <span>Dashboard</span>
        </router-link>
        <router-link to="/admin/products" class="sidebar-nav-item" active-class="sidebar-nav-active">
          <span>Sản phẩm</span>
        </router-link>
        <router-link to="/admin/brands" class="sidebar-nav-item" active-class="sidebar-nav-active">
          <span>Nhãn hàng</span>
        </router-link>
        <router-link to="/admin/categories" class="sidebar-nav-item" active-class="sidebar-nav-active">
          <span>Danh mục</span>
        </router-link>
        <router-link to="/admin/users" class="sidebar-nav-item" active-class="sidebar-nav-active">
          <span>Người dùng</span>
        </router-link>
        <router-link to="/admin/orders" class="sidebar-nav-item" active-class="sidebar-nav-active">
          <span>Đơn hàng</span>
        </router-link>

        <hr class="border-slate-700 my-4" />
      </nav>
      
      <div class="px-4 py-4">
        <router-link 
          v-if="authStore.user?.role === 'Admin'"
          to="/"
          title="Trang chủ"
          class="sidebar-nav-item text-sky-300 hover:text-white">
          <span>Trang chủ website</span>
        </router-link>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col">
      <!-- Topbar (Condensed) -->
     <header class="h-12 bg-white border-b border-slate-200 flex items-center justify-end px-6 shadow-sm z-10 relative">
        <!-- User Info with Dropdown -->
        <div class="relative">
          <div 
            id="admin-menu-btn"
            @click.stop="toggleDropdown"
            class="flex items-center gap-3 cursor-pointer hover:bg-slate-100 px-3 py-1.5 rounded-xl transition select-none"
          >
            <div class="text-right">
              <div class="text-sm font-semibold text-slate-700">
                {{ user?.fullName || 'Admin' }}
              </div>
              <div class="text-xs text-sky-600 font-medium">
                {{ user?.role || 'Role' }}
              </div>
            </div>

            <div class="w-9 h-9 rounded-full bg-sky-600 text-white flex items-center justify-center font-bold text-sm shadow-md relative overflow-hidden group">
              {{ user?.fullName?.charAt(0) || 'A' }}
              <!-- Subtle hover ring overlay -->
              <div class="absolute inset-0 bg-black/10 opacity-0 group-hover:opacity-100 transition duration-150"></div>
            </div>

            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2.5" stroke="currentColor" 
                 class="w-3.5 h-3.5 text-slate-500 transition-transform duration-200"
                 :class="{ 'rotate-180': isDropdownOpen }">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
            </svg>
          </div>

          <!-- Beautiful Premium Dropdown Menu for Admin -->
          <transition name="dropdown-slide">
            <div v-if="isDropdownOpen"
                 id="admin-dropdown"
                 class="absolute right-0 top-full mt-2 w-60 bg-white/95 backdrop-blur-md rounded-2xl shadow-xl border border-slate-200/50 py-1.5 z-50 overflow-hidden transform origin-top-right transition-all duration-200">
              
              <!-- User header info -->
              <div class="px-4 py-3 border-b border-slate-100/80 bg-slate-50/50">
                <p class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Tài khoản Admin</p>
                <p class="text-sm font-bold text-slate-800 truncate mt-0.5">{{ user?.fullName }}</p>
                <p class="text-[11px] font-semibold text-sky-600 mt-0.5">{{ user?.role }}</p>
              </div>

              <!-- Menu options -->
              <div class="p-1 space-y-0.5">
                <router-link
                  to="/profile"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-sky-600 hover:bg-sky-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-slate-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                  </svg>
                  Xem hồ sơ cá nhân
                </router-link>

                <router-link
                  to="/profile?changePassword=true"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-sky-600 hover:bg-sky-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-slate-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 5.25a3 3 0 013 3m3 0a6 6 0 01-7.029 5.912c-.563-.097-1.159.026-1.563.43L10.5 17.25H8.25v2.25H6v2.25H2.25v-2.818c0-.597.237-1.17.659-1.591l6.499-6.499c.404-.404.527-1 .43-1.563A6 6 0 1121.75 8.25z" />
                  </svg>
                  Đổi mật khẩu
                </router-link>

                <!-- Shortcut to homepage -->
                <router-link
                  to="/"
                  @click="closeDropdown"
                  class="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-slate-700 hover:text-indigo-600 hover:bg-indigo-50/50 rounded-xl transition duration-150"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4 text-indigo-400">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" />
                  </svg>
                  Về trang chủ
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
      </header>

      <!-- Content -->
      <main class="flex-1 overflow-y-auto bg-slate-50">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter, RouterView } from 'vue-router' 
import { useAuthStore } from '@/stores/authStore'

const route = useRoute()
const router = useRouter() 
const authStore = useAuthStore()

const user = computed(() => authStore.user)
const isDropdownOpen = ref(false)

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value
}

const closeDropdown = () => {
  isDropdownOpen.value = false
}

const handleLogout = () => {
  closeDropdown()
  authStore.clearAuth()
  router.push('/')
}

const handleClickOutside = (event) => {
  const dropdown = document.getElementById('admin-dropdown')
  const userBtn = document.getElementById('admin-menu-btn')
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
.sidebar-nav-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  border-radius: 8px;
  font-weight: 500;
  color: #cbd5e1;
  transition: all 0.3s ease-in-out;
  white-space: nowrap;
}

.sidebar-nav-item:hover {
  background: #334155; 
  color: white;
  transform: translateX(4px);
}

.sidebar-nav-active {
  background: #0ea5e9;
  color: white;
  box-shadow: 0 4px 15px rgba(14, 165, 233, 0.4);
  transform: translateX(4px);
}

.sidebar-logout-button {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%; 
  padding: 8px 12px;
  border-radius: 8px;
  font-weight: 500;
  color: #fca5a5;
  transition: all 0.3s ease-in-out;
  background-color: transparent; 
  border: none; 
  cursor: pointer;
}

.sidebar-logout-button:hover {
  background: #dc2626; 
  color: white;
  transform: translateX(4px);
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