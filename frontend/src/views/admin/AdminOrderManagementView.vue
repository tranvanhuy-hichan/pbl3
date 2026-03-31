<!-- src/views/admin/OrderManagement.vue -->
<template>
  <div class="space-y-6 p-6 bg-blue-50 min-h-screen">

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
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col md:flex-row items-center justify-between gap-4 ">
      <!-- Search -->
      <div class="relative w-full md:max-w-md">
         <label class="text-xs text-slate-500 mb-1 font-medium bg-white">Tìm kiếm</label>
        <input 
          type="text"
          v-model="searchQuery"
          @input="debouncedFetch"
          placeholder="Tìm theo mã đơn, ID khách..."
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-blue-50"
        />
        <svg class="absolute left-3 top-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>

      <!-- Filters -->
      <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto items-end ">

        <!-- Status Filter -->
        <div class="flex flex-col w-full md:w-auto ">
          <label class="text-xs text-slate-500 mb-1 font-medium">Trạng thái đơn</label>
          <select
            v-model="statusFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-blue-50 text-slate-700 text-sm "
          >
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
          <select
            v-model="paymentStatusFilter"
            @change="loadData"
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-blue-50 text-slate-700 text-sm"
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
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-blue-50 text-slate-700 text-sm"
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
            class="border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 bg-blue-50 text-slate-700 text-sm"
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
              <th class="py-4 px-6 text-xs font-bold text-slate-500 uppercase tracking-wider">Thanh toán</th>
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
              <!-- Cột TRẠNG THÁI -->
              <td class="py-2.5 px-4">
                <span :class="[
                  'text-[10px] font-bold rounded-full px-3 py-1 border uppercase tracking-tight whitespace-nowrap inline-block', 
                  getStatusClass(order.status)
                ]">
                  {{ getStatusLabel(order.status) }}
                </span>
              </td>

              <!-- Cột THANH TOÁN -->
              <td class="py-2.5 px-4">
                <span :class="[
                  'text-[10px] font-bold rounded-full px-3 py-1 border uppercase tracking-tight whitespace-nowrap inline-block', 
                  getPaymentStatusClass(order.paymentStatus)
                ]">
                  {{ getPaymentStatusLabel(order.paymentStatus) }}
                </span>
              </td>
              <td class="py-4 px-6 text-right">
                <button
                  @click="viewOrderDetails(order.orderId)"
                  class="text-blue-600 hover:text-blue-700 font-bold text-sm bg-blue-50 hover:bg-blue-100 px-4 py-2 rounded-lg transition-colors"
                >
                  Chi tiết
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
      <div class="px-6 py-4 bg-blue-50 border-t border-slate-200 flex flex-col sm:flex-row items-center justify-between gap-4">
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
                    <td class="py-4 px-4 flex items-center gap-3">
                      <!-- Image -->
                      <router-link :to="`/product/${item.productId}`" class="flex-shrink-0">
                        <img
                          :src="item.imageURL ? `https://localhost:7010${item.imageURL}` : 'https://via.placeholder.com/150'"
                          :alt="item.productName"
                          class="h-16 w-16 object-contain rounded-lg transition-transform duration-300 hover:scale-105"
                        />
                      </router-link>

                      <!-- Name & SKU -->
                      <div class="flex flex-col">
                        <router-link
                          :to="`/product/${item.productId}`"
                          class="font-bold text-slate-800 hover:text-blue-600 hover:underline transition-colors text-sm"
                        >
                          {{ item.productName }}
                        </router-link>
                        <div class="text-xs text-slate-400">Code: {{ item.productCode }}</div>
                      </div>
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

          <!-- Payment & Notes inside Modal Body -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Thông tin thanh toán & Trạng thái</h4>
              <div class="flex gap-3 flex-wrap">
                
                <!-- Hiển thị Phương thức thanh toán (Mới thêm) -->
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1">PHƯƠNG THỨC</label>
                  <div class="text-xs font-bold rounded-lg px-3 py-2 border bg-slate-50 border-slate-200 text-slate-600 flex items-center gap-2">
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                    </svg>
                    {{ selectedOrder.paymentMethod }}
                  </div>
                </div>

                <!-- Select Trạng thái thanh toán -->
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1">THANH TOÁN</label>
                  <select
                    v-model="selectedOrder.paymentStatus"
                    @change="handlePaymentStatusChange(selectedOrder)"
                    :disabled="updatingId === selectedOrder.orderId"
                    :class="[
                      'text-xs font-bold rounded-lg px-3 py-2 border appearance-none cursor-pointer focus:outline-none focus:ring-2 transition-all',
                      getPaymentStatusClass(selectedOrder.paymentStatus)
                    ]"
                  >
                    <option value="Paid">Paid</option>
                    <option value="Unpaid">Unpaid</option>
                  </select>
                </div>

                <!-- Select Trạng thái đơn hàng -->
                <div class="flex flex-col">
                  <label class="text-[10px] font-bold text-slate-400 mb-1">TRẠNG THÁI ĐƠN</label>
                  <select
                    v-model="selectedOrder.status"
                    @change="handleStatusChange(selectedOrder)"
                    :disabled="updatingId === selectedOrder.orderId"
                    :class="[
                      'text-xs font-bold rounded-lg px-3 py-2 border appearance-none cursor-pointer focus:outline-none focus:ring-2 transition-all',
                      getStatusClass(selectedOrder.status)
                    ]"
                  >
                    <option value="Pending" :disabled="statusOrder['Pending'] < statusOrder[selectedOrder._originalStatus]">Pending</option>
                    <option value="Processing" :disabled="statusOrder['Processing'] < statusOrder[selectedOrder._originalStatus]">Processing</option>
                    <option value="Shipped" :disabled="statusOrder['Shipped'] < statusOrder[selectedOrder._originalStatus]">Shipped</option>
                    <option value="Delivered" :disabled="statusOrder['Delivered'] < statusOrder[selectedOrder._originalStatus]">Delivered</option>
                    <option value="Cancelled" :disabled="statusOrder['Cancelled'] < statusOrder[selectedOrder._originalStatus]">Cancelled</option>
                  </select>
                </div>

              </div>
            </div>

            <!-- Ghi chú -->
            <div v-if="selectedOrder.notes" class="space-y-3">
              <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest">Ghi chú từ khách</h4>
              <div class="bg-amber-50 border border-amber-100 p-3 rounded-xl">
                <p class="text-sm text-amber-800 italic leading-relaxed">"{{ selectedOrder.notes }}"</p>
              </div>
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

    <!-- Confirm Delete Modal -->
    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-blue-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <h3 class="text-xl font-bold text-blue-600 mb-2 text-center">Xác nhận xóa</h3>
        <p class="text-blue-700 mb-6 text-center">Xóa sản phẩm <span class="font-bold text-blue-900">"{{ productToDelete?.productName }}"</span>?</p>
        <div class="flex gap-3 justify-center">
          <button @click="showDeleteDialog = false" class="px-4 py-2 text-blue-500 font-bold hover:bg-blue-50 rounded-xl">Hủy</button>
          <button @click="executeDelete" class="px-6 py-2 bg-blue-600 text-white font-bold rounded-xl shadow-lg">Xác nhận Xóa</button>
        </div>
      </div>
    </div>

    <!-- Confirm Status Modal -->
    <div v-if="showStatusDialog && pendingStatusChange" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <div class="flex justify-center mb-4">
          <div class="w-14 h-14 rounded-full bg-amber-50 flex items-center justify-center">
            <svg class="w-7 h-7 text-amber-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" />
            </svg>
          </div>
        </div>
        <h3 class="text-xl font-bold text-slate-800 mb-2 text-center">Xác nhận</h3>
        <p class="text-slate-500 text-sm text-center mb-6">
          Đơn hàng <span class="font-bold text-slate-800">#{{ pendingStatusChange.order.orderId }}</span>
          <span :class="getStatusClass(pendingStatusChange.oldStatus)" class="inline-block font-bold rounded-full px-3 py-0.5 text-xs border mx-1">{{ pendingStatusChange.oldStatus }}</span>
          →
          <span :class="getStatusClass(pendingStatusChange.newStatus)" class="inline-block font-bold rounded-full px-3 py-0.5 text-xs border mx-1">{{ pendingStatusChange.newStatus }}</span>
        </p>
        <div class="flex gap-3 justify-center">
          <button @click="cancelStatusChange" class="px-5 py-2 text-slate-600 font-bold hover:bg-slate-100 rounded-xl transition-colors">Hủy</button>
          <button @click="confirmStatusChange" class="px-6 py-2 bg-blue-600 text-white font-bold rounded-xl shadow hover:bg-blue-700 transition-colors">Xác nhận</button>
        </div>
      </div>
    </div>

    <div v-if="showPaymentStatusDialog && pendingPaymentStatusChange" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6">
        <div class="flex justify-center mb-4">
          <div class="w-14 h-14 rounded-full bg-amber-50 flex items-center justify-center">
            <svg class="w-7 h-7 text-amber-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" />
            </svg>
          </div>
        </div>
        <h3 class="text-xl font-bold text-slate-800 mb-2 text-center">Xác nhận</h3>
        <p class="text-slate-500 text-sm text-center mb-6">
          Đơn hàng <span class="font-bold text-slate-800">#{{ pendingPaymentStatusChange.order.orderId }}</span>
          <span :class="getStatusClass(pendingPaymentStatusChange.oldPaymentStatus)" class="inline-block font-bold rounded-full px-3 py-0.5 text-xs border mx-1">{{ pendingPaymentStatusChange.oldPaymentStatus }}</span>
          →
          <span :class="getStatusClass(pendingPaymentStatusChange.newPaymentStatus)" class="inline-block font-bold rounded-full px-3 py-0.5 text-xs border mx-1">{{ pendingPaymentStatusChange.newPaymentStatus }}</span>
        </p>
        <div class="flex gap-3 justify-center">
          <button @click="cancelPaymentStatusChange" class="px-5 py-2 text-slate-600 font-bold hover:bg-slate-100 rounded-xl transition-colors">Hủy</button>
          <button @click="confirmPaymentStatusChange" class="px-6 py-2 bg-blue-600 text-white font-bold rounded-xl shadow hover:bg-blue-700 transition-colors">Xác nhận</button>
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

// --- Pagination ---
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value) || 1)
const startEntry = computed(() => totalCount.value === 0 ? 0 : (page.value - 1) * pageSize.value + 1)
const endEntry   = computed(() => Math.min(page.value * pageSize.value, totalCount.value))

const loading = ref(false)
const error = ref(null)
const updatingId = ref(null) // ID của đơn hàng đang trong quá trình gọi API cập nhật
const selectedOrder = ref(null) // Đơn hàng đang hiển thị trong Modal

// --- Dialogs (Xác nhận) ---
const showStatusDialog = ref(false)
const pendingStatusChange = ref(null)

const showPaymentStatusDialog = ref(false)
const pendingPaymentStatusChange = ref(null)

// --- Thứ tự trạng thái (Để chặn việc cập nhật ngược) ---
const statusOrder = {
  Pending: 1,
  Processing: 2,
  Shipped: 3,
  Delivered: 4,
  Cancelled: 5
}

// ================= LOAD DATA =================

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

// ================= VIEW DETAILS =================

const viewOrderDetails = async (orderId) => {
  try {
    const data = await orderStore.fetchOrderById(orderId)
    
    // Khởi tạo các giá trị gốc để so sánh và phục hồi nếu cần
    data._originalStatus = data.status
    data._originalPaymentStatus = data.paymentStatus
    
    selectedOrder.value = data
  } catch (err) {
    alert("Không thể tải chi tiết đơn hàng này.")
  }
}

// ================= LOGIC THAY ĐỔI TRẠNG THÁI ĐƠN HÀNG =================

const handleStatusChange = (orderInModal) => {
  const oldStatus = orderInModal._originalStatus
  const newStatus = orderInModal.status

  // 1. Kiểm tra không cho phép quay lại trạng thái trước đó
  if (statusOrder[newStatus] < statusOrder[oldStatus]) {
    alert("Không thể quay lại trạng thái trước đó (VD: Đã giao không thể quay về Chờ xử lý)!")
    orderInModal.status = oldStatus // Revert UI
    return
  }

  if (newStatus === oldStatus) return

  // 2. Mở dialog xác nhận
  pendingStatusChange.value = { order: orderInModal, oldStatus, newStatus }
  showStatusDialog.value = true
}

const confirmStatusChange = async () => {
  const { order, oldStatus, newStatus } = pendingStatusChange.value
  showStatusDialog.value = false
  updatingId.value = order.orderId

  try {
    await orderStore.updateOrderStatus(order.orderId, newStatus)
    
    // Cập nhật trạng thái gốc trong Modal
    order._originalStatus = newStatus
    
    // ĐỒNG BỘ: Cập nhật lại dòng đơn hàng tương ứng ngoài danh sách chính
    const orderInList = orders.value.find(o => o.orderId === order.orderId)
    if (orderInList) {
      orderInList.status = newStatus
    }
  } catch (err) {
    alert("Cập nhật thất bại: " + err.message)
    order.status = oldStatus // Rollback UI trong modal
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

// ================= LOGIC THAY ĐỔI THANH TOÁN =================

const handlePaymentStatusChange = (orderInModal) => {
  const oldPaymentStatus = orderInModal._originalPaymentStatus
  const newPaymentStatus = orderInModal.paymentStatus

  if (newPaymentStatus === oldPaymentStatus) return

  pendingPaymentStatusChange.value = {
    order: orderInModal,
    oldPaymentStatus,
    newPaymentStatus
  }
  showPaymentStatusDialog.value = true
}

const confirmPaymentStatusChange = async () => {
  if (!pendingPaymentStatusChange.value) return
  const { order, oldPaymentStatus, newPaymentStatus } = pendingPaymentStatusChange.value

  showPaymentStatusDialog.value = false
  updatingId.value = order.orderId

  try {
    await orderStore.updatePaymentStatus(order.orderId, newPaymentStatus)
    
    // Cập nhật trạng thái gốc trong Modal
    order._originalPaymentStatus = newPaymentStatus
    
    // ĐỒNG BỘ: Cập nhật lại ngoài danh sách chính
    const orderInList = orders.value.find(o => o.orderId === order.orderId)
    if (orderInList) {
      orderInList.paymentStatus = newPaymentStatus
    }
  } catch (err) {
    alert("Cập nhật thanh toán thất bại: " + err.message)
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

// ================= HELPER FUNCTIONS =================

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

// Map chuyển đổi ngôn ngữ cho Trạng thái đơn hàng
const statusLabels = {
  Pending: 'Chờ xử lý',
  Processing: 'Đang xử lý',
  Shipped: 'Đang giao',
  Delivered: 'Đã giao',
  Cancelled: 'Đã hủy'
}

// Map chuyển đổi ngôn ngữ cho Trạng thái thanh toán
const paymentStatusLabels = {
  Paid: 'Đã thanh toán',
  Unpaid: 'Chưa thanh toán'
}

// Hàm helper để lấy nhãn tiếng Việt
const getStatusLabel = (status) => statusLabels[status] || status
const getPaymentStatusLabel = (status) => paymentStatusLabels[status] || status

onMounted(loadData)
</script>
