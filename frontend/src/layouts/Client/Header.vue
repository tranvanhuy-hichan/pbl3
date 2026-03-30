<template>
  <header class="bg-white border-b border-slate-200 py-4 sticky top-0 z-50 shadow-sm">
    <div class="container mx-auto px-4 flex justify-between items-center gap-4 lg:gap-8 flex-wrap lg:flex-nowrap">
      
      <div class="text-2xl font-extrabold text-slate-800 tracking-tight">
        <router-link to="/">Cửa hàng thiết bị điện tử</router-link>
      </div>

      <nav class="hidden md:flex gap-6">
        <router-link to="/" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Trang chủ</router-link>
        <router-link to="/products" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Sản phẩm</router-link>
        <router-link to="/categories" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Danh mục</router-link>
        <router-link to="/categories" class="text-slate-500 font-medium hover:text-blue-500 transition-colors">Nhãn hàng</router-link>
      </nav>

      <div class="flex items-center gap-4">
        <!-- Cart -->
        <router-link to="/cart" class="relative p-2 text-slate-600 hover:text-brand-600 transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          <span class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-brand-500 rounded-full border-2 border-white">2</span>
        </router-link>
        <router-link to="/login" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2 rounded-full font-semibold transition-colors">Đăng nhập</router-link>
      </div>

    </div>
  </header>
</template>

<script setup>
</script>
