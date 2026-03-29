<!-- src/views/admin/OrderManagement.vue -->
<template>
  <div class="space-y-6 p-6 bg-slate-50 min-h-screen">

    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý đơn hàng</h2>
        <p class="text-slate-500 text-sm mt-1">Quản lý và theo dõi tất cả đơn hàng của khách hàng</p>
      </div>
      <div class="flex gap-2">
        <div class="bg-amber-50 text-amber-700 px-3 py-1.5 rounded-lg text-sm border border-amber-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-amber-500"></span>
          <span class="font-medium">{{ pendingCount }} Pending</span>
        </div>
        <div class="bg-blue-50 text-blue-700 px-3 py-1.5 rounded-lg text-sm border border-blue-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-blue-500"></span>
          <span class="font-medium">{{ processingCount }} Processing</span>
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col md:flex-row items-center justify-between gap-4">
      <!-- Search -->
      <div class="relative w-full md:max-w-md">
        <input
          type="text"
          v-model="searchQuery"
          @input="debouncedFetch"
          placeholder="Search by Order ID or User ID..."
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>

      <!-- Filters -->
      <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto items-end">

        <!-- Status Filter -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Trạng thái đơn</label>
          <select
            v-model="statusFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-slate-700 text-sm"
          >
            <option value="">Tất cả</option>
            <option value="Pending">Pending</option>
            <option value="Processing">Processing</option>
            <option value="Shipped">Shipped</option>
            <option value="Delivered">Delivered</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <!-- Payment Status Filter -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Trạng thái thanh toán</label>
          <select
            v-model="paymentStatusFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-slate-700 text-sm"
          >
            <option value="">Tất cả</option>
            <option value="Paid">Đã thanh toán</option>
            <option value="Unpaid">Chưa thanh toán</option>
          </select>
        </div>

        <!-- From Date -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Từ ngày</label>
          <input
            type="date"
            v-model="fromDateFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-slate-700 text-sm"
          />
        </div>

        <!-- To Date -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Đến ngày</label>
          <input
            type="date"
            v-model="toDateFilter"
            :min="fromDateFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-slate-700 text-sm"
          />
        </div>

        <button
          @click="resetAndFetch"
          class="text-sm text-white bg-slate-500 hover:bg-slate-600 font-medium whitespace-nowrap px-4 py-2 rounded-lg transition-colors self-end"
        >
          Reset
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="py-20 text-center">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
      <p class="mt-2 text-slate-500 font-medium">Loading orders...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="bg-red-50 text-red-700 p-6 rounded-xl border border-red-100 text-center">
      <p class="font-medium">{{ error }}</p>
      <button @click="loadData" class="mt-3 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
        Retry
      </button>
    </div>

    <!-- Table -->
    <div v-else class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã đơn</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Khách hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Ngày đặt hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Tổng tiền</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Trạng thái</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr
              v-for="order in orders"
              :key="order.orderId"
              class="hover:bg-slate-50/80 transition-colors"
            >
              <td class="py-4 px-6 text-sm">
                <div class="font-bold text-slate-800">#{{ order.orderId }}</div>
              </td>
              <td class="py-4 px-6 text-sm">
                <div class="font-medium text-slate-800">{{ order.fullName }}</div>
                <div class="text-xs text-slate-500 mt-1">ID: {{ order.userId }}</div>
              </td>
              <td class="py-4 px-6 text-sm text-slate-600">
                {{ formatDate(order.orderDate) }}
              </td>
              <td class="py-4 px-6 text-sm font-bold text-slate-900">
                {{ Number(order.totalAmount).toLocaleString('vi-VN') }}₫
              </td>
              <td class="py-4 px-6">
                <select
                  v-model="order.status"
                  @change="handleStatusChange(order)"
                  :disabled="updatingId === order.orderId"
                  :class="[
                    'text-xs font-bold rounded-full px-4 py-1.5 border appearance-none cursor-pointer focus:outline-none focus:ring-2 focus:ring-offset-1',
                    getStatusClass(order.status),
                    updatingId === order.orderId ? 'opacity-50 cursor-not-allowed' : ''
                  ]"
                >
                  <option value="Pending">Pending</option>
                  <option value="Processing">Processing</option>
                  <option value="Shipped">Shipped</option>
                  <option value="Delivered">Delivered</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </td>
              <td class="py-4 px-6 text-right">
                <button
                  @click="viewOrderDetails(order.orderId)"
                  class="text-blue-600 hover:text-blue-700 font-bold text-sm bg-blue-50 hover:bg-blue-100 px-4 py-2 rounded-lg transition-colors"
                >
                  View Details
                </button>
              </td>
            </tr>

            <tr v-if="orders.length === 0">
              <td colspan="6" class="py-20 text-center text-slate-400">
                <svg class="mx-auto h-12 w-12 text-slate-200 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                </svg>
                <p class="text-lg font-medium">Không tìm thấy đơn hàng nào</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="px-6 py-4 bg-slate-50/50 border-t border-slate-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="text-sm text-slate-500 font-medium">
          Hiển thị
          <span class="text-slate-800 font-bold">{{ startEntry }}</span> -
          <span class="text-slate-800 font-bold">{{ endEntry }}</span>
          trong tổng số
          <span class="text-indigo-600 font-bold">{{ totalCount }}</span> đơn hàng
        </div>

        <div class="flex items-center gap-6">
          <!-- Page size -->
          <div class="flex items-center gap-2">
            <span class="text-xs text-slate-400 font-bold uppercase">Mỗi trang:</span>
            <select
              :value="pageSize"
              @change="e => { orderStore.pageSize = Number(e.target.value); orderStore.page = 1; loadData() }"
              class="bg-white border border-slate-200 rounded-lg px-2 py-1 text-sm font-bold text-slate-700 focus:outline-none"
            >
              <option :value="10">10</option>
              <option :value="20">20</option>
              <option :value="50">50</option>
            </select>
          </div>

          <!-- Prev / Page / Next -->
          <div class="flex items-center bg-white border border-slate-200 rounded-xl p-1 shadow-sm">
            <button
              @click="orderStore.changePage(page - 1)"
              :disabled="page === 1"
              class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
              </svg>
            </button>

            <div class="flex items-center px-2 gap-1">
              <span class="px-3 py-1 bg-indigo-50 text-indigo-600 rounded-lg text-sm font-bold">{{ page }}</span>
              <span class="text-sm font-bold text-slate-400">/ {{ totalPages }}</span>
            </div>

            <button
              @click="orderStore.changePage(page + 1)"
              :disabled="page >= totalPages"
              class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Chi Tiết -->
    <div v-if="selectedOrder" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">

        <!-- Modal Header -->
        <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
          <div>
            <h3 class="text-xl font-bold text-slate-800">Đơn hàng #{{ selectedOrder.orderId }}</h3>
            <p class="text-sm text-slate-500 mt-1">Dặt hàng lúc {{ formatDate(selectedOrder.orderDate, true) }}</p>
          </div>
          <button @click="selectedOrder = null" class="text-slate-400 hover:text-slate-600 p-2 hover:bg-white rounded-full transition-all">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Modal Body -->
        <div class="p-6 overflow-y-auto space-y-8">

          <!-- Customer & Shipping -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Thông tin khách hàng</h4>
              <div class="bg-slate-50 p-4 rounded-xl border border-slate-100">
                <p class="font-bold text-slate-800 text-lg">{{ selectedOrder.userName }}</p>
                <p class="text-sm text-slate-500 mt-1">ID: {{ selectedOrder.userId }}</p>
              </div>
            </div>
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Địa chỉ giao hàng</h4>
              <div class="bg-slate-50 p-4 rounded-xl border border-slate-100">
                <p class="text-sm text-slate-700 leading-relaxed">{{ selectedOrder.shippingAddress }}</p>
              </div>
            </div>
          </div>

          <!-- Order Items -->
          <div class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Sản phẩm</h4>
            <div class="border border-slate-100 rounded-xl overflow-hidden">
              <table class="w-full text-left">
                <thead class="bg-slate-50">
                  <tr>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500">Sản phẩm</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-center">SL</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-right">Đơn giá</th>
                    <th class="py-3 px-4 text-xs font-bold text-slate-500 text-right">Thành tiền</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                  <tr v-for="item in selectedOrder.orderDetails" :key="item.productId">
                    <td class="py-4 px-4">
                      <div class="font-bold text-slate-800">{{ item.productName }}</div>
                      <div class="text-xs text-slate-400">SKU: {{ item.productId }}</div>
                    </td>
                    <td class="py-4 px-4 text-center text-sm font-medium">{{ item.quantity }}</td>
                    <td class="py-4 px-4 text-right text-sm">{{ Number(item.unitPrice).toLocaleString('vi-VN') }}₫</td>
                    <td class="py-4 px-4 text-right text-sm font-bold text-slate-900">{{ Number(item.subtotal).toLocaleString('vi-VN') }}₫</td>
                  </tr>
                </tbody>
                <tfoot class="bg-slate-50/50">
                  <tr>
                    <td colspan="3" class="py-4 px-4 text-right font-bold text-slate-500">Tổng cộng:</td>
                    <td class="py-4 px-4 text-right font-black text-indigo-600 text-lg">{{ Number(selectedOrder.totalAmount).toLocaleString('vi-VN') }}₫</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <!-- Payment & Notes -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Thanh toán</h4>
              <div class="flex gap-3 flex-wrap">
                <div class="px-4 py-2 bg-slate-100 rounded-lg text-xs font-bold text-slate-600 uppercase">
                  {{ selectedOrder.paymentMethod }}
                </div>
                <div class="px-4 py-2 bg-green-50 rounded-lg text-xs font-bold text-green-600 uppercase">
                  {{ selectedOrder.paymentStatus }}
                </div>
                <div class="px-4 py-2 bg-green-50 rounded-lg text-xs font-bold text-green-600 uppercase">
                  {{ selectedOrder.status }}
                </div>
              </div>
            </div>
            <div v-if="selectedOrder.notes" class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Ghi chú</h4>
              <p class="text-sm text-slate-600 italic">"{{ selectedOrder.notes }}"</p>
            </div>
          </div>

        </div>

        <!-- Modal Footer -->
        <div class="px-6 py-4 border-t border-slate-100 bg-slate-50 flex justify-end">
          <button
            @click="selectedOrder = null"
            class="px-6 py-2.5 bg-slate-800 text-white rounded-xl font-bold hover:bg-slate-900 transition-all"
          >
            Đóng
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useOrderStore } from '../../stores/orders'

const orderStore = useOrderStore()
const {
  orders, totalCount, page, pageSize, paymentStatusFilter,
  searchQuery, statusFilter, fromDateFilter, toDateFilter,
  pendingCount, processingCount
} = storeToRefs(orderStore)

// --- Computed phân trang ---
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value) || 1)
const startEntry = computed(() => totalCount.value === 0 ? 0 : (page.value - 1) * pageSize.value + 1)
const endEntry   = computed(() => Math.min(page.value * pageSize.value, totalCount.value))

// --- Local UI State ---
const loading = ref(false)
const error = ref(null)
const updatingId = ref(null)
const selectedOrder = ref(null)

// --- Methods ---
const loadData = async () => {
  loading.value = true
  error.value = null
  try {
    await orderStore.fetchOrders()
  } catch (err) {
    error.value = err.message || "Failed to load orders."
  } finally {
    loading.value = false
  }
}

const handleStatusChange = async (order) => {
  updatingId.value = order.orderId
  try {
    await orderStore.updateOrderStatus(order.orderId, order.status)
  } catch (err) {
    alert("Update failed: " + err.message)
    loadData()
  } finally {
    updatingId.value = null
  }
}

const viewOrderDetails = async (orderId) => {
  try {
    const data = await orderStore.fetchOrderById(orderId)
    selectedOrder.value = data
  } catch (err) {
    alert("Could not load order details.")
  }
}

let timeout = null
const debouncedFetch = () => {
  clearTimeout(timeout)
  timeout = setTimeout(() => {
    orderStore.page = 1
    loadData()
  }, 500)
}

const resetAndFetch = () => {
  orderStore.resetFilters()
  loadData()
}

const formatDate = (dateString, includeTime = false) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    ...(includeTime && { hour: '2-digit', minute: '2-digit' })
  })
}

const getStatusClass = (status) => {
  switch (String(status).toLowerCase()) {
    case 'pending':    return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'processing': return 'bg-blue-100 text-blue-700 border-blue-200'
    case 'shipped':    return 'bg-purple-100 text-purple-700 border-purple-200'
    case 'delivered':  return 'bg-green-100 text-green-700 border-green-200'
    case 'cancelled':
    case 'cancel':     return 'bg-red-100 text-red-700 border-red-200'
    default:           return 'bg-slate-100 text-slate-600 border-slate-200'
  }
}

onMounted(loadData)
</script>

<style scoped>
::-webkit-scrollbar { width: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
::-webkit-scrollbar-thumb:hover { background: #cbd5e1; }
</style>