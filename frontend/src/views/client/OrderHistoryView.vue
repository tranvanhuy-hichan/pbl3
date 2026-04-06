<!-- src/views/user/MyOrders.vue -->
<template>
  <div class="space-y-6 p-6 bg-blue-50 min-h-screen font-sans">
    
    <!-- Header: Title + Filter & Stats -->
    <div class="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-blue-900 tracking-tight">Đơn hàng của tôi</h2>
        <p class="text-slate-500 text-sm">Quản lý và theo dõi trạng thái đơn hàng trực tuyến</p>
      </div>

      <div class="flex items-center gap-3 w-full lg:w-auto">
        <!-- Stats Badge -->
        <div class="hidden sm:flex items-center gap-2 bg-blue-100/50 px-3 py-2 rounded-xl border border-blue-200/50 shadow-sm">
          <span class="w-2 h-2 rounded-full bg-blue-500 animate-pulse"></span>
          <span class="text-sm font-semibold text-blue-800">
            {{ totalCount }} <span class="text-blue-600/70 text-xs font-normal uppercase ml-1">đơn hàng</span>
          </span>
        </div>

        <!-- Minimal Select Filter -->
        <div class="relative group flex-grow lg:flex-grow-0">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400 group-focus-within:text-blue-500">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
            </svg>
          </div>
          
          <select 
            v-model="statusFilter" 
            @change="onFilterChange"
            class="w-full lg:w-48 pl-9 pr-8 py-2.5 bg-white/70 hover:bg-white border border-slate-200 text-slate-700 text-sm rounded-xl focus:ring-4 focus:ring-blue-500/10 focus:border-blue-500 outline-none transition-all appearance-none cursor-pointer shadow-sm"
          >
            <option value="">Tất cả đơn hàng</option>
            <option value="Pending">Chờ xử lý</option>
            <option value="Processing">Đang xử lý</option>
            <option value="Shipped">Đang giao</option>
            <option value="Delivered">Đã giao</option>
            <option value="Cancelled">Đã hủy</option>
          </select>
          <div class="absolute inset-y-0 right-0 pr-2.5 flex items-center pointer-events-none text-slate-400">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" /></svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Table Danh sách đơn hàng -->
    <div v-if="!loading" class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã đơn</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Ngày đặt</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Tổng tiền</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Trạng thái</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="order in myOrders" :key="order.orderId" class="hover:bg-slate-50/80 transition-colors">
              <td class="py-4 px-6 text-sm font-bold text-slate-800">#{{ order.orderId }}</td>
              <td class="py-4 px-6 text-sm text-slate-600">{{ formatDate(order.orderDate) }}</td>
              <td class="py-4 px-6 text-sm font-bold text-slate-900">{{ Number(order.totalAmount).toLocaleString('vi-VN') }}₫</td>
              <td class="py-4 px-6">
                <span :class="['text-[10px] font-bold rounded-full px-3 py-1 border uppercase tracking-tight', getStatusClass(order.status)]">
                  {{ getStatusLabel(order.status) }}
                </span>
              </td>
              <td class="py-4 px-6 text-right">
                <div class="flex justify-end gap-2">
                  <button v-if="String(order.status).toLowerCase() === 'pending'" @click="handleCancelOrder(order.orderId)" class="text-red-600 hover:text-red-700 font-bold text-sm bg-red-50 hover:bg-red-100 px-3 py-2 rounded-lg transition-colors border border-red-100">Hủy đơn</button>
                  <button @click="viewOrderDetails(order.orderId)" class="text-blue-600 hover:text-blue-700 font-bold text-sm bg-blue-50 hover:bg-blue-100 px-4 py-2 rounded-lg transition-colors">Chi tiết</button>
                </div>
              </td>
            </tr>
            <tr v-if="myOrders.length === 0">
              <td colspan="5" class="py-20 text-center text-slate-400">
                <p class="text-lg font-medium text-slate-500">Không tìm thấy đơn hàng nào</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="px-6 py-4 bg-slate-50 border-t border-slate-100 flex items-center justify-between">
        <span class="text-sm text-slate-500 font-medium">Trang <span class="text-slate-800">{{ page }}</span> / {{ totalPages }}</span>
        <div class="flex items-center bg-white border border-slate-200 rounded-xl p-1 shadow-sm">
          <button @click="changePage(page - 1)" :disabled="page === 1" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" /></svg></button>
          <span class="px-4 text-sm font-bold text-indigo-600">{{ page }}</span>
          <button @click="changePage(page + 1)" :disabled="page >= totalPages" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg></button>
        </div>
      </div>
    </div>

    <!-- MODAL CHI TIẾT -->
    <div v-if="selectedOrder" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col scale-in-center">
        
        <!-- Header -->
        <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
          <div>
            <h3 class="text-xl font-bold text-slate-800">Chi tiết đơn hàng #{{ selectedOrder.orderId }}</h3>
            <p class="text-sm text-slate-500 mt-1">Ngày đặt: {{ formatDate(selectedOrder.orderDate, true) }}</p>
          </div>
          <button @click="selectedOrder = null" class="text-slate-400 hover:text-slate-600 p-2 hover:bg-white rounded-full transition-all">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>

        <div class="p-6 overflow-y-auto space-y-8">
          
          <!-- HÀNG 1: ĐỊA CHỈ & THÔNG TIN GIAO DỊCH (CHUNG 1 DÒNG) -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <!-- Địa chỉ -->
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Địa chỉ giao hàng</h4>
              <div class="bg-slate-50 p-4 rounded-xl border border-slate-100 h-full flex items-center">
                <p class="text-sm text-slate-700 leading-relaxed font-medium">{{ selectedOrder.shippingAddress }}</p>
              </div>
            </div>

            <!-- Thông tin giao dịch -->
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Thông tin giao dịch</h4>
              <div class="grid grid-cols-3 gap-2">
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1 text-center">PHƯƠNG THỨC</label>
                  <div class="text-[11px] font-bold rounded-lg py-2 border bg-white border-slate-200 text-slate-600 text-center shadow-sm">
                    {{ selectedOrder.paymentMethod }}
                  </div>
                </div>
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1 text-center">THANH TOÁN</label>
                  <div :class="['text-[11px] font-bold rounded-lg py-2 border text-center shadow-sm', getPaymentStatusClass(selectedOrder.paymentStatus)]">
                    {{ getPaymentStatusLabel(selectedOrder.paymentStatus) }}
                  </div>
                </div>
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1 text-center">TRẠNG THÁI</label>
                  <div :class="['text-[11px] font-bold rounded-lg py-2 border text-center shadow-sm', getStatusClass(selectedOrder.status)]">
                    {{ getStatusLabel(selectedOrder.status) }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- DANH SÁCH SẢN PHẨM -->
          <div class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Sản phẩm trong đơn</h4>
            <div class="border border-slate-100 rounded-xl overflow-hidden shadow-sm">
              <table class="w-full text-left border-collapse">
                <thead class="bg-slate-50/80">
                  <tr>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500">Sản phẩm</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-center">SL</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-right">Đơn giá</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-right">Thành tiền</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 bg-white">
                  <tr v-for="item in selectedOrder.orderDetails" :key="item.productId">
                    <td class="py-4 px-4 flex items-center gap-3">
                      <img :src="item.imageURL ? `https://localhost:7010${item.imageURL}` : 'https://via.placeholder.com/150'" class="h-14 w-14 object-contain rounded-lg border bg-white" />
                      <div class="flex flex-col">
                        <span class="font-bold text-slate-800 text-sm line-clamp-1">{{ item.productName }}</span>
                        <div class="text-[10px] text-slate-400 font-medium uppercase">MÃ: {{ item.productCode }}</div>
                      </div>
                    </td>
                    <td class="py-4 px-4 text-center text-sm font-medium">{{ item.quantity }}</td>
                    <td class="py-4 px-4 text-right text-sm">{{ Number(item.unitPrice).toLocaleString('vi-VN') }}₫</td>
                    <td class="py-4 px-4 text-right text-sm font-bold text-slate-900">{{ Number(item.subtotal).toLocaleString('vi-VN') }}₫</td>
                  </tr>
                </tbody>
                <tfoot class="bg-slate-50/50">
                  <tr>
                    <td colspan="3" class="py-4 px-4 text-right font-bold text-slate-500">Tổng thanh toán:</td>
                    <td class="py-4 px-4 text-right font-black text-blue-600 text-lg">{{ Number(selectedOrder.totalAmount).toLocaleString('vi-VN') }}₫</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <!-- GHI CHÚ NẰM RIÊNG -->
          <div v-if="selectedOrder.notes" class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Ghi chú</h4>
            <div class="bg-amber-50 border border-amber-100 p-4 rounded-xl italic text-sm text-amber-800">
              "{{ selectedOrder.notes }}"
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="px-6 py-4 border-t border-slate-100 bg-slate-50 flex justify-end gap-3">
          <button v-if="String(selectedOrder.status).toLowerCase() === 'pending'" @click="handleCancelOrder(selectedOrder.orderId)" class="px-6 py-2.5 bg-red-50 text-red-600 rounded-xl font-bold hover:bg-red-100 transition-all border border-red-200">Hủy đơn hàng</button>
          <button @click="selectedOrder = null" class="px-8 py-2.5 bg-slate-800 text-white rounded-xl font-bold hover:bg-slate-900 transition-all">Đóng</button>
        </div>
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="py-20 text-center">
       <div class="inline-block animate-spin rounded-full h-10 w-10 border-4 border-blue-500 border-t-transparent"></div>
       <p class="text-slate-500 mt-4 font-medium animate-pulse">Đang tải dữ liệu...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useOrderStore } from '@/stores/orders'

const orderStore = useOrderStore()
const { myOrders, page, pageSize, totalCount, statusFilter } = storeToRefs(orderStore)

const loading = ref(false)
const selectedOrder = ref(null)

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value) || 1)

const loadData = async () => {
  loading.value = true
  try { await orderStore.fetchMyOrders() } finally { loading.value = false }
}

const viewOrderDetails = async (orderId) => {
  try {
    const data = await orderStore.fetchMyOrderDetails(orderId)
    if (data) selectedOrder.value = data
  } catch (err) { alert("Không thể tải thông tin đơn hàng.") }
}

const handleCancelOrder = async (orderId) => {
  if (confirm(`Xác nhận hủy đơn hàng #${orderId}?`)) {
    loading.value = true
    try {
      await orderStore.cancelOrder(orderId)
      alert("Đã hủy đơn thành công.")
      selectedOrder.value = null 
      await loadData()
    } catch (err) { alert(err.response?.data?.message || "Lỗi khi hủy đơn.") }
    finally { loading.value = false }
  }
}

const changePage = (p) => { if (p >= 1 && p <= totalPages.value) { orderStore.page = p; loadData() } }
const onFilterChange = () => { orderStore.page = 1; loadData() }

const formatDate = (dateString, includeTime = false) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', ...(includeTime && { hour: '2-digit', minute: '2-digit' }) })
}

const getStatusClass = (status) => {
  switch (String(status).toLowerCase()) {
    case 'pending': return 'bg-amber-100 text-amber-700 border-amber-200';
    case 'processing': return 'bg-blue-100 text-blue-700 border-blue-200';
    case 'shipped': return 'bg-purple-100 text-purple-700 border-purple-200';
    case 'delivered': return 'bg-green-100 text-green-700 border-green-200';
    case 'cancelled': return 'bg-red-100 text-red-700 border-red-200';
    default: return 'bg-slate-100 text-slate-600 border-slate-200';
  }
}

const getPaymentStatusClass = (status) => {
  return String(status).toLowerCase() === 'paid' ? 'bg-green-50 text-green-600 border-green-200' : 'bg-red-50 text-red-600 border-red-200';
}

const getStatusLabel = (s) => ({ Pending: 'Chờ xử lý', Processing: 'Đang xử lý', Shipped: 'Đang giao', Delivered: 'Đã giao', Cancelled: 'Đã hủy' }[s] || s)
const getPaymentStatusLabel = (s) => ({ Paid: 'Đã thanh toán', Unpaid: 'Chưa thanh toán' }[s] || s)

onMounted(loadData)
</script>

<style scoped>
.scale-in-center { animation: scale-in-center 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94) both; }
@keyframes scale-in-center { 0% { transform: scale(0.95); opacity: 0; } 100% { transform: scale(1); opacity: 1; } }
</style>