<template>
  <section class="bg-slate-50 py-16 md:py-24 overflow-hidden relative">
    <div class="container mx-auto px-4 flex flex-col lg:flex-row items-center gap-12 lg:gap-16">
      
      <!-- Nội dung chữ -->
      <div class="flex-1 max-w-2xl z-10 mx-auto text-center lg:text-left">
        <span class="inline-block px-4 py-1.5 bg-indigo-100 text-indigo-600 rounded-full font-bold text-sm mb-6 uppercase tracking-wider">
          Mới Ra Mắt
        </span>
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-extrabold text-slate-900 leading-tight mb-6 tracking-tight">
          Nâng Tầm <br class="hidden lg:block">
          <span class="bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500 relative">
            Hiệu Suất.
          </span>
        </h1>
        <p class="text-lg lg:text-xl text-slate-600 leading-relaxed mb-10 max-w-lg mx-auto lg:mx-0">
          Khám phá bộ sưu tập laptop mới nhất, thiết kế dành cho những nhà sáng tạo, game thủ và chuyên gia cần hiệu năng tối đa.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start">
          <router-link
            to="/products" 
            class="px-8 py-4 bg-blue-500 hover:bg-blue-600 text-white rounded-lg text-lg font-semibold shadow-lg shadow-blue-500/30 transform hover:-translate-y-1 transition-all duration-300 text-center"
          >
            Mua Ngay
          </router-link>
          <router-link
            to="/products"
            class="px-8 py-4 bg-transparent border-2 border-slate-300 hover:border-slate-400 text-slate-700 rounded-lg text-lg font-semibold transition-all duration-300 hover:bg-slate-100"
          >
            Tìm Hiểu Thêm
          </router-link>
        </div>

      </div>

      <!-- Hình ảnh -->
      <div class="flex-1 relative w-full max-w-2xl mx-auto">
        <img
          src="https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=1471&auto=format&fit=crop"
          alt="Laptop Cao Cấp"
          class="w-full rounded-3xl shadow-2xl relative z-10"
        />
        
        <!-- Hình dạng glow trang trí -->
        <div class="absolute -top-16 -right-16 w-80 h-80 bg-blue-300 rounded-full blur-3xl opacity-40 z-0 animate-pulse"></div>
        <div class="absolute -bottom-16 -left-16 w-72 h-72 bg-purple-300 rounded-full blur-3xl opacity-40 z-0 animate-pulse"></div>
      </div>
      
    </div>
  </section>
</template>
