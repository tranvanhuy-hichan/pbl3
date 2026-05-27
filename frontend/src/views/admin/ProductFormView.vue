<template>
  <div class="min-h-screen bg-slate-50 pb-20">
    <div class="top-0 z-30 bg-slate-50 backdrop-blur-md border-b border-slate-200 mb-6">
      <div class="max-w-6xl mx-auto px-4 h-20 flex items-center justify-between">
        <div class="flex items-center gap-4">
          <button @click="router.back()" class="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-xl transition-all">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
          </button>
          <div>
            <h2 class="text-lg font-bold text-slate-800 leading-tight">{{ isEditing ? 'Chi tiết sản phẩm' : 'Tạo sản phẩm mới' }}</h2>
            <p class="text-xs text-slate-500 font-medium uppercase tracking-wider">Quản lý kho hàng & Hệ thống bán lẻ</p>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <button type="button" @click="router.back()" class="px-6 py-2.5 text-sm font-bold text-slate-500 hover:bg-slate-100 rounded-xl transition-all">
            Hủy bỏ
          </button>
          <button
            v-if="isEditing && !isTrashView"
            type="button"
            @click="confirmDelete({ productId: formData.id, productName: formData.productName })"
            class="px-6 py-2.5 text-sm font-bold text-rose-600 hover:bg-rose-50 rounded-xl transition-all"
          >
            Xóa sản phẩm
          </button>
          <button
            v-if="isTrashView"
            type="button"
            @click="confirmRestore({ productId: formData.id, productName: formData.productName })"
            class="px-6 py-2.5 text-sm font-bold text-green-600 hover:bg-rose-50 rounded-xl transition-all"
          >
            Khôi phục
          </button>
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
        
        <div class="lg:col-span-4 space-y-6">
          <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-200">
            <label class="block text-[11px] font-black text-slate-400 uppercase tracking-widest mb-4 text-center">Hình ảnh minh họa</label>
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

        <div class="lg:col-span-8 space-y-6">

          <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-200 space-y-6">
            <div>
              <label class="block text-[10px] font-black text-indigo-500 uppercase tracking-widest mb-2">Tên sản phẩm chính thức</label>
              <input v-model="formData.productName" type="text" required placeholder="Nhập tên sản phẩm..."
                class="w-full px-5 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-50 focus:border-indigo-500 outline-none transition-all text-xl font-bold text-slate-800">
            </div>

            <div class="grid grid-cols-2 gap-6">
              <div>
                <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2">Mã sản phẩm (SKU)</label>
                <input v-model="formData.productCode" type="text" readonly required placeholder="SKU-123..."
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

      <div v-if="isEditing && !isTrashView" class="mt-8">
        <div class="bg-white rounded-3xl shadow-sm border border-slate-200 p-8">
          <div class="flex items-center justify-between mb-6 border-b border-slate-200 pb-4">
            <div>
              <h3 class="text-lg font-bold text-slate-800">Đánh giá sản phẩm</h3>
              <p class="text-xs text-slate-500 mt-1">Quản lý và xem đánh giá từ khách hàng</p>
            </div>
            <div class="flex items-center gap-4">
              <div class="text-center">
                <div class="text-3xl font-bold text-indigo-600">{{ reviewStore.averageRating.toFixed(1) }}</div>
                <div class="flex gap-0.5 text-yellow-400 text-sm mt-1">
                  <span v-for="i in 5" :key="i">{{ i <= Math.round(reviewStore.averageRating) ? '★' : '☆' }}</span>
                </div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-slate-700">{{ reviewStore.totalCount }}</div>
                <div class="text-xs text-slate-500">đánh giá</div>
              </div>
            </div>
          </div>

          <div v-if="reviewStore.loading" class="text-center py-10">
            <div class="text-slate-400">Đang tải đánh giá...</div>
          </div>

          <div v-else-if="reviewStore.reviews.length === 0" class="text-center py-10">
            <svg class="w-16 h-16 mx-auto text-slate-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z"></path>
            </svg>
            <p class="text-slate-400 font-medium">Chưa có đánh giá nào</p>
            <p class="text-slate-300 text-sm mt-1">Sản phẩm này chưa nhận được đánh giá từ khách hàng</p>
          </div>

          <div v-else>
            <div class="space-y-4 max-h-[600px] overflow-y-auto pr-2">
              <div
                v-for="review in reviewStore.reviews"
                :key="review.reviewId"
                class="border border-slate-100 rounded-2xl p-5 hover:shadow-md transition-shadow"
              >
                <div class="flex items-start justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white font-bold text-sm shadow-md">
                      {{ review.userName?.charAt(0).toUpperCase() || '?' }}
                    </div>
                    <div>
                      <div class="font-bold text-slate-800 text-sm">{{ review.userName || 'Người dùng' }}</div>
                      <div class="flex gap-0.5 text-yellow-400 text-sm mt-1">
                        <span v-for="i in 5" :key="i">{{ i <= review.rating ? '★' : '☆' }}</span>
                      </div>
                    </div>
                  </div>
                  <div class="text-right">
                    <span class="text-xs text-slate-400">{{ formatDate(review.createdAt) }}</span>
                  </div>
                </div>
                <p class="text-slate-600 text-sm mt-3 pl-13">{{ review.comment }}</p>
              </div>
            </div>

            <div v-if="reviewStore.totalCount > reviewStore.pageSize" class="flex justify-center gap-2 mt-6 pt-4 border-t border-slate-100">
              <button
                @click="loadReviewsPage(reviewStore.currentPage - 1)"
                :disabled="reviewStore.currentPage === 1"
                class="px-4 py-2 border border-slate-200 rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
              >
                ← Trước
              </button>
              
              <div class="flex gap-1">
                <button
                  v-for="page in visiblePages"
                  :key="page"
                  @click="loadReviewsPage(page)"
                  :class="[
                    'w-10 h-10 rounded-xl text-sm font-medium transition-all',
                    reviewStore.currentPage === page
                      ? 'bg-indigo-600 text-white shadow-md'
                      : 'text-slate-600 hover:bg-slate-100'
                  ]"
                >
                  {{ page }}
                </button>
              </div>
              
              <button
                @click="loadReviewsPage(reviewStore.currentPage + 1)"
                :disabled="reviewStore.currentPage >= totalPages"
                class="px-4 py-2 border border-slate-200 rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
              >
                Sau →
              </button>
            </div>

            <div class="text-center mt-4 text-xs text-slate-400">
              Hiển thị {{ (reviewStore.currentPage - 1) * reviewStore.pageSize + 1 }} - 
              {{ Math.min(reviewStore.currentPage * reviewStore.pageSize, reviewStore.totalCount) }} 
              trong số {{ reviewStore.totalCount }} đánh giá
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6 transform transition-all">
        <div class="text-center">
          <div class="w-16 h-16 mx-auto mb-4 bg-red-100 rounded-full flex items-center justify-center">
            <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold text-slate-800 mb-2">Xác nhận xóa</h3>
          <p class="text-slate-500 mb-6">
            Bạn có chắc chắn muốn xóa sản phẩm 
            <span class="font-bold text-slate-800">"{{ productToDelete?.productName }}"</span>?
          </p>
          <div class="flex gap-3 justify-center">
            <button @click="showDeleteDialog = false" class="px-6 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-100 rounded-xl transition-all">
              Hủy bỏ
            </button>
            <button @click="executeDelete" class="px-6 py-2.5 bg-red-600 hover:bg-red-700 text-white font-medium rounded-xl shadow-lg transition-all">
              Xóa sản phẩm
            </button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showRestoreDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6 transform transition-all">
        <div class="text-center">
          <div class="w-16 h-16 mx-auto mb-4 bg-green-100 rounded-full flex items-center justify-center">
            <svg class="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold text-slate-800 mb-2">Xác nhận khôi phục</h3>
          <p class="text-slate-500 mb-6">
            Bạn có chắc chắn muốn khôi phục sản phẩm 
            <span class="font-bold text-slate-800">"{{ productToRestore?.productName }}"</span>?
          </p>
          <div class="flex gap-3 justify-center">
            <button @click="showRestoreDialog = false" class="px-6 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-100 rounded-xl transition-all">
              Hủy bỏ
            </button>
            <button @click="executeRestore" class="px-6 py-2.5 bg-green-600 hover:bg-green-700 text-white font-medium rounded-xl shadow-lg transition-all">
              Khôi phục
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAdminProductStore } from '../../stores/admin_products'
import { useReviewStore } from '@/stores/review'

const route = useRoute()
const router = useRouter()

const productStore = useAdminProductStore()
const reviewStore = useReviewStore()

const API_URL = 'https://localhost:7010'
const isEditing = computed(() => !!route.params.id)
const isTrashView = computed(() => route.path.includes('/trash'))

const showDeleteDialog = ref(false)
const showRestoreDialog = ref(false)
const productToDelete = ref(null)
const productToRestore = ref(null)

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
    return formData.value.imageUrl.startsWith('http')
      ? formData.value.imageUrl
      : API_URL + formData.value.imageUrl
  }
  return null
})

const totalPages = computed(() => {
  return Math.ceil(reviewStore.totalCount / reviewStore.pageSize)
})

const visiblePages = computed(() => {
  const current = reviewStore.currentPage
  const total = totalPages.value
  const delta = 2
  const range = []
  
  for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
    range.push(i)
  }
  
  if (current - delta > 2) {
    range.unshift('...')
  }
  if (current + delta < total - 1) {
    range.push('...')
  }
  
  range.unshift(1)
  if (total !== 1) {
    range.push(total)
  }
  
  return range.filter((v, i, a) => a.indexOf(v) === i)
})

const onImageChange = (e) => {
  const file = e.target.files[0]
  formData.value.imageFile = file || null
}

const formatPriceDisplay = (val) =>
  val ? new Intl.NumberFormat('vi-VN').format(val) : ''

const onPriceInputChange = (e) => {
  const rawValue = e.target.value.replace(/\D/g, '')
  formData.value.price = rawValue ? parseInt(rawValue, 10) : 0
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('vi-VN') + ' ' + date.toLocaleTimeString('vi-VN')
  } catch {
    return ''
  }
}

const loadReviewsPage = async (page) => {
  if (page < 1 || page > totalPages.value) return
  await reviewStore.fetchReviews(formData.value.id, page, reviewStore.pageSize)
}

const fetchProductReviews = async (productId) => {
  if (!productId) return
  reviewStore.reset()
  await reviewStore.fetchReviews(productId, 1, 5)
}

onMounted(async () => {
  await Promise.all([
    productStore.fetchCategories(),
    productStore.fetchBrands()
  ])

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
        
        await fetchProductReviews(data.productId)
      }
    } catch (err) {
      router.push('/admin/products')
    }
  }
})

watch(
  () => route.params.id,
  async (newId, oldId) => {
    if (newId && newId !== oldId && isEditing.value) {
      try {
        const data = await productStore.fetchProductById(newId)
        if (data) {
          formData.value.id = data.productId
          await fetchProductReviews(data.productId)
        }
      } catch (err) {
        console.error('Error fetching product:', err)
      }
    }
  }
)

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

    if (formData.value.imageFile) {
      await productStore.uploadProductImage(id, formData.value.imageFile)
    }

    if (isTrashView.value) {
      router.push('/admin/products/trash')
    } else {
      router.push('/admin/products')
    }
  } catch (error) {
    alert('Lưu thất bại!')
  }
}

const confirmDelete = (product) => {
  productToDelete.value = product
  showDeleteDialog.value = true
}

const executeDelete = async () => {
  if (!productToDelete.value) return

  const idToDelete =
    productToDelete.value.productId || productToDelete.value.id

  if (!idToDelete) return

  try {
    await productStore.deleteProduct(idToDelete)
    showDeleteDialog.value = false
    router.push('/admin/products')
  } catch (error) {
    alert('Lỗi khi xóa sản phẩm!')
  }
}

const confirmRestore = (product) => {
  productToRestore.value = product
  showRestoreDialog.value = true
}

const executeRestore = async () => {
  if (!productToRestore.value) return

  const idToRestore =
    productToRestore.value.productId || productToRestore.value.id

  if (!idToRestore) return

  try {
    await productStore.restoreProduct(idToRestore)
    showRestoreDialog.value = false
    router.push('/admin/products')
  } catch (error) {
    alert('Lỗi khi khôi phục sản phẩm!')
  }
}
</script>

<style scoped>
.pl-13 {
  padding-left: 3.25rem;
}
</style>