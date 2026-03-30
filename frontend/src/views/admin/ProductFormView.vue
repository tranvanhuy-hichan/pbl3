<template>
  <div class="min-h-screen bg-blue-50 px-4 py-8 sm:px-6 lg:px-8">
    <div class="max-w-6xl mx-auto space-y-6">
      
      <!-- NÚT QUAY LẠI -->
      <div class="flex items-center gap-3">
        <button @click="router.back()" class="p-2.5 text-slate-400 hover:text-indigo-600 hover:bg-white rounded-xl shadow-sm border border-slate-200 transition-all bg-white/50">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
        </button>
        <h2 class="text-xl font-bold text-slate-800 uppercase tracking-tight">{{ isEditing ? 'Chỉnh sửa sản phẩm' : 'Tạo sản phẩm mới' }}</h2>
      </div>

      <form @submit.prevent="saveProduct" enctype="multipart/form-data" class="space-y-6">
        
        <!-- TOP SECTION: ẢNH LỚN & THÔNG TIN CHÍNH -->
        <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 items-stretch">
          
          <!-- CỘT 1: HIỂN THỊ ẢNH CỰC LỚN (CHIẾM 5/12) -->
          <div class="lg:col-span-5 bg-white p-6 rounded-3xl shadow-sm border border-slate-200 flex flex-col items-center justify-between min-h-[400px]">
            <div class="w-full h-full max-h-[350px] bg-slate-50 rounded-2xl border-2 border-dashed border-slate-100 flex items-center justify-center overflow-hidden relative group">
              
              <img v-if="previewImage" :src="previewImage" alt="Large Preview" 
                class="max-w-full max-h-full object-contain p-4 transition-transform duration-700 group-hover:scale-110">
              
              <div v-else class="flex flex-col items-center text-slate-300">
                <svg class="w-20 h-20 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                <span class="text-sm font-bold uppercase tracking-widest">Chưa có ảnh</span>
              </div>

              <!-- Nút chọn file đè lên ảnh (overlay) -->
              <div class="absolute inset-0 bg-indigo-900/40 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer">
                <label class="px-6 py-2.5 bg-white text-indigo-600 rounded-full font-bold text-sm shadow-xl cursor-pointer">Thay đổi ảnh</label>
                <input type="file" @change="onImageChange" accept="image/*" class="absolute inset-0 opacity-0 cursor-pointer">
              </div>
            </div>
            <p class="mt-4 text-[11px] text-slate-400 text-center uppercase font-bold tracking-tighter">Click vào ảnh để thay đổi • Khuyên dùng ảnh HD</p>
          </div>

          <!-- CỘT 2: TÊN, MÃ, GIÁ, KHO (CHIẾM 7/12) -->
          <div class="lg:col-span-7 bg-white p-8 rounded-3xl shadow-sm border border-slate-200 flex flex-col justify-between">
            <div class="space-y-6">
              <div>
                <label class="block text-[10px] font-black text-indigo-500 uppercase tracking-widest mb-2">Tên sản phẩm chính thức</label>
                <input v-model="formData.productName" type="text" required placeholder="Ví dụ: iPhone 15 Pro Max 512GB..."
                  class="w-full px-5 py-4 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-100 focus:border-indigo-500 outline-none transition-all text-lg font-bold text-slate-800">
              </div>

              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Mã SKU / Code</label>
                  <input v-model="formData.productCode" type="text" required placeholder="IP15-PM-512"
                    class="w-full px-5 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-4 focus:ring-indigo-100 outline-none font-mono text-center uppercase font-bold">
                </div>
                <div>
                  <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Số lượng tồn kho</label>
                  <input v-model.number="formData.stockQuantity" type="number" min="0" required
                    class="w-full px-5 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-4 focus:ring-indigo-100 outline-none text-center font-bold text-emerald-600">
                </div>
              </div>

              <div>
                <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Giá niêm yết (VNĐ)</label>
                <div class="relative">
                  <input type="text" :value="formatPriceDisplay(formData.price)" @input="onPriceInputChange" required
                    class="w-full pl-5 pr-12 py-4 bg-indigo-50/50 border border-indigo-100 rounded-2xl focus:ring-4 focus:ring-indigo-100 outline-none text-2xl font-black text-indigo-600 tracking-tight">
                  <span class="absolute inset-y-0 right-6 flex items-center font-bold text-indigo-300">₫</span>
                </div>
              </div>
            </div>

            <!-- ACTION BUTTONS (Đặt ngay trong cột thông tin chính) -->
            <div class="mt-8 pt-6 border-t border-slate-50 flex gap-4">
              <button type="submit" class="flex-1 py-4 bg-indigo-600 hover:bg-indigo-700 text-white rounded-2xl font-black text-sm uppercase tracking-widest shadow-lg shadow-indigo-100 transition-all active:scale-95 flex items-center justify-center gap-3">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7"></path></svg>
                {{ isEditing ? 'Cập nhật hệ thống' : 'Đăng bán ngay' }}
              </button>
              <button type="button" @click="router.back()" class="px-8 py-4 bg-slate-100 text-slate-500 rounded-2xl font-bold text-sm hover:bg-slate-200 transition-all uppercase tracking-wider">Hủy</button>
            </div>
          </div>
        </div>

        <!-- BOTTOM SECTION: DANH MỤC & MÔ TẢ -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          
          <!-- PHÂN LOẠI -->
          <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-200 space-y-6">
            <h3 class="text-sm font-black text-slate-800 uppercase border-b pb-4 border-slate-50">Phân loại hàng</h3>
            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-2">
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Danh mục sản phẩm</label>
                <select v-model="formData.categoryId" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm font-medium">
                  <option :value="null" disabled>Chọn danh mục</option>
                  <option v-for="cat in productStore.categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.categoryName }}</option>
                </select>
              </div>
              <div class="space-y-2">
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Thương hiệu</label>
                <select v-model="formData.brandId" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm font-medium">
                  <option :value="null" disabled>Chọn nhãn hàng</option>
                  <option v-for="brand in productStore.brands" :key="brand.brandId" :value="brand.brandId">{{ brand.brandName }}</option>
                </select>
              </div>
            </div>
          </div>

          <!-- MÔ TẢ NHANH -->
          <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-200 space-y-6">
            <h3 class="text-sm font-black text-slate-800 uppercase border-b pb-4 border-slate-50">Mô tả & Thông số</h3>
            <div class="space-y-4">
              <textarea v-model="formData.description" rows="1" placeholder="Mô tả ngắn gọn về sản phẩm..." 
                class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm"></textarea>
              <textarea v-model="formData.specifications" rows="2" placeholder="Thông số kỹ thuật chính..." 
                class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm font-mono"></textarea>
            </div>
          </div>

        </div>
      </form>
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

const API_URL = 'https://localhost:7010'
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
  imageFile: null,
  imageUrl: ''
})

const previewImage = computed(() => {
  if (formData.value.imageFile) return URL.createObjectURL(formData.value.imageFile)
  if (formData.value.imageUrl) {
    return formData.value.imageUrl.startsWith('http') ? formData.value.imageUrl : API_URL + formData.value.imageUrl
  }
  return null
})

const onImageChange = (e) => {
  const file = e.target.files[0]
  formData.value.imageFile = file || null
}

const formatPriceDisplay = (val) => val ? new Intl.NumberFormat('vi-VN').format(val) : ''
const onPriceInputChange = (e) => {
  const rawValue = e.target.value.replace(/\D/g, '')
  formData.value.price = rawValue ? parseInt(rawValue, 10) : 0
}

onMounted(async () => {
  await Promise.all([productStore.fetchCategories(), productStore.fetchBrands()])
  if (isEditing.value) {
    try {
      const data = await productStore.fetchProductById(route.params.id)
      if (data) {
        formData.value = {
          id: data.productId,
          productName: data.productName,
          productCode: data.productCode,
          description: data.description || '',
          specifications: data.specifications || '',
          categoryId: data.category?.id || data.categoryId,
          brandId: data.brand?.id || data.brandId,
          price: data.price,
          stockQuantity: data.stockQuantity,
          imageFile: null,
          imageUrl: data.imageURL
        }
      }
    } catch (err) { router.push('/admin/products') }
  }
})

const saveProduct = async () => {
  try {
    const fd = new FormData()
    Object.keys(formData.value).forEach(key => {
      if (key !== 'imageFile' && key !== 'imageUrl') fd.append(key, formData.value[key])
    })
    if (formData.value.imageFile) fd.append('ImageFile', formData.value.imageFile)

    if (isEditing.value) await productStore.updateProduct(formData.value.id, fd)
    else await productStore.addProduct(fd)
    
    router.push('/admin/products')
  } catch (error) { alert('Lưu thất bại!') }
}
</script>