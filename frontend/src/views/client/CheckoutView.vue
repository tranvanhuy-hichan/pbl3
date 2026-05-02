<template>
  <div class="bg-slate-50 min-h-screen py-10">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

      <div class="mb-8 flex items-center justify-between">
        <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">Thanh toán</h1>
        <router-link to="/cart" class="text-brand-600 font-medium hover:underline flex items-center gap-1 text-sm">
          ← Trở lại giỏ hàng
        </router-link>
      </div>

      <div v-if="cartStore.cartItems.length === 0" class="text-center py-20">
        <h2 class="text-2xl font-bold text-slate-700 mb-4">Giỏ hàng trống</h2>
        <p class="text-slate-500 mb-6">Bạn chưa có sản phẩm nào để thanh toán.</p>
        <router-link 
          to="/products" 
          class="px-6 py-3 bg-brand-500 text-white rounded-xl font-semibold hover:bg-brand-400 transition"
        >
          Tiếp tục mua sắm
        </router-link>
      </div>

      <div v-else class="flex flex-col lg:flex-row gap-8">

        <div class="lg:w-2/3 space-y-6">

          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6">
              1. Thông tin giao hàng
            </h2>

            <div class="space-y-4">
              <input 
                type="text" 
                v-model="checkoutStore.shippingAddress"
                placeholder="Nhập đầy đủ địa chỉ để đơn hàng được giao đúng thời gian..."
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

          <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 sm:p-8">
            <h2 class="text-xl font-bold text-slate-800 mb-6">
              2. Thanh toán
            </h2>

            <div class="space-y-4">
              <label class="flex gap-3 cursor-pointer items-center">
                <input type="radio" value="COD" v-model="checkoutStore.paymentMethod">
                Thanh toán khi nhận hàng (COD)
              </label>

              <label class="flex gap-3 cursor-pointer items-center">
                <input type="radio" value="Credit Card" v-model="checkoutStore.paymentMethod">
                Chuyển khoản / Quét QR
              </label>
              <div 
                v-if="checkoutStore.paymentMethod === 'Credit Card'" 
                class="bg-blue-50 border border-blue-200 text-blue-800 p-4 rounded-xl text-sm"
              >
                <p>Bạn sẽ được hướng dẫn chuyển khoản và cung cấp mã QR sau khi đặt hàng thành công.</p>
                <p class="mt-2 text-xs">Vui lòng kiểm tra email hoặc trang chi tiết đơn hàng để hoàn tất thanh toán.</p>
              </div>
            </div>
          </div>

        </div>

        <div class="lg:w-1/3">
          <div class="bg-slate-900 text-white p-6 rounded-2xl sticky top-24">

            <h2 class="text-xl font-bold mb-4">Sản phẩm </h2>

            <div class="space-y-4 mb-4 max-h-60 overflow-y-auto custom-scrollbar">
              <div v-for="item in cartStore.cartItems" :key="item.id" @click="goToProduct(item.productId)" class="flex gap-3 items-center p-2 bg-slate-800/50 rounded-lg cursor-pointer">
                <div class="w-14 h-14 bg-white rounded border flex-shrink-0 overflow-hidden">
                  <img 
                    :src="item.imageUrl ? `https://localhost:7010${item.imageUrl}` : 'https://via.placeholder.com/100'" 
                    class="w-full h-full object-cover" 
                    alt="product"
                  >
                </div>

                <div class="flex-1">
                  <router-link 
                    class="font-semibold hover:underline inline-block"
                  >
                    {{ item.productName }}
                  </router-link>
                  <p class="text-xs text-gray-300">x{{ item.quantity }}</p>
                </div>

                <div class="text-sm">
                  {{ formatCurrency(item.price * item.quantity) }}
                </div>

              </div>
            </div>

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
              Đặt hàng
            </button>

          </div>
        </div>

      </div>
    </div>

    <div v-if="showPaymentModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-2xl p-6 shadow-lg max-w-sm w-full relative">
        <button 
          @click="closePaymentModal" 
          class="absolute top-4 right-4 text-slate-400 hover:text-slate-600 transition"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>

        <h2 class="text-2xl font-bold text-slate-800 mb-4 text-center">Hoàn tất thanh toán</h2>
        <p class="text-slate-600 mb-2 text-center">Đơn hàng của bạn đã được tạo: <span class="font-semibold text-brand-600">#{{ currentOrder.orderId }}</span></p>
        <p class="text-slate-600 mb-6 text-center">Vui lòng quét mã QR để thực hiện thanh toán</p>

        <div class="bg-white p-3 rounded-xl border border-slate-200 mx-auto w-fit">
          <img 
            src="/payment-qr.png" 
            alt="QR thanh toán"
            class="w-44 h-44 object-contain"
          />
        </div>

        <div class="text-center text-sm text-slate-700 space-y-2 mt-6">
          <p>Ngân hàng: <span class="text-slate-900 font-semibold">Techcombank</span></p>
          <p>Số TK: <span class="text-slate-900 font-semibold">26032603200606</span></p>
          <p>Chủ TK: <span class="text-slate-900 font-semibold">TRAN VAN HUY</span></p>
          <p class="text-lg font-bold text-brand-600">Tổng tiền: {{ formatCurrency(currentOrder.total) }}</p>
        </div>

        <div class="bg-yellow-100 text-yellow-800 p-3 rounded-xl text-xs w-full text-center mt-6">
          Nội dung CK: <strong>DH-{{ currentOrder.orderId }}</strong>. Đơn hàng sẽ được xác nhận sau khi nhận tiền.
        </div>

        <button 
          @click="closePaymentModalAndRedirect"
          class="w-full mt-6 py-3 bg-brand-500 text-white rounded-xl font-bold hover:bg-brand-400 transition"
        >
          Tôi đã chuyển khoản
        </button>
      </div>
    </div>

  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { useCheckoutStore } from '@/stores/checkout'

const router = useRouter()
const cartStore = useCartStore()
const checkoutStore = useCheckoutStore()

const showPaymentModal = ref(false)
const currentOrder = ref(null) 

onMounted(async () => {
  await cartStore.fetchCart()
})

const subtotal = computed(() =>
  cartStore.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0)
)

const shippingFee = computed(() => {
  if (subtotal.value === 0) return 0
  return subtotal.value > 1000000 ? 0 : 50000
})

const tax = computed(() =>{
  return subtotal.value * 0.08
})

const total = computed(() => subtotal.value + shippingFee.value + tax.value)

const canCheckout = computed(() =>
  cartStore.cartItems.length > 0 &&
  checkoutStore.shippingAddress &&
  (checkoutStore.paymentMethod === 'COD' || checkoutStore.paymentMethod === 'Credit Card')
)

const formatCurrency = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)

const placeOrder = async () => {
  try {
    if (!canCheckout.value) {
      alert("Vui lòng nhập đầy đủ thông tin giao hàng và chọn phương thức thanh toán!")
      return
    }

    checkoutStore.shippingFee = shippingFee.value

    const orderData = {
      shippingAddress: checkoutStore.shippingAddress,
      notes: checkoutStore.note,
      paymentMethod: checkoutStore.paymentMethod,
      shippingFee: shippingFee.value,
    }

    console.log("Dữ liệu đặt hàng:", orderData)

    const orderResponse = await checkoutStore.checkout(orderData) 

    currentOrder.value = { 
      orderId: orderResponse.orderId, 
      total: orderResponse.total || total.value
    }

    if (checkoutStore.paymentMethod === 'Credit Card') {
      showPaymentModal.value = true 
    } else { 
      alert(`Đặt hàng thành công! Mã: ${currentOrder.value.orderId}`)
      checkoutStore.reset()
      await cartStore.fetchCart()
      router.push('/my/orders')
    }
  } catch (err) {
    console.error(err)
    alert("Đặt hàng không thành công. Vui lòng thử lại.")
  }
}

const closePaymentModal = () => {
  showPaymentModal.value = false
}

const closePaymentModalAndRedirect = async () => {
  showPaymentModal.value = false
  alert(`Đơn hàng #${currentOrder.value.orderId} đã được tạo và chờ bạn chuyển khoản!`)
  checkoutStore.reset()
  await cartStore.fetchCart()
  router.push(`/my/orders`) 
}


const goToProduct = (productId) => {
  router.push(`/product/${productId}`)
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