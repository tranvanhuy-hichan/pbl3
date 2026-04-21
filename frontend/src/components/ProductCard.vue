<template>
  <div class="bg-white rounded-lg overflow-hidden shadow-sm hover:shadow-md hover:-translate-y-1 transition-all duration-300 h-full flex flex-col border border-slate-200">
    
    <router-link
      :to="`/product/${product.id}`"
      class="block relative h-48 overflow-hidden bg-slate-50 border-b border-slate-100 flex items-center justify-center p-4 group"
    >
      <img
        :src="product.image ? `https://localhost:7010${product.image}` : 'https://via.placeholder.com/150'"
        :alt="product.name"
        class="max-h-full max-w-full object-contain transition-transform duration-500 group-hover:scale-105 dark:mix-blend-normal"
      />

      <div class="absolute top-2 left-2 flex flex-col gap-1">
        <span
          v-if="product.isNew"
          class="bg-indigo-500 text-white text-[10px] font-bold px-2 py-0.5 rounded shadow-sm"
        >
          NEW
        </span>
        <span
          v-if="product.discount"
          class="bg-red-500 text-white text-[10px] font-bold px-2 py-0.5 rounded shadow-sm"
        >
          -{{ product.discount }}%
        </span>
      </div>
    </router-link>

    <div class="p-4 flex flex-col flex-grow">

      <div class="flex items-center justify-between text-[10px] font-bold uppercase tracking-tight mb-1.5">
  
        <div class="flex items-center gap-1.5">
          <span class="text-blue-600">{{ product.category }}</span>
          <span class="text-slate-300">•</span>
          <span class="text-amber-600">{{ product.brand }}</span>
        </div>

        <div>
          <span v-if="product.stock > 0" class="text-green-600">
            Còn: {{ product.stock }}
          </span>
          <span v-else class="text-red-500">
            Hết hàng
          </span>
        </div>
      </div>

      <router-link :to="`/product/${product.id}`" class="mb-1">
        <h3 class="font-bold text-slate-800 text-sm leading-snug line-clamp-2 h-10 hover:text-blue-600 transition-colors">
          {{ product.name }}
        </h3>
      </router-link>

      <div class="flex items-center gap-2 mt-auto mb-3">
        <span class="text-base font-bold text-slate-900">
          {{ formatCurrency(product.price) }}
        </span>
        <span v-if="product.discount" class="text-[10px] text-slate-400 line-through">
           {{ formatCurrency(product.price * 1.2) }} 
        </span>
      </div>

      <button
        v-if="!isAdmin && product.stock > 0"
        class="w-full py-2 bg-white border border-blue-500 text-blue-500 text-sm font-bold rounded-md hover:bg-blue-500 hover:text-white transition-all active:scale-95"
        @click="addToCart"
      >
        Thêm vào giỏ
      </button>

      <div
        v-else-if="!isAdmin && product.stock <= 0"
        class="w-full py-2 text-center text-sm text-red-500 italic border border-gray-300 rounded-md"
      >
        Hết hàng
      </div>        

      <div v-else-if="isAdmin" class="text-sm text-slate-500 italic">
        Đăng nhập với tài khoản khách hàng để mua sản phẩm này.
      </div>

    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useCartStore } from '@/stores/cart'
import { useRouter } from 'vue-router'

const authStore = useAuthStore() 
const router = useRouter()

const cartStore = useCartStore()
const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const isGuest = computed(() => !authStore.user)
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const formatCurrency = (val) => {
  if (!val) return '0₫'
  return new Intl.NumberFormat('vi-VN').format(val) + '₫'
}

const addToCart = () => {
  if (isGuest.value) {
    router.push('/login')
    return
  }
  cartStore.addToCart(props.product.id, 1)
}
</script>