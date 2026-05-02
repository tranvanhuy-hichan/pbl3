<!-- src/views/admin/OrderManagement.vue -->
<template>
  <div class="space-y-6 p-6 bg-slate-50 min-h-screen">

    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-blue-900 tracking-tight">Quản lý và theo dõi tất cả đơn hàng</h2>
      </div>
      <div class="flex gap-2">
        <div class="bg-amber-50 text-amber-700 px-3 py-1.5 rounded-lg text-sm border border-amber-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-amber-500"></span>
          <span class="font-medium">{{ pendingCount }} Chờ xử lý</span>
        </div>
        <div class="bg-slate-50 text-blue-700 px-3 py-1.5 rounded-lg text-sm border border-blue-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-slate-500"></span>
          <span class="font-medium">{{ processingCount }} Đang xử lý</span>
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col md:flex-row items-center justify-between gap-4 ">
      <!-- Search -->
      <div class="relative w-full md:max-w-md">
         <label class="text-xs text-slate-500 mb-1 font-medium bg-white">Tìm kiếm</label>
        <input 
          type="text"
          v-model="searchQuery"
          @input="debouncedFetch"
          placeholder="Tìm theo mã đơn, ID khách..."
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-slate-50"
        />
        <svg class="absolute left-3 top-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>

      <!-- Filters -->
      <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto items-end">

        <!-- Status Filter -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Trạng thái đơn</label>
          <select v-model="statusFilter" @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-slate-50 text-slate-700 text-sm">
            <option value="">Tất cả</option>
            <option value="Pending">Chờ xử lý</option>
            <option value="Processing">Đang xử lý</option>
            <option value="Shipped">Đang giao</option>
            <option value="Delivered">Đã giao</option>
            <option value="Cancelled">Đã hủy</option>
          </select>
        </div>

        <!-- Payment Status Filter -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Trạng thái thanh toán</label>
          <select v-model="paymentStatusFilter" @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-slate-50 text-slate-700 text-sm">
            <option value="">Tất cả</option>
            <option value="Paid">Đã thanh toán</option>
            <option value="Unpaid">Chưa thanh toán</option>
          </select>
        </div>

        <!-- From Date -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Từ ngày</label>
          <input type="date" v-model="fromDateFilter" @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-slate-50 text-slate-700 text-sm" />
        </div>

        <!-- To Date -->
        <div class="flex flex-col w-full md:w-auto">
          <label class="text-xs text-slate-500 mb-1 font-medium">Đến ngày</label>
          <input type="date" v-model="toDateFilter" :min="fromDateFilter" @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-slate-50 text-slate-700 text-sm" />
        </div>

        <!-- Sort + Reset -->
        <div class="flex items-end gap-2">
          <!-- Sort -->
          <div class="w-24">
            <label class="text-xs text-slate-500 mb-1 font-medium">Sắp xếp</label>
            <select v-model="sortOrder" @change="onSortChange"
              class="w-full pl-3 pr-2 py-2 bg-slate-50 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all text-sm cursor-pointer">
              <option value="desc">Mới nhất</option>
              <option value="asc">Cũ nhất</option>
            </select>
          </div>

          <!-- Reset -->
          <button @click="resetAndFetch"
            class="text-sm text-white bg-slate-500 hover:bg-slate-600 font-medium px-4 py-2 rounded-lg transition-colors">
            Reset
          </button>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="py-20 text-center">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
      <p class="mt-2 text-slate-500 font-medium">Đang tải đơn hàng...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="bg-red-50 text-red-700 p-6 rounded-xl border border-red-100 text-center">
      <p class="font-medium">{{ error }}</p>
      <button @click="loadData" class="mt-3 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
        Thử lại
      </button>
    </div>

    <!-- Table -->
    <div v-else class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse ">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã đơn</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Khách hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Ngày đặt hàng</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Tổng tiền</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Trạng thái</th>
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thanh toán</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr
              v-for="order in orders"
              :key="order.orderId"
              class="hover:bg-slate-50/80 transition-colors"
              @click="viewOrderDetails(order.orderId)"
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
              <td class="py-2.5 px-4">
                <span :class="[
                  'text-[10px] font-bold rounded-full px-3 py-1 border uppercase tracking-tight whitespace-nowrap inline-block', 
                  getStatusClass(order.status)
                ]">
                  {{ getStatusLabel(order.status) }}
                </span>
              </td>
              <td class="py-2.5 px-4 text-right">
                <span :class="[
                  'text-[10px] font-bold rounded-full px-3 py-1 border uppercase tracking-tight whitespace-nowrap inline-block ', 
                  getPaymentStatusClass(order.paymentStatus)
                ]">
                  {{ getPaymentStatusLabel(order.paymentStatus) }}
                </span>
              </td>
            </tr>

            <tr v-if="orders.length === 0">
              <td colspan="7" class="py-20 text-center text-slate-400">
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
      <div class="px-6 py-4 bg-slate-50 border-t border-slate-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="text-sm text-slate-500 font-medium">
          Hiển thị
          <span class="text-slate-800 font-bold">{{ startEntry }}</span> -
          <span class="text-slate-800 font-bold">{{ endEntry }}</span>
          trong tổng số
          <span class="text-indigo-600 font-bold">{{ totalCount }}</span> đơn hàng
        </div>

        <div class="flex items-center gap-6">
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
  <div class="bg-white rounded-2xl shadow-2xl w-full max-w-5xl max-h-[90vh] overflow-hidden flex flex-col">

    <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
      <div>
        <h3 class="text-xl font-bold text-slate-800">
          Đơn hàng #{{ selectedOrder.orderId }}
        </h3>
        <p class="text-sm text-slate-500 mt-1">
          Đặt hàng lúc {{ formatDate(selectedOrder.orderDate, true) }}
        </p>
      </div>

      <button
        @click="selectedOrder = null"
        class="text-slate-400 hover:text-slate-600 p-2 hover:bg-white rounded-full transition-all"
      >
        ✕
      </button>
    </div>

    <div class="p-6 overflow-y-auto">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <div class="lg:col-span-2 space-y-8">

          <div class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">
              Thông tin khách hàng
            </h4>

            <div class="bg-slate-50 p-4 rounded-xl border border-slate-100">
              <p class="font-bold text-slate-800 text-lg">
                {{ selectedOrder.userName }}
              </p>
              <p class="text-sm text-slate-500 mt-1">
                Điện thoại: {{ selectedOrder.phoneNumber }}
              </p>
            </div>
          </div>

          <div class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">
              Thanh toán & Trạng thái
            </h4>

            <div class="flex gap-3 flex-wrap">

              <div class="flex flex-col">
                <label class="text-[10px] font-bold text-slate-400 mb-1">PHƯƠNG THỨC</label>
                <div class="text-xs font-bold rounded-lg px-3 py-2 border bg-slate-50 border-slate-200 text-slate-600">
                  {{ selectedOrder.paymentMethod }}
                </div>
              </div>

              <div class="flex flex-col">
                <label class="text-[10px] font-bold text-slate-400 mb-1">THANH TOÁN</label>
                <select
                  v-model="selectedOrder.paymentStatus"
                  @change="handlePaymentStatusChange(selectedOrder)"
                  class="text-xs font-bold rounded-lg px-3 py-2 border"
                >
                  <option value="Paid">Đã thanh toán</option>
                  <option value="Unpaid">Chưa thanh toán</option>
                </select>
              </div>

              <div class="flex flex-col">
                <label class="text-[10px] font-bold text-slate-400 mb-1">TRẠNG THÁI</label>
                <select
                  v-model="selectedOrder.status"
                  @change="handleStatusChange(selectedOrder)"
                  class="text-xs font-bold rounded-lg px-3 py-2 border"
                >
                  <option value="Pending">Chờ xử lý</option>
                  <option value="Processing">Đang xử lý</option>
                  <option value="Shipped">Đang giao</option>
                  <option value="Delivered">Đã giao</option>
                  <option value="Cancelled">Hủy</option>
                </select>
              </div>

            </div>
          </div>

          <div class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">
              Địa chỉ giao hàng
            </h4>

            <div class="bg-slate-50 p-4 rounded-xl border border-slate-100">
              {{ selectedOrder.shippingAddress }}
            </div>
          </div>

          <div v-if="selectedOrder.notes" class="space-y-3">
            <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">
              Ghi chú
            </h4>

            <div class="bg-amber-50 p-3 rounded-xl border border-amber-100">
              {{ selectedOrder.notes }}
            </div>
          </div>

        </div>

        <div class="lg:col-span-1 space-y-3 sticky top-4">

          <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">
            Sản phẩm
          </h4>

          <div class="border border-slate-100 rounded-xl overflow-hidden">
            <table class="w-full text-left">
              <tbody class="divide-y divide-slate-100">

                <tr v-for="item in selectedOrder.orderDetails" :key="item.productId">
                  <td class="p-3 cursor-pointer" @click="goToProduct(item.productId)">
                    <div class="flex gap-3 items-center">

                      <img
                        :src="item.imageURL ? `https://localhost:7010${item.imageURL}` : 'https://via.placeholder.com/150'"
                        class="h-12 w-12 object-contain rounded-lg"
                      />

                      <div class="flex-1">
                        <router-link  class="font-bold text-sm hover:text-blue-600 transition-colors">
                          {{ item.productName }}
                        </router-link>
                        <p class="text-xs text-slate-400 flex justify-between w-full">
                          <span class="text-green-500">x{{ item.quantity }}</span>
                          <span class="text-red-500 font-bold">
                            {{ Number(item.subtotal).toLocaleString('vi-VN') }}₫
                          </span>
                        </p>
                      </div>

                    </div>
                  </td>
                </tr>

              </tbody>

              <tfoot>
                <tr>
                  <td class="p-3 font-bold text-right text-indigo-600">
                    Tổng: {{ Number(selectedOrder.totalAmount).toLocaleString('vi-VN') }}₫
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>

        </div>

      </div>
    </div>

    <div class="px-6 py-4 border-t border-slate-100 bg-slate-50 flex justify-end gap-3">
      
      <button
        v-if="selectedOrder.status === 'Processing'"
        @click="exportPDF(selectedOrder.orderId)"
        class="px-5 py-2 bg-green-600 text-white rounded-xl font-bold hover:bg-green-700"
      >
        Xuất PDF
      </button>

      <button
        @click="selectedOrder = null"
        class="px-5 py-2 bg-slate-800 text-white rounded-xl font-bold hover:bg-slate-900"
      >
        Đóng
      </button>

    </div>

  </div>
</div>

    <div v-if="showStatusDialog && pendingStatusChange" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <h3 class="text-xl font-bold text-slate-800 mb-2 text-center">Xác nhận chuyển trạng thái</h3>
        <p class="text-slate-500 text-sm text-center mb-6">
          Chuyển đơn hàng <span class="font-bold text-slate-800">#{{ pendingStatusChange.order.orderId }}</span> từ
          <span class="font-bold text-blue-600">{{ getStatusLabel(pendingStatusChange.oldStatus) }}</span> sang
          <span class="font-bold text-green-600">{{ getStatusLabel(pendingStatusChange.newStatus) }}</span>?
        </p>
        <div class="flex gap-3 justify-center">
          <button @click="cancelStatusChange" class="px-5 py-2 text-slate-600 font-bold hover:bg-slate-100 rounded-xl">Hủy</button>
          <button @click="confirmStatusChange" class="px-6 py-2 bg-blue-600 text-white font-bold rounded-xl shadow hover:bg-blue-700">Xác nhận</button>
        </div>
      </div>
    </div>

    <div v-if="showPaymentStatusDialog && pendingPaymentStatusChange" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <h3 class="text-xl font-bold text-slate-800 mb-2 text-center">Xác nhận thanh toán</h3>
        <p class="text-slate-500 text-sm text-center mb-6">
          Cập nhật thanh toán cho đơn hàng <span class="font-bold text-slate-800">#{{ pendingPaymentStatusChange.order.orderId }}</span> thành 
          <span class="font-bold text-green-600">{{ getPaymentStatusLabel(pendingPaymentStatusChange.newPaymentStatus) }}</span>?
        </p>
        <div class="flex gap-3 justify-center">
          <button @click="cancelPaymentStatusChange" class="px-5 py-2 text-slate-600 font-bold hover:bg-slate-100 rounded-xl">Hủy</button>
          <button @click="confirmPaymentStatusChange" class="px-6 py-2 bg-blue-600 text-white font-bold rounded-xl shadow">Xác nhận</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useOrderStore } from '../../stores/orders'
import { useRouter } from 'vue-router'

const router = useRouter()
const orderStore = useOrderStore()
const {
  orders, totalCount, page, pageSize, paymentStatusFilter,
  searchQuery, statusFilter, fromDateFilter, toDateFilter,
  pendingCount, processingCount, sortOrder
} = storeToRefs(orderStore)

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value) || 1)
const startEntry = computed(() => totalCount.value === 0 ? 0 : (page.value - 1) * pageSize.value + 1)
const endEntry   = computed(() => Math.min(page.value * pageSize.value, totalCount.value))

const loading = ref(false)
const error = ref(null)
const updatingId = ref(null) 
const selectedOrder = ref(null) 
const canExportPDF = computed(() => selectedOrder.value && selectedOrder.value.status === 'Processing')

const showStatusDialog = ref(false)
const pendingStatusChange = ref(null)
const showPaymentStatusDialog = ref(false)
const pendingPaymentStatusChange = ref(null)

const statusOrder = {
  Pending: 1,
  Processing: 2,
  Shipped: 3,
  Delivered: 4,
  Cancelled: 5
}

const loadData = async () => {
  loading.value = true
  error.value = null
  try {
    await orderStore.fetchOrders()
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách đơn hàng."
  } finally {
    loading.value = false
  }
}

const onSortChange = () => {
  orderStore.page = 1 
  loadData()
}

const viewOrderDetails = async (orderId) => {
  try {
    const data = await orderStore.fetchOrderById(orderId)
    data._originalStatus = data.status
    data._originalPaymentStatus = data.paymentStatus
    selectedOrder.value = data
  } catch (err) {
    alert("Không thể tải chi tiết đơn hàng này.")
  }
}

const handleStatusChange = (orderInModal) => {
  const oldStatus = orderInModal._originalStatus
  const newStatus = orderInModal.status

  const paymentStatus = orderInModal.paymentStatus
  const paymentMethod = orderInModal.paymentMethod
  if (paymentStatus === 'Unpaid' && paymentMethod === 'Credit Card') {
    alert("Chỉ có thể thay đổi trạng thái khi đơn hàng đã được thanh toán.")
    orderInModal.status = oldStatus 
    return
  }

  if (statusOrder[newStatus] < statusOrder[oldStatus]) {
    alert("Không thể quay lại trạng thái trước đó!")
    orderInModal.status = oldStatus 
    return
  }
  if (newStatus === oldStatus) return

  pendingStatusChange.value = { order: orderInModal, oldStatus, newStatus }
  showStatusDialog.value = true
}

const confirmStatusChange = async () => {
  const { order, oldStatus, newStatus } = pendingStatusChange.value
  showStatusDialog.value = false
  updatingId.value = order.orderId
  try {
    await orderStore.updateOrderStatus(order.orderId, newStatus)
    order._originalStatus = newStatus
    const orderInList = orders.value.find(o => o.orderId === order.orderId)
    if (orderInList) orderInList.status = newStatus
  } catch (err) {
    alert("Cập nhật thất bại: " + err.message)
    order.status = oldStatus 
  } finally {
    updatingId.value = null
    pendingStatusChange.value = null
  }
}

const cancelStatusChange = () => {
  if (pendingStatusChange.value) {
    pendingStatusChange.value.order.status = pendingStatusChange.value.oldStatus
  }
  showStatusDialog.value = false
  pendingStatusChange.value = null
}

// ================= PAYMENT STATUS CHANGE =================

const handlePaymentStatusChange = (orderInModal) => {
  const oldPaymentStatus = orderInModal._originalPaymentStatus
  const newPaymentStatus = orderInModal.paymentStatus
  if (newPaymentStatus === oldPaymentStatus) return

  pendingPaymentStatusChange.value = { order: orderInModal, oldPaymentStatus, newPaymentStatus }
  showPaymentStatusDialog.value = true
}

const confirmPaymentStatusChange = async () => {
  if (!pendingPaymentStatusChange.value) return
  const { order, oldPaymentStatus, newPaymentStatus } = pendingPaymentStatusChange.value
  showPaymentStatusDialog.value = false
  updatingId.value = order.orderId
  try {
    await orderStore.updatePaymentStatus(order.orderId, newPaymentStatus)
    order._originalPaymentStatus = newPaymentStatus
    const orderInList = orders.value.find(o => o.orderId === order.orderId)
    if (orderInList) orderInList.paymentStatus = newPaymentStatus
  } catch (err) {
    alert("Cập nhật thanh toán thất bại")
    order.paymentStatus = oldPaymentStatus
  } finally {
    updatingId.value = null
    pendingPaymentStatusChange.value = null
  }
}

const cancelPaymentStatusChange = () => {
  if (pendingPaymentStatusChange.value) {
    pendingPaymentStatusChange.value.order.paymentStatus = pendingPaymentStatusChange.value.oldPaymentStatus
  }
  showPaymentStatusDialog.value = false
  pendingPaymentStatusChange.value = null
}

// ================= SEARCH & FILTERS =================

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

// ================= HELPERS =================

const formatDate = (dateString, includeTime = false) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  date.setHours(date.getHours() + 7)
  return date.toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    ...(includeTime && { hour: '2-digit', minute: '2-digit' })
  })
}

const exportPDF = async (id) => {
  try {
    if (!canExportPDF.value) {
      alert("Chỉ có thể xuất PDF cho đơn hàng ở trạng thái đang xử lý")
      return
    }
    await orderStore.exportPDF(id)
  } catch {
    alert("Xuất PDF thất bại")
  }
}


const getStatusClass = (status) => {
  switch (String(status).toLowerCase()) {
    case 'pending':    return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'processing': return 'bg-blue-100 text-blue-700 border-blue-200'
    case 'shipped':    return 'bg-purple-100 text-purple-700 border-purple-200'
    case 'delivered':  return 'bg-green-100 text-green-700 border-green-200'
    case 'cancelled':  return 'bg-red-100 text-red-700 border-red-200'
    default:           return 'bg-slate-100 text-slate-600 border-slate-200'
  }
}

const getPaymentStatusClass = (status) => {
  switch (String(status).toLowerCase()) {
    case 'paid':   return 'bg-green-50 text-green-600 border-green-200'
    case 'unpaid': return 'bg-red-50 text-red-600 border-red-200'
    default:       return 'bg-slate-100 text-slate-600 border-slate-200'
  }
}

const statusLabels = {
  Pending: 'Chờ xử lý', Processing: 'Đang xử lý', Shipped: 'Đang giao', Delivered: 'Đã giao', Cancelled: 'Đã hủy'
}
const paymentStatusLabels = { Paid: 'Đã thanh toán', Unpaid: 'Chưa thanh toán' }

const getStatusLabel = (status) => statusLabels[status] || status
const getPaymentStatusLabel = (status) => paymentStatusLabels[status] || status

const goToProduct = (productId) => {
  router.push(`/product/${productId}`)
}

onMounted(loadData)
</script>