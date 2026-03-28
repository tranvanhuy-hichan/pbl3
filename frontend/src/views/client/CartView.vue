<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      
      <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight mb-8">Shopping Cart</h1>

      <div class="flex flex-col lg:flex-row gap-8">
        
        <!-- Cart Items List -->
        <div class="lg:w-2/3">
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
            <!-- Table Header -->
            <div class="hidden sm:grid grid-cols-12 gap-4 p-4 border-b border-slate-100 bg-slate-50 text-xs font-semibold text-slate-500 uppercase tracking-wider">
              <div class="col-span-6">Product</div>
              <div class="col-span-2 text-center">Price</div>
              <div class="col-span-2 text-center">Quantity</div>
              <div class="col-span-2 text-right">Total</div>
            </div>

            <!-- Cart Items -->
            <div class="divide-y divide-slate-100">
              <!-- Item 1 -->
              <div v-for="item in cartItems" :key="item.id" class="p-4 sm:p-6 grid grid-cols-1 sm:grid-cols-12 gap-4 items-center hover:bg-slate-50 transition-colors">
                
                <!-- Product Info -->
                <div class="col-span-1 sm:col-span-6 flex items-center gap-4">
                  <button class="text-slate-400 hover:text-red-500 transition-colors p-1" title="Remove item">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                  </button>
                  <div class="w-20 h-20 sm:w-24 sm:h-24 bg-slate-100 rounded-xl overflow-hidden flex-shrink-0 border border-slate-200">
                    <img :src="item.image" :alt="item.name" class="w-full h-full object-contain mix-blend-multiply p-2">
                  </div>
                  <div>
                    <h3 class="font-bold text-slate-800 line-clamp-2"><router-link :to="`/product/${item.id}`" class="hover:text-brand-600">{{ item.name }}</router-link></h3>
                    <p class="text-sm text-slate-500 mt-1">Category: {{ item.category }}</p>
                    <div class="sm:hidden mt-2 font-semibold text-brand-600">${{ item.price.toFixed(2) }}</div>
                  </div>
                </div>

                <!-- Price (Desktop) -->
                <div class="hidden sm:block col-span-2 text-center font-semibold text-slate-700">
                  ${{ item.price.toFixed(2) }}
                </div>

                <!-- Quantity Control -->
                <div class="col-span-1 sm:col-span-2 flex justify-center">
                  <div class="flex items-center border border-slate-300 rounded-lg overflow-hidden bg-white w-28">
                    <button class="px-3 py-2 text-slate-500 hover:bg-slate-50 transition-colors">&minus;</button>
                    <input type="number" :value="item.quantity" class="w-full text-center font-semibold text-slate-800 focus:outline-none text-sm" readonly>
                    <button class="px-3 py-2 text-slate-500 hover:bg-slate-50 transition-colors">&plus;</button>
                  </div>
                </div>

                <!-- Item Total -->
                <div class="hidden sm:block col-span-2 text-right font-bold text-slate-900">
                  ${{ (item.price * item.quantity).toFixed(2) }}
                </div>
              </div>
            </div>

            <div v-if="cartItems.length === 0" class="p-12 text-center text-slate-500">
               <svg class="w-16 h-16 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path></svg>
               <h3 class="text-xl font-bold text-slate-700 mb-2">Your cart is empty</h3>
               <p class="mb-6 text-sm">Looks like you haven't added anything to your cart yet.</p>
               <router-link to="/" class="inline-block bg-brand-500 hover:bg-brand-600 text-white font-semibold py-2.5 px-6 rounded-lg transition-colors">
                 Continue Shopping
               </router-link>
            </div>

            <!-- Cart Footer Actions -->
            <div v-if="cartItems.length > 0" class="p-4 border-t border-slate-100 bg-slate-50 flex justify-between items-center">
              <div class="relative max-w-xs w-full hidden sm:block">
                <input type="text" placeholder="Promo code" class="w-full pl-4 pr-20 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 text-sm">
                <button class="absolute right-1 top-1 bottom-1 px-3 bg-slate-800 text-white rounded text-xs font-semibold hover:bg-slate-700 transition-colors">APPLY</button>
              </div>
              <button class="text-sm font-semibold text-slate-500 hover:text-red-500 transition-colors">
                Clear Cart
              </button>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="lg:w-1/3">
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sticky top-24">
            <h2 class="text-xl font-bold text-slate-800 mb-6 border-b border-slate-100 pb-4">Order Summary</h2>
            
            <div class="space-y-4 text-slate-600 border-b border-slate-100 pb-4 mb-4">
              <div class="flex justify-between">
                <span>Subtotal ({{ cartItems.reduce((acc, item) => acc + item.quantity, 0) }} items)</span>
                <span class="font-medium text-slate-800">${{ cartSubtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between">
                <span>Shipping Estimate</span>
                <span class="font-medium text-slate-800">{{ shippingCost === 0 ? 'Free' : '$' + shippingCost.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between">
                <span>Tax (8%)</span>
                <span class="font-medium text-slate-800">${{ taxAmount.toFixed(2) }}</span>
              </div>
            </div>

            <div class="flex justify-between items-end mb-8">
              <span class="font-bold text-slate-800 text-lg">Total</span>
              <span class="font-extrabold text-2xl text-brand-600">${{ orderTotal.toFixed(2) }}</span>
            </div>

            <router-link to="/checkout" :class="[
              'w-full py-4 rounded-xl font-bold flex justify-center items-center shadow-sm transition-all text-center',
              cartItems.length > 0 ? 'bg-brand-500 hover:bg-brand-600 text-white shadow-brand-200' : 'bg-slate-200 text-slate-400 cursor-not-allowed pointer-events-none'
            ]">
              Proceed to Checkout
            </router-link>

            <div class="mt-4 flex items-center justify-center gap-2 text-xs text-slate-400">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
              Secure Checkout & Encrypted payments
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

// Mock Data for UI Preview
const cartItems = ref([
  { id: 'P001', name: 'MacBook Pro 16" M3 Max', category: 'Laptops', price: 3499.00, quantity: 1, image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=100&auto=format&fit=crop' },
  { id: 'P002', name: 'Sony WH-1000XM5 Noise Cancelling Headphones', category: 'Audio', price: 398.00, quantity: 2, image: 'https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?q=80&w=100&auto=format&fit=crop' },
])

const cartSubtotal = computed(() => {
  return cartItems.value.reduce((total, item) => total + (item.price * item.quantity), 0)
})

const shippingCost = computed(() => cartSubtotal.value > 1000 ? 0 : 25)
const taxAmount = computed(() => cartSubtotal.value * 0.08)
const orderTotal = computed(() => cartSubtotal.value + shippingCost.value + taxAmount.value)
</script>
