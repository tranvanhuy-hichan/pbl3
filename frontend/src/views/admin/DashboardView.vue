<template>
  <div class="space-y-6">
    <!-- Stats Row -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Total Revenue</p>
            <h3 class="text-2xl font-bold text-slate-800">$24,590.00</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-emerald-100 flex items-center justify-center text-emerald-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          </div>
        </div>
        <p class="text-xs text-emerald-600 font-medium flex items-center gap-1 mt-auto">
          <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" /></svg>
          12% from last month
        </p>
      </div>
      
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Active Orders</p>
            <h3 class="text-2xl font-bold text-slate-800">142</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" /></svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          <span class="font-medium text-blue-600">32</span> pending processing
        </p>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Total Products</p>
            <h3 class="text-2xl font-bold text-slate-800">1,245</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" /></svg>
          </div>
        </div>
        <p class="text-xs text-slate-400 mt-auto">
          <span class="font-medium text-red-500">12</span> out of stock
        </p>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6 flex flex-col">
        <div class="flex justify-between items-start mb-4">
          <div>
            <p class="text-sm font-medium text-slate-500 mb-1">Registered Users</p>
            <h3 class="text-2xl font-bold text-slate-800">8,942</h3>
          </div>
          <div class="w-10 h-10 rounded-full bg-purple-100 flex items-center justify-center text-purple-600">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" /></svg>
          </div>
        </div>
        <p class="text-xs text-emerald-600 font-medium flex items-center gap-1 mt-auto">
          <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" /></svg>
          4.2% from last week
        </p>
      </div>
    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6 font-sans">Revenue Overview (Last 7 Days)</h3>
        <div class="h-[300px] w-full relative">
          <Line :data="revenueChartData" :options="chartOptions" />
        </div>
      </div>
      
      <div class="bg-white rounded-xl shadow-sm border border-slate-100 p-6">
        <h3 class="font-bold text-slate-800 mb-6 font-sans">Orders by Status</h3>
        <div class="h-[300px] w-full relative">
          <Bar :data="orderStatusChartData" :options="barOptions" />
        </div>
      </div>
    </div>

    <!-- Recent Orders Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50">
        <h3 class="font-bold text-slate-800">Recent Orders</h3>
        <router-link to="/admin/orders" class="text-sm text-brand-600 font-bold hover:text-brand-700 transition-colors flex items-center gap-1">
          View All <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg>
        </router-link>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Order ID</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Customer</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Date</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Amount</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="order in recentOrders" :key="order.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6 font-medium text-slate-800 text-sm">#{{ order.id }}</td>
              <td class="py-4 px-6 text-sm text-slate-600">{{ order.customer }}</td>
              <td class="py-4 px-6 text-sm text-slate-500">{{ order.date }}</td>
              <td class="py-4 px-6 text-sm font-bold text-slate-800">${{ order.amount.toFixed(2) }}</td>
              <td class="py-4 px-6">
                <span :class="[
                  'px-2.5 py-1 text-xs font-semibold rounded-full',
                  getStatusClass(order.status)
                ]">
                  {{ order.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import {
  Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, BarElement, Filler
} from 'chart.js'
import { Line, Bar } from 'vue-chartjs'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, Title, Tooltip, Legend, Filler)

// ---------------- CHART DATA ----------------
const revenueChartData = {
  labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
  datasets: [
    {
      label: 'Revenue ($)',
      data: [1200, 1900, 1500, 2200, 1800, 3200, 2800],
      borderColor: '#3b82f6', // brand-500
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      borderWidth: 3,
      pointBackgroundColor: '#fff',
      pointBorderColor: '#3b82f6',
      pointBorderWidth: 2,
      pointRadius: 4,
      pointHoverRadius: 6,
      fill: true,
      tension: 0.4 // curve
    }
  ]
}

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#1e293b',
      padding: 12,
      titleFont: { size: 13, family: 'Inter, sans-serif' },
      bodyFont: { size: 14, weight: 'bold', family: 'Inter, sans-serif' },
      displayColors: false,
      callbacks: {
        label: (context) => `$${context.parsed.y.toLocaleString()}`
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: '#f1f5f9', drawBorder: false },
      ticks: {
        color: '#64748b',
        font: { family: 'Inter, sans-serif' },
        callback: (value) => `$${value}`
      }
    },
    x: {
      grid: { display: false, drawBorder: false },
      ticks: { color: '#64748b', font: { family: 'Inter, sans-serif' } }
    }
  }
}

const orderStatusChartData = {
  labels: ['Pending', 'Processing', 'Shipped', 'Delivered'],
  datasets: [
    {
      label: 'Orders',
      data: [32, 45, 28, 65],
      backgroundColor: [
        '#f59e0b', // amber-500
        '#3b82f6', // blue-500
        '#a855f7', // purple-500
        '#22c55e', // green-500
      ],
      borderRadius: 6,
      borderSkipped: false
    }
  ]
}

const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: '#f1f5f9', drawBorder: false },
      ticks: { color: '#64748b', precision: 0 }
    },
    x: {
      grid: { display: false, drawBorder: false },
      ticks: { color: '#64748b', font: { size: 11 } }
    }
  }
}

// ---------------- RECENT ORDERS DATA ----------------
const recentOrders = ref([
  { id: 'ORD-1001', customer: 'John Buyer', date: 'Oct 25, 2023', amount: 1199.00, status: 'Pending' },
  { id: 'ORD-1002', customer: 'Sarah Smith', date: 'Oct 24, 2023', amount: 3748.00, status: 'Processing' },
  { id: 'ORD-1003', customer: 'Michael Johnson', date: 'Oct 22, 2023', amount: 1299.00, status: 'Shipped' },
  { id: 'ORD-1004', customer: 'Emily Clark', date: 'Oct 20, 2023', amount: 2499.00, status: 'Delivered' },
  { id: 'ORD-1005', customer: 'David Lee', date: 'Oct 26, 2023', amount: 1099.00, status: 'Cancelled' },
])

const getStatusClass = (status) => {
  const classes = {
    'Pending': 'bg-amber-100 text-amber-800',
    'Processing': 'bg-blue-100 text-blue-800',
    'Shipped': 'bg-purple-100 text-purple-800',
    'Delivered': 'bg-green-100 text-green-800',
    'Cancelled': 'bg-red-100 text-red-800',
  }
  return classes[status] || 'bg-slate-100 text-slate-800'
}
</script>
