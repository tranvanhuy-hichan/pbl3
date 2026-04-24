<template>
  <div class="space-y-6 p-6 bg-slate-50 min-h-screen">
    <!-- FILTER -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-4">
  
      <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
        
        <!-- LEFT: DATE -->
        <div class="flex flex-wrap items-center gap-4">
          
          <div class="flex items-center gap-2">
            <span class="text-sm text-slate-500">Từ</span>
            <input 
              type="date" 
              v-model="fromDate"
              class="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div class="flex items-center gap-2">
            <span class="text-sm text-slate-500">Đến</span>
            <input 
              type="date" 
              v-model="toDate"
              class="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <button 
            @click="resetFilter"
            class="bg-slate-200 hover:bg-red-300 text-slate-700 px-4 py-2 rounded-lg text-sm transition"
          >
            Reset
          </button>

        </div>

        <!-- RIGHT: BUTTON -->
        <div class="flex items-center gap-3">

          <!-- EXPORT -->
          <button 
            @click="exportExcel"
            :disabled="isExporting"
            class="bg-emerald-500 hover:bg-emerald-600 disabled:opacity-60 disabled:cursor-not-allowed text-white px-4 py-2 rounded-lg text-sm transition shadow-sm flex items-center gap-2"
          >
            <svg v-if="isExporting" class="w-4 h-4 animate-spin" viewBox="0 0 24 24" fill="none">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
            </svg>

            {{ isExporting ? 'Đang xuất...' : 'Xuất Excel' }}
          </button>

          <button 
            @click="applyFilter"
            class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg text-sm transition shadow-sm"
          >
            Xem
          </button>
        </div>

      </div>
    </div>

    <!-- STATS -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      
      <!-- DOANH THU -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Doanh thu</p>
            <h3 class="text-2xl font-bold text-slate-800">
              {{ statisticsStore.stats.totalRevenue.toLocaleString() }} VNĐ
            </h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-emerald-100 flex items-center justify-center text-emerald-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
      </div>

      <!-- ĐƠN HÀNG -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Đơn hàng</p>
            <h3 class="text-2xl font-bold text-slate-800">
              {{ statisticsStore.stats.totalOrders }}
            </h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          <span class="font-medium text-blue-600">
            {{ statisticsStore.stats.pendingOrders }}
          </span> đơn đang chờ xử lý
        </p>
      </div>

      <!-- SẢN PHẨM -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Tổng sản phẩm</p>
            <h3 class="text-2xl font-bold text-slate-800">
              {{ statisticsStore.stats.totalProducts }}
            </h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          Hết hàng (<span class="font-medium text-red-500">
            {{ statisticsStore.stats.lowStockProducts }}
          </span>)
        </p>
      </div>

      <!-- USER -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Người dùng đăng ký mới</p>
            <h3 class="text-2xl font-bold text-slate-800">
              {{ statisticsStore.stats.totalUsers }}
            </h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-purple-100 flex items-center justify-center text-purple-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
          </div>
        </div>
      </div>

    </div>

    <!-- CHART -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      <div class="lg:col-span-2 bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6">Tổng quan doanh thu (Theo ngày)</h3>
        <div class="h-[300px] w-full">
          <Line 
            v-if="statisticsStore.revenueChartData.labels.length"
            :key="statisticsStore.revenueChartData.labels.join('-')"
            :data="statisticsStore.revenueChartData"
            :options="chartOptions"
          />
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6">Đơn hàng theo trạng thái</h3>
        <div class="h-[300px] w-full">
          <Bar 
            v-if="statisticsStore.orderStatusChartData.labels.length"
            :key="statisticsStore.orderStatusChartData.datasets[0].data.join('-')"
            :data="statisticsStore.orderStatusChartData"
            :options="barOptions"
          />
        </div>
      </div>

    </div>

    <!-- TABLE -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="px-6 py-4 border-b border-slate-100 bg-slate-50">
        <h3 class="font-bold text-slate-800">Top Sản Phẩm Bán Chạy Nhất</h3>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase">Mã SP</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase">Tên</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase">Đã bán</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase">Doanh thu</th>
            </tr>
          </thead>

          <tbody>
            <tr 
              v-for="p in statisticsStore.topProducts"
              :key="p.ProductId || p.productId"
              class="hover:bg-slate-50 cursor-pointer transition duration-150"

              @click="goToProduct(p)"
            >
              <td class="py-4 px-6 font-medium">
                #{{ p.ProductId || p.productId }}
              </td>

              <td class="py-4 px-6 font-bold">
                {{ p.ProductName || p.productName }}
              </td>

              <td class="py-4 px-6">
                {{ p.TotalSold || p.totalSold || 0 }}
              </td>

              <td class="py-4 px-6 font-bold text-emerald-600">
                {{ (p.TotalRevenue || p.totalRevenue || 0).toLocaleString() }} VNĐ
              </td>
            </tr>
          </tbody>

        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'
import { useRouter } from 'vue-router'
import { Line, Bar } from 'vue-chartjs'
import { useStatisticsStore } from '@/stores/statistics' 
import { ref } from 'vue'

const fromDate = ref('')
const toDate = ref('')
const isExporting = ref(false)

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

const statisticsStore = useStatisticsStore()
const router = useRouter()

onMounted(() => {
  statisticsStore.fetchAll()
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: { y: { beginAtZero: true } }
}

const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } }
}

const goToProduct = (p) => {
  const id = p.ProductId || p.productId
  router.push(`/product/${id}`)
}

const applyFilter = () => {
  statisticsStore.fetchAll(fromDate.value, toDate.value)
}

const resetFilter = () => {
  fromDate.value = ''
  toDate.value = ''
  statisticsStore.fetchAll()
}

const exportExcel = async () => {
  try {
    isExporting.value = true
    await statisticsStore.exportExcel(fromDate.value, toDate.value)
  } finally {
    isExporting.value = false
  }
}

</script>
