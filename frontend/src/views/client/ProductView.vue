<template>
  <div class="bg-slate-50 min-h-screen">

    <div class="sticky top-0 z-50 bg-blue-50 border-b shadow-sm">
      <div class="flex items-center gap-4 px-6 py-3">

        <div class="flex-1 relative">
          <input
            :value="clientProductStore.searchQuery"
            @input="onSearchInput"
            placeholder="Tìm sản phẩm..."
            class="w-full pl-10 pr-4 py-2 rounded-full border border-slate-200 focus:ring-2 focus:ring-blue-400 outline-none text-sm bg-slate-50"
          />
          <svg class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400"
            fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M21 21l-4.35-4.35M10 18a8 8 0 100-16 8 8 0 000 16z" />
          </svg>
        </div>

        <select
          :value="clientProductStore.selectedCategory ?? ''"
          @change="onCategoryChange"
          class="px-3 py-2 rounded-full border border-slate-200 text-sm bg-white hover:border-blue-400"
        >
          <option value="">Danh mục</option>
          <option v-for="c in clientProductStore.categories" :key="c.categoryId" :value="c.categoryId">
            {{ c.categoryName }}
          </option>
        </select>

        <select
          :value="clientProductStore.selectedBrand ?? ''"
          @change="onBrandChange"
          class="px-3 py-2 rounded-full border border-slate-200 text-sm bg-white hover:border-blue-400"
        >
          <option value="">Hãng</option>
          <option v-for="b in clientProductStore.brands" :key="b.brandId" :value="b.brandId">
            {{ b.brandName }}
          </option>
        </select>

        <div class="flex items-center gap-1 border border-slate-200 rounded-full px-2 py-1 bg-white">
          <input
            :value="clientProductStore.minPrice ?? ''"
            @input="e => onPriceInput('min', e)"
            placeholder="Từ"
            class="w-16 text-sm outline-none"
          />
          <span class="text-slate-400">-</span>
          <input
            :value="clientProductStore.maxPrice ?? ''"
            @input="e => onPriceInput('max', e)"
            placeholder="Đến"
            class="w-16 text-sm outline-none"
          />
        </div>
        <button
          v-if="hasFilters"
          @click="resetFilters"
          class="px-3 py-2 text-sm text-white bg-slate-500 rounded-full hover:bg-blue-600 transition"
        >
          Reset
        </button>

      </div>
    </div>


    <div class="p-6 space-y-6">

      <!-- Product Grid -->
      <div
        v-if="clientProductStore.products.length > 0"
        ref="productGridRef"
        class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-5 gap-6"
      >
        <ProductCard
          v-for="p in clientProductStore.products"
          :key="p.productId"
          :product="mapProduct(p)"
        />
      </div>

      <!-- Empty -->
      <div v-else class="text-center py-16">
        <p class="text-xl font-semibold text-gray-500">
          Không có sản phẩm nào
        </p>
        <p class="text-sm text-gray-400 mt-2">
          Thử thay đổi bộ lọc hoặc tìm kiếm khác nhé!
        </p>
      </div>

      <!-- Pagination -->
      <div class="flex justify-center items-center gap-4 mt-6">

        <button
          @click="goPrevPage"
          :disabled="clientProductStore.page === 1"
          class="px-4 py-2 bg-white border border-blue-300 rounded-lg disabled:opacity-30 hover:bg-blue-100 transition-colors"
        >
          Prev
        </button>

        <span class="font-bold text-blue-700">
          {{ clientProductStore.page }} / {{ totalPages }}
        </span>

        <button
          @click="goNextPage"
          :disabled="clientProductStore.page >= totalPages"
          class="px-4 py-2 bg-white border border-blue-300 rounded-lg disabled:opacity-30 hover:bg-blue-100 transition-colors"
        >
          Next
        </button>

      </div>

    </div>
  </div>
</template>

<script setup>
import { onMounted, computed, ref } from 'vue'
import { useClientProductStore } from '@/stores/client_products'
import ProductCard from '@/components/ProductCard.vue'

const clientProductStore = useClientProductStore()
const productGridRef = ref(null)

// pagination
const totalPages = computed(() =>
  Math.ceil(clientProductStore.totalCount / clientProductStore.pageSize) || 1
)

// debounce
let timeout = null
const debounceFetch = () => {
  clearTimeout(timeout)
  timeout = setTimeout(() => {
    clientProductStore.fetchProducts()
  }, 400)
}

// handlers
const onSearchInput = (e) => {
  clientProductStore.searchQuery = e.target.value
  debounceFetch()
}

const onCategoryChange = (e) => {
  clientProductStore.selectedCategory = e.target.value || null
  clientProductStore.page = 1
  clientProductStore.fetchProducts()
}

const onBrandChange = (e) => {
  clientProductStore.selectedBrand = e.target.value || null
  clientProductStore.page = 1
  clientProductStore.fetchProducts()
}

const onPriceInput = (type, e) => {
  const value = e.target.value ? parseInt(e.target.value) : null
  if (type === 'min') clientProductStore.minPrice = value
  else clientProductStore.maxPrice = value
  debounceFetch()
}

const resetFilters = () => {
  clientProductStore.searchQuery = ''
  clientProductStore.selectedCategory = null
  clientProductStore.selectedBrand = null
  clientProductStore.minPrice = null
  clientProductStore.maxPrice = null
  clientProductStore.page = 1
  clientProductStore.fetchProducts()
}

const hasFilters = computed(() => {
  return (
    clientProductStore.searchQuery ||
    clientProductStore.selectedCategory ||
    clientProductStore.minPrice ||
    clientProductStore.maxPrice ||
    clientProductStore.selectedBrand
  )
})

// map API -> UI
const mapProduct = (p) => ({
  id: p.productId,
  name: p.productName,
  image: p.imageURL,
  price: p.price,
  category: p.categoryName,
  brand: p.brandName,
  reviews: 0,
  specifications: p.specifications,
  stock: p.stockQuantity
})

// init
onMounted(() => {
  clientProductStore.fetchProducts()
  clientProductStore.fetchCategories()
  clientProductStore.fetchBrands()
})

// pagination
const goPrevPage = () => {
  if (clientProductStore.page > 1) {
    clientProductStore.changePage(clientProductStore.page - 1)
    scrollToTopGrid()
  }
}

const goNextPage = () => {
  if (clientProductStore.page < totalPages.value) {
    clientProductStore.changePage(clientProductStore.page + 1)
    scrollToTopGrid()
  }
}

const scrollToTopGrid = () => {
  if (productGridRef.value) {
    productGridRef.value.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
}
</script>
