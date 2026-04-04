<template>
  <div class="h-screen flex flex-col bg-slate-50 text-slate-800">

    <!-- Topbar -->
    <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-4 sm:px-6 shadow-sm">

      <!-- Logo -->
      <div class="text-lg font-bold">
        Admin<span class="text-brand-500">Panel</span>
      </div>

      <!-- Menu (responsive) -->
      <nav class="flex flex-wrap sm:flex-nowrap gap-2 overflow-x-auto">
        <router-link to="/admin" class="nav-item" exact-active-class="nav-active">
          Thống kê
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
        <router-link to="/" class="text-sm text-slate-500 hover:text-brand-500">
          Trang chủ
        </router-link>

        <!-- Avatar -->
        <div class="w-8 h-8 sm:w-9 sm:h-9 rounded-full bg-brand-500 text-white flex items-center justify-center font-bold">
          A
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

const route = useRoute()
const currentRouteName = computed(() => route.meta.title || route.name || 'Dashboard')
</script>

<style scoped>
.nav-item {
  padding: 6px 12px;
  border-radius: 10px;
  font-weight: 500;
  color: #475569;
  transition: all 0.2s;
  white-space: nowrap;
  flex-shrink: 0; /* tránh bị thu nhỏ trên mobile */
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
