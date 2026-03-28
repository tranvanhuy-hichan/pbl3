<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      
      <!-- Breadcrumb -->
      <nav class="flex mb-8" aria-label="Breadcrumb">
        <ol class="inline-flex items-center space-x-1 md:space-x-3 text-sm text-slate-500">
          <li class="inline-flex items-center">
            <router-link to="/" class="hover:text-brand-600 transition-colors">Home</router-link>
          </li>
          <li>
            <div class="flex items-center">
              <svg class="w-4 h-4 mx-1" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path></svg>
              <span class="hover:text-brand-600 transition-colors cursor-pointer">{{ product?.category || 'Category' }}</span>
            </div>
          </li>
          <li aria-current="page">
            <div class="flex items-center">
              <svg class="w-4 h-4 mx-1" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-slate-400">{{ product?.name || 'Loading...' }}</span>
            </div>
          </li>
        </ol>
      </nav>

      <div v-if="product" class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 p-8">
          
          <!-- Product Image Gallery -->
          <div class="space-y-4">
            <div class="aspect-square bg-slate-50 rounded-xl overflow-hidden border border-slate-100 flex items-center justify-center p-4">
              <img :src="product.image" :alt="product.name" class="max-w-full max-h-full object-contain mix-blend-multiply">
            </div>
            <!-- Thumbnails -->
            <div class="grid grid-cols-4 gap-4">
              <button v-for="i in 4" :key="i" class="aspect-square bg-slate-50 rounded-lg border-2 border-transparent hover:border-brand-500 focus:border-brand-500 transition-colors overflow-hidden p-2">
                <img :src="product.image" alt="Thumbnail" class="w-full h-full object-cover mix-blend-multiply opacity-70 hover:opacity-100">
              </button>
            </div>
          </div>

          <!-- Product Info -->
          <div class="flex flex-col">
            <div class="mb-2 flex items-center gap-2">
               <span class="px-2.5 py-1 text-xs font-bold uppercase tracking-wider text-brand-700 bg-brand-50 rounded-full">
                  {{ product.category }}
               </span>
               <span v-if="product.stock > 0" class="flex items-center text-xs font-semibold text-green-600">
                  <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path></svg>
                  In Stock ({{ product.stock }})
               </span>
               <span v-else class="flex items-center text-xs font-semibold text-red-600">
                  <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path></svg>
                  Out of Stock
               </span>
            </div>

            <h1 class="text-3xl sm:text-4xl font-extrabold text-slate-900 tracking-tight mb-4">
              {{ product.name }}
            </h1>

            <div class="flex items-end gap-4 mb-6">
              <span class="text-4xl font-bold text-brand-600">
                ${{ product.price.toFixed(2) }}
              </span>
              <span v-if="product.price > 1000" class="text-lg text-slate-400 line-through mb-1">
                ${{ (product.price * 1.2).toFixed(2) }}
              </span>
            </div>

            <p class="text-slate-600 mb-8 leading-relaxed">
              Experience the pinnacle of technology with the {{ product.name }}. Designed for ultimate performance and portability, this device features state-of-the-art architecture, a stunning display, and all-day battery life. Perfect for professionals and enthusiasts alike.
            </p>

            <div class="border-t border-slate-100 py-6 mb-6">
              <h3 class="font-semibold text-slate-800 mb-4">Color</h3>
              <div class="flex gap-3">
                <button class="w-10 h-10 rounded-full bg-slate-800 ring-2 ring-offset-2 ring-brand-500 outline-none"></button>
                <button class="w-10 h-10 rounded-full bg-slate-200 ring-2 ring-offset-2 ring-transparent hover:ring-slate-300 outline-none"></button>
                <button class="w-10 h-10 rounded-full bg-slate-400 ring-2 ring-offset-2 ring-transparent hover:ring-slate-300 outline-none"></button>
              </div>
            </div>

            <div class="mt-auto flex flex-col sm:flex-row gap-4">
              <div class="flex items-center border border-slate-300 rounded-xl overflow-hidden bg-white w-full sm:w-32 flex-shrink-0">
                <button @click="quantity > 1 && quantity--" class="px-4 py-3 text-slate-500 hover:bg-slate-50 hover:text-brand-600 transition-colors">&minus;</button>
                <input type="number" v-model.number="quantity" class="w-full text-center font-semibold text-slate-800 focus:outline-none" min="1" :max="product.stock">
                <button @click="quantity < product.stock && quantity++" class="px-4 py-3 text-slate-500 hover:bg-slate-50 hover:text-brand-600 transition-colors">&plus;</button>
              </div>
              
              <button 
                @click="addToCart"
                :disabled="product.stock === 0"
                class="flex-1 bg-brand-500 hover:bg-brand-600 disabled:bg-slate-300 disabled:cursor-not-allowed text-white px-8 py-3.5 rounded-xl font-bold text-lg shadow-sm shadow-brand-200 transition-all flex items-center justify-center gap-2"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path></svg>
                <span>{{ product.stock > 0 ? 'Add to Cart' : 'Out of Stock' }}</span>
              </button>
            </div>

            <!-- Perks -->
            <div class="grid grid-cols-2 gap-4 mt-8 pt-6 border-t border-slate-100">
              <div class="flex items-center gap-3 text-sm text-slate-600">
                <svg class="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path></svg>
                <span>Free shipping on orders over $50</span>
              </div>
              <div class="flex items-center gap-3 text-sm text-slate-600">
                <svg class="w-5 h-5 text-brand-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"></path></svg>
                <span>1 Year Warranty included</span>
              </div>
            </div>

          </div>
        </div>
      </div>
      
      <!-- Loading State -->
      <div v-else class="py-20 text-center text-slate-500">
        <svg class="animate-spin h-10 w-10 text-brand-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <p>Loading product details...</p>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProductStore } from '../../stores/products'

const route = useRoute()
const productStore = useProductStore()
const product = ref(null)
const quantity = ref(1)

onMounted(() => {
  // In a real app we'd fetch this from API by route.params.id
  // Here we use the mock store
  const productId = route.params.id
  if (productStore.products.length > 0) {
     product.value = productStore.products.find(p => p.id === productId) || productStore.products[0] // fallback to first item
  }
})

const addToCart = () => {
  if (!product.value) return
  alert(`Added ${quantity.value}x ${product.value.name} to cart!`)
  // Later: call cartStore.addToCart(product, quantity)
}
</script>
