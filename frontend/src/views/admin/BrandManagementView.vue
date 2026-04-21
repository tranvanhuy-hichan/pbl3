<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý nhãn hàng</h2>
      <button @click="openAddModal" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2.5 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Thêm mới</span>
      </button>
    </div>

    <!-- Error Alert -->
    <div v-if="brandStore.error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg flex items-center justify-between">
      <span>{{ brandStore.error }}</span>
      <button @click="brandStore.clearError" class="text-red-500 hover:text-red-700">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
      </button>
    </div>

    <!-- Toolbar: Search -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Tìm kiếm...." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
      <div class="text-sm text-slate-500 ml-4">
        Tổng: <span class="font-semibold text-slate-700">{{ brandStore.totalBrands }}</span> nhãn hàng
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="brandStore.loading" class="bg-white rounded-xl shadow-sm border border-slate-100 p-12 text-center">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-brand-500 mx-auto mb-4"></div>
      <p class="text-slate-500">Loading...</p>
    </div>

    <!-- Data Table -->
    <div v-else class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider w-16 text-center">ID</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Logo & Tên</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">hành động</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="brand in filteredBrands" :key="brand.brandId || brand.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6 text-center">
                <span class="inline-flex items-center justify-center w-8 h-8 rounded-full bg-brand-50 text-brand-600 text-sm font-semibold">
                  {{ brand.brandId || brand.id }}
                </span>
              </td>
              <td class="py-4 px-6">
                <div class="flex items-center gap-4">
                  <div class="w-12 h-12 rounded-lg bg-slate-100 border border-slate-200 p-2 flex items-center justify-center flex-shrink-0">
                    <img v-if="brand.logoURL || brand.logo" :src="brand.logoURL || brand.logo" :alt="brand.brandName || brand.name" class="max-w-full max-h-full object-contain mix-blend-multiply">
                    <div v-else class="text-xl font-bold text-slate-400">{{ (brand.brandName || brand.name || '?').charAt(0) }}</div>
                  </div>
                  <div class="font-bold text-slate-800">{{ brand.brandName || brand.name }}</div>
                </div>
              </td>
              <td class="py-4 px-6 text-right space-x-2">
                <button @click="openEditModal(brand)" class="p-1.5 text-brand-600 hover:bg-brand-50 rounded-lg transition-colors inline-block" title="Edit">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg>
                </button>
                <button @click="confirmDelete(brand)" class="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors inline-block" title="Delete">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                </button>
              </td>
            </tr>
            <tr v-if="filteredBrands.length === 0 && !brandStore.loading">
              <td colspan="3" class="py-12 text-center text-slate-500">
                <p>Không tìm thấy nhãn hàng.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Form Modal -->
    <div v-if="showModal" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-slate-900 bg-opacity-50 transition-opacity" @click="closeModal"></div>
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
        <div class="relative inline-block align-bottom bg-white rounded-xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg w-full">
          <form @submit.prevent="saveBrand">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <h3 class="text-xl font-bold text-slate-800 mb-5 border-b border-slate-100 pb-3" id="modal-title">
                {{ isEditing ? 'Edit Brand' : 'Add New Brand' }}
              </h3>
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Brand Name <span class="text-red-500">*</span></label>
                  <input v-model="formData.brandName" type="text" required class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Logo URL</label>
                  <input v-model="formData.logoURL" type="url" placeholder="https://..." class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500">
                </div>
                <!-- Logo Preview -->
                <div v-if="formData.logoURL" class="flex items-center gap-3">
                  <span class="text-sm text-slate-500">Preview:</span>
                  <div class="w-16 h-16 rounded-lg bg-slate-100 border border-slate-200 p-2 flex items-center justify-center">
                    <img :src="formData.logoURL" alt="Logo preview" class="max-w-full max-h-full object-contain">
                  </div>
                </div>
              </div>
            </div>
            <div class="bg-slate-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse border-t border-slate-200">
              <button type="submit" :disabled="saving" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-brand-600 text-base font-medium text-white hover:bg-brand-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:ml-3 sm:w-auto sm:text-sm transition-colors disabled:opacity-50">
                {{ saving ? 'Saving...' : 'Save' }}
              </button>
              <button type="button" @click="closeModal" class="mt-3 w-full inline-flex justify-center rounded-lg border border-slate-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm transition-colors">
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Delete Modal -->
    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-slate-900 bg-opacity-50 transition-opacity" @click="showDeleteDialog = false"></div>
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
        <div class="relative inline-block align-bottom bg-white rounded-xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-md w-full p-6">
          <h3 class="text-lg font-bold text-slate-900 mb-2">Delete Brand</h3>
          <p class="text-sm text-slate-500 mb-6">Are you sure you want to delete <span class="font-bold text-slate-800">"{{ itemToDelete?.brandName || itemToDelete?.name }}"</span>? This might affect products associated with this brand.</p>
          <div class="flex flex-row-reverse gap-3">
            <button @click="executeDelete" :disabled="deleting" class="btn bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 font-medium disabled:opacity-50">
              {{ deleting ? 'Deleting...' : 'Delete' }}
            </button>
            <button @click="showDeleteDialog = false" class="btn bg-white border border-slate-300 px-4 py-2 rounded-lg hover:bg-slate-50 font-medium text-slate-700">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useBrandStore } from '../../stores/brands'

const brandStore = useBrandStore()

const searchQuery = ref('')
const showModal = ref(false)
const isEditing = ref(false)
const showDeleteDialog = ref(false)
const itemToDelete = ref(null)
const saving = ref(false)
const deleting = ref(false)

const formData = ref({ brandId: null, brandName: '', logoURL: '' })

// Filter brands by search query
const filteredBrands = computed(() => {
  const brands = brandStore.brands || []
  if (!searchQuery.value) return brands
  return brands.filter(b => {
    const name = b.brandName || b.name || ''
    return name.toLowerCase().includes(searchQuery.value.toLowerCase())
  })
})

// Open Add Modal
const openAddModal = () => {
  isEditing.value = false
  formData.value = { brandId: null, brandName: '', logoURL: '' }
  showModal.value = true
}

// Open Edit Modal
const openEditModal = (brand) => {
  isEditing.value = true
  formData.value = { 
    brandId: brand.brandId || brand.id,
    brandName: brand.brandName || brand.name,
    logoURL: brand.logoURL || brand.logo || ''
  }
  showModal.value = true
}

// Close Modal
const closeModal = () => { 
  showModal.value = false 
  formData.value = { brandId: null, brandName: '', logoURL: '' }
}

// Save Brand (Add or Update)
const saveBrand = async () => {
  saving.value = true
  try {
    const data = {
      brandName: formData.value.brandName,
      logoURL: formData.value.logoURL || null
    }
    
    if (isEditing.value) {
      await brandStore.updateBrand(formData.value.brandId, data)
      alert('Brand updated successfully!')
    } else {
      await brandStore.addBrand(data)
      alert('Brand added successfully!')
    }
    closeModal()
  } catch (error) {
    alert(brandStore.error || 'An error occurred while saving the brand')
  } finally {
    saving.value = false
  }
}

// Confirm Delete
const confirmDelete = (brand) => {
  itemToDelete.value = brand
  showDeleteDialog.value = true
}

// Execute Delete
const executeDelete = async () => {
  if (!itemToDelete.value) return
  
  deleting.value = true
  try {
    const id = itemToDelete.value.brandId || itemToDelete.value.id
    await brandStore.deleteBrand(id)
    alert('Brand deleted successfully!')
    showDeleteDialog.value = false
    itemToDelete.value = null
  } catch (error) {
    alert(brandStore.error || 'An error occurred while deleting the brand')
  } finally {
    deleting.value = false
  }
}

// Fetch brands on mount
onMounted(() => {
  brandStore.fetchBrands()
})
</script>
