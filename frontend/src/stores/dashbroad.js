import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/apiClient'

export const useDashbroadStore = defineStore('statistics', () => {

    const stats = ref({})
    const topProducts = ref([])
    const revenueChartData = ref({})
    const orderStatusChartData = ref({})
    const loading = ref(false)

    const fetchAll = async () => {
        loading.value = true

        try {
            // ===== DASHBOARD =====
            const dashboard = await apiClient.get('/statistics/dashboard')
            stats.value = dashboard.data

            // ===== REVENUE =====
            const revenue = await apiClient.get('/statistics/revenue', {
                params: { groupBy: 'day' }
            })

            revenueChartData.value = {
                labels: revenue.data.chart.map(x => x.label),
                datasets: [{
                    label: 'Doanh thu',
                    data: revenue.data.chart.map(x => x.revenue)
                }]
            }

            // ===== ORDER STATUS =====
            const orders = await apiClient.get('/statistics/orders')

            orderStatusChartData.value = {
                labels: ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'],
                datasets: [{
                    data: [
                        orders.data.pending,
                        orders.data.processing,
                        orders.data.shipped,
                        orders.data.delivered,
                        orders.data.cancelled
                    ]
                }]
            }

            // ===== TOP PRODUCTS =====
            const top = await apiClient.get('/statistics/top-products')
            topProducts.value = top.data

        } catch (err) {
            console.error('Dashboard error:', err)
        }

        loading.value = false
    }

    return {
        stats,
        topProducts,
        revenueChartData,
        orderStatusChartData,
        loading,
        fetchAll
    }
})
