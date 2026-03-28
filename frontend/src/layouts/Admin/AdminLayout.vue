<template>
  <div class="flex h-screen bg-slate-50 font-sans text-slate-800">
    <!-- Sidebar -->
    <aside class="w-64 bg-slate-900 text-slate-300 flex flex-col flex-shrink-0">
      <div class="h-16 flex items-center px-6 border-b border-slate-800">
        <span class="text-white text-xl font-bold tracking-tight">Admin<span class="text-brand-500">Panel</span></span>
      </div>
      
      <nav class="flex-1 overflow-y-auto py-4">
        <ul class="space-y-1 px-3">
          <li>
            <router-link to="/admin" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors group" active-class="bg-brand-600 text-white hover:bg-brand-600">
              <span class="font-medium">Dashboard</span>
            </router-link>
          </li>
          
          <li class="pt-4 pb-2 px-3 text-xs font-semibold text-slate-500 uppercase tracking-wider">Management</li>
          
          <li>
            <router-link to="/admin/products" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors" active-class="bg-slate-800 text-white">
              <span class="font-medium">Products</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/brands" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors" active-class="bg-slate-800 text-white">
              <span class="font-medium">Brands</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/categories" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors" active-class="bg-slate-800 text-white">
              <span class="font-medium">Categories</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/users" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors" active-class="bg-slate-800 text-white">
              <span class="font-medium">Users</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/orders" class="flex items-center px-3 py-2.5 rounded-lg hover:bg-slate-800 hover:text-white transition-colors" active-class="bg-slate-800 text-white">
              <span class="font-medium">Orders</span>
            </router-link>
          </li>
        </ul>
      </nav>
      
      <div class="p-4 border-t border-slate-800">
        <router-link to="/" class="flex items-center px-3 py-2 text-sm text-slate-400 hover:text-white transition-colors">
          &larr; Back to Store
        </router-link>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col min-w-0 overflow-hidden">
      <!-- Admin Header -->
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 flex-shrink-0">
        <div class="flex items-center">
          <h1 class="text-xl font-semibold text-slate-800">{{ currentRouteName }}</h1>
        </div>
        
        <div class="flex items-center gap-4">
          <div class="w-8 h-8 rounded-full bg-brand-100 flex items-center justify-center text-brand-700 font-bold">
            A
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 overflow-y-auto bg-slate-50 p-6">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, RouterView } from 'vue-router'

const route = useRoute()
const currentRouteName = computed(() => {
  return route.meta.title || route.name || 'Dashboard'
})
</script>
