<template>
  <div class="bg-slate-50 min-h-screen px-4 sm:px-6 lg:px-16 py-10">

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

    <div v-if="!product" class="text-center py-32">
      <div class="text-slate-400 text-lg font-medium animate-pulse">
        Đang tải sản phẩm...
      </div>
    </div>

    <div v-else class="bg-white rounded-3xl shadow-lg lg:flex lg:gap-6 overflow-hidden">

      <div class="lg:w-1/2 flex justify-center items-center p-6">
        <img
          :src="product.image ? `https://localhost:7010${product.image}` : 'https://via.placeholder.com/400'"
          class="rounded-2xl object-contain max-h-[600px] hover:scale-105 transition"
        />
      </div>

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
            Còn hàng ({{ product.stock }})
          </span>
          <span v-else class="text-red-500 font-semibold">
            Hết hàng
          </span>
        </div>

        <div class="text-slate-700 space-y-2">
          <p><b>Mô tả:</b> {{ product.description }}</p>
          <p><b>Thông số:</b> {{ product.specifications }}</p>
        </div>

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

    <div class="mt-8 bg-white rounded-3xl shadow-lg p-8">
      <h2 class="text-2xl font-bold text-slate-800 mb-6 border-b pb-3">
        Đánh giá sản phẩm
      </h2>

      <div v-if="reviewStore.reviews.length > 0" class="flex items-center gap-4 mb-8">
        <div class="text-5xl font-extrabold text-blue-600">
          {{ avgRating.toFixed(1) }}
        </div>
        <div>
          <div class="flex gap-1 text-yellow-400 text-2xl">
            <span v-for="i in 5" :key="i">{{ i <= Math.round(avgRating) ? '★' : '☆' }}</span>
          </div>
          <p class="text-sm text-slate-500 mt-1">{{ reviewStore.totalCount }} đánh giá</p>
        </div>
      </div>

      <div v-if="isGuest" class="bg-slate-50 rounded-2xl p-6 text-center mb-6">
        <p class="text-slate-500 mb-3">Đăng nhập để đánh giá sản phẩm này</p>
        <router-link to="/login"
          class="inline-block bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg font-semibold transition">
          Đăng nhập
        </router-link>
      </div>

      <div v-else-if="reviewStore.myReview" class="bg-blue-50 border border-blue-100 rounded-2xl p-5 mb-6">
        <p class="text-sm text-blue-600 font-semibold mb-2">✔ Đánh giá của bạn</p>
        <div class="flex gap-1 text-yellow-400 text-lg mb-1">
          <span v-for="i in 5" :key="i">{{ i <= reviewStore.myReview.rating ? '★' : '☆' }}</span>
        </div>
        <p class="text-slate-700 text-sm">{{ reviewStore.myReview.comment }}</p>
        <p class="text-xs text-slate-400 mt-2">{{ formatDate(reviewStore.myReview.createdAt) }}</p>
      </div>

      <div v-else-if="!isAdmin" class="bg-slate-50 rounded-2xl p-6 mb-6">
        <p class="font-semibold text-slate-700 mb-3">Viết đánh giá của bạn</p>

        <div class="flex gap-2 text-3xl mb-4 cursor-pointer">
          <span
            v-for="i in 5" :key="i"
            @click="newRating = i"
            :class="i <= newRating ? 'text-yellow-400' : 'text-slate-300'"
          >★</span>
        </div>

        <textarea
          v-model="newComment"
          placeholder="Nhận xét về sản phẩm..."
          rows="3"
          class="w-full border border-slate-200 rounded-xl px-4 py-3 text-sm outline-none focus:ring-2 focus:ring-blue-400 resize-none mb-3"
        />

        <button
          @click="submitReview"
          :disabled="newRating === 0 || submitting"
          class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg font-semibold disabled:opacity-50 transition"
        >
          {{ submitting ? 'Đang gửi...' : 'Gửi đánh giá' }}
        </button>

        <p v-if="submitError" class="text-red-500 text-sm mt-2">{{ submitError }}</p>
      </div>

      <div v-if="reviewStore.loading" class="text-center py-10 text-slate-400 animate-pulse">
        Đang tải đánh giá...
      </div>

      <div v-else-if="reviewStore.reviews.length === 0" class="text-center py-10 text-slate-400">
        Chưa có đánh giá nào. Hãy là người đầu tiên!
      </div>

      <div v-else>
        <div class="space-y-4">
          <div
            v-for="r in reviewStore.reviews"
            :key="r.reviewId"
            class="border border-slate-100 rounded-2xl p-5"
          >
            <div class="flex items-center justify-between mb-2">
              <div class="flex items-center gap-2">
                <div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-sm">
                  {{ r.userName?.charAt(0).toUpperCase() }}
                </div>
                <span class="font-semibold text-slate-800 text-sm">{{ r.userName }}</span>
              </div>
              <span class="text-xs text-slate-400">{{ formatDate(r.createdAt) }}</span>
            </div>
            <div class="flex gap-1 text-yellow-400 text-base mb-1">
              <span v-for="i in 5" :key="i">{{ i <= r.rating ? '★' : '☆' }}</span>
            </div>
            <p class="text-slate-600 text-sm">{{ r.comment }}</p>
          </div>
        </div>

        <div v-if="reviewStore.totalCount > reviewStore.pageSize" class="flex justify-center gap-2 mt-6">
          <button
            @click="loadReviewsPage(reviewStore.currentPage - 1)"
            :disabled="reviewStore.currentPage === 1"
            class="px-3 py-1 border rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
          >
            ← Trước
          </button>
          
          <span class="px-3 py-1 text-slate-600">
            Trang {{ reviewStore.currentPage }} / {{ totalPages }}
          </span>
          
          <button
            @click="loadReviewsPage(reviewStore.currentPage + 1)"
            :disabled="reviewStore.currentPage >= totalPages"
            class="px-3 py-1 border rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
          >
            Sau →
          </button>
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
import { useReviewStore } from '@/stores/review'
import ProductCard from '@/components/ProductCard.vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()
const productStore = useClientProductStore()
const authStore = useAuthStore()
const cartStore = useCartStore()
const reviewStore = useReviewStore()

const product = ref(null)
const quantity = ref(1)
const newRating = ref(0)
const newComment = ref('')
const submitting = ref(false)
const submitError = ref('')

const isGuest = computed(() => !authStore.user)
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const avgRating = computed(() => {
  if (!reviewStore.reviews.length) return 0
  const sum = reviewStore.reviews.reduce((sum, r) => sum + (r.rating || 0), 0)
  return sum / reviewStore.reviews.length
})

const totalPages = computed(() => {
  return Math.ceil(reviewStore.totalCount / reviewStore.pageSize)
})

const formatCurrency = (val) => {
  if (val == null) return '0 ₫'
  return new Intl.NumberFormat('vi-VN').format(val) + ' ₫'
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    return new Date(dateStr).toLocaleDateString('vi-VN')
  } catch {
    return ''
  }
}

const mapProduct = (p) => ({
  id: p.productId,
  name: p.productName,
  price: p.price,
  image: p.imageURL,
  category: p.categoryName,
  brand: p.brandName,
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

    if (res.category?.id) {
      productStore.suggestedCategories = res.category.id
      await productStore.fetchSuggestedProducts()
    }
  } catch (err) {
    console.error('Lỗi fetchProduct:', err)
  }
}

const fetchAllReviews = async (productId) => {
  if (!productId) return
  
  reviewStore.reset()
  await reviewStore.fetchReviews(productId, 1, 5)
  
  if (!isGuest.value && productId) {
    await reviewStore.fetchMyReview(productId)
  }
}

const loadReviewsPage = async (page) => {
  if (page < 1 || page > totalPages.value) return
  await reviewStore.fetchReviews(product.value.id, page, reviewStore.pageSize)
}

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
  
  try {
    await cartStore.addToCart(product.value.id, quantity.value)
    alert("Đã thêm vào giỏ hàng")
  } catch (error) {
    console.error('Lỗi thêm vào giỏ:', error)
    alert('Có lỗi xảy ra khi thêm vào giỏ hàng')
  }
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

const submitReview = async () => {
  if (newRating.value === 0) {
    submitError.value = 'Vui lòng chọn số sao đánh giá'
    return
  }
  
  submitting.value = true
  submitError.value = ''

  const { success, message } = await reviewStore.addReview(
    product.value.id,
    newRating.value,
    newComment.value
  )

  if (success) {
    newRating.value = 0
    newComment.value = ''
    submitError.value = ''
  } else {
    submitError.value = message
  }
  submitting.value = false
}

onMounted(async () => {
  const id = route.params.id
  if (id) {
    await fetchProduct(id)
    await fetchAllReviews(id)
  }
})

watch(
  () => route.params.id,
  async (newId, oldId) => {
    if (newId && newId !== oldId) {
      await fetchProduct(newId)
      await fetchAllReviews(newId)
      quantity.value = 1 
      newRating.value = 0
      newComment.value = ''
      submitError.value = ''
    }
  }
)

watch(
  () => authStore.user,
  async (newUser, oldUser) => {
    if (product.value?.id) {
      if (!oldUser && newUser) {
        await reviewStore.fetchMyReview(product.value.id)
      } else if (oldUser && !newUser) {
        reviewStore.myReview = null
      }
    }
  }
)
</script>