// src/stores/orders.js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: { 'Content-Type': 'application/json' }
})

export const useOrderStore = defineStore('orders', () => {
  // --- State ---
  const orders = ref([])
  const totalCount = ref(0)
  const page = ref(1)
  const pageSize = ref(20)
  const searchQuery = ref('')
  const statusFilter = ref('')
  const paymentStatusFilter = ref('')
  const fromDateFilter = ref('')
  const toDateFilter = ref('')

  // --- Computed ---
  const pendingCount = computed(() => {
    if (!Array.isArray(orders.value)) return 0
    return orders.value.filter(o => (o.status || o.Status) === 'Pending').length
  })

  const processingCount = computed(() => {
    if (!Array.isArray(orders.value)) return 0
    return orders.value.filter(o => (o.status || o.Status) === 'Processing').length
  })

  // --- Actions ---
  const fetchOrders = async () => {
    try {
      const params = {
        page: page.value,
        pageSize: pageSize.value,
        status: statusFilter.value || undefined,
        paymentStatus: paymentStatusFilter.value || undefined,
        fromDate: fromDateFilter.value || undefined,
        toDate: toDateFilter.value || undefined,
      }

      const res = await api.get('/orders', { params })
      console.log('Raw API response:', res.data)

      if (res.data && Array.isArray(res.data.data)) {
        orders.value = res.data.data
        totalCount.value = res.data.totalCount ?? 0
      } else {
        console.warn('Unexpected response shape:', res.data)
        orders.value = []
        totalCount.value = 0
      }
    } catch (error) {
      console.error('Failed to fetch orders:', error)
      orders.value = []
      throw error
    }
  }

  const fetchOrderById = async (id) => {
    try {
      const res = await api.get(`/orders/${id}`)
      return res.data
    } catch (error) {
      console.error(`Failed to fetch order ${id}:`, error)
      throw error
    }
  }

  const updateOrderStatus = async (orderId, newStatus) => {
    try {
      await api.patch(`/orders/${orderId}/status`, JSON.stringify(newStatus), {
        headers: { 'Content-Type': 'application/json' }
      })
      await fetchOrders()
    } catch (error) {
      console.error('Update status error:', error)
      throw error
    }
  }

  const resetFilters = () => {
    searchQuery.value = ''
    statusFilter.value = ''
    paymentStatusFilter.value = ''
    fromDateFilter.value = ''
    toDateFilter.value = ''
    page.value = 1
  }

  const changePage = (newPage) => {
    if (newPage < 1) return
    page.value = newPage
    fetchOrders()
  }

  return {
    orders, totalCount, page, pageSize,
    searchQuery, statusFilter, paymentStatusFilter,
    fromDateFilter, toDateFilter,
    pendingCount, processingCount,
    fetchOrders, fetchOrderById, updateOrderStatus, resetFilters, changePage
  }
})