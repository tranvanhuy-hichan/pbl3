<template>
  <div class="bg-white rounded-xl overflow-hidden shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 h-full flex flex-col border border-slate-100">
    <router-link :to="`/product/${product.id}`" class="block relative h-64 overflow-hidden bg-slate-50 border-b border-slate-100 flex items-center justify-center p-6 group-hover:bg-slate-100 transition-colors">
      <img :src="product.image" :alt="product.name" class="max-h-full max-w-full object-contain mix-blend-multiply transition-transform duration-500 group-hover:scale-105" />
      
      <!-- Badges -->
      <div class="absolute top-4 left-4 flex flex-col gap-2">
        <span v-if="product.isNew" class="bg-brand-500 text-white text-xs font-bold px-3 py-1.5 rounded-full shadow-sm">NEW</span>
        <span v-if="product.discount" class="bg-red-500 text-white text-xs font-bold px-3 py-1.5 rounded-full shadow-sm">{{ product.discount }}% OFF</span>
      </div>
    </router-link>
    
    <div class="p-6 flex flex-col flex-grow">
      <div class="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">{{ product.category }}</div>
      
      <router-link :to="`/product/${product.id}`">
        <h3 class="font-bold text-slate-800 text-lg mb-2 line-clamp-2 hover:text-brand-600 transition-colors min-h-[56px]">{{ product.name }}</h3>
      </router-link>
      
      <div class="flex items-center gap-2 mb-4">
        <span class="text-amber-400 tracking-widest text-sm">★★★★★</span>
        <span class="text-sm text-slate-400">({{ product.reviews || 0 }})</span>
      </div>
      
      <div class="flex items-center gap-3 mt-auto mb-5">
        <span class="text-xl font-extrabold text-slate-900">${{ discountedPrice }}</span>
        <span v-if="product.discount" class="text-sm text-slate-400 line-through">${{ product.price }}</span>
      </div>
      
      <button class="w-full py-3 bg-transparent border-2 border-blue-500 text-blue-500 font-semibold rounded-lg hover:bg-blue-500 hover:text-white transition-colors" @click="addToCart">
        Add to Cart
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  product: {
    type: Object,
    required: true,
    default: () => ({
      id: 1,
      name: 'Premium Wireless Headphones',
      category: 'Audio',
      price: 299.99,
      discount: 15,
      isNew: true,
      reviews: 124,
      image: ''
    })
  }
})

const discountedPrice = computed(() => {
  if (!props.product.discount) return props.product.price
  return (props.product.price * (1 - props.product.discount / 100)).toFixed(2)
})

const addToCart = () => {
  console.log(`Added ${props.product.name} to cart`)
}
</script>
