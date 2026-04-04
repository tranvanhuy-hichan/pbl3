<template>
  <header class="bg-white border-b border-slate-200 py-4 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 flex justify-between items-center gap-4 lg:gap-8 flex-wrap lg:flex-nowrap">
      
      <div class="text-2xl font-extrabold text-slate-800 tracking-tight">
        <router-link to="/">ElectroShop</router-link>
      </div>

      <nav class="hidden md:flex gap-6">
        <router-link to="/" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Home</router-link>
        <router-link to="/products" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Products</router-link>
        <router-link to="/categories" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Categories</router-link>
      </nav>

      <div class="flex-1 w-full order-3 lg:order-none lg:max-w-md flex bg-slate-100 rounded-full overflow-hidden px-2 py-1 border border-transparent focus-within:border-blue-500 focus-within:bg-white transition-colors">
        <input type="text" placeholder="Search for electronics..." class="flex-1 bg-transparent border-none px-4 py-2 outline-none text-sm text-slate-700" />
        <button class="p-2 text-slate-500 hover:text-blue-500 flex items-center justify-center" aria-label="Search">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001q.044.06.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1 1 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0"/>
          </svg>
        </button>
      </div>

      <div class="flex items-center gap-4">
        <!-- Cart -->
        <router-link to="/cart" class="relative p-2 text-slate-600 hover:text-brand-600 transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          <span class="absolute top-0 right-0 inline-flex items-center justify-center px-1.5 py-0.5 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-brand-500 rounded-full border-2 border-white">2</span>
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
