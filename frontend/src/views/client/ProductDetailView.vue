<template>
  <div class="bg-slate-50 min-h-screen px-4 sm:px-6 lg:px-16 py-10">

    <!-- Breadcrumb -->
    <div class="text-sm text-slate-600 mb-6 flex flex-wrap gap-2 items-center">
      <router-link to="/" class="hover:text-blue-600">Trang chủ</router-link>
      <span>/</span>
      <router-link to="/products" class="hover:text-blue-600">Sản phẩm</router-link>
      <span>/</span>
      <span
        v-if="product?.category"
        @click="goToProductsWithFilter('category', product.categoryId)"
        class="cursor-pointer hover:underline"
      >
        {{ product.category }}
      </span>
      <span v-if="product?.category && product?.brand">/</span>
      <span
        v-if="product?.brand"
        @click="goToProductsWithFilter('brand', product.brandId)"
        class="cursor-pointer hover:underline"
      >
        {{ product.brand }}
      </span>
      <span>/</span>
      <span class="text-slate-800 font-medium truncate max-w-xs">
        {{ product?.name || 'Đang tải...' }}
      </span>
    </div>

    <!-- Loading -->
    <div v-if="!product" class="text-center py-32">
      <div class="text-slate-400 text-lg font-medium animate-pulse">
        ⏳ Đang tải sản phẩm...
      </div>
    </div>

    <!-- Product Detail -->
    <div v-else class="bg-white rounded-3xl shadow-lg lg:flex lg:gap-6 overflow-hidden">

      <!-- Image -->
      <div class="lg:w-1/2 flex justify-center items-center p-6">
        <img
          :src="product.image ? `https://localhost:7010${product.image}` : 'https://via.placeholder.com/400'"
          class="rounded-2xl object-contain max-h-[600px] hover:scale-105 transition"
        />
      </div>

      <!-- Info -->
      <div class="lg:w-1/2 p-8 flex flex-col gap-6">

        <div class="text-sm text-slate-600 uppercase">
          <span class="text-red-500 font-bold">{{ product.category }}</span>
          <span> • </span>
          <span class="text-green-600 font-bold">{{ product.brand }}</span>
        </div>

        <h1 class="text-4xl font-extrabold text-slate-900">
          {{ product.name }}
        </h1>

        <div class="text-3xl font-bold text-blue-600">
          {{ formatCurrency(product.price) }}
        </div>

        <div>
          <span v-if="product.stock > 0" class="text-green-600 font-semibold">
            ✔ Còn hàng ({{ product.stock }})
          </span>
          <span v-else class="text-red-500 font-semibold">
            ✖ Hết hàng
          </span>
        </div>

        <div class="text-slate-700 space-y-2">
          <p><b>Mô tả:</b> {{ product.description }}</p>
          <p><b>Thông số:</b> {{ product.specifications }}</p>
        </div>

        <!-- Action -->
        
        <div v-if="product.stock > 0 && !isAdmin" class="flex gap-4 mt-auto" >
          <input
            type="number"
            v-model.number="quantity"
            min="1"
            :max="product.stock"
            class="w-24 border rounded-lg text-center py-2"
          />

          <button
            @click="addToCart"
            :disabled="product.stock === 0"
            class="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-lg font-semibold disabled:opacity-50"
          >
            🛒 Thêm vào giỏ
          </button>
        </div>

        <div v-else-if="product.stock <= 0 && !isAdmin" class="text-sm text-red-500 bold">
          Sản phẩm này hiện đang hết hàng. Vui lòng quay lại sau hoặc chọn sản phẩm khác.
        </div>

        <div v-else-if="isAdmin" class="text-sm text-slate-500 italic">
          Đăng nhập với tài khoản khách hàng để mua sản phẩm này.
        </div>

      </div>
    </div>

    <div v-if="productStore.suggestedProducts.length > 0" class="mt-12">
      <h2 class="text-2xl font-bold mb-6 text-gray-800 border-b pb-2">
        Sản phẩm liên quan
      </h2>

      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        <ProductCard
          v-for="p in productStore.suggestedProducts"
          :key="p.productId"
          :product="mapProduct(p)"
        />
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useClientProductStore } from '@/stores/client_products'
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/authStore'
import ProductCard from '@/components/ProductCard.vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()
const productStore = useClientProductStore()
const authStore = useAuthStore()
const cartStore = useCartStore()

const product = ref(null)
const quantity = ref(1)

const isGuest = computed(() => !authStore.user)
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const formatCurrency = (val) => {
  if (val == null) return '0 ₫'
  return new Intl.NumberFormat('vi-VN').format(val) + ' ₫'
}

const mapProduct = (p) => ({
  id: p.productId,
  name: p.productName,
  price: p.price,
  image: p.imageURL,
  category: p.categoryName,  // sửa đây
  brand: p.brandName,        // sửa đây
  stock: p.stockQuantity,
})

const fetchProduct = async (id) => {
  product.value = null 
  try {
    const res = await productStore.fetchProductById(id)
    product.value = {
      id: res.productId,
      name: res.productName,
      price: res.price,
      image: res.imageURL,
      category: res.category?.name,
      categoryId: res.category?.id,
      brandId: res.brand?.id,
      brand: res.brand?.name,
      stock: res.stockQuantity,
      description: res.description,
      specifications: res.specifications,
    }

    productStore.suggestedCategories = res.category?.id
    await productStore.fetchSuggestedProducts()
  } catch (err) {
    console.error('Lỗi fetchProduct:', err)
  }
}

onMounted(() => fetchProduct(route.params.id))

watch(
  () => route.params.id,
  (newId) => {
    fetchProduct(newId)
    quantity.value = 1 
  }
)

const addToCart = async () => {
  if (!product.value) return

  if (quantity.value < 1 || quantity.value > product.value.stock) {
    alert(`Vui lòng chọn số lượng từ 1 đến ${product.value.stock}`)
    return
  }

  if (isGuest.value) {
    router.push('/login')
    return
  }
  await cartStore.addToCart(product.value.id, quantity.value)
  alert("Đã thêm vào giỏ hàng")
}

const goToProductsWithFilter = (type, id) => {
  if (type === 'category') {
    productStore.selectedCategory = id
    productStore.selectedBrand = null
  } else {
    productStore.selectedBrand = id
    productStore.selectedCategory = null
  }
  productStore.page = 1
  router.push('/products')
}
</script>
