<template>
  <div class="flex flex-col">
    <HeroBanner />
    
    <section class="py-20 lg:py-28 bg-white">
      <div class="container mx-auto px-4">
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4 mb-12 border-b-2 border-slate-100 pb-6">
          <h2 class="text-3xl sm:text-4xl font-extrabold text-slate-900 tracking-tight">Featured Products</h2>
          <a href="#" class="text-blue-500 font-semibold flex items-center gap-2 hover:text-blue-700 hover:gap-3 transition-all">
            View All <span aria-hidden="true">&rarr;</span>
          </a>
        </div>
        
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
          <div v-for="product in featuredProducts" :key="product.id" class="h-full">
            <ProductCard :product="product" />
          </div>
        </div>
      </div>
    </section>

    <section class="py-20 lg:py-28 bg-slate-50">
      <div class="container mx-auto px-4">
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4 mb-12 border-b-2 border-slate-200 pb-6">
          <h2 class="text-3xl sm:text-4xl font-extrabold text-slate-900 tracking-tight">New Arrivals</h2>
          <a href="#" class="text-blue-500 font-semibold flex items-center gap-2 hover:text-blue-700 hover:gap-3 transition-all">
            View All <span aria-hidden="true">&rarr;</span>
          </a>
        </div>
        
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
          <div v-for="product in newArrivals" :key="product.id" class="h-full">
            <ProductCard :product="product" />
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import HeroBanner from '../../components/HeroBanner.vue'
import ProductCard from '../../components/ProductCard.vue'

// Mock Data
const featuredProducts = ref([
  {
    id: 1,
    name: 'MacBook Pro 16" M3 Max',
    category: 'Laptops',
    price: 3499.00,
    discount: 0,
    isNew: true,
    reviews: 45,
    image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 2,
    name: 'Sony WH-1000XM5 Headphones',
    category: 'Audio',
    price: 398.00,
    discount: 10,
    isNew: false,
    reviews: 320,
    image: 'https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 3,
    name: 'iPhone 15 Pro Max 256GB',
    category: 'Smartphones',
    price: 1199.00,
    discount: 0,
    isNew: true,
    reviews: 89,
    image: 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 4,
    name: 'Samsung Odyssey OLED G9',
    category: 'Monitors',
    price: 1799.99,
    discount: 15,
    isNew: false,
    reviews: 56,
    image: 'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?q=80&w=1000&auto=format&fit=crop'
  }
])

const newArrivals = ref([
  {
    id: 5,
    name: 'iPad Pro 13-inch (M4)',
    category: 'Tablets',
    price: 1299.00,
    discount: 0,
    isNew: true,
    reviews: 12,
    image: 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 6,
    name: 'Logitech MX Master 3S',
    category: 'Accessories',
    price: 99.99,
    discount: 0,
    isNew: true,
    reviews: 1450,
    image: 'https://images.unsplash.com/photo-1527814050087-37938154799f?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 7,
    name: 'Sony Alpha a7 IV Mirrorless',
    category: 'Cameras',
    price: 2498.00,
    discount: 5,
    isNew: false,
    reviews: 215,
    image: 'https://images.unsplash.com/photo-1516035069371-29a1b244cc32?q=80&w=1000&auto=format&fit=crop'
  },
  {
    id: 8,
    name: 'Keychron Q1 Pro Mechanical',
    category: 'Accessories',
    price: 199.00,
    discount: 0,
    isNew: true,
    reviews: 48,
    image: 'https://images.unsplash.com/photo-1595225476474-87563907a212?q=80&w=1000&auto=format&fit=crop'
  }
])
</script>
