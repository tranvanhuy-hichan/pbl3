<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Order Management</h2>
        <p class="text-slate-500 text-sm mt-1">View and manage all customer orders</p>
      </div>
      
      <!-- Summary Badges -->
      <div class="flex gap-2">
        <div class="bg-amber-50 text-amber-700 px-3 py-1.5 rounded-lg text-sm border border-amber-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-amber-500"></span>
          <span class="font-medium">{{ pendingOrdersCount }} Pending</span>
        </div>
        <div class="bg-blue-50 text-blue-700 px-3 py-1.5 rounded-lg text-sm border border-blue-200 shadow-sm flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-blue-500"></span>
          <span class="font-medium">{{ processingOrdersCount }} Processing</span>
        </div>
      </div>
    </div>

    <!-- Toolbar: Search & Filter -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col md:flex-row items-center justify-between gap-4">
      <div class="relative w-full md:max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search by Order ID or Customer Name..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>

      <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
        <select v-model="statusFilter" class="w-full sm:w-40 border border-slate-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-brand-500 bg-white text-slate-700">
          <option value="">All Statuses</option>
          <option value="pending">Pending</option>
          <option value="processing">Processing</option>
          <option value="shipped">Shipped</option>
          <option value="delivered">Delivered</option>
          <option value="cancelled">Cancelled</option>
        </select>
        
        <input type="date" v-model="dateFilter" class="w-full sm:w-auto border border-slate-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-brand-500 bg-white text-slate-700">
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Order</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Customer</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Date</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Total</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="order in filteredOrders" :key="order.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6 text-sm">
                <div class="font-bold text-slate-800">#{{ order.id }}</div>
                <div class="text-xs text-slate-500">{{ order.itemCount }} items</div>
              </td>
              <td class="py-4 px-6 text-sm">
                <div class="font-medium text-slate-800">{{ order.customerName }}</div>
                <div class="text-xs text-slate-500">{{ order.customerEmail }}</div>
              </td>
              <td class="py-4 px-6 text-sm text-slate-700">
                {{ formatDate(order.date) }}
              </td>
              <td class="py-4 px-6 text-sm font-bold text-slate-800">
                ${{ order.total.toFixed(2) }}
              </td>
              <td class="py-4 px-6">
                <!-- Status Dropdown inside the table for quick updates -->
                <select 
                  v-model="order.status" 
                  @change="updateOrderStatus(order)"
                  :class="[
                    'text-xs font-bold rounded-full px-3 py-1.5 border appearance-none cursor-pointer focus:outline-none focus:ring-2 focus:ring-offset-1',
                    getStatusClass(order.status)
                  ]"
                >
                  <option value="pending" class="text-slate-800 bg-white">Pending</option>
                  <option value="processing" class="text-slate-800 bg-white">Processing</option>
                  <option value="shipped" class="text-slate-800 bg-white">Shipped</option>
                  <option value="delivered" class="text-slate-800 bg-white">Delivered</option>
                  <option value="cancelled" class="text-slate-800 bg-white">Cancelled</option>
                </select>
              </td>
              <td class="py-4 px-6 text-right space-x-2">
                <button @click="openOrderDetails(order)" class="text-brand-600 hover:text-brand-800 font-medium text-sm transition-colors py-1 px-3 border border-brand-200 hover:bg-brand-50 rounded-lg shadow-sm">
                  View Details
                </button>
              </td>
            </tr>
            <tr v-if="filteredOrders.length === 0">
              <td colspan="6" class="py-12 text-center text-slate-500">
                <svg class="mx-auto h-12 w-12 text-slate-300 mb-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                </svg>
                <p class="text-lg font-medium">No orders found</p>
                <p class="text-sm">Try adjusting your filters or search query.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Order Details Modal (Read-Only View) -->
    <div v-if="selectedOrder" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-slate-900 bg-opacity-60 transition-opacity backdrop-blur-sm" @click="closeOrderDetails"></div>
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
        <div class="relative inline-block align-bottom bg-white rounded-2xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-3xl w-full">
          
          <!-- Modal Header -->
          <div class="bg-slate-50 px-6 py-4 border-b border-slate-200 flex justify-between items-center">
            <div>
              <h3 class="text-xl font-bold text-slate-800">Order #{{ selectedOrder.id }}</h3>
              <p class="text-sm text-slate-500 mt-1">Placed on {{ formatDate(selectedOrder.date, true) }}</p>
            </div>
            <button @click="closeOrderDetails" class="text-slate-400 hover:text-slate-600 bg-white p-2 rounded-full shadow-sm border border-slate-200">
              <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
            </button>
          </div>

          <!-- Modal Body -->
          <div class="px-6 py-5 max-h-[60vh] overflow-y-auto">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
              <!-- Customer Info -->
              <div>
                <h4 class="text-sm font-bold text-slate-500 uppercase tracking-wider mb-3">Customer Information</h4>
                <div class="bg-slate-50 p-4 rounded-xl border border-slate-100 space-y-2">
                  <p class="font-medium text-slate-800">{{ selectedOrder.customerName }}</p>
                  <p class="text-sm text-slate-600 flex items-center gap-2"><svg class="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg> {{ selectedOrder.customerEmail }}</p>
                  <p class="text-sm text-slate-600 flex items-center gap-2"><svg class="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path></svg> {{ selectedOrder.customerPhone || 'No phone provided' }}</p>
                </div>
              </div>

              <!-- Shipping Info -->
              <div>
                <h4 class="text-sm font-bold text-slate-500 uppercase tracking-wider mb-3">Shipping Address</h4>
                <div class="bg-slate-50 p-4 rounded-xl border border-slate-100">
                  <p class="text-sm text-slate-700 whitespace-pre-line leading-relaxed">{{ selectedOrder.shippingAddress }}</p>
                </div>
              </div>
            </div>

            <!-- Order Items -->
            <div>
              <h4 class="text-sm font-bold text-slate-500 uppercase tracking-wider mb-3 flex justify-between">
                <span>Order Items</span>
                <span class="text-slate-800">{{ selectedOrder.items.length }} Items</span>
              </h4>
              <div class="border border-slate-200 rounded-xl overflow-hidden divide-y divide-slate-100">
                <div v-for="(item, index) in selectedOrder.items" :key="index" class="flex items-center p-4 bg-white">
                  <div class="w-16 h-16 bg-slate-100 rounded-lg flex items-center justify-center flex-shrink-0 p-2 border border-slate-200">
                    <img :src="item.image" :alt="item.name" class="max-w-full max-h-full object-contain mix-blend-multiply">
                  </div>
                  <div class="ml-4 flex-1">
                    <h5 class="text-sm font-bold text-slate-800 line-clamp-1">{{ item.name }}</h5>
                    <p class="text-xs text-slate-500 mt-1">Color: <span class="capitalize">{{ item.color }}</span></p>
                  </div>
                  <div class="text-right ml-4">
                    <p class="text-sm font-bold text-slate-800">${{ item.price.toFixed(2) }}</p>
                    <p class="text-xs text-slate-500 mt-1">Qty: {{ item.quantity }}</p>
                  </div>
                  <div class="text-right ml-6 w-20">
                    <p class="text-sm font-bold text-brand-600">${{ (item.price * item.quantity).toFixed(2) }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Payment & Summary Form -->
            <div class="mt-8 flex flex-col md:flex-row gap-8 justify-between border-t border-slate-200 pt-6">
              <div class="w-full md:w-1/2">
                <h4 class="text-sm font-bold text-slate-500 uppercase tracking-wider mb-3">Payment Information</h4>
                <div class="flex items-center gap-3">
                  <div class="w-10 h-6 bg-slate-200 rounded flex items-center justify-center text-xs font-bold text-slate-600">
                    {{ selectedOrder.paymentMethod === 'card' ? 'CARD' : selectedOrder.paymentMethod.toUpperCase() }}
                  </div>
                  <span class="text-sm text-slate-700 font-medium">Status: <span class="text-green-600">Paid</span></span>
                </div>
              </div>

              <div class="w-full md:w-1/2 bg-slate-50 p-5 rounded-xl border border-slate-100 shadow-sm">
                <div class="space-y-3 text-sm">
                  <div class="flex justify-between text-slate-600">
                    <span>Subtotal</span>
                    <span>${{ selectedOrder.subtotal.toFixed(2) }}</span>
                  </div>
                  <div class="flex justify-between text-slate-600">
                    <span>Shipping</span>
                    <span>${{ selectedOrder.shippingFee.toFixed(2) }}</span>
                  </div>
                  <div class="flex justify-between text-slate-600">
                    <span>Tax</span>
                    <span>${{ selectedOrder.tax.toFixed(2) }}</span>
                  </div>
                  <div class="pt-3 flex justify-between font-bold text-lg text-slate-900 border-t border-slate-200">
                    <span>Total</span>
                    <span>${{ selectedOrder.total.toFixed(2) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="bg-slate-50 px-6 py-4 border-t border-slate-200 sm:flex sm:flex-row-reverse items-center justify-between">
            <button type="button" @click="closeOrderDetails" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-6 py-2.5 bg-brand-600 text-base font-medium text-white hover:bg-brand-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:ml-3 sm:w-auto sm:text-sm transition-colors">
              Close Details
            </button>
            <div class="mt-3 sm:mt-0 flex items-center gap-3 text-sm text-slate-600">
               Update Status: 
               <select 
                  v-model="selectedOrder.status" 
                  @change="updateOrderStatus(selectedOrder)"
                  class="border border-slate-300 rounded-lg px-3 py-1.5 focus:outline-none focus:ring-1 focus:ring-brand-500 bg-white"
                >
                  <option value="pending">Pending</option>
                  <option value="processing">Processing</option>
                  <option value="shipped">Shipped</option>
                  <option value="delivered">Delivered</option>
                  <option value="cancelled">Cancelled</option>
                </select>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const searchQuery = ref('')
const statusFilter = ref('')
const dateFilter = ref('')

// Mock Data
const orders = ref([
  { 
    id: 'ORD-2023-1001', 
    customerName: 'John Buyer', 
    customerEmail: 'john@example.com',
    customerPhone: '0909876543',
    date: '2023-10-25T14:30:00Z', 
    status: 'pending',
    paymentMethod: 'card',
    shippingAddress: '123 Main St, Apt 4B\nNew York, NY 10001\nUnited States',
    items: [
      { name: 'iPhone 15 Pro Max', color: 'Titanium Blue', price: 1199.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/42/305658/iphone-15-pro-max-blue-thumbnew-600x600.jpg' }
    ],
    get itemCount() { return this.items.reduce((sum, item) => sum + item.quantity, 0) },
    get subtotal() { return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0) },
    get shippingFee() { return 15.00 },
    get tax() { return this.subtotal * 0.08 },
    get total() { return this.subtotal + this.shippingFee + this.tax }
  },
  { 
    id: 'ORD-2023-1002', 
    customerName: 'Sarah Smith', 
    customerEmail: 'sarah@example.com',
    customerPhone: '0912345678',
    date: '2023-10-24T09:15:00Z', 
    status: 'processing',
    paymentMethod: 'paypal',
    shippingAddress: '456 Market St\nSan Francisco, CA 94105\nUnited States',
    items: [
      { name: 'MacBook Pro M3 Max', color: 'Space Black', price: 3499.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/44/318222/macbook-pro-16-inch-m3-max-2023-space-black-thumb-600x600.jpg' },
      { name: 'AirPods Pro 2', color: 'White', price: 249.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/54/289773/tai-nghe-bluetooth-airpods-pro-2-thumbtgdd-600x600.jpg' }
    ],
    get itemCount() { return this.items.reduce((sum, item) => sum + item.quantity, 0) },
    get subtotal() { return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0) },
    get shippingFee() { return 0.00 },
    get tax() { return this.subtotal * 0.08 },
    get total() { return this.subtotal + this.shippingFee + this.tax }
  },
  { 
    id: 'ORD-2023-1003', 
    customerName: 'Michael Johnson', 
    customerEmail: 'michael@example.com',
    customerPhone: '',
    date: '2023-10-22T16:45:00Z', 
    status: 'shipped',
    paymentMethod: 'cod',
    shippingAddress: '789 Oak Ave\nAustin, TX 78701\nUnited States',
    items: [
      { name: 'Samsung Galaxy S24 Ultra', color: 'Titanium Gray', price: 1299.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/42/319665/samsung-galaxy-s24-ultra-grey-thumb-600x600.jpg' }
    ],
    get itemCount() { return this.items.reduce((sum, item) => sum + item.quantity, 0) },
    get subtotal() { return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0) },
    get shippingFee() { return 15.00 },
    get tax() { return this.subtotal * 0.08 },
    get total() { return this.subtotal + this.shippingFee + this.tax }
  },
  { 
    id: 'ORD-2023-1004', 
    customerName: 'Emily Clark', 
    customerEmail: 'emily@example.com',
    customerPhone: '0933445566',
    date: '2023-10-20T11:20:00Z', 
    status: 'delivered',
    paymentMethod: 'card',
    shippingAddress: '321 Pine Ln\nSeattle, WA 98101\nUnited States',
    items: [
      { name: 'Sony Alpha a7 IV', color: 'Black', price: 2499.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/4728/280053/sony-alpha-a7-iv-body-thumb-600x600.jpg' }
    ],
    get itemCount() { return this.items.reduce((sum, item) => sum + item.quantity, 0) },
    get subtotal() { return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0) },
    get shippingFee() { return 0.00 },
    get tax() { return this.subtotal * 0.08 },
    get total() { return this.subtotal + this.shippingFee + this.tax }
  },
  { 
    id: 'ORD-2023-1005', 
    customerName: 'David Lee', 
    customerEmail: 'david@example.com',
    customerPhone: '0955667788',
    date: '2023-10-26T08:10:00Z', 
    status: 'cancelled',
    paymentMethod: 'paypal',
    shippingAddress: '654 Cedar Rd\nDenver, CO 80202\nUnited States',
    items: [
      { name: 'iPad Pro 12.9 M2', color: 'Space Gray', price: 1099.00, quantity: 1, image: 'https://cdn.tgdd.vn/Products/Images/522/295058/ipad-pro-m2-129-inch-wifi-xam-thumb-600x600.jpg' }
    ],
    get itemCount() { return this.items.reduce((sum, item) => sum + item.quantity, 0) },
    get subtotal() { return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0) },
    get shippingFee() { return 15.00 },
    get tax() { return this.subtotal * 0.08 },
    get total() { return this.subtotal + this.shippingFee + this.tax }
  }
])

const selectedOrder = ref(null)

// Computed Stats
const pendingOrdersCount = computed(() => orders.value.filter(o => o.status === 'pending').length)
const processingOrdersCount = computed(() => orders.value.filter(o => o.status === 'processing').length)

// Filter Logic
const filteredOrders = computed(() => {
  return orders.value.filter(order => {
    // Search filter
    const searchMatch = 
      order.id.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
      order.customerName.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    // Status filter
    const statusMatch = statusFilter.value === '' || order.status === statusFilter.value
    
    // Date filter (simple YYYY-MM-DD string inclusion for demo)
    const dateMatch = dateFilter.value === '' || order.date.includes(dateFilter.value)

    return searchMatch && statusMatch && dateMatch
  }).sort((a, b) => new Date(b.date) - new Date(a.date)) // Sort newest first
})

// UI Helpers
const formatDate = (dateString, includeTime = false) => {
  const options = { year: 'numeric', month: 'short', day: 'numeric' }
  if (includeTime) {
    options.hour = '2-digit'
    options.minute = '2-digit'
  }
  return new Date(dateString).toLocaleDateString('en-US', options)
}

const getStatusClass = (status) => {
  const classes = {
    pending: 'bg-amber-100 text-amber-800 border-amber-200',
    processing: 'bg-blue-100 text-blue-800 border-blue-200',
    shipped: 'bg-purple-100 text-purple-800 border-purple-200',
    delivered: 'bg-green-100 text-green-800 border-green-200',
    cancelled: 'bg-red-100 text-red-800 border-red-200',
  }
  return classes[status] || 'bg-slate-100 text-slate-800'
}

// Actions
const updateOrderStatus = (order) => {
  console.log(`Order ${order.id} status updated to ${order.status}`)
  // In a real app, you would make an API call here to persist the change.
}

const openOrderDetails = (order) => {
  selectedOrder.value = order
}

const closeOrderDetails = () => {
  selectedOrder.value = null
}

</script>
