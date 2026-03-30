<template>
  <div class="bg-blue-50 min-h-screen">

    <div class="max-w-7xl mx-auto p-6 space-y-6">

      <!-- Filter -->
      <div class="bg-white p-4 rounded-2xl shadow-sm border border-blue-200 flex flex-wrap gap-3">

        <!-- Search -->
        <input
          :value="productStore.searchQuery"
          @input="onSearchInput"
          placeholder="Tìm sản phẩm..."
          class="flex-1 min-w-[220px] px-4 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        />

        <!-- Category -->
        <select
          :value="productStore.selectedCategory ?? ''"
          @change="onCategoryChange"
          class="px-3 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        >
          <option value="">Danh mục</option>
          <option
            v-for="c in productStore.categories"
            :key="c.categoryId"
            :value="c.categoryId"
          >
            {{ c.categoryName }}
          </option>
        </select>

        <!-- Brand -->
        <select
          :value="productStore.selectedBrand ?? ''"
          @change="onBrandChange"
          class="px-3 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        >
          <option value="">Nhãn hàng</option>
          <option
            v-for="b in productStore.brands"
            :key="b.brandId"
            :value="b.brandId"
          >
            {{ b.brandName }}
          </option>
        </select>

        <!-- Price -->
        <input
          :value="productStore.minPrice ?? ''"
          @input="e => onPriceInput('min', e)"
          placeholder="Giá từ"
          class="w-28 px-2 py-2 border rounded-xl focus:ring-2 focus:ring-blue-400 outline-none bg-blue-50"
        />

        <input
          :value="productStore.maxPrice ?? ''"
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
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">

        <ProductCard
          v-for="p in productStore.products"
          :key="p.productId"
          :product="mapProduct(p)"
        />
      </div>
  

      <!-- Pagination -->
      <div class="flex justify-center items-center gap-4 mt-6">

        <button
          @click="() => { productStore.changePage(productStore.page - 1); window.scrollTo(0, 0) }"
          :disabled="productStore.page === 1"
          class="px-4 py-2 bg-white border border-blue-300 rounded-lg disabled:opacity-30 hover:bg-blue-100 transition-colors"
        >
          Prev
        </button>

        <span class="font-bold text-blue-700">
          {{ productStore.page }} / {{ totalPages }}
        </span>

        <button
          @click="() => { productStore.changePage(productStore.page + 1); window.scrollTo(0, 0) }"
          :disabled="productStore.page >= totalPages"
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
import { useProductStore } from '@/stores/products'
import ProductCard from '@/components/ProductCard.vue'

const productStore = useProductStore()

// pagination
const totalPages = computed(() =>
  Math.ceil(productStore.totalCount / productStore.pageSize) || 1
)

// debounce
let timeout = null
const debounceFetch = () => {
  clearTimeout(timeout)
  timeout = setTimeout(() => {
    productStore.fetchProducts()
  }, 400)
}

// handlers
const onSearchInput = (e) => {
  productStore.searchQuery = e.target.value
  debounceFetch()
}

const onCategoryChange = (e) => {
  productStore.selectedCategory = e.target.value || null
  productStore.page = 1
  productStore.fetchProducts()
}

const onBrandChange = (e) => {
  productStore.selectedBrand = e.target.value || null
  productStore.page = 1
  productStore.fetchProducts()
}   

const onPriceInput = (type, e) => {
  const value = e.target.value ? parseInt(e.target.value) : null
  if (type === 'min') productStore.minPrice = value
  else productStore.maxPrice = value

  debounceFetch()
}

const resetFilters = () => {
  productStore.searchQuery = ''
  productStore.selectedCategory = null
  productStore.selectedBrand = null
  productStore.minPrice = null
  productStore.maxPrice = null
  productStore.page = 1
  productStore.fetchProducts()
}

// check filter
const hasFilters = computed(() => {
  return (
    productStore.searchQuery ||
    productStore.selectedCategory ||
    productStore.minPrice ||
    productStore.maxPrice ||
    productStore.selectedBrand
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
  productStore.fetchProducts()
  productStore.fetchCategories()
  productStore.fetchBrands()
})

</script>
