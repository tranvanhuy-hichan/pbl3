import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '@/services/apiClient'

export const useOrderStore = defineStore('orders', () => {
  const orders = ref([])
  const totalCount = ref(0)
  const page = ref(1)
  const pageSize = ref(10)
  const searchQuery = ref('')
  const statusFilter = ref('')
  const sortOrder = ref('desc')
  const paymentStatusFilter = ref('')
  const fromDateFilter = ref('')
  const toDateFilter = ref('')
  const myOrders = ref([])

  const pendingCount = computed(() => {
    if (!Array.isArray(orders.value)) return 0
    return orders.value.filter(o => (o.status || o.Status) === 'Pending').length
  })

  const processingCount = computed(() => {
    if (!Array.isArray(orders.value)) return 0
    return orders.value.filter(o => (o.status || o.Status) === 'Processing').length
  })

  const fetchOrders = async () => {
    try {
      const params = {
        page: page.value,
        pageSize: pageSize.value,
        search: searchQuery.value || undefined,
        status: statusFilter.value || undefined,
        paymentStatus: paymentStatusFilter.value || undefined,
        fromDate: fromDateFilter.value || undefined,
        toDate: toDateFilter.value || undefined,
        sortOrder: sortOrder.value || undefined
      }

      const res = await apiClient.get('/orders', { params })

      if (res && Array.isArray(res.data)) {
        orders.value = res.data
        totalCount.value = res.totalCount ?? 0
      } else {
        console.warn('Unexpected response shape:', res)
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
      const res = await apiClient.get(`/orders/${id}`)
      return res
    } catch (error) {
      console.error(`Failed to fetch order ${id}:`, error)
      throw error
    }
  }

  const fetchMyOrderDetails = async (id) => {
    try {
      const res = await apiClient.get(`/orders/my/${id}`)
      return res
    } catch (error) {
      console.error(`Failed to fetch my order ${id}:`, error)
      throw error
    }
  }

  const fetchMyOrders = async () => {
    try {
      const params = {
        page: page.value,
        pageSize: pageSize.value,
        status: statusFilter.value || undefined
      }

      const res = await apiClient.get('/orders/my', { params })

      if (res && Array.isArray(res.data)) {
        myOrders.value = res.data
        totalCount.value = res.totalCount ?? 0
      } else {
        myOrders.value = []
        totalCount.value = 0
      }

    } catch (err) {
      console.error('Fetch my orders failed:', err)
      myOrders.value = []
    }
  }

  const cancelOrder = async (orderId) => {
    try {
      await apiClient.patch(`/orders/my/${orderId}/cancel`)
      await fetchMyOrders()
    } catch (error) {
      console.error('Cancel order error:', error)
      throw error
    }
  }

  const updateOrderStatus = async (orderId, newStatus) => {
    try {
      await apiClient.patch(`/orders/${orderId}/status`, JSON.stringify(newStatus), {
        headers: { 'Content-Type': 'application/json' }
      })
      await fetchOrders()
    } catch (error) {
      console.error('Update status error:', error)
      throw error
    }
  }

  const updatePaymentStatus = async (orderId, newPaymentStatus) => {
    try {
      await apiClient.patch(`/orders/${orderId}/payment-status`, JSON.stringify(newPaymentStatus), {
        headers: { 'Content-Type': 'application/json' }
      })
      await fetchOrders()
    } catch (error) {
      console.error('Update payment status error:', error)
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

  const onSortChange = () => {
    orderStore.page = 1
    loadData()
  }

  const exportPDF = async (orderId) => {
    try {
      const response = await apiClient.get(`/orders/${orderId}/export`, {
        responseType: 'blob'
      })

      // tạo file download
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', `order_${orderId}.pdf`)
      document.body.appendChild(link)
      link.click()

      // cleanup
      link.remove()
      window.URL.revokeObjectURL(url)

    } catch (error) {
      console.error('Export PDF error:', error)
      throw error
    }
  }


  return {
    orders,
    myOrders, 
    totalCount, 
    page, 
    pageSize,
    searchQuery, 
    statusFilter, 
    paymentStatusFilter,
    fromDateFilter, 
    toDateFilter, 
    sortOrder,
    pendingCount, 
    processingCount,
    fetchOrders, 
    fetchOrderById,
    fetchMyOrders,
    fetchMyOrderDetails,
    updateOrderStatus, 
    resetFilters, 
    changePage, 
    updatePaymentStatus, 
    onSortChange,
    cancelOrder,
    exportPDF
  }
})