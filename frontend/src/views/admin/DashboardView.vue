<template>
  <div class="space-y-6 p-6 bg-slate-50 min-h-screen">
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Tổng doanh thu</p>
            <h3 class="text-2xl font-bold text-slate-800">{{ stats.totalRevenue.toLocaleString() }} VNĐ</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-emerald-100 flex items-center justify-center text-emerald-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          </div>
        </div>
      </div>
      
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Đơn hàng đang hoạt động</p>
            <h3 class="text-2xl font-bold text-slate-800">{{ stats.totalOrders }}</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" /></svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          <span class="font-medium text-blue-600">{{ stats.pendingOrders }}</span> đơn đang chờ xử lý
        </p>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Tổng sản phẩm</p>
            <h3 class="text-2xl font-bold text-slate-800">{{ stats.totalProducts }}</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" /></svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          Hết hàng (<span class="font-medium text-red-500">{{ stats.lowStockProducts }} sản phẩm</span>)
        </p>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Người dùng đã đăng ký</p>
            <h3 class="text-2xl font-bold text-slate-800">{{ stats.totalUsers }}</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-purple-100 flex items-center justify-center text-purple-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" /></svg>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6 font-sans">Tổng quan doanh thu (Theo ngày)</h3>
        <div class="h-[300px] w-full relative">
          <Line v-if="revenueChartData.labels.length > 0" :data="revenueChartData" :options="chartOptions" />
        </div>
      </div>
      
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6 font-sans">Đơn hàng theo trạng thái</h3>
        <div class="h-[300px] w-full relative">
          <Bar v-if="orderStatusChartData.labels.length > 0" :data="orderStatusChartData" :options="barOptions" />
        </div>
      </div>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50">
        <h3 class="font-bold text-slate-800">Top Sản Phẩm Bán Chạy Nhất</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Mã SP</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Tên Sản Phẩm</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Đã Bán</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Doanh Thu</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="product in topProducts" :key="product.ProductId || product.productId" class="hover:bg-slate-50 transition-colors">
              
              <td class="py-4 px-6 font-medium text-slate-800 text-sm">
                #{{ product.ProductId || product.productId }}
              </td>
              
              <td class="py-4 px-6 text-sm text-slate-600 font-bold">
                {{ product.ProductName || product.productName }}
              </td>
              
              <td class="py-4 px-6 text-sm text-slate-500">
                {{ product.TotalSold || product.totalSold || 0 }} chiếc
              </td>
              
              <td class="py-4 px-6 text-sm font-bold text-emerald-600">
                {{ (product.TotalRevenue || product.totalRevenue || 0).toLocaleString() }} VNĐ
              </td>

            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import {
  Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, BarElement, Filler
} from 'chart.js'
import { Line, Bar } from 'vue-chartjs'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, Title, Tooltip, Legend, Filler)

// --- KHAI BÁO BIẾN DỮ LIỆU ---
const stats = ref({
  totalRevenue: 0,
  totalOrders: 0,
  totalProducts: 0,
  totalUsers: 0,
  pendingOrders: 0,
  lowStockProducts: 0
});

const topProducts = ref([]);

const revenueChartData = ref({
  labels: [],
  datasets: [{
    label: 'Doanh thu (VNĐ)',
    data: [],
    borderColor: '#3b82f6',
    backgroundColor: 'rgba(59, 130, 246, 0.1)',
    borderWidth: 3,
    fill: true,
    tension: 0.4
  }]
});

const orderStatusChartData = ref({
  labels: ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'],
  datasets: [{
    label: 'Số lượng đơn',
    data: [0, 0, 0, 0, 0],
    backgroundColor: ['#f59e0b', '#3b82f6', '#a855f7', '#22c55e', '#ef4444'],
    borderRadius: 6
  }]
});

// --- GỌI API KHI LOAD TRANG ---
onMounted(() => {
  const API_BASE = 'https://localhost:7010/api/statistics';

  // 1. Lấy 4 ô số liệu
  fetch(`${API_BASE}/dashboard`)
    .then(res => res.json())
    .then(data => { stats.value = data; });

  // 2. Lấy Biểu đồ doanh thu
  fetch(`${API_BASE}/revenue?groupBy=day`)
    .then(res => res.json())
    .then(data => {
      revenueChartData.value = {
        labels: data.chart.map(item => item.label),
        datasets: [{ ...revenueChartData.value.datasets[0], data: data.chart.map(item => item.revenue) }]
      };
    });

  // 3. Lấy Biểu đồ trạng thái đơn
  fetch(`${API_BASE}/orders`)
    .then(res => res.json())
    .then(data => {
      orderStatusChartData.value = {
        labels: ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'],
        datasets: [{ ...orderStatusChartData.value.datasets[0], data: [data.pending, data.processing, data.shipped, data.delivered, data.cancelled] }]
      };
    });

  // 4. Lấy Top sản phẩm bán chạy
  fetch(`${API_BASE}/top-products?top=10`)
    .then(res => res.json())
    .then(data => { topProducts.value = data; });
});

// --- CẤU HÌNH BIỂU ĐỒ (Giữ nguyên để đẹp) ---
const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: { y: { beginAtZero: true } }
};

const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } }
};
</script>