<template>
  <div class="space-y-6">
    <!-- Header (Giữ nguyên) -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý sản phẩm</h2>
        <p class="text-sm text-slate-500">Quản lý danh sách, tìm kiếm và lọc sản phẩm hệ thống</p>
      </div>
      <router-link to="/admin/products/new" class="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-2.5 rounded-xl font-semibold shadow-md shadow-indigo-100 transition-all flex items-center gap-2 active:scale-95">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Thêm sản phẩm</span>
      </router-link>
    </div>

    <!-- Toolbar: Tất cả bộ lọc trên 1 hàng -->
    <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-200">
      <div class="flex flex-wrap items-center gap-3">
        <!-- Ô tìm kiếm (Chiếm không gian còn lại) -->
        <div class="flex-1 min-w-[280px] relative group">
          <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
            <svg class="w-5 h-5 text-slate-400 group-focus-within:text-indigo-500 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
          </div>
          <input type="text" :value="productStore.searchQuery" @input="onSearchInput" placeholder="Tìm theo tên hoặc mã..." class="w-full pl-10 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all text-sm" />
        </div>

        <!-- Danh mục -->
        <div class="w-full sm:w-44">
          <select :value="productStore.selectedCategory ?? ''" @change="onCategoryChange" class="w-full pl-3 pr-8 py-2 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all text-sm appearance-none cursor-pointer bg-no-repeat bg-[right_0.7rem_center] bg-[length:1em]" style="background-image: url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2224%22%20height%3D%2224%22%20viewBox%3D%220%200%2024%2024%22%20fill%3D%22none%22%20stroke%3D%22%2364748b%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E');">
            <option value="">Tất cả danh mục</option>
            <option v-for="cat in productStore.categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.categoryName }}</option>
          </select>
        </div>

        <!-- Nhãn hàng -->
        <div class="w-full sm:w-44">
          <select :value="productStore.selectedBrand ?? ''" @change="onBrandChange" class="w-full pl-3 pr-8 py-2 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all text-sm appearance-none cursor-pointer bg-no-repeat bg-[right_0.7rem_center] bg-[length:1em]" style="background-image: url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2224%22%20height%3D%2224%22%20viewBox%3D%220%200%2024%2024%22%20fill%3D%22none%22%20stroke%3D%22%2364748b%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E');">
            <option value="">Tất cả nhãn hàng</option>
            <option v-for="brand in productStore.brands" :key="brand.brandId" :value="brand.brandId">{{ brand.brandName }}</option>
          </select>
        </div>

        <!-- Khoảng giá -->
        <div class="flex items-center gap-1.5 bg-slate-50 p-1 border border-slate-200 rounded-xl">
          <div class="flex items-center">
            <button @click="adjustPrice('min', -500000)" class="p-1.5 hover:bg-slate-200 text-slate-400 rounded-lg transition-colors"><svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"/></svg></button>
            <input type="text" :value="productStore.minPrice ?? ''" @input="e => onPriceInput('min', e)" placeholder="Giá từ" class="w-20 py-1 bg-transparent text-xs focus:outline-none text-center font-medium" />
            <button @click="adjustPrice('min', 500000)" class="p-1.5 hover:bg-slate-200 text-slate-400 rounded-lg transition-colors"><svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg></button>
          </div>
          <span class="text-slate-300">—</span>
          <div class="flex items-center">
            <button @click="adjustPrice('max', -500000)" class="p-1.5 hover:bg-slate-200 text-slate-400 rounded-lg transition-colors"><svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"/></svg></button>
            <input type="text" :value="productStore.maxPrice ?? ''" @input="e => onPriceInput('max', e)" placeholder="Đến" class="w-20 py-1 bg-transparent text-xs focus:outline-none text-center font-medium" />
            <button @click="adjustPrice('max', 500000)" class="p-1.5 hover:bg-slate-200 text-slate-400 rounded-lg transition-colors"><svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg></button>
          </div>
        </div>

        <!-- Nút Reset (Đặt lại) -->
        <button v-if="hasFilters" @click="resetAllFilters" class="flex items-center gap-1 px-3 py-2 text-sm font-medium text-rose-500 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all whitespace-nowrap">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4l16 16m0-16L4 20"></path></svg>
          Đặt lại
        </button>
      </div>
    </div>

    <!-- Data Table (Giữ nguyên) -->
    <div class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50/50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Thông tin sản phẩm</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Danh mục</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Nhãn hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Giá bán</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Tồn kho</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="product in productStore.products" :key="product.id" class="hover:bg-slate-50/80 transition-colors group">
              <td class="py-4 px-6">
                <div class="flex items-center gap-4">
                  <div class="w-14 h-14 rounded-xl bg-slate-100 overflow-hidden flex-shrink-0 border border-slate-200 shadow-sm">
                    <img :src="product.image" :alt="product.productName" class="w-full h-full object-cover">
                  </div>
                  <div>
                    <div class="font-bold text-slate-800">{{ product.productName }}</div>
                    <div class="text-xs text-slate-400">Mã: {{ product.productCode }}</div>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6">
                <span class="px-2 py-1 rounded-lg text-xs font-bold bg-indigo-50 text-indigo-700">{{ product.categoryName }}</span>
              </td>
              <td class="py-4 px-6 text-slate-600 text-sm font-medium">{{ product.brandName }}</td>
              <td class="py-4 px-6 font-bold text-slate-800">{{ formatCurrency(product.price) }}</td>
              <td class="py-4 px-6">
                <span :class="['px-2.5 py-1 text-[11px] font-bold rounded-full', product.stockQuantity > 0 ? 'bg-emerald-100 text-emerald-700' : 'bg-rose-100 text-rose-700']">
                  {{ product.stockQuantity > 0 ? `Còn ${product.stockQuantity}` : 'Hết hàng' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right space-x-1">
                <router-link :to="`/admin/products/${product.productId}/edit`" class="p-2 text-indigo-600 hover:bg-indigo-50 rounded-xl transition-colors inline-flex"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg></router-link>
                <button @click="confirmDelete(product)" class="p-2 text-rose-500 hover:bg-rose-50 rounded-xl transition-colors inline-flex"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination Footer (Giữ nguyên) -->
      <div class="px-6 py-4 bg-slate-50/50 border-t border-slate-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="text-sm text-slate-500 font-medium">
          Hiển thị <span class="text-slate-800 font-bold">{{ startEntry }}</span> - <span class="text-slate-800 font-bold">{{ endEntry }}</span> 
          trong tổng số <span class="text-indigo-600 font-bold">{{ productStore.totalCount }}</span> sản phẩm
        </div>
        <div class="flex items-center gap-6">
          <div class="flex items-center gap-2">
            <span class="text-xs text-slate-400 font-bold uppercase">Mỗi trang:</span>
            <select :value="productStore.pageSize" @change="onPageSizeChange" class="bg-white border border-slate-200 rounded-lg px-2 py-1 text-sm font-bold text-slate-700 focus:outline-none">
              <option :value="12">12</option>
              <option :value="24">24</option>
              <option :value="48">48</option>
            </select>
          </div>
          <div class="flex items-center bg-white border border-slate-200 rounded-xl p-1 shadow-sm">
            <button @click="productStore.changePage(productStore.page - 1)" :disabled="productStore.page === 1" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path></svg>
            </button>
            <div class="flex items-center px-2 gap-1">
              <span class="px-3 py-1 bg-indigo-50 text-indigo-600 rounded-lg text-sm font-bold">{{ productStore.page }}</span>
              <span class="text-sm font-bold text-slate-400">/ {{ totalPages }}</span>
            </div>
            <button @click="productStore.changePage(productStore.page + 1)" :disabled="productStore.page >= totalPages" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Modal (Giữ nguyên) -->
    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <h3 class="text-xl font-bold text-rose-600 mb-2 text-center">Xác nhận xóa</h3>
        <p class="text-slate-600 mb-6 text-center">Xóa sản phẩm <span class="font-bold text-slate-800">"{{ productToDelete?.productName }}"</span>?</p>
        <div class="flex gap-3 justify-center">
          <button @click="showDeleteDialog = false" class="px-4 py-2 text-slate-500 font-bold hover:bg-slate-100 rounded-xl">Hủy</button>
          <button @click="executeDelete" class="px-6 py-2 bg-rose-600 text-white font-bold rounded-xl shadow-lg">Xác nhận Xóa</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue'
  import { useProductStore } from '../../stores/products'

  const productStore = useProductStore()
  const showDeleteDialog = ref(false)
  const productToDelete = ref(null)

  const totalPages = computed(() => Math.ceil(productStore.totalCount / productStore.pageSize) || 1)
  const startEntry = computed(() => (productStore.page - 1) * productStore.pageSize + 1)
  const endEntry = computed(() => Math.min(productStore.page * productStore.pageSize, productStore.totalCount))

  const onPageSizeChange = (e) => {
    productStore.changePageSize(parseInt(e.target.value))
  }

  const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

  const hasFilters = computed(() => {
    return productStore.searchQuery !== '' || productStore.selectedCategory !== null || 
          productStore.selectedBrand !== null || productStore.minPrice !== null || productStore.maxPrice !== null
  })

  let debounceTimeout = null
  const debounceFetch = (delay = 400) => {
    clearTimeout(debounceTimeout)
    debounceTimeout = setTimeout(() => { productStore.fetchProducts() }, delay)
  }

  const onSearchInput = (e) => { productStore.searchQuery = e.target.value; debounceFetch(500) }
  
  const onCategoryChange = (e) => { 
    const val = e.target.value; 
    productStore.selectedCategory = val === '' ? null : parseInt(val);
    productStore.page = 1; productStore.fetchProducts(); 
  }

  const onBrandChange = (e) => { 
    const val = e.target.value; 
    productStore.selectedBrand = val === '' ? null : parseInt(val);
    productStore.page = 1; productStore.fetchProducts(); 
  }

  const onPriceInput = (type, e) => {
    const rawValue = e.target.value.replace(/\D/g, '')
    const numValue = rawValue === '' ? null : parseInt(rawValue)
    
    if (type === 'min') productStore.minPrice = numValue
    else productStore.maxPrice = numValue
    
    debounceFetch(800)
  }

  const adjustPrice = (type, amount) => {
    const current = (type === 'min' ? productStore.minPrice : productStore.maxPrice) || 0
    const newVal = Math.max(0, current + amount)
    if (type === 'min') productStore.minPrice = newVal; else productStore.maxPrice = newVal
    debounceFetch(300)
  }

  const resetAllFilters = () => {
    productStore.searchQuery = ''; productStore.selectedCategory = null; productStore.selectedBrand = null;
    productStore.minPrice = null; productStore.maxPrice = null; productStore.page = 1;
    productStore.fetchProducts()
  }

  onMounted(() => {
    productStore.fetchCategories(); productStore.fetchBrands(); productStore.fetchProducts()
  })

const confirmDelete = (product) => { 
  productToDelete.value = product; 
  showDeleteDialog.value = true; 
}

const executeDelete = async () => {
  if (productToDelete.value) {
    const idToDelete = productToDelete.value.productId || productToDelete.value.id;
    if (!idToDelete) return;
    try {
      await productStore.deleteProduct(idToDelete);
      showDeleteDialog.value = false;
    } catch (error) {
      alert("Lỗi khi xóa sản phẩm!");
    }
  }
}
</script>