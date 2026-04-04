<template>
  <div class="bg-blue-50 min-h-screen">

    <div class="max-w-7xl mx-auto p-6 space-y-6">

      <!-- Filter -->
      <div class="bg-white p-4 rounded-2xl shadow-sm border border-blue-200 flex flex-wrap gap-3">

        <!-- Search -->
        <input
          :value="clientProductStore.searchQuery"
          @input="onSearchInput"
          placeholder="Tìm sản phẩm..."
          class="flex-1 min-w-[220px] px-4 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        />

        <!-- Category -->
        <select
          :value="clientProductStore.selectedCategory ?? ''"
          @change="onCategoryChange"
          class="px-3 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        >
          <option value="">Danh mục</option>
          <option
            v-for="c in clientProductStore.categories"
            :key="c.categoryId"
            :value="c.categoryId"
          >
            {{ c.categoryName }}
          </option>
        </select>

        <!-- Brand -->
        <select
          :value="clientProductStore.selectedBrand ?? ''"
          @change="onBrandChange"
          class="px-3 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        >
          <option value="">Nhãn hàng</option>
          <option
            v-for="b in clientProductStore.brands"
            :key="b.brandId"
            :value="b.brandId"
          >
            {{ b.brandName }}
          </option>
        </select>

        <!-- Price -->
        <input
          :value="clientProductStore.minPrice ?? ''"
          @input="e => onPriceInput('min', e)"
          placeholder="Giá từ"
          class="w-28 px-2 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        />

        <input
          :value="clientProductStore.maxPrice ?? ''"
          @input="e => onPriceInput('max', e)"
          placeholder="Đến"
          class="w-28 px-2 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        />

        <!-- Reset -->
        <button
          v-if="hasFilters"
          @click="resetFilters"
          class="px-4 py-2 bg-gradient-to-r from-blue-500 to-blue-600 text-white rounded-xl hover:from-blue-600 hover:to-blue-700 transition-all"
        >
          Reset
        </button>

      </div>
      
      <!-- Product Grid -->
      <div v-if="clientProductStore.products.length > 0"
        ref="productGridRef"
        class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">

        <ProductCard
          v-for="p in clientProductStore.products"
          :key="p.productId"
          :product="mapProduct(p)"
        />
      </div>

      <!-- Empty state -->
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
import { onMounted, computed } from 'vue'
import { useClientProductStore } from '@/stores/client_products'
import ProductCard from '@/components/ProductCard.vue'
import { ref } from 'vue'

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

// check filter
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
