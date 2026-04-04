<template>
  <div class="bg-blue-50 min-h-screen px-4 sm:px-6 lg:px-16 py-10">

    <!-- Breadcrumb -->
    <div class="text-sm text-slate-600 mb-6 flex flex-wrap gap-2">
      <router-link to="/" class="hover:text-blue-600">Trang chủ</router-link>
      <span>/</span>
      <router-link to="/products" class="hover:text-blue-600">Sản phẩm</router-link>
      <span>/</span>
      <span class="text-slate-800 font-medium truncate max-w-xs">{{ product?.name }}</span>
    </div>

    <!-- Product Card -->
    <div v-if="product" class="bg-white rounded-3xl shadow-lg lg:flex lg:gap-6 overflow-hidden">

      <!-- Left: Image -->
      <div class="lg:w-1/2 flex justify-center items-center bg-white p-6">
        <img
          :src="product.image ? `https://localhost:7010${product.image}` : 'https://via.placeholder.com/400'"
          alt="Product Image"
          class="rounded-2xl object-contain max-h-[600px] transition-transform duration-300 hover:scale-105"
        />
      </div>

      <!-- Right: Info -->
      <div class="lg:w-1/2 p-8 flex flex-col gap-6">
        <div class="text-sm text-slate-600 uppercase tracking-wide">
          <span class="text-red-600 font-bold">{{ product.category }}</span>
          <span> • </span>
          <span class="text-green-600 font-bold">{{ product.brand }}</span>
        </div>

        <h1 class="text-4xl font-extrabold text-slate-900">{{ product.name }}</h1>

        <div class="text-3xl font-bold text-blue-700">{{ formatCurrency(product.price) }}</div>

        <div>
          <span v-if="product.stock > 0" class="text-green-600 font-semibold">Còn hàng ({{ product.stock }})</span>
          <span v-else class="text-red-500 font-semibold">Hết hàng</span>
        </div>

        <div class="text-slate-700 space-y-2">
          <p><span class="font-semibold">Mô tả:</span> {{ product.description }}</p>
          <p><span class="font-semibold">Thông số kỹ thuật:</span> {{ product.specifications }}</p>
        </div>

        <!-- Quantity & Button -->
        <div class="flex gap-4 mt-auto">
          <input
            type="number"
            v-model.number="quantity"
            min="1"
            :max="product.stock"
            class="w-24 border rounded-lg text-center py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          />
          <button
            @click="addToCart"
            :disabled="product.stock === 0"
            class="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-lg font-semibold shadow-md transition-colors duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            🛒 Thêm vào giỏ hàng
          </button>
        </div>
      </div>

    </div>

    <!-- Loading -->
    <div v-else class="text-center py-32 text-slate-400 text-lg font-medium">
      Đang tải sản phẩm...
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useClientProductStore } from '@/stores/client_products'
import { useCartStore } from '@/stores/cart'

const cartStore = useCartStore()
const productStore = useClientProductStore()
const route = useRoute()

const product = ref(null)
const quantity = ref(1)

const formatCurrency = (val) => {
  if (!val) return '0 ₫'
  return new Intl.NumberFormat('vi-VN').format(val) + ' ₫'
}

onMounted(async () => {
  try {
    const id = route.params.id
    const res = await productStore.fetchProductById(id)

    product.value = {
      id: res.productId,
      name: res.productName,
      price: res.price,
      image: res.imageURL,
      category: res.category.name,
      brand: res.brand.name,
      stock: res.stockQuantity,
      description: res.description,
      specifications: res.specifications,
    }
  } catch (err) {
    console.error(err)
  }
})

const addToCart = async () => {
  try {
    await cartStore.addToCart(product.value.id, quantity.value)
    alert(`Đã thêm ${quantity.value} x ${product.value.name} vào giỏ hàng`)
  } catch (err) {
    console.error(err)
  }
}
</script>
