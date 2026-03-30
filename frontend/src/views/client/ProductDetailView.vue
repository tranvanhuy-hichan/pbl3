<template>
  <div class="bg-slate-50 min-h-screen py-16">
    <div class="max-w-6xl mx-auto px-6">

      <!-- Breadcrumb -->
      <div class="text-base text-slate-500 mb-8">
        Home / {{ product?.category }} / {{ product?.name }}
      </div>

      <!-- Content -->
      <div v-if="product" class="bg-white rounded-3xl shadow-md border p-10 grid md:grid-cols-2 gap-12">

        <!-- Image -->
        <div class="bg-slate-50 rounded-2xl flex items-center justify-center p-10">
          <img :src="product.image" class="max-h-[400px] object-contain" />
        </div>

        <!-- Info -->
        <div class="flex flex-col">

          <!-- Category + Brand -->
          <div class="text-sm text-slate-500 mb-3 uppercase tracking-wide">
            {{ product.category }} • {{ product.brand }}
          </div>

          <!-- Name -->
          <h1 class="text-4xl font-extrabold mb-6 leading-tight">
            {{ product.name }}
          </h1>

          <!-- Price -->
          <div class="text-3xl font-bold text-indigo-600 mb-8">
            {{ formatCurrency(product.price) }}
          </div>

          <!-- Description -->
          <p class="text-lg text-slate-600 mb-8 leading-relaxed">
            Mô tả: {{ product.description }}
          </p>

          <!-- Specifications -->
          <p class="text-lg text-slate-600 mb-8 leading-relaxed">
            Thông số kỹ thuật: {{ product.specifications }}
          </p>

          <!-- Stock -->
          <div class="mb-8 text-base">
            <span v-if="product.stock > 0" class="text-green-600 font-semibold">
              ✔ Còn hàng ({{ product.stock }})
            </span>
            <span v-else class="text-red-500 font-semibold">
              ✖ Hết hàng
            </span>
          </div>

          <!-- Quantity + Button -->
          <div class="flex gap-5 mt-auto">

            <input
              type="number"
              v-model.number="quantity"
              min="1"
              :max="product.stock"
              class="w-24 text-lg border rounded-xl text-center py-2"
            />

            <button
              @click="addToCart"
              :disabled="product.stock === 0"
              class="flex-1 bg-indigo-600 text-white py-3 rounded-xl text-lg font-semibold hover:bg-indigo-700 disabled:bg-gray-300"
            >
              Thêm vào giỏ hàng
            </button>

          </div>

        </div>

      </div>

      <!-- Loading -->
      <div v-else class="text-center py-32 text-slate-500 text-lg">
        Loading...
      </div>

    </div>
  </div>
</template>


<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProductStore } from '@/stores/products'

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

const addToCart = () => {
  alert(`Đã thêm ${quantity.value} x ${product.value.name}`)
}
</script>
