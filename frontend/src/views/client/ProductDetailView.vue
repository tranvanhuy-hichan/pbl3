<template>
  <div class="bg-blue-50 min-h-screen">

    <!-- Breadcrumb -->
    <div class="px-6 lg:px-16 py-4 text-sm text-slate-500 bg-blue-50 border-b">
      <router-link to="/" class="hover:text-indigo-600">Trang chủ</router-link>
      <span class="mx-2">/</span>
      <router-link to="/products" class="hover:text-indigo-600">
        Sản phẩm
      </router-link>
      <span class="mx-2">/</span>
      <span class="text-slate-700 font-medium">{{ product?.name }}</span>
    </div>

    <!-- Content FULL -->
    <div v-if="product"
      class="grid md:grid-cols-2 gap-10 px-6 lg:px-16 py-10">

      <!-- Image -->
      <div class="bg-white rounded-2xl flex items-center justify-center p-10 shadow-sm">
        <img :src="product.image" class="max-h-[450px] object-contain" />
      </div>

      <!-- Info -->
      <div class="bg-white rounded-2xl p-8 shadow-sm flex flex-col">

        <div class="text-sm text-slate-500 mb-2 uppercase">
          {{ product.category }} • {{ product.brand }}
        </div>

        <h1 class="text-3xl font-bold mb-4 text-slate-800">
          {{ product.name }}
        </h1>

        <div class="text-2xl font-bold text-blue-500 mb-6">
          {{ formatCurrency(product.price) }}
        </div>

        <p class="text-slate-600 mb-6">
          Mô tả: {{ product.description }}
        </p>

        <p class="text-slate-600 mb-6">
          Thông số kỹ thuật: {{ product.specifications }}
        </p>

        <!-- Stock -->
        <div class="mb-6">
          <span v-if="product.stock > 0" class="text-green-600 font-semibold">
            Còn hàng ({{ product.stock }})
          </span>
          <span v-else class="text-red-500 font-semibold">
            Hết hàng
          </span>
        </div>

        <!-- Action -->
        <div class="flex gap-4 mt-auto">
          <input
            type="number"
            v-model.number="quantity"
            min="1"
            :max="product.stock"
            class="w-20 border rounded-lg text-center py-2"
          />

          <button
            @click="addToCart"
            :disabled="product.stock === 0"
            class="flex-1 bg-blue-500 text-white py-3 rounded-lg font-semibold hover:bg-blue-600"
          >
            🛒 Thêm vào giỏ hàng
          </button>
        </div>

      </div>
    </div>

    <!-- Loading -->
    <div v-else class="text-center py-32 text-slate-400">
      Loading...
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProductStore } from '@/stores/products'
import { useCartStore } from '@/stores/cart'

const cartStore = useCartStore()

const route = useRoute()
const productStore = useProductStore()

const product = ref(null)
const quantity = ref(1)

// format VND
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
      image: res.image,
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
  // try {
  //   await cartStore.addToCart(product.value.id, quantity.value)

  //   alert(`Đã thêm ${quantity.value} x ${product.value.name}`)
  // } catch (err) {
  //   console.error(err)
  // }
}

</script>
