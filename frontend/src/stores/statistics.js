import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useStatisticsStore = defineStore('statistics', () => {
  const stats = ref({
    totalRevenue: 0,
    totalOrders: 0,
    totalProducts: 0,
    totalUsers: 0,
    pendingOrders: 0,
    lowStockProducts: 0,
  })

  const topProducts = ref([])

  const revenueChartData = ref({
    labels: [],
    datasets: [
      {
        label: 'Doanh thu (VNĐ)',
        data: [],
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59,130,246,0.2)',
        borderWidth: 3,
        fill: true,
        tension: 0.4,
        pointRadius: 3,
      },
    ],
  })

  const orderStatusChartData = ref({
    labels: ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'],
    datasets: [
      {
        label: 'Số lượng đơn',
        data: [],
        backgroundColor: ['#f59e0b', '#3b82f6', '#a855f7', '#22c55e', '#ef4444'],
        borderRadius: 6,
      },
    ],
  })

  const API = 'https://localhost:7010/api/statistics'

  const buildQuery = (from, to) => {
    const params = new URLSearchParams()
    if (from) params.append('from', from)
    if (to) params.append('to', to)
    return params.toString()
  }

  const fetchDashboard = async (from, to) => {
    const res = await fetch(`${API}/dashboard?${buildQuery(from, to)}`)
    stats.value = await res.json()
  }

  // ✅ CHỈ UPDATE DATA → KHÔNG MẤT MÀU
  const fetchRevenue = async (from, to) => {
    const res = await fetch(`${API}/revenue?groupBy=day&${buildQuery(from, to)}`)
    const data = await res.json()

    revenueChartData.value.labels = data.chart.map((i) => i.label)
    revenueChartData.value.datasets[0].data = data.chart.map((i) => i.revenue)
  }

  const fetchOrders = async (from, to) => {
    const res = await fetch(`${API}/orders?${buildQuery(from, to)}`)
    const data = await res.json()

    orderStatusChartData.value.datasets[0].data = [
      data.pending,
      data.processing,
      data.shipped,
      data.delivered,
      data.cancelled,
    ]
  }

  const fetchTopProducts = async (from, to) => {
    const res = await fetch(`${API}/top-products?top=10&${buildQuery(from, to)}`)
    topProducts.value = await res.json()
  }

  const fetchAll = async (from, to) => {
    await Promise.all([
      fetchDashboard(from, to),
      fetchRevenue(from, to),
      fetchOrders(from, to),
      fetchTopProducts(from, to),
    ])
  }

  const exportExcel = async (from, to) => {
    const params = new URLSearchParams()

    if (from) params.append('from', from)
    if (to) params.append('to', to)

    const res = await fetch(`${API}/export?${params.toString()}`)

    if (!res.ok) {
      const errText = await res.text()
      console.error('Export failed:', errText)
      return
    }

    const now = new Date().toISOString().slice(0, 10).replaceAll('-', '')
    const blob = await res.blob()

    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `Bao_cao_${now}.xlsx`
    document.body.appendChild(a)
    a.click()
    a.remove()

    window.URL.revokeObjectURL(url)
  }

  return {
    stats,
    topProducts,
    revenueChartData,
    orderStatusChartData,
    fetchAll,
    exportExcel,
  }
})
