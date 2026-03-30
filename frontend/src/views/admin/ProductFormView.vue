<template>
  <div class="min-h-screen bg-blue-50 px-4 py-8 sm:px-6 lg:px-8">
    <div class="max-w-5xl mx-auto space-y-6">

      <!-- HEADER -->
      <div class="flex items-center gap-4">
        <button @click="router.back()" class="p-2 text-slate-400 hover:text-indigo-600 hover:bg-white rounded-xl shadow-sm border border-slate-200 transition-all">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
        </button>
        <div>
          <h2 class="text-2xl font-bold text-slate-900 tracking-tight">
            {{ isEditing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm mới' }}
          </h2>
          <p class="text-sm text-slate-500">Cung cấp thông tin chi tiết để quản lý kho hàng.</p>
        </div>
      </div>

      <!-- FORM -->
      <div class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
        <form @submit.prevent="saveProduct" class="p-6 sm:p-10 space-y-10">

          <!-- SECTION 1: THÔNG TIN CƠ BẢN -->
          <div class="space-y-6">
            <div class="flex items-center gap-2 pb-3 border-b border-slate-100">
              <div class="p-1.5 bg-indigo-50 text-indigo-600 rounded-lg">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <h3 class="text-md font-bold text-slate-800 uppercase tracking-wider">Thông tin cơ bản</h3>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-12 gap-5">
              <div class="md:col-span-9">
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Tên sản phẩm *</label>
                <input v-model="formData.productName" type="text" required placeholder="Nhập tên sản phẩm..."
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:border-indigo-500 transition-all text-sm font-medium">
              </div>

              <div class="md:col-span-3">
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Code *</label>
                <input v-model="formData.productCode" type="text" required placeholder="DT-IP15-512"
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-400 transition-all font-mono text-sm text-center uppercase">
              </div>

              <div class="md:col-span-6">
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Danh mục *</label>
                <div class="relative">
                  <select v-model="formData.categoryId" required
                    class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-400 appearance-none cursor-pointer text-sm">
                    <option :value="null" disabled>Chọn danh mục</option>
                    <option v-for="cat in productStore.categories" :key="cat.categoryId" :value="Number(cat.categoryId)">
                      {{ cat.categoryName }}
                    </option>
                  </select>
                  <div class="absolute inset-y-0 right-4 flex items-center pointer-events-none text-slate-400">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path d="M19 9l-7 7-7-7" stroke-width="2"></path>
                    </svg>
                  </div>
                </div>
              </div>

              <div class="md:col-span-6">
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Thương hiệu *</label>
                <div class="relative">
                  <select v-model="formData.brandId" required
                    class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-400 appearance-none cursor-pointer text-sm">
                    <option :value="null" disabled>Chọn nhãn hàng</option>
                    <option v-for="brand in productStore.brands" :key="brand.brandId" :value="Number(brand.brandId)">
                      {{ brand.brandName }}
                    </option>
                  </select>
                  <div class="absolute inset-y-0 right-4 flex items-center pointer-events-none text-slate-400">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path d="M19 9l-7 7-7-7" stroke-width="2"></path>
                    </svg>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 2: GIÁ & KHO -->
          <div class="space-y-6">
            <div class="flex items-center gap-2 pb-3 border-b border-slate-100">
              <div class="p-1.5 bg-emerald-50 text-emerald-600 rounded-lg">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <h3 class="text-md font-bold text-slate-800 uppercase tracking-wider">Thương mại & Kho</h3>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Giá bán (VNĐ) *</label>
                <div class="relative">
                  <input type="text" :value="formatPriceDisplay(formData.price)" @input="onPriceInputChange" required
                    placeholder="0"
                    class="w-full pl-5 pr-12 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-500 transition-all font-bold text-xl text-indigo-600 shadow-inner">
                  <div class="absolute inset-y-0 right-5 flex items-center pointer-events-none text-slate-400 font-bold">₫</div>
                </div>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Số lượng tồn kho *</label>
                <div class="relative">
                  <input v-model.number="formData.stockQuantity" type="number" min="0" required
                    class="w-full px-5 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-500 transition-all font-bold text-xl shadow-inner">
                  <div class="absolute inset-y-0 right-5 flex items-center pointer-events-none text-slate-400 text-xs font-bold">ĐƠN VỊ</div>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 3: MÔ TẢ -->
          <div class="space-y-6">
            <div class="flex items-center gap-2 pb-3 border-b border-slate-100">
              <div class="p-1.5 bg-amber-50 text-amber-600 rounded-lg">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
              </div>
              <h3 class="text-md font-bold text-slate-800 uppercase tracking-wider">Mô tả & Thông số</h3>
            </div>

            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Mô tả chi tiết</label>
                <textarea v-model="formData.description" rows="5" placeholder="Mô tả về sản phẩm..."
                  class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-400 focus:border-amber-500 transition-all text-sm leading-relaxed"></textarea>
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">Thông số kỹ thuật</label>
                <textarea v-model="formData.specifications" rows="5" placeholder="Kích thước, CPU, RAM..."
                  class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-400 focus:border-amber-500 transition-all font-mono text-sm leading-relaxed"></textarea>
              </div>
            </div>
          </div>

          <!-- SECTION 4: HÌNH ẢNH -->
          <div class="space-y-6">
            <div class="flex items-center gap-2 pb-3 border-b border-slate-100">
              <div class="p-1.5 bg-rose-50 text-rose-600 rounded-lg">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z">
                  </path>
                </svg>
              </div>
              <h3 class="text-md font-bold text-slate-800 uppercase tracking-wider">Hình ảnh</h3>
            </div>

            <div class="flex flex-col md:flex-row gap-6">
              <div class="flex-1">
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1.5 ml-1">URL Hình ảnh</label>
                <input v-model="formData.image" type="url" placeholder="Gán link ảnh..."
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-rose-400 focus:border-rose-500 transition-all text-sm">
              </div>

              <div class="w-full md:w-32 h-32 bg-slate-50 rounded-xl border border-slate-200 flex items-center justify-center overflow-hidden group shadow-inner relative">
                <img v-if="formData.image" :src="formData.image" alt="Preview" class="w-full h-full object-contain p-2 transition-all duration-300" @error="handleImageError">
                <div v-else class="text-slate-300">
                  <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                  </svg>
                </div>
              </div>
            </div>
          </div>

          <!-- ACTION BUTTONS -->
          <div class="pt-6 flex flex-col sm:flex-row items-center justify-end gap-3 border-t border-slate-100">
            <button type="button" @click="router.back()" class="w-full sm:w-auto px-6 py-2.5 text-slate-500 font-bold text-sm hover:bg-slate-100 rounded-xl transition-all order-2 sm:order-1">Hủy bỏ</button>
            <button type="submit" class="w-full sm:w-auto px-8 py-2.5 bg-indigo-600 text-white rounded-xl font-bold text-sm hover:bg-indigo-700 shadow-md shadow-indigo-100 transition-all active:scale-95 flex items-center justify-center gap-2 order-1 sm:order-2">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
              </svg>
              {{ isEditing ? 'Cập nhật' : 'Đăng sản phẩm' }}
            </button>
          </div>

        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProductStore } from '../../stores/products'

const route = useRoute()
const router = useRouter()
const productStore = useProductStore()

const isEditing = computed(() => !!route.params.id)

const formData = ref({
  id: 0,
  productName: '',
  productCode: '',
  description: '',
  specifications: '',
  categoryId: null,
  brandId: null,
  price: 0,
  stockQuantity: 0,
  image: ''
})

const formatPriceDisplay = (val) => {
  if (val === null || val === undefined || val === '' || val === 0) return ''
  return new Intl.NumberFormat('vi-VN').format(val)
}

const onPriceInputChange = (e) => {
  const rawValue = e.target.value.replace(/\D/g, '')
  formData.value.price = rawValue === '' ? 0 : parseInt(rawValue, 10)
}

onMounted(async () => {
  await Promise.all([
    productStore.fetchCategories(),
    productStore.fetchBrands()
  ])

  if (isEditing.value) {
    const productId = route.params.id
    try {
      const data = await productStore.fetchProductById(productId)
      if (data) {
        const catId = data.categoryId ?? data.CategoryId ?? (data.category?.id);
        const brId = data.brandId ?? data.BrandId ?? (data.brand?.id);

        formData.value = {
          id: data.id || data.Id || productId,
          productName: data.productName || data.ProductName || '',
          productCode: data.productCode || data.ProductCode || '',
          description: data.description || data.Description || '',
          specifications: data.specifications || data.Specifications || '',
          categoryId: catId != null ? Number(catId) : null,
          brandId: brId != null ? Number(brId) : null,
          price: data.price || data.Price || 0,
          stockQuantity: data.stockQuantity || data.StockQuantity || 0,
          image: data.image || data.Image || ''
        }
      }
    } catch (error) {
      console.error(error)
      router.push('/admin/products')
    }
  }
})

const handleImageError = (e) => {
  e.target.src = 'https://via.placeholder.com/150?text=Error'
}

const saveProduct = async () => {
  try {
    if (isEditing.value) {
      await productStore.updateProduct(formData.value.id, formData.value)
    } else {
      await productStore.addProduct(formData.value)
    }
    router.push('/admin/products')
  } catch (error) {
    console.error(error)
    alert("Lưu thất bại!")
  }
}
</script>