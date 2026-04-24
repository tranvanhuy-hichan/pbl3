<template>
  <div class="min-h-screen bg-slate-50 pb-20">
    <!-- STICKY HEADER: Nơi đặt các nút hành động chính -->
    <div class="top-0 z-30 bg-slate-50 backdrop-blur-md border-b border-slate-200 mb-6">
      <div class="max-w-6xl mx-auto px-4 h-20 flex items-center justify-between">
        <div class="flex items-center gap-4">
          <button @click="router.back()" class="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-xl transition-all">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
          </button>
          <div>
            <h2 class="text-lg font-bold text-slate-800 leading-tight">{{ isEditing ? 'Chỉnh sửa sản phẩm' : 'Tạo sản phẩm mới' }}</h2>
            <p class="text-xs text-slate-500 font-medium uppercase tracking-wider">Quản lý kho hàng & Hệ thống bán lẻ</p>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <button type="button" @click="router.back()" class="px-6 py-2.5 text-sm font-bold text-slate-500 hover:bg-slate-100 rounded-xl transition-all">
            Hủy bỏ
          </button>
          <!-- Nút chính đặt ở góc phải trên cùng - chuẩn UI Dashboard -->
          <button type="submit" form="product-form"
            class="px-8 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-bold text-sm shadow-lg shadow-indigo-200 transition-all active:scale-95 flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7"></path></svg>
            {{ isEditing ? 'Lưu thay đổi' : 'Đăng bán ngay' }}
          </button>
        </div>
      </div>
    </div>

    <div class="max-w-6xl mx-auto px-4">
      <form id="product-form" @submit.prevent="saveProduct" enctype="multipart/form-data" class="grid grid-cols-1 lg:grid-cols-12 gap-8">
        
        <!-- CỘT TRÁI (4/12): ẢNH & PHÂN LOẠI -->
        <div class="lg:col-span-4 space-y-6">
          <!-- Khối Ảnh -->
          <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-200">
            <label class="block text-[11px] font-black text-slate-400 uppercase tracking-widest mb-4 text-center">Hình ảnh đại diện</label>
            <div class="aspect-square bg-slate-50 rounded-2xl border-2 border-dashed border-slate-200 flex items-center justify-center overflow-hidden relative group">
              <img v-if="previewImage" :src="previewImage" alt="Preview" class="w-full h-full object-contain p-4 group-hover:scale-105 transition-transform duration-500">
              <div v-else class="text-center">
                <svg class="w-12 h-12 mx-auto text-slate-300 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                <span class="text-[10px] font-bold text-slate-400 uppercase">Chưa có ảnh</span>
              </div>
              <div class="absolute inset-0 bg-slate-900/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center cursor-pointer">
                <label class="bg-white text-slate-900 px-4 py-2 rounded-lg text-xs font-bold cursor-pointer shadow-xl">Chọn tệp tin</label>
                <input type="file" @change="onImageChange" accept="image/*" class="absolute inset-0 opacity-0 cursor-pointer">
              </div>
            </div>
          </div>

          <!-- Khối Phân loại (Đưa lên đây cho gọn) -->
          <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-200 space-y-5">
            <h3 class="text-[11px] font-black text-slate-800 uppercase tracking-widest border-b pb-3 border-slate-50">Phân loại hàng</h3>
            <div class="space-y-4">
              <div>
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Danh mục</label>
                <select v-model="formData.categoryId" required class="w-full mt-1.5 px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-xs font-bold">
                  <option :value="null" disabled>Chọn danh mục</option>
                  <option v-for="cat in productStore.categories" :key="cat.categoryId" :value="cat.categoryId">{{ cat.categoryName }}</option>
                </select>
              </div>
              <div>
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Thương hiệu</label>
                <select v-model="formData.brandId" required class="w-full mt-1.5 px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl outline-none focus:ring-2 focus:ring-indigo-400 text-xs font-bold">
                  <option :value="null" disabled>Chọn nhãn hàng</option>
                  <option v-for="brand in productStore.brands" :key="brand.brandId" :value="brand.brandId">{{ brand.brandName }}</option>
                </select>
              </div>
            </div>
          </div>
        </div>

        <!-- CỘT PHẢI (8/12): THÔNG TIN CHI TIẾT -->
        <div class="lg:col-span-8 space-y-6">
          <!-- Thông tin cơ bản -->
          <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-200 space-y-6">
            <div>
              <label class="block text-[10px] font-black text-indigo-500 uppercase tracking-widest mb-2">Tên sản phẩm chính thức</label>
              <input v-model="formData.productName" type="text" required placeholder="Nhập tên sản phẩm..."
                class="w-full px-5 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-50 focus:border-indigo-500 outline-none transition-all text-xl font-bold text-slate-800">
            </div>

            <div class="grid grid-cols-2 gap-6">
              <div>
                <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Mã sản phẩm (SKU)</label>
                <input v-model="formData.productCode" type="text" required placeholder="SKU-123..."
                  class="w-full px-5 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none font-mono font-bold uppercase">
              </div>
              <div>
                <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Số lượng tồn kho</label>
                <input v-model.number="formData.stockQuantity" type="number" min="0" required
                  class="w-full px-5 py-3 bg-slate-50 border border-slate-200 rounded-xl outline-none font-bold text-emerald-600">
              </div>
            </div>

            <div>
              <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Giá niêm yết (VNĐ)</label>
              <div class="relative max-w-xs">
                <input type="text" :value="formatPriceDisplay(formData.price)" @input="onPriceInputChange" required
                  class="w-full pl-5 pr-12 py-3.5 bg-indigo-50/30 border border-indigo-100 rounded-2xl focus:ring-4 focus:ring-indigo-100 outline-none text-2xl font-black text-indigo-600">
                <span class="absolute inset-y-0 right-5 flex items-center font-bold text-indigo-300">₫</span>
              </div>
            </div>
          </div>

          <!-- Mô tả & Thông số -->
          <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-200 space-y-6">
            <h3 class="text-[11px] font-black text-slate-800 uppercase tracking-widest border-b pb-3 border-slate-50">Nội dung chi tiết</h3>
            <div class="space-y-6">
              <div>
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1 block mb-2">Mô tả sản phẩm</label>
                <textarea v-model="formData.description" rows="4" placeholder="Viết mô tả ngắn gọn về tính năng sản phẩm..." 
                  class="w-full px-5 py-4 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm leading-relaxed"></textarea>
              </div>
              <div>
                <label class="text-[10px] font-bold text-slate-400 uppercase ml-1 block mb-2">Thông số kỹ thuật</label>
                <textarea v-model="formData.specifications" rows="6" placeholder="Kích thước, chất liệu, cân nặng..." 
                  class="w-full px-5 py-4 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:ring-2 focus:ring-indigo-400 text-sm font-mono leading-relaxed"></textarea>
              </div>
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
import { useAdminProductStore } from '../../stores/admin_products'

const route = useRoute()
const router = useRouter()
const productStore = useAdminProductStore()

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

    const payload = {
      productName: formData.value.productName,
      productCode: formData.value.productCode.toUpperCase(),
      categoryId: Number(formData.value.categoryId),
      brandId: Number(formData.value.brandId),
      description: formData.value.description,
      specifications: formData.value.specifications,
      price: Number(formData.value.price),
      stockQuantity: Number(formData.value.stockQuantity),
    }

    let id

    if (isEditing.value) {
      await productStore.updateProduct(formData.value.id, payload)
      id = formData.value.id
    } else {
      const res = await productStore.addProduct(payload)
      id = res.productId
    }

    // upload ảnh riêng
    if (formData.value.imageFile) {
      console.log('Uploading image for product ID:', id)
      await productStore.uploadProductImage(id, formData.value.imageFile)
    }

    router.push('/admin/products')

  } catch (error) {
    alert('Lưu thất bại!')
  }
}

</script>