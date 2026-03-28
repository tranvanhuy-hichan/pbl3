<template>
  <footer class="bg-slate-900 text-slate-300 pt-16 mt-16">
    <div class="container mx-auto px-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-12 mb-12">
        <div class="lg:col-span-2">
          <h3 class="text-white text-2xl font-bold mb-4">ElectroShop</h3>
          <p class="leading-relaxed mb-6">Your premium destination for the latest electronics, laptops, smartphones, and accessories. Experience technology at its best.</p>
          <div class="flex gap-4">
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Facebook">FB</a>
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Twitter">TW</a>
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Instagram">IG</a>
          </div>
        </div>
        
        <div>
          <h4 class="text-white text-lg font-semibold mb-5">Shop By Category</h4>
          <ul class="space-y-3">
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Laptops & Computers</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Smartphones</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Tablets</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Audio & Headphones</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Wearables</a></li>
          </ul>
        </div>
        
        <div>
          <h4 class="text-white text-lg font-semibold mb-5">Customer Service</h4>
          <ul class="space-y-3">
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Contact Us</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Shipping & Delivery</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Returns & Exchanges</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">FAQ</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Track Order</a></li>
          </ul>
        </div>
        
        <div class="lg:col-span-1">
          <h4 class="text-white text-lg font-semibold mb-5">Newsletter</h4>
          <p class="mb-4 text-sm">Subscribe to get special offers, free giveaways, and deals.</p>
          <form class="flex mt-4" @submit.prevent>
            <input type="email" placeholder="Enter your email" class="flex-1 w-full px-4 py-3 rounded-l text-slate-900 outline-none" required />
            <button type="submit" class="bg-blue-500 hover:bg-blue-600 text-white px-5 rounded-r font-semibold transition-colors">Subscribe</button>
          </form>
        </div>
      </div>
      
      <div class="border-t border-slate-800 py-6 flex flex-col sm:flex-row justify-between items-center text-sm gap-4 text-center">
        <p>&copy; {{ new Date().getFullYear() }} ElectroShop. All rights reserved.</p>
        <div class="flex gap-6">
          <a href="#" class="text-slate-400 hover:text-white transition-colors">Privacy Policy</a>
          <a href="#" class="text-slate-400 hover:text-white transition-colors">Terms of Service</a>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
</script>
