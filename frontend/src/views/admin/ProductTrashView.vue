<template>
  <div class="p-6 bg-slate-50-50 min-h-screen space-y-6">

    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-rose-600">Thùng rác</h2>
      <router-link to="/admin/products" class="text-blue-600 font-bold px-3 py-2 hover:bg-blue-50 rounded-xl">Quay lại danh sách</router-link>
    </div>

    <div class="bg-white rounded-2xl shadow-sm border border-rose-200 overflow-x-auto">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-slate-50 border-b border-rose-200">
            <th class="py-3 px-4 text-xs font-bold text-rose-700 uppercase">Tên sản phẩm</th>
            <th class="py-3 px-4 text-xs font-bold text-rose-700 uppercase">Mã</th>
            <th class="py-3 px-4 text-xs font-bold text-rose-700 uppercase">Giá bán</th>
            <th class="py-3 px-4 text-xs font-bold text-rose-700 uppercase text-right">Khôi phục</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-rose-100">
          <tr v-for="product in deletedProducts" :key="product.id" class="hover:bg-rose-50 transition-colors">
            <td class="py-2 px-4 font-bold text-rose-900">
            <router-link 
                :to="`/admin/products/${product.productId}/edit`" 
                class="hover:underline hover:text-blue-600 transition-colors">
                {{ product.productName }}
            </router-link>
            </td>
            <td class="py-2 px-4 text-rose-500">{{ product.productCode }}</td>
            <td class="py-2 px-4 font-bold text-rose-900">{{ formatCurrency(product.price) }}</td>
            <td class="py-2 px-4 text-right">
              <button @click="restoreProduct(product)" class="px-3 py-1 bg-rose-500 text-white font-bold rounded-xl hover:bg-rose-600 transition-colors">
                Khôi phục
              </button>
            </td>
          </tr>
          <tr v-if="deletedProducts.length === 0">
            <td colspan="4" class="py-4 text-center text-rose-400">Chưa có sản phẩm nào bị xóa</td>
          </tr>
        </tbody>
      </table>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAdminProductStore } from '../../stores/admin_products'

const productStore = useAdminProductStore()
const deletedProducts = ref([])

const fetchDeletedProducts = async () => {
  deletedProducts.value = await productStore.fetchDeletedProducts()
}

const restoreProduct = async (product) => {
  try {
    await productStore.restoreProduct(product.productId || product.id)
    deletedProducts.value = deletedProducts.value.filter(p => p.id !== product.id)
    await fetchDeletedProducts()
    productStore.fetchProducts() 
  } catch (err) {
    alert("Khôi phục thất bại!")
  }
}

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

onMounted(() => {
  fetchDeletedProducts()
})
</script>


