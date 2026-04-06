```vue
<template>
  <div class="h-screen flex flex-col bg-slate-50 text-slate-800">

    <!-- Topbar -->
    <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-4 sm:px-6 shadow-sm">

      <!-- Logo -->
      <div class="text-lg font-bold">
        Admin<span class="text-brand-500">Panel</span>
      </div>

      <!-- Menu -->
      <nav class="flex flex-wrap sm:flex-nowrap gap-2 overflow-x-auto">
        <router-link to="/admin" class="nav-item" exact-active-class="nav-active">
          Dashboard
        </router-link>
        <router-link to="/admin/products" class="nav-item" active-class="nav-active">
          Sản phẩm
        </router-link>
        <router-link to="/admin/brands" class="nav-item" active-class="nav-active">
          Nhãn hàng
        </router-link>
        <router-link to="/admin/categories" class="nav-item" active-class="nav-active">
          Danh mục
        </router-link>
        <router-link to="/admin/users" class="nav-item" active-class="nav-active">
          Người dùng
        </router-link>
        <router-link to="/admin/orders" class="nav-item" active-class="nav-active">
          Đơn hàng
        </router-link>
      </nav>

      <!-- Right -->
      <div class="flex items-center gap-3">

        <!-- Back Home -->
        <router-link 
          v-if="authStore.user?.role === 'Admin'"
          to="/"
          title="Trang chủ"
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
              d="M3 9.75L12 4.5l9 5.25M4.5 10.5V19.5a.75.75 0 00.75.75h4.5v-6h4.5v6h4.5a.75.75 0 00.75-.75V10.5" />
          </svg>

        </router-link>

        <!-- User Info -->
        <div class="text-right hidden sm:block">
          <div class="text-sm font-semibold text-slate-700">
            {{ user?.fullName || 'Admin' }}
          </div>
          <div class="text-xs text-indigo-500 font-medium">
            {{ user?.role || 'Role' }}
          </div>
        </div>

        <!-- Avatar -->
        <div class="w-8 h-8 sm:w-9 sm:h-9 rounded-full bg-brand-500 text-white flex items-center justify-center font-bold">
          {{ user?.fullName?.charAt(0) || 'A' }}
        </div>

      </div>

    </header>

    <!-- Content -->
    <main class="flex-1 overflow-y-auto p-2 sm:p-4">
      <div class="bg-white rounded-xl shadow-sm p-2 min-h-full">
        <RouterView />
      </div>
    </main>

  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const route = useRoute()
const authStore = useAuthStore()

const currentRouteName = computed(() => route.meta.title || route.name || 'Dashboard')

// lấy user từ store
const user = computed(() => authStore.user)
</script>

<style scoped>
.nav-item {
  padding: 6px 12px;
  border-radius: 10px;
  font-weight: 500;
  color: #475569;
  transition: all 0.2s;
  white-space: nowrap;
  flex-shrink: 0;
}

.nav-item:hover {
  background: #f1f5f9;
  color: #0f172a;
}

.nav-active {
  background: #6366f1;
  color: white;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.3);
}
</style>

