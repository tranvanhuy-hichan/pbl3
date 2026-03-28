<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Product Management</h2>
      <router-link to="/admin/products/new" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2.5 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Add New Product</span>
      </router-link>
    </div>

    <!-- Toolbar: Search and Filter -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col sm:flex-row gap-4 items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search products by name, ID or category..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
      <div class="flex items-center gap-2 w-full sm:w-auto">
        <select v-model="selectedCategory" class="w-full sm:w-auto border border-slate-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-brand-500 bg-white text-slate-700">
          <option value="">All Categories</option>
          <option v-for="cat in productStore.categories" :key="cat" :value="cat">{{ cat }}</option>
        </select>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Product Info</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Category</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Price</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Stock</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="product in filteredProducts" :key="product.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6">
                <div class="flex items-center gap-4">
                  <div class="w-12 h-12 rounded-lg bg-slate-100 overflow-hidden flex-shrink-0 border border-slate-200">
                    <img :src="product.image" :alt="product.name" class="w-full h-full object-cover">
                  </div>
                  <div>
                    <div class="font-bold text-slate-800">{{ product.name }}</div>
                    <div class="text-xs text-slate-400">ID: {{ product.id }}</div>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6 text-sm text-slate-600">
                <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-slate-100 text-slate-800">
                  {{ product.category }}
                </span>
              </td>
              <td class="py-4 px-6 font-semibold text-slate-800">${{ parseFloat(product.price).toFixed(2) }}</td>
              <td class="py-4 px-6">
                <span :class="[
                  'px-2.5 py-1 text-xs font-semibold rounded-full',
                  product.stock > 10 ? 'bg-green-100 text-green-700' : 
                  product.stock > 0 ? 'bg-amber-100 text-amber-700' : 'bg-red-100 text-red-700'
                ]">
                  {{ product.stock }} in stock
                </span>
              </td>
              <td class="py-4 px-6 text-right space-x-2">
                <router-link :to="`/admin/products/${product.id}/edit`" class="p-1.5 text-brand-600 hover:bg-brand-50 rounded-lg transition-colors inline-flex" title="Edit">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg>
                </router-link>
                <button @click="confirmDelete(product)" class="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors inline-flex" title="Delete">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                </button>
              </td>
            </tr>
            <tr v-if="filteredProducts.length === 0">
              <td colspan="5" class="py-12 text-center text-slate-500">
                <div class="flex flex-col items-center">
                  <svg class="w-12 h-12 text-slate-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                  <p>No products found matching your search.</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-slate-900 bg-opacity-50 transition-opacity" @click="showDeleteDialog = false"></div>
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
        <div class="relative inline-block align-bottom bg-white rounded-xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-md w-full p-6">
          <div class="sm:flex sm:items-start">
            <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-red-100 sm:mx-0 sm:h-10 sm:w-10">
              <svg class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
            </div>
            <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
              <h3 class="text-lg leading-6 font-bold text-slate-900" id="modal-title">Delete Product</h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">Are you sure you want to delete <span class="font-bold text-slate-800">"{{ productToDelete?.name }}"</span>? This action cannot be undone.</p>
              </div>
            </div>
          </div>
          <div class="mt-6 sm:flex sm:flex-row-reverse">
            <button type="button" @click="executeDelete" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-red-600 text-base font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 sm:ml-3 sm:w-auto sm:text-sm transition-colors">
              Delete
            </button>
            <button type="button" @click="showDeleteDialog = false" class="mt-3 w-full inline-flex justify-center rounded-lg border border-slate-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm transition-colors">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useProductStore } from '../../stores/products'

const productStore = useProductStore()

// Filter State
const searchQuery = ref('')
const selectedCategory = ref('')

// Modal State for Delete
const showDeleteDialog = ref(false)
const productToDelete = ref(null)

// Computed Filtered List
const filteredProducts = computed(() => {
  return productStore.products.filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
                          p.id.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesCategory = selectedCategory.value === '' || p.category === selectedCategory.value
    return matchesSearch && matchesCategory
  })
})

const confirmDelete = (product) => {
  productToDelete.value = product
  showDeleteDialog.value = true
}

const executeDelete = () => {
  if (productToDelete.value) {
    productStore.deleteProduct(productToDelete.value.id)
  }
  showDeleteDialog.value = false
  productToDelete.value = null
}
</script>
