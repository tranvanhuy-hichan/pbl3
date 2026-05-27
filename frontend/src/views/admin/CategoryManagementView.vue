<template>
  <div class="space-y-6 p-6 bg-slate-50 min-h-screen">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý danh mục</h2>
      <button @click="openAddModal" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2.5 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Thêm</span>
      </button>
    </div>

    <!-- Error Alert -->
    <div v-if="categoryStore.error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg flex items-center justify-between">
      <span>{{ categoryStore.error }}</span>
      <button @click="categoryStore.clearError" class="text-red-500 hover:text-red-700">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Tìm kiếm..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
      <div class="text-sm text-slate-500 ml-4">
        Tổng: <span class="font-semibold text-slate-700">{{ categoryStore.totalCategories }}</span> danh mục
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="categoryStore.loading" class="bg-white rounded-xl shadow-sm border border-slate-100 p-12 text-center">
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
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Tên</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Mô tả</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Hành động</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="category in filteredCategories" :key="category.categoryId || category.id" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6 text-center">
                <span class="inline-flex items-center justify-center w-8 h-8 rounded-full bg-brand-50 text-brand-600 text-sm font-semibold">
                  {{ category.categoryId || category.id }}
                </span>
              </td>
              <td class="py-4 px-6">
                <div class="font-bold text-slate-800">{{ category.categoryName || category.name }}</div>
              </td>
              <td class="py-4 px-6">
                <div class="text-sm text-slate-500 max-w-xs truncate">{{ category.description || 'No description' }}</div>
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
            <tr v-if="filteredCategories.length === 0 && !categoryStore.loading">
              <td colspan="4" class="py-12 text-center text-slate-500">
                <p>Không tìm thấy danh mục</p>
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
                  <input v-model="formData.categoryName" type="text" required class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Description</label>
                  <textarea v-model="formData.description" rows="3" class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 resize-none" placeholder="Enter category description (optional)"></textarea>
                </div>
              </div>
            </div>
            <div class="bg-slate-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse border-t border-slate-200">
              <button type="submit" :disabled="saving" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-brand-600 text-base font-medium text-white hover:bg-brand-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 sm:ml-3 sm:w-auto sm:text-sm transition-colors disabled:opacity-50">
                {{ saving ? 'Saving...' : 'Save' }}
              </button>
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
          <p class="text-sm text-slate-500 mb-6">Are you sure you want to delete <span class="font-bold text-slate-800">"{{ itemToDelete?.categoryName || itemToDelete?.name }}"</span>? Products in this category might lose their grouping.</p>
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
import { useCategoryStore } from '../../stores/categories'

const categoryStore = useCategoryStore()

const searchQuery = ref('')
const showModal = ref(false)
const isEditing = ref(false)
const showDeleteDialog = ref(false)
const itemToDelete = ref(null)
const saving = ref(false)
const deleting = ref(false)

const formData = ref({ categoryId: null, categoryName: '', description: '' })

// Filter categories by search query
const filteredCategories = computed(() => {
  const categories = categoryStore.categories || []
  if (!searchQuery.value) return categories
  return categories.filter(c => {
    const name = c.categoryName || c.name || ''
    return name.toLowerCase().includes(searchQuery.value.toLowerCase())
  })
})

// Open Add Modal
const openAddModal = () => {
  isEditing.value = false
  formData.value = { categoryId: null, categoryName: '', description: '' }
  showModal.value = true
}

// Open Edit Modal
const openEditModal = (cat) => {
  isEditing.value = true
  formData.value = { 
    categoryId: cat.categoryId || cat.id,
    categoryName: cat.categoryName || cat.name,
    description: cat.description || ''
  }
  showModal.value = true
}

// Close Modal
const closeModal = () => { 
  showModal.value = false 
  formData.value = { categoryId: null, categoryName: '', description: '' }
}

// Save Category (Add or Update)
const saveCategory = async () => {
  saving.value = true
  try {
    const data = {
      categoryName: formData.value.categoryName,
      description: formData.value.description || null
    }
    
    if (isEditing.value) {
      await categoryStore.updateCategory(formData.value.categoryId, data)
      alert('Category updated successfully!')
    } else {
      await categoryStore.addCategory(data)
      alert('Category added successfully!')
    }
    closeModal()
  } catch (error) {
    alert(categoryStore.error || 'An error occurred while saving the category')
  } finally {
    saving.value = false
  }
}

// Confirm Delete
const confirmDelete = (cat) => {
  itemToDelete.value = cat
  showDeleteDialog.value = true
}

// Execute Delete
const executeDelete = async () => {
  if (!itemToDelete.value) return
  
  deleting.value = true
  try {
    const id = itemToDelete.value.categoryId || itemToDelete.value.id
    await categoryStore.deleteCategory(id)
    alert('Category deleted successfully!')
    showDeleteDialog.value = false
    itemToDelete.value = null
  } catch (error) {
    alert(categoryStore.error || 'An error occurred while deleting the category')
  } finally {
    deleting.value = false
  }
}

// Fetch categories on mount
onMounted(() => {
  categoryStore.fetchCategories()
})
</script>
