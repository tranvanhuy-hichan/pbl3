<template>
  <div class="flex flex-col">
    <HeroBanner />

    <!-- Featured -->
    <section class="py-20 lg:py-28 bg-blue-50">
      <div class="container mx-auto px-4">
        <div class="flex justify-between mb-12 border-b border-blue-200 pb-6">
          <h2 class="text-3xl font-bold text-blue-700">Sản phẩm nổi bật</h2>

          <router-link
            to="/products"
            class="text-blue-500 font-semibold hover:text-blue-700"
          >
            Xem tất cả →
          </router-link>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
          <ProductCard
            v-for="product in featuredProducts"
            :key="product.id"
            :product="product"
            class="hover:shadow-xl hover:-translate-y-1 transition-all duration-300"
          />
        </div>
      </div>
    </section>

    <!-- New Arrivals -->
    <section class="py-20 lg:py-28 bg-red-50">
      <div class="container mx-auto px-4">
        <div class="flex justify-between mb-12 border-b border-blue-200 pb-6">
          <h2 class="text-3xl font-bold text-blue-700">Sản phẩm mới</h2>

          <router-link
            to="/products"
            class="text-blue-500 font-semibold hover:text-blue-700"
          >
            Xem tất cả →
          </router-link>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
          <ProductCard
            v-for="product in newArrivals"
            :key="product.id"
            :product="product"
            class="hover:shadow-xl hover:-translate-y-1 transition-all duration-300"
          />
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { onMounted, computed } from 'vue'
import { useClientProductStore } from '@/stores/client_products'
import HeroBanner from '@/components/HeroBanner.vue'
import ProductCard from '@/components/ProductCard.vue'

const productStore = useClientProductStore()

onMounted(async () => {
  await productStore.fetchProducts()
})

// map API → UI
const mapProduct = (p) => ({
  id: p.productId,
  name: p.productName,
  image: p.imageURL,
  price: p.price,
  category: p.categoryName,
  brand: p.brandName,
  specifications: p.specifications,
  stock: p.stockQuantity,
  isNew:
    p.createdAt
      ? (new Date() - new Date(p.createdAt)) / (1000 * 60 * 60 * 24) < 14
      : false,
  reviews: 0
})

// Featured (lấy 8 sản phẩm đầu)
const featuredProducts = computed(() => {
  return productStore.products
    .sort(() => Math.random() - 0.5) 
    .slice(0, 8)
    .map(mapProduct)
})

// New Arrivals (mới nhất theo ngày)
const newArrivals = computed(() => {
  return [...productStore.products]
    .sort((a, b) => {
      const dateA = new Date(a.createdAt || a.createdDate || 0)
      const dateB = new Date(b.createdAt || b.createdDate || 0)
      return dateB - dateA
    })
    .slice(0, 12)
    .map(mapProduct)
})
</script>
