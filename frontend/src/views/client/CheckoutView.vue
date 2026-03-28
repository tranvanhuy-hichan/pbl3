<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      
      <div class="mb-8 flex items-center justify-between">
        <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">Checkout</h1>
        <router-link to="/cart" class="text-brand-600 font-medium hover:underline flex items-center gap-1 text-sm">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
          Back to Cart
        </router-link>
      </div>

      <div class="flex flex-col lg:flex-row gap-8">
        
        <!-- Checkout Form -->
        <div class="lg:w-2/3 space-y-6">
          
          <!-- Contact Info -->
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6 flex items-center gap-2">
              <span class="bg-brand-100 text-brand-700 w-8 h-8 rounded-full flex items-center justify-center text-sm">1</span>
              Contact Information
            </h2>
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">Email Address</label>
                <input type="email" placeholder="you@example.com" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
              <div class="flex items-center gap-2 text-sm text-slate-600 mt-2">
                <input type="checkbox" id="newsletter" class="rounded text-brand-500 focus:ring-brand-500 w-4 h-4 border-slate-300">
                <label for="newsletter">Email me with news and offers</label>
              </div>
            </div>
          </div>

          <!-- Shipping Address -->
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6 flex items-center gap-2">
              <span class="bg-brand-100 text-brand-700 w-8 h-8 rounded-full flex items-center justify-center text-sm">2</span>
              Shipping Address
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
               <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">First Name</label>
                <input type="text" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">Last Name</label>
                <input type="text" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
              <div class="md:col-span-2">
                <label class="block text-sm font-medium text-slate-700 mb-1">Address</label>
                <input type="text" placeholder="123 Main St" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
               <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">City</label>
                <input type="text" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
               <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">Postal Code</label>
                <input type="text" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
               <div class="md:col-span-2">
                <label class="block text-sm font-medium text-slate-700 mb-1">Phone Number</label>
                <input type="tel" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
            </div>
          </div>

          <!-- Payment -->
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6 flex items-center gap-2">
              <span class="bg-brand-100 text-brand-700 w-8 h-8 rounded-full flex items-center justify-center text-sm">3</span>
              Payment Method
            </h2>

            <div class="space-y-4">
               <div class="border border-brand-500 bg-brand-50 rounded-xl p-4 flex items-center gap-3 cursor-pointer">
                 <input type="radio" name="payment" id="card" checked class="w-5 h-5 text-brand-600 focus:ring-brand-500 border-slate-300">
                 <label for="card" class="font-semibold text-slate-800 flex-1 cursor-pointer">Credit or Debit Card</label>
                 <div class="flex gap-1">
                   <div class="w-8 h-5 bg-blue-600 rounded flex items-center justify-center text-[8px] text-white font-bold italic">VISA</div>
                   <div class="w-8 h-5 bg-red-500 rounded flex items-center justify-center text-[8px] text-white font-bold italic">MC</div>
                 </div>
               </div>

               <!-- Card form details -->
               <div class="pl-12 pr-4 pb-4 space-y-4">
                  <div>
                    <label class="block text-xs font-medium text-slate-500 mb-1 uppercase tracking-wide">Card Number</label>
                    <input type="text" placeholder="0000 0000 0000 0000" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 font-mono text-sm tracking-widest bg-white">
                  </div>
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <label class="block text-xs font-medium text-slate-500 mb-1 uppercase tracking-wide">Expiration (MM/YY)</label>
                      <input type="text" placeholder="MM/YY" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 text-center bg-white">
                    </div>
                     <div>
                      <label class="block text-xs font-medium text-slate-500 mb-1 uppercase tracking-wide">CVC</label>
                      <input type="text" placeholder="123" class="w-full px-4 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-brand-500 text-center bg-white">
                    </div>
                  </div>
               </div>

               <!-- Other methods -->
               <div class="border border-slate-200 hover:border-slate-300 rounded-xl p-4 flex items-center gap-3 cursor-pointer transition-colors">
                 <input type="radio" name="payment" id="paypal" class="w-5 h-5 text-brand-600 focus:ring-brand-500 border-slate-300">
                 <label for="paypal" class="font-semibold text-slate-800 flex-1 cursor-pointer">PayPal</label>
               </div>
               <div class="border border-slate-200 hover:border-slate-300 rounded-xl p-4 flex items-center gap-3 cursor-pointer transition-colors">
                 <input type="radio" name="payment" id="cash" class="w-5 h-5 text-brand-600 focus:ring-brand-500 border-slate-300">
                 <label for="cash" class="font-semibold text-slate-800 flex-1 cursor-pointer">Cash on Delivery</label>
               </div>
            </div>
          </div>
        </div>

        <!-- Order Summary Side Panel -->
        <div class="lg:w-1/3">
          <div class="bg-slate-900 rounded-2xl shadow-xl p-6 sm:p-8 text-white sticky top-24">
            <h2 class="text-xl font-bold mb-6 border-b border-slate-700 pb-4">Check Order</h2>
            
            <!-- Items tiny list -->
            <div class="space-y-4 mb-6 max-h-60 overflow-y-auto pr-2 custom-scrollbar">
              <div v-for="i in 2" :key="i" class="flex gap-3 items-center">
                 <div class="w-12 h-12 bg-white rounded border border-slate-700 flex-shrink-0 relative">
                   <img src="https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?q=80&w=100&auto=format&fit=crop" class="w-full h-full object-cover rounded opacity-80" alt="">
                   <span class="absolute -top-2 -right-2 w-5 h-5 bg-slate-700 text-xs rounded-full flex items-center justify-center font-bold border border-slate-900">1</span>
                 </div>
                 <div class="flex-1 text-sm">
                   <p class="font-semibold text-slate-200 line-clamp-1">iPad Pro 13-inch</p>
                   <p class="text-slate-400">$1,299.00</p>
                 </div>
              </div>
            </div>

            <!-- Totals -->
            <div class="space-y-3 text-sm text-slate-400 border-t border-slate-700 pt-6 mb-6">
              <div class="flex justify-between">
                <span>Subtotal</span>
                <span class="font-medium text-slate-200">$2,598.00</span>
              </div>
              <div class="flex justify-between">
                <span>Shipping</span>
                <span class="font-medium text-slate-200">Free</span>
              </div>
              <div class="flex justify-between">
                <span>Estimated Tax</span>
                <span class="font-medium text-slate-200">$207.84</span>
              </div>
            </div>

            <div class="flex justify-between items-end mb-8 pt-4 border-t border-slate-700">
              <span class="font-bold text-slate-200 text-lg">Total</span>
              <span class="font-extrabold text-3xl text-white tracking-tight">$2,805.84</span>
            </div>

            <button @click="placeOrder" class="w-full bg-brand-500 hover:bg-brand-400 text-white font-bold text-lg py-4 rounded-xl shadow-lg shadow-brand-500/30 transition-all flex items-center justify-center gap-2">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
              Pay Now
            </button>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const placeOrder = () => {
  // In a real app we'd validate the form and send to API
  alert('Payment successful! Your order has been placed.')
  router.push('/')
}
</script>

<style scoped>
/* Custom scrollbar for checkout items */
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #334155;
  border-radius: 4px;
}
</style>
