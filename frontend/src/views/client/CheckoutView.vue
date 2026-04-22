<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

      <!-- Header -->
      <div class="mb-8 flex items-center justify-between">
        <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">Thanh toán</h1>
        <router-link to="/cart" class="text-brand-600 font-medium hover:underline flex items-center gap-1 text-sm">
          ← Trở lại giỏ hàng
        </router-link>
      </div>

      <!-- 🟥 GIỎ HÀNG TRỐNG -->
      <div v-if="cartStore.cartItems .length === 0" class="text-center py-20">
        <h2 class="text-2xl font-bold text-slate-700 mb-4">Giỏ hàng trống</h2>
        <p class="text-slate-500 mb-6">Bạn chưa có sản phẩm nào để thanh toán.</p>
        <router-link 
          to="/products" 
          class="px-6 py-3 bg-brand-500 text-white rounded-xl font-semibold hover:bg-brand-400 transition"
        >
          Tiếp tục mua sắm
        </router-link>
      </div>

      <!-- 🟩 CHECKOUT -->
      <div v-else class="flex flex-col lg:flex-row gap-8">

        <!-- Form -->
        <div class="lg:w-2/3 space-y-6">

          <!-- Shipping -->
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6">
              1. Thông tin giao hàng
            </h2>

            <div class="space-y-4">
              <input 
                type="text" 
                v-model="checkoutStore.shippingAddress"
                placeholder="Nhập địa chỉ nhận hàng..."
                class="w-full px-4 py-3 border rounded-xl"
              >

              <textarea 
                v-model="checkoutStore.note"
                rows="3"
                placeholder="Ghi chú..."
                class="w-full px-4 py-3 border rounded-xl"
              ></textarea>
            </div>
          </div>

          <!-- Payment -->
          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6">
              2. Thanh toán
            </h2>

            <div class="space-y-4">
              <label class="flex gap-3 cursor-pointer items-center">
                <input type="radio" value="COD" v-model="checkoutStore.paymentMethod">
                COD
              </label>

              <label class="flex gap-3 cursor-pointer items-center">
                <input type="radio" value="Credit Card" v-model="checkoutStore.paymentMethod">
                Credit Card
              </label>

              <div 
                v-if="checkoutStore.paymentMethod === 'Credit Card'" 
                class="bg-yellow-100 text-yellow-800 p-3 rounded-xl text-sm"
              >
                Thanh toán bằng thẻ hiện đang phát triển. Vui lòng chọn COD.
              </div>
            </div>
          </div>

        </div>

        <!-- Summary -->
        <div class="lg:w-1/3">
          <div class="bg-slate-900 text-white p-6 rounded-2xl sticky top-24">

            <h2 class="text-xl font-bold mb-4">Sản phẩm </h2>

            <div class="space-y-4 mb-4 max-h-60 overflow-y-auto custom-scrollbar">
              <div v-for="item in cartStore.cartItems " :key="item.id" class="flex gap-3 items-center">

                <!-- Hình sản phẩm -->
                <div class="w-14 h-14 bg-white rounded border flex-shrink-0 overflow-hidden">
                  <img 
                    :src="item.imageUrl ? `https://localhost:7010${item.imageUrl}` : 'https://via.placeholder.com/100'" 
                    class="w-full h-full object-cover" 
                    alt="product"
                  >
                </div>

                <!-- Tên & số lượng -->
                <div class="flex-1">
                  <router-link 
                    :to="`/product/${item.productId}`" 
                    class="font-semibold hover:underline inline-block"
                  >
                    {{ item.productName }}
                  </router-link>
                  <p class="text-xs text-gray-300">x{{ item.quantity }}</p>
                </div>

                <!-- Tổng tiền -->
                <div class="text-sm">
                  {{ formatCurrency(item.price * item.quantity) }}
                </div>

              </div>
            </div>

            <!-- Tổng -->
            <div class="border-t border-slate-700 pt-4 space-y-2 text-sm">
              <div class="flex justify-between">
                <span>Tạm tính</span>
                <span>{{ formatCurrency(subtotal) }}</span>
              </div>

              <div class="flex justify-between">
                <span>Ship</span>
                <span>{{ shippingFee > 0 ? formatCurrency(shippingFee) : 'Free' }}</span>
              </div>

              <div class="flex justify-between">
                <span>Tax</span>
                <span>{{ formatCurrency(tax) }}</span>
              </div>
            </div>

            <div class="flex justify-between text-lg font-bold mt-4">
              <span>Tổng</span>
              <span>{{ formatCurrency(total) }}</span>
            </div>

            <button 
              @click="placeOrder"
              :disabled="!canCheckout"
              class="w-full mt-6 py-3 rounded-xl font-bold transition"
              :class="canCheckout 
                ? 'bg-brand-500 hover:bg-brand-400' 
                : 'bg-gray-400 cursor-not-allowed'"
            >
              Mua hàng
            </button>

          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { useCheckoutStore } from '@/stores/checkout'

const router = useRouter()
const cartStore = useCartStore()
const checkoutStore = useCheckoutStore()

// Load cart
onMounted(async () => {
  await cartStore.fetchCart()
})

// Subtotal
const subtotal = computed(() =>
  cartStore.cartItems .reduce((sum, item) => sum + item.price * item.quantity, 0)
)

// Shipping
const shippingFee = computed(() => {
  if (subtotal.value === 0) return 0
  return subtotal.value > 1000000 ? 0 : 50000
})

const tax = computed(() =>{
  return subtotal.value * 0.08
})

// Total
const total = computed(() => subtotal.value + shippingFee.value + tax.value)

// Chỉ cho COD
const canCheckout = computed(() =>
  cartStore.cartItems .length > 0 &&
  checkoutStore.shippingAddress &&
  checkoutStore.paymentMethod === 'COD'
)

// Format tiền
const formatCurrency = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)

// Đặt hàng
const placeOrder = async () => {
  try {
    if (!canCheckout.value) {
      alert("Vui lòng nhập đầy đủ thông tin hoặc chọn COD!")
      return
    }

    checkoutStore.shippingFee = shippingFee.value

    const order = await checkoutStore.checkout()

    alert(`Đặt hàng thành công! Mã: ${order.orderId}`)

    checkoutStore.reset()
    await cartStore.fetchCart()

    router.push('/my/orders')
  } catch (err) {
    console.error(err)
    alert("Đặt hàng không thành công")
  }
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #334155;
  border-radius: 4px;
}
</style>
