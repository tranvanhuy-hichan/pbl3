<template>
  <div class="space-y-6 max-w-4xl mx-auto">
    <!-- Header -->
    <div class="flex items-center gap-4">
      <router-link to="/admin/products" class="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
      </router-link>
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">
        {{ isEditing ? 'Edit Product' : 'Add New Product' }}
      </h2>
    </div>

    <!-- Form Container -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <form @submit.prevent="saveProduct" class="p-6 sm:p-8 space-y-6">
        
        <!-- Basic Info Section -->
        <div class="space-y-4">
          <h3 class="text-lg font-semibold text-slate-800 border-b border-slate-100 pb-2">Basic Information</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Product Name *</label>
              <input v-model="formData.name" type="text" required placeholder="Ex: MacBook Pro 16-inch M3 Max" class="w-full px-4 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
            </div>
            
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Category *</label>
              <select v-model="formData.category" required class="w-full px-4 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
                <option value="" disabled>Select a category</option>
                <option v-for="cat in productStore.categories" :key="cat" :value="cat">{{ cat }}</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Image URL</label>
              <input v-model="formData.image" type="url" placeholder="https://example.com/image.jpg" class="w-full px-4 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
            </div>
          </div>
        </div>

        <!-- Pricing & Inventory Section -->
        <div class="space-y-4 pt-4">
          <h3 class="text-lg font-semibold text-slate-800 border-b border-slate-100 pb-2">Pricing & Inventory</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Price (USD) *</label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-slate-500 sm:text-sm">$</span>
                </div>
                <input v-model.number="formData.price" type="number" step="0.01" min="0" required class="w-full pl-8 pr-4 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Stock Quantity *</label>
              <input v-model.number="formData.stock" type="number" min="0" required class="w-full px-4 py-2.5 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 bg-slate-50 focus:bg-white transition-colors">
            </div>
          </div>
        </div>

        <!-- Image Preview (Optional visual enhancement) -->
        <div v-if="formData.image" class="pt-4">
           <h3 class="text-sm font-medium text-slate-700 mb-2">Image Preview</h3>
           <div class="w-32 h-32 rounded-lg border-2 border-dashed border-slate-300 overflow-hidden bg-slate-50 flex items-center justify-center p-2">
             <img :src="formData.image" alt="Preview" class="max-w-full max-h-full object-contain" @error="handleImageError">
           </div>
        </div>

        <!-- Actions -->
        <div class="pt-6 border-t border-slate-100 flex items-center justify-end gap-3">
          <router-link to="/admin/products" class="px-6 py-2.5 border border-slate-300 rounded-lg text-slate-700 font-medium hover:bg-slate-50 transition-colors">
            Cancel
          </router-link>
          <button type="submit" class="px-6 py-2.5 bg-brand-500 text-white rounded-lg font-medium hover:bg-brand-600 shadow-sm transition-all focus:ring-2 focus:ring-offset-2 focus:ring-brand-500">
            {{ isEditing ? 'Save Changes' : 'Create Product' }}
          </button>
        </div>
        
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProductStore } from '../../stores/products'

const route = useRoute()
const router = useRouter()
const productStore = useProductStore()

const isEditing = computed(() => !!route.params.id)

const formData = ref({
  id: '',
  name: '',
  category: '',
  price: 0,
  stock: 0,
  image: ''
})

onMounted(() => {
  if (isEditing.value) {
    const productId = route.params.id
    const product = productStore.products.find(p => p.id === productId)
    
    if (product) {
      formData.value = { ...product }
    } else {
      // Product not found, redirect back
      router.push('/admin/products')
    }
  }
})

const handleImageError = (e) => {
  e.target.src = 'https://via.placeholder.com/150?text=Invalid+Image+URL'
}

const saveProduct = () => {
  if (isEditing.value) {
    productStore.updateProduct(formData.value.id, formData.value)
  } else {
    productStore.addProduct(formData.value)
  }
  // Navigate back to the product list after saving
  router.push('/admin/products')
}
</script>
