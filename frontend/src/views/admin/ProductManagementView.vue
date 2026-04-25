<template>
  <div class="space-y-6 bg-slate-50 min-h-screen p-6">

    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-blue-900 tracking-tight">Quản lý danh sách, tìm kiếm và lọc sản phẩm hệ thống</h2>
      </div>
      <div class="flex items-center gap-2">
        <router-link to="/admin/products/trash"
          class="bg-rose-500 hover:bg-rose-600 text-white px-6 py-2.5 rounded-xl font-semibold shadow-md shadow-rose-100 transition-all flex items-center gap-2 active:scale-95">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
              d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3"/>
          </svg>
          <span>Thùng rác</span>
        </router-link>
        
        <router-link to="/admin/products/new" 
          class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2.5 rounded-xl font-semibold shadow-md shadow-blue-100 transition-all flex items-center gap-2 active:scale-95">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
          </svg>
          <span>Thêm</span>
        </router-link>
      </div>
    </div>
    <div class="bg-white p-4 rounded-2xl shadow-sm border border-blue-200">
      <div class="flex flex-wrap items-center gap-3">

        <div class="flex-1 min-w-[280px] relative group">
          <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
            <svg class="w-5 h-5 text-blue-400 group-focus-within:text-blue-600 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </div>
          <input type="text" :value="adminProductStore.searchQuery" @input="onSearchInput" placeholder="Tìm theo tên hoặc mã sản phẩm" 
            class="w-full pl-10 pr-4 py-2 bg-slate-50 border border-blue-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-sm"/>
        </div>

        <div class="w-full sm:w-44 relative">
          <select :value="adminProductStore.selectedCategory ?? ''" @change="onCategoryChange" 
            class="w-full pl-3 pr-10 py-2 bg-slate-50 border border-blue-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-sm appearance-none cursor-pointer">
            <option value="">Tất cả danh mục</option>
            <option v-for="cat in adminProductStore.categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.categoryName }}</option>
          </select>
          <div class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none text-blue-400">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
            </svg>
          </div>
        </div>

        <div class="w-full sm:w-44 relative">
          <select :value="adminProductStore.selectedBrand ?? ''" @change="onBrandChange" 
            class="w-full pl-3 pr-10 py-2 bg-slate-50 border border-blue-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-sm appearance-none cursor-pointer">
            <option value="">Tất cả nhãn hàng</option>
            <option v-for="brand in adminProductStore.brands" :key="brand.brandId" :value="brand.brandId">{{ brand.brandName }}</option>
          </select>
          <div class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none text-blue-400">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
            </svg>
          </div>
        </div>

        <div class="flex items-center gap-1.5 bg-slate-50 p-1 border border-blue-200 rounded-xl">
          <div class="flex items-center">
            <button @click="adjustPrice('min', -500000)" class="p-1.5 hover:bg-blue-100 text-blue-500 rounded-lg transition-colors">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"/></svg>
            </button>
            <input type="text" :value="adminProductStore.minPrice ?? ''" @input="e => onPriceInput('min', e)" placeholder="Giá từ" class="w-20 py-1 bg-transparent text-xs focus:outline-none text-center font-medium"/>
            <button @click="adjustPrice('min', 500000)" class="p-1.5 hover:bg-blue-100 text-blue-500 rounded-lg transition-colors">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
            </button>
          </div>
          <span class="text-blue-300">—</span>
          <div class="flex items-center">
            <button @click="adjustPrice('max', -500000)" class="p-1.5 hover:bg-blue-100 text-blue-500 rounded-lg transition-colors">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"/></svg>
            </button>
            <input type="text" :value="adminProductStore.maxPrice ?? ''" @input="e => onPriceInput('max', e)" placeholder="Đến" class="w-20 py-1 bg-transparent text-xs focus:outline-none text-center font-medium"/>
            <button @click="adjustPrice('max', 500000)" class="p-1.5 hover:bg-blue-100 text-blue-500 rounded-lg transition-colors">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
            </button>
          </div>
        </div>

        <button v-if="hasFilters" @click="resetAllFilters" class="flex items-center gap-1 px-3 py-2 text-sm font-medium text-blue-600 hover:text-blue-700 hover:bg-slate-50 rounded-xl transition-all whitespace-nowrap">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4l16 16m0-16L4 20"></path></svg>
          Đặt lại
        </button>
      </div>
    </div>

    <div class="bg-white rounded-2xl shadow-sm border border-blue-200 overflow-hidden flex flex-col">
      <div class="overflow-x-auto" ref="tableWrapperRef">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50/50 border-b border-blue-200">
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Thông tin sản phẩm</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Danh mục</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Nhãn hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Giá bán</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Tồn kho</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-blue-100">
            <tr v-for="product in adminProductStore.products" :key="product.productId" @click="goToEdit(product.productId)" class="hover:bg-slate-50 transition-colors group cursor-pointer">
              <td class="py-4 px-6">
                <div class="flex items-center gap-5">
                  <div class="w-20 h-20 rounded-xl bg-blue-100 overflow-hidden flex-shrink-0 border border-blue-200 shadow-sm">
                    <img :src="product.imageURL ? `https://localhost:7010${product.imageURL}` : 'https://via.placeholder.com/150'" :alt="product.productName" class="w-full h-full object-cover">
                  </div>
                  <div>
                    <div class="font-bold text-blue-900">{{ product.productName }}</div>
                    <div class="text-xs text-blue-500">Mã: {{ product.productCode }}</div>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6 whitespace-nowrap">
                <span class="px-2 py-1 rounded-lg text-[11px] font-bold bg-emerald-50 text-emerald-700 whitespace-nowrap inline-block border border-emerald-100">
                  {{ product.categoryName }}
                </span>
              </td>

              <td class="py-4 px-6 whitespace-nowrap">
                <span class="px-2 py-1 rounded-lg text-[11px] font-bold bg-red-50 text-red-700 whitespace-nowrap inline-block border border-red-100">
                  {{ product.brandName }}
                </span>
              </td>
              <td class="py-4 px-6 font-bold text-blue-900">{{ formatCurrency(product.price) }}</td>
              <td class="py-4 px-6 whitespace-nowrap"> 
                <span :class="[
                  'px-2.5 py-1 text-[11px] font-bold rounded-full inline-block whitespace-nowrap', 
                  product.stockQuantity > 0 ? 'bg-blue-100 text-blue-700' : 'bg-rose-100 text-rose-700'
                ]">
                  {{ product.stockQuantity > 0 ? `Còn ${product.stockQuantity}` : 'Hết hàng' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="px-6 py-4 bg-slate-50 border-t border-blue-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="text-sm text-blue-700 font-medium">
          Hiển thị <span class="text-blue-900 font-bold">{{ startEntry }}</span> - <span class="text-blue-900 font-bold">{{ endEntry }}</span> 
          trong tổng số <span class="text-blue-600 font-bold">{{ adminProductStore.totalCount }}</span> sản phẩm
        </div>
        <div class="flex items-center gap-6">
          <div class="flex items-center gap-2">
            <span class="text-xs text-blue-400 font-bold uppercase">Mỗi trang:</span>
            <select :value="adminProductStore.pageSize" @change="onPageSizeChange" class="bg-white border border-blue-200 rounded-lg px-2 py-1 text-sm font-bold text-blue-700 focus:outline-none">
              <option :value="20">20</option>
              <option :value="30">30</option>
              <option :value="48">48</option>
            </select>
          </div>
          <div class="flex items-center bg-white border border-blue-200 rounded-xl p-1 shadow-sm">
            <button @click="goToPage(adminProductStore.page - 1)" :disabled="adminProductStore.page === 1" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path></svg>
            </button>
            <div class="flex items-center px-2 gap-1">
              <span class="px-3 py-1 bg-slate-50 text-blue-600 rounded-lg text-sm font-bold">{{ adminProductStore.page }}</span>
              <span class="text-sm font-bold text-blue-400">/ {{ totalPages }}</span>
            </div>
            <button @click="goToPage(adminProductStore.page + 1)" :disabled="adminProductStore.page >= totalPages" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAdminProductStore } from '../../stores/admin_products'
import { useRouter } from 'vue-router'

const router = useRouter()
const adminProductStore = useAdminProductStore()
const tableWrapperRef = ref(null)

const totalPages = computed(() => Math.ceil(adminProductStore.totalCount / adminProductStore.pageSize) || 1)
const startEntry = computed(() => (adminProductStore.page - 1) * adminProductStore.pageSize + 1)
const endEntry = computed(() => Math.min(adminProductStore.page * adminProductStore.pageSize, adminProductStore.totalCount))

const onPageSizeChange = (e) => {
  adminProductStore.changePageSize(parseInt(e.target.value))
  scrollToTop()
}

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    adminProductStore.changePage(page)
    scrollToTop()
  }
}

const scrollToTop = () => {
  if (tableWrapperRef.value) {
    tableWrapperRef.value.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const formatCurrency = (val) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

const hasFilters = computed(() => {
  return adminProductStore.searchQuery !== '' || adminProductStore.selectedCategory !== null || 
         adminProductStore.selectedBrand !== null || adminProductStore.minPrice !== null || adminProductStore.maxPrice !== null
})

let debounceTimeout = null
const debounceFetch = (delay = 400) => {
  clearTimeout(debounceTimeout)
  debounceTimeout = setTimeout(() => { adminProductStore.fetchProducts() }, delay)
}

const onSearchInput = (e) => { adminProductStore.searchQuery = e.target.value; debounceFetch(500) }

const onCategoryChange = (e) => { 
  const val = e.target.value; 
  adminProductStore.selectedCategory = val === '' ? null : parseInt(val);
  adminProductStore.page = 1; adminProductStore.fetchProducts(); scrollToTop();
}

const onBrandChange = (e) => { 
  const val = e.target.value; 
  adminProductStore.selectedBrand = val === '' ? null : parseInt(val);
  adminProductStore.page = 1; adminProductStore.fetchProducts(); scrollToTop();
}

const onPriceInput = (type, e) => {
  const rawValue = e.target.value.replace(/\D/g, '')
  const numValue = rawValue === '' ? null : parseInt(rawValue)
  if (type === 'min') adminProductStore.minPrice = numValue
  else adminProductStore.maxPrice = numValue
  debounceFetch(800)
}

const adjustPrice = (type, amount) => {
  const current = (type === 'min' ? adminProductStore.minPrice : adminProductStore.maxPrice) || 0
  const newVal = Math.max(0, current + amount)
  if (type === 'min') adminProductStore.minPrice = newVal; else adminProductStore.maxPrice = newVal
  debounceFetch(300)
}

const resetAllFilters = () => {
  adminProductStore.searchQuery = ''; adminProductStore.selectedCategory = null; adminProductStore.selectedBrand = null;
  adminProductStore.minPrice = null; adminProductStore.maxPrice = null; adminProductStore.page = 1;
  adminProductStore.fetchProducts(); scrollToTop();
}

onMounted(() => {
  adminProductStore.fetchCategories(); 
  adminProductStore.fetchBrands(); 
  adminProductStore.fetchProducts()
})

const goToEdit = (id) => {
  router.push(`/admin/products/${id}/edit`)
}

</script>
