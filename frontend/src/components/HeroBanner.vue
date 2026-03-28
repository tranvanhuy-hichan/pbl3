<template>
  <section class="bg-slate-50 py-16 md:py-24 overflow-hidden relative">
    <div class="container mx-auto px-4 flex flex-col lg:flex-row items-center gap-12 lg:gap-16">
      
      <div class="flex-1 max-w-2xl z-10 mx-auto text-center lg:text-left">
        <span class="inline-block px-4 py-1.5 bg-indigo-100 text-indigo-600 rounded-full font-bold text-sm mb-6 uppercase tracking-wider">New Arrival</span>
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-extrabold text-slate-900 leading-tight mb-6 tracking-tight">
          Next-Gen <br class="hidden lg:block"> <span class="text-blue-500 relative">Performance.</span>
        </h1>
        <p class="text-lg lg:text-xl text-slate-600 leading-relaxed mb-10 max-w-lg mx-auto lg:mx-0">
          Experience the ultimate power with our latest collection of premium laptops designed for professionals and creators.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start">
          <button class="px-8 py-4 bg-blue-500 hover:bg-blue-600 text-white rounded-lg text-lg font-semibold shadow-lg shadow-blue-500/30 hover:-translate-y-0.5 transition-all">Shop Now</button>
          <button class="px-8 py-4 bg-transparent border-2 border-slate-300 hover:border-slate-400 text-slate-700 rounded-lg text-lg font-semibold transition-colors">Learn More</button>
        </div>
      </div>

      <div class="flex-1 relative w-full max-w-2xl mx-auto">
        <img src="https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=1471&auto=format&fit=crop" alt="Premium Laptop" class="w-full rounded-3xl shadow-2xl relative z-10" />
        
        <!-- Decorative glowing shapes -->
        <div class="absolute -top-12 -right-12 w-72 h-72 bg-blue-200 rounded-full blur-3xl opacity-50 z-0"></div>
        <div class="absolute -bottom-10 -left-10 w-64 h-64 bg-purple-200 rounded-full blur-3xl opacity-50 z-0"></div>
      </div>
      
    </div>
  </section>
</template>

<script setup>
</script>
