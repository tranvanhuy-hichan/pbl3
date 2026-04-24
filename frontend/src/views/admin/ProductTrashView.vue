<template>
  <div class="p-6 bg-slate-50 min-h-screen space-y-6">

    <div class="flex justify-between items-center">
      <div>
        <h2 class="text-2xl font-extrabold text-slate-800 tracking-tight">Thùng rác</h2>
      </div>

      <router-link 
        to="/admin/products"
        class="flex items-center gap-2 text-indigo-600 font-bold px-4 py-2 hover:bg-indigo-50 rounded-xl transition-all"
      >
        ← Quay lại
      </router-link>
    </div>

    <div class="bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden">

      <div class="px-5 py-3 border-b border-slate-100 flex justify-between items-center bg-slate-50">
        <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">
          Danh sách sản phẩm đã xóa
        </span>
        <span class="text-xs font-bold text-slate-400">
          {{ deletedProducts.length }} sản phẩm
        </span>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-5 text-[11px] font-black text-slate-500 uppercase tracking-widest">
                Sản phẩm
              </th>
              <th class="py-3 px-5 text-[11px] font-black text-slate-500 uppercase tracking-widest">
                Mã
              </th>
              <th class="py-3 px-5 text-[11px] font-black text-slate-500 uppercase tracking-widest">
                Giá bán
              </th>
            </tr>
          </thead>

          <tbody class="divide-y divide-slate-100">
            <tr
              v-for="product in deletedProducts"
              :key="product.productId || product.id"
              class="group cursor-pointer hover:bg-slate-50 transition-all"
              @click="goToProduct(product.productId)"
            >
              <td class="py-3 px-5 font-bold text-slate-800">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-xl bg-slate-100 flex items-center justify-center text-slate-500 font-black">
                    {{ product.productName?.charAt(0) }}
                  </div>

                  <div class="flex flex-col">
                    <span class="group-hover:text-indigo-600 transition-colors">
                      {{ product.productName }}
                    </span>
                  </div>
                </div>
              </td>

              <td class="py-3 px-5 text-slate-500 font-mono text-xs">
                {{ product.productCode }}
              </td>

              <td class="py-3 px-5 font-bold text-indigo-600">
                {{ formatCurrency(product.price) }}
              </td>
            </tr>

            <tr v-if="deletedProducts.length === 0">
              <td colspan="3" class="py-16 text-center">
                <div class="flex flex-col items-center justify-center gap-3 text-slate-300">
                  <div class="w-16 h-16 rounded-full bg-slate-100 flex items-center justify-center text-2xl">
                    🗑️
                  </div>
                  <p class="font-bold text-slate-400">Thùng rác trống</p>
                  <span class="text-xs text-slate-300">Không có sản phẩm nào bị xóa</span>
                </div>
              </td>
            </tr>

          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>


<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAdminProductStore } from '../../stores/admin_products'

const router = useRouter()
const productStore = useAdminProductStore()

const deletedProducts = ref([])

const fetchDeletedProducts = async () => {
  deletedProducts.value = await productStore.fetchDeletedProducts()
}

const restoreProduct = async (product) => {
  try {
    const id = product.productId || product.id

    await productStore.restoreProduct(id)

    deletedProducts.value = deletedProducts.value.filter(
      p => (p.productId || p.id) !== id
    )

    await productStore.fetchProducts()

    router.push('/admin/products')

  } catch (err) {
    alert("Khôi phục thất bại!")
  }
}

const goToProduct = (id) => {
  router.push(`/admin/products/trash/${id}`)
}

const formatCurrency = (val) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

onMounted(() => {
  fetchDeletedProducts()
})
</script>
