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

        <!-- Back Home (if Admin) -->
        <router-link 
          v-if="authStore.user?.role === 'Admin'"
          to="/"
          title="Trang chủ"
          class="sidebar-nav-item text-sky-300 hover:text-white">
          <span>Trang chủ website</span>
        </router-link>
      </nav>
      
      <div class="px-4 py-4">
        <button @click="handleLogout" class="w-full flex items-center justify-center gap-10px 
                                            text-red-300 hover:text-white hover:bg-red-700 
                                            py-8px px-12px rounded-8px font-medium transition-all 
                                            duration-300 ease-in-out mt-auto"> <!-- mt-auto đẩy xuống cuối -->
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l3 3m0 0l-3 3m3-3H9" />
          </svg>
          <span>Đăng xuất</span>
        </button>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col">
      <!-- Topbar (Condensed) -->
     <header class="h-12 bg-white border-b border-slate-200 flex items-center justify-end px-6 shadow-sm z-10">
        <!-- User Info -->
        <router-link 
          to="/profile"
          class="flex items-center gap-3 cursor-pointer hover:bg-slate-100 px-3 py-1.5 rounded-xl transition"
        >
          <div class="text-right">
            <div class="text-sm font-semibold text-slate-700">
              {{ user?.fullName || 'Admin' }}
            </div>
            <div class="text-xs text-sky-600 font-medium">
              {{ user?.role || 'Role' }}
            </div>
          </div>

          <div class="w-9 h-9 rounded-full bg-sky-600 text-white flex items-center justify-center font-bold text-sm shadow-md">
            {{ user?.fullName?.charAt(0) || 'A' }}
          </div>
        </router-link>
      </header>

      <!-- Content -->
      <main class="flex-1 overflow-y-auto">
        <div class="bg-white rounded-xl shadow-lg p-4 min-h-full">
          <RouterView />
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter, RouterView } from 'vue-router' 
import { useAuthStore } from '@/stores/authStore'

const route = useRoute()
const router = useRouter() 
const authStore = useAuthStore()


const user = computed(() => authStore.user)

const handleLogout = () => {
  authStore.clearAuth();
  router.push('/'); 
}
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
</style>