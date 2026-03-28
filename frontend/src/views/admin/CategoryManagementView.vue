<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Category Management</h2>
      <button @click="openAddModal" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2.5 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Add New Category</span>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search categories..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider w-16 text-center">Icon</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Name & Slug</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="category in filteredCategories" :key="category.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6 text-center">
                <div class="w-10 h-10 mx-auto rounded-lg bg-brand-50 text-brand-600 flex items-center justify-center border border-brand-100" v-html="category.iconSvg"></div>
              </td>
              <td class="py-4 px-6">
                <div class="font-bold text-slate-800">{{ category.name }}</div>
                <div class="text-xs text-slate-400 font-mono mt-0.5">/category/{{ category.slug }}</div>
              </td>
              <td class="py-4 px-6">
                <span :class="[
                  'px-2.5 py-1 text-xs font-semibold rounded-full',
                  category.isActive ? 'bg-green-100 text-green-700' : 'bg-slate-100 text-slate-600'
                ]">
                  {{ category.isActive ? 'Active' : 'Hidden' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right space-x-2">
                <button @click="openEditModal(category)" class="p-1.5 text-brand-600 hover:bg-brand-50 rounded-lg transition-colors inline-block" title="Edit">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg>
                </button>
                <button @click="confirmDelete(category)" class="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors inline-block" title="Delete">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                </button>
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
        <div class="relative inline-block align-bottom bg-white rounded-xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-md w-full">
          <form @submit.prevent="saveCategory">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <h3 class="text-xl font-bold text-slate-800 mb-5 border-b border-slate-100 pb-3" id="modal-title">
                {{ isEditing ? 'Edit Category' : 'Add New Category' }}
              </h3>
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Category Name <span class="text-red-500">*</span></label>
                  <input v-model="formData.name" type="text" required class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500" @input="generateSlug">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">URL Slug</label>
                  <input v-model="formData.slug" type="text" required class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 font-mono text-sm">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">SVG Icon (Raw HTML)</label>
                  <textarea v-model="formData.iconSvg" rows="3" class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 font-mono text-xs text-slate-500"></textarea>
                </div>
                <div class="flex items-center gap-2">
                  <input type="checkbox" id="isActive" v-model="formData.isActive" class="w-4 h-4 text-brand-600 rounded border-slate-300 focus:ring-brand-500">
                  <label for="isActive" class="text-sm font-medium text-slate-700">Active</label>
                </div>
              </div>
            </div>
            <div class="bg-slate-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse border-t border-slate-200">
              <button type="submit" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-brand-600 text-base font-medium text-white hover:bg-brand-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:ml-3 sm:w-auto sm:text-sm transition-colors">Save</button>
              <button type="button" @click="closeModal" class="mt-3 w-full inline-flex justify-center rounded-lg border border-slate-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm transition-colors">Cancel</button>
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
          <h3 class="text-lg font-bold text-slate-900 mb-2">Delete Category</h3>
          <p class="text-sm text-slate-500 mb-6">Are you sure you want to delete <span class="font-bold text-slate-800">"{{ itemToDelete?.name }}"</span>? Products in this category might lose their grouping.</p>
          <div class="flex flex-row-reverse gap-3">
            <button @click="executeDelete" class="btn bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 font-medium">Delete</button>
            <button @click="showDeleteDialog = false" class="btn bg-white border border-slate-300 px-4 py-2 rounded-lg hover:bg-slate-50 font-medium text-slate-700">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const categories = ref([
  { id: 'C1', name: 'Smartphones', slug: 'smartphones', isActive: true, iconSvg: '<svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path></svg>' },
  { id: 'C2', name: 'Laptops', slug: 'laptops', isActive: true, iconSvg: '<svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>' },
  { id: 'C3', name: 'Audio', slug: 'audio', isActive: true, iconSvg: '<svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19V6l12-3v13M9 19c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zm12-3c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zM9 10l12-3"></path></svg>' },
])

const searchQuery = ref('')
const showModal = ref(false)
const isEditing = ref(false)
const showDeleteDialog = ref(false)
const itemToDelete = ref(null)

const formData = ref({ id: '', name: '', slug: '', iconSvg: '', isActive: true })

const filteredCategories = computed(() => {
  return categories.value.filter(c => c.name.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

// Auto-generate slug from name
const generateSlug = () => {
  if (!isEditing.value) {
    formData.value.slug = formData.value.name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)+/g, '')
  }
}

const openAddModal = () => {
  isEditing.value = false
  formData.value = { id: Date.now().toString(), name: '', slug: '', iconSvg: '', isActive: true }
  showModal.value = true
}

const openEditModal = (cat) => {
  isEditing.value = true
  formData.value = { ...cat }
  showModal.value = true
}

const closeModal = () => { showModal.value = false }

const saveCategory = () => {
  if (isEditing.value) {
    const index = categories.value.findIndex(c => c.id === formData.value.id)
    if (index !== -1) categories.value[index] = { ...formData.value }
  } else {
    categories.value.push({ ...formData.value })
  }
  closeModal()
}

const confirmDelete = (cat) => {
  itemToDelete.value = cat
  showDeleteDialog.value = true
}

const executeDelete = () => {
  if (itemToDelete.value) {
    categories.value = categories.value.filter(c => c.id !== itemToDelete.value.id)
  }
  showDeleteDialog.value = false
  itemToDelete.value = null
}
</script>
