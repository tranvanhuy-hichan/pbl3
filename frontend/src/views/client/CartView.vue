<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      
      <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight mb-8">Giỏ hàng</h1>

      <!-- Error Alert -->
      <div v-if="cartStore.error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg flex items-center justify-between mb-6">
        <span>{{ cartStore.error }}</span>
        <button @click="cartStore.clearError" class="text-red-500 hover:text-red-700">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
        </button>
      </div>

      <!-- Loading State -->
      <div v-if="cartStore.loading && !cartStore.cartItems.length" class="bg-white rounded-2xl shadow-sm border border-slate-100 p-12 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-brand-500 mx-auto mb-4"></div>
        <p class="text-slate-500">Đang tải...</p>
      </div>

      <div v-else class="flex flex-col lg:flex-row gap-8">
        
        <!-- Cart Items List -->
        <div class="lg:w-2/3">
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
            <!-- Table Header -->
            <div class="hidden sm:grid grid-cols-12 gap-4 p-4 border-b border-slate-100 bg-slate-50 text-xs font-semibold text-slate-500 uppercase tracking-wider">
              <div class="col-span-6">Sản phẩm</div>
              <div class="col-span-2 text-center">Giá</div>
              <div class="col-span-2 text-center">Số lượng</div>
              <div class="col-span-2 text-right">Tạm tính</div>
            </div>

            <!-- Cart Items -->
            <div class="divide-y divide-slate-100">
              <div v-for="item in cartStore.cartItems" :key="item.cartId" class="p-4 sm:p-6 grid grid-cols-1 sm:grid-cols-12 gap-4 items-center hover:bg-slate-50 transition-colors">
                
                <!-- Product Info -->
                <div class="col-span-1 sm:col-span-6 flex items-center gap-4">
                  <button 
                    @click="handleRemoveItem(item.cartId)" 
                    :disabled="removingItemId === item.cartId"
                    class="text-slate-400 hover:text-red-500 transition-colors p-1 disabled:opacity-50" 
                    title="Remove item"
                  >
                    <svg v-if="removingItemId !== item.cartId" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                    <svg v-else class="w-5 h-5 animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path></svg>
                  </button>
                  <div class="w-20 h-20 sm:w-24 sm:h-24 bg-slate-100 rounded-xl overflow-hidden flex-shrink-0 border border-slate-200">
                    <img 
                      :src="item.imageUrl ? `https://localhost:7010${item.imageUrl}` : 'https://via.placeholder.com/100'" 
                      :alt="item.productName" 
                      class="w-full h-full object-contain mix-blend-multiply p-2"
                    >
                  </div>
                  <div>
                    <h3 class="font-bold text-slate-800 line-clamp-2">
                      <router-link :to="`/product/${item.productId}`" class="hover:text-brand-600">{{ item.productName }}</router-link>
                    </h3>
                    <p class="text-sm text-slate-500 mt-1">ID: {{ item.productId }}</p>
                    <div class="sm:hidden mt-2 font-semibold text-brand-600">{{ formatCurrency(item.price) }}</div>
                  </div>
                </div>

                <!-- Price (Desktop) -->
                <div class="hidden sm:block col-span-2 text-center font-semibold text-slate-700">
                  {{ formatCurrency(item.price) }}
                </div>

                <!-- Quantity Control -->
                <div class="col-span-1 sm:col-span-2 flex justify-center">
                  <div class="flex items-center border border-slate-300 rounded-lg overflow-hidden bg-white w-28">
                    <button 
                      @click="handleDecreaseQuantity(item)" 
                      :disabled="updatingItemId === item.cartId"
                      class="px-3 py-2 text-slate-500 hover:bg-slate-50 transition-colors disabled:opacity-50"
                    >
                      &minus;
                    </button>
                    <input 
                      type="number" 
                      :value="item.quantity" 
                      @change="handleQuantityChange(item, $event)"
                      class="w-full text-center font-semibold text-slate-800 focus:outline-none text-sm" 
                      min="1"
                    >
                    <button 
                      @click="handleIncreaseQuantity(item)" 
                      :disabled="updatingItemId === item.cartId"
                      class="px-3 py-2 text-slate-500 hover:bg-slate-50 transition-colors disabled:opacity-50"
                    >
                      &plus;
                    </button>
                  </div>
                </div>

                <!-- Item Total -->
                <div class="hidden sm:block col-span-2 text-right font-bold text-slate-900">
                  {{ formatCurrency(item.subtotal || item.price * item.quantity) }}
                </div>
              </div>
            </div>

            <div v-if="cartStore.isEmpty && !cartStore.loading" class="p-12 text-center text-slate-500">
               <svg class="w-16 h-16 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path></svg>
               <h3 class="text-xl font-bold text-slate-700 mb-2">Giỏ hàng trống</h3>
               <router-link to="/" class="inline-block bg-brand-500 hover:bg-brand-600 text-white font-semibold py-2.5 px-6 rounded-lg transition-colors">
                 Tiếp tục mua sắm
               </router-link>
            </div>

            <!-- Cart Footer Actions -->
            <div v-if="!cartStore.isEmpty" class="p-4 border-t border-slate-100 bg-slate-50 flex justify-between items-center">
              <button 
                @click="handleClearCart" 
                :disabled="clearingCart"
                class="text-sm font-semibold text-slate-500 hover:text-red-500 transition-colors disabled:opacity-50"
              >
                {{ clearingCart ? 'Đang xóa...' : 'Xóa tất cả' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="lg:w-1/3">
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sticky top-24">
            <h2 class="text-xl font-bold text-slate-800 mb-6 border-b border-slate-100 pb-4">Chi tiết đơn hàng</h2>
            
            <div class="space-y-4 text-slate-600 border-b border-slate-100 pb-4 mb-4">
              <div class="flex justify-between">
                <span>Tạm tính ({{ cartStore.cartCount }} sản phẩm)</span>
                <span class="font-medium text-slate-800">{{ formatCurrency(cartStore.cartSubtotal) }}</span>
              </div>
              <div class="flex justify-between">
                <span>Phí ship</span>
                <span class="font-medium text-slate-800">{{ shippingCost === 0 ? 'Free' : formatCurrency(shippingCost) }}</span>
              </div>
              <div class="flex justify-between">
                <span>Tax (8%)</span>
                <span class="font-medium text-slate-800">{{ formatCurrency(taxAmount) }}</span>
              </div>
            </div>

            <div class="flex justify-between items-end mb-8">
              <span class="font-bold text-slate-800 text-lg">Tổng</span>
              <span class="font-extrabold text-2xl text-brand-600">{{ formatCurrency(orderTotal) }}</span>
            </div>

            <router-link to="/checkout" :class="[
              'w-full py-4 rounded-xl font-bold flex justify-center items-center shadow-sm transition-all text-center',
              !cartStore.isEmpty ? 'bg-brand-500 hover:bg-brand-600 text-white shadow-brand-200' : 'bg-slate-200 text-slate-400 cursor-not-allowed pointer-events-none'
            ]">
              Mua ngay
            </router-link>

            <div class="mt-4 flex items-center justify-center gap-2 text-xs text-slate-400">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
              Thanh toán an toàn và mã hóa
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useCartStore } from '../../stores/cart'

const cartStore = useCartStore()

// Local state
const promoCode = ref('')
const updatingItemId = ref(null)
const removingItemId = ref(null)
const clearingCart = ref(false)

// Computed values
const shippingCost = computed(() => cartStore.cartSubtotal > 1000000 ? 0 : 50000) // Free shipping over 1M VND
const taxAmount = computed(() => cartStore.cartSubtotal * 0.08)
const orderTotal = computed(() => cartStore.cartSubtotal + shippingCost.value + taxAmount.value)

// Format currency as VND
const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value || 0)
}

// Handle increase quantity
const handleIncreaseQuantity = async (item) => {
  updatingItemId.value = item.cartId
  try {
    await cartStore.increaseQuantity(item.cartId)
  } catch (error) {
    alert(cartStore.error || 'Failed to update quantity')
  } finally {
    updatingItemId.value = null
  }
}

// Handle decrease quantity
const handleDecreaseQuantity = async (item) => {
  updatingItemId.value = item.cartId
  try {
    await cartStore.decreaseQuantity(item.cartId)
  } catch (error) {
    alert(cartStore.error || 'Failed to update quantity')
  } finally {
    updatingItemId.value = null
  }
}

// Handle quantity change via input
const handleQuantityChange = async (item, event) => {
  const newQuantity = parseInt(event.target.value)
  if (isNaN(newQuantity) || newQuantity < 0) {
    event.target.value = item.quantity
    return
  }
  
  updatingItemId.value = item.cartId
  try {
    if (newQuantity === 0) {
      await cartStore.removeFromCart(item.cartId)
    } else {
      await cartStore.updateCartItem(item.cartId, newQuantity)
    }
  } catch (error) {
    event.target.value = item.quantity
    alert(cartStore.error || 'Failed to update quantity')
  } finally {
    updatingItemId.value = null
  }
}

// Handle remove item
const handleRemoveItem = async (cartId) => {
  removingItemId.value = cartId
  try {
    await cartStore.removeFromCart(cartId)
  } catch (error) {
    alert(cartStore.error || 'Failed to remove item')
  } finally {
    removingItemId.value = null
  }
}

// Handle clear cart
const handleClearCart = async () => {
  if (!confirm('Are you sure you want to clear your entire cart?')) return
  
  clearingCart.value = true
  try {
    await cartStore.clearCart()
  } catch (error) {
    alert(cartStore.error || 'Failed to clear cart')
  } finally {
    clearingCart.value = false
  }
}

// Fetch cart on mount
onMounted(() => {
  // Check if user is logged in before fetching cart
  const token = localStorage.getItem('access_token')
  if (token) {
    cartStore.fetchCart()
  }
})
</script>
