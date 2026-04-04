<template>
  <div class="bg-white rounded-xl overflow-hidden shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 h-full flex flex-col border border-slate-100">
    
    <!-- Image -->
    <router-link
      :to="`/product/${product.id}`"
      class="block relative h-64 overflow-hidden bg-slate-50 border-b border-slate-100 flex items-center justify-center p-6 group"
    >
      <img
        :src="product.image ? `https://localhost:7010${product.image}` : 'https://via.placeholder.com/150'"
        :alt="product.name"
        class="max-h-full max-w-full object-contain mix-blend-multiply transition-transform duration-500 group-hover:scale-105"
      />

      <!-- Badges -->
      <div class="absolute top-4 left-4 flex flex-col gap-2">
        <span
          v-if="product.isNew"
          class="bg-indigo-500 text-white text-xs font-bold px-3 py-1.5 rounded-full shadow-sm"
        >
          NEW
        </span>

        <span
          v-if="product.discount"
          class="bg-red-500 text-white text-xs font-bold px-3 py-1.5 rounded-full shadow-sm"
        >
          -{{ product.discount }}%
        </span>
      </div>
    </router-link>

    <!-- Content -->
    <div class="p-6 flex flex-col flex-grow">

      <!-- Category Brand Stock -->
      <div class="flex items-center gap-2 text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">
        <span class="text-blue-600 font-bold">{{ product.category }}</span>
        <span>•</span>
        <span class="text-yellow-600 font-bold">{{ product.brand }}</span>
        <span>•</span>
        <span class="text-green-600 font-bold">Còn {{ product.stock }}</span>
      </div>


      <!-- Name -->
      <router-link :to="`/product/${product.id}`">
        <h3 class="font-bold text-slate-800 text-lg mb-2 line-clamp-2 hover:text-indigo-600 transition-colors ">
          {{ product.name }}
        </h3>
      </router-link>

      <div class="flex items-center gap-2 text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">
        <span>{{ product.specifications }}</span>
      </div>

      <!-- Price -->
      <div class="flex items-center gap-3 mt-auto mb-5 ">
        <span class="text-xl font-extrabold text-slate-900">
          {{ formatCurrency(product.price) }}
        </span>
      </div>

      <!-- Button -->
      <button
        v-if="isCustomer"
        class="w-full py-3 bg-transparent border-2 border-blue-500 text-blue-500 font-semibold rounded-lg hover:bg-blue-500 hover:text-white transition-colors"
        @click="addToCart"
      >
        Thêm vào giỏ hàng
      </button>

    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useCartStore } from '@/stores/cart'

const authStore = useAuthStore() 
const cartStore = useCartStore()
const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const isCustomer = computed(() => authStore.user?.role === 'Customer')

const formatCurrency = (val) => {
  if (!val) return '0 ₫'
  return new Intl.NumberFormat('vi-VN').format(val) + ' ₫'
}

const discountedPrice = computed(() => {
  if (!props.product.discount) return props.product.price
  return props.product.price * (1 - props.product.discount / 100)
})

const addToCart = () => {
  cartStore.addToCart(props.product.id, 1)
}
</script>
