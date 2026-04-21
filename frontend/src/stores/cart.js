import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: { 'Content-Type': 'application/json' }
})

// Gắn token tự động
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) config.headers.Authorization = `Bearer ${token}`
    return config
  },
  (error) => Promise.reject(error)
)

// Log lỗi cho dễ debug
api.interceptors.response.use(
  res => res,
  err => {
    console.error('[Cart API Error]', err.response?.status, err.response?.data)
    return Promise.reject(err)
  }
)

export const useCartStore = defineStore('cart', () => {
  // State
  const cartItems = ref([])
  const loading = ref(false)
  const error = ref(null)

  // Computed
  const cartCount = computed(() =>
    cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
  )

  const cartSubtotal = computed(() =>
    cartItems.value.reduce(
      (sum, item) => sum + (item.subtotal || item.price * item.quantity),
      0
    )
  )

  const isEmpty = computed(() => cartItems.value.length === 0)

  // ================= ACTIONS =================

  // Lấy giỏ hàng
  const fetchCart = async () => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get('/cart')
      cartItems.value = res.data.data || res.data || []
      return cartItems.value
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để xem giỏ hàng'
        cartItems.value = []
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi lấy giỏ hàng'
      }
      throw err
    } finally {
      loading.value = false
    }
  }

  // Thêm vào giỏ
  const addToCart = async (productId, quantity = 1) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.post('/cart', { productId, quantity })
      await fetchCart()
      return res.data
    } catch (err) {
      if (err.response?.status === 400) {
        error.value = err.response?.data?.message || 'Dữ liệu không hợp lệ'
      } else if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập'
      } else {
        error.value = err.response?.data?.message || 'Lỗi thêm giỏ hàng'
      }
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update số lượng
  const updateCartItem = async (cartId, quantity) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.put(`/cart/${cartId}`, { quantity })

      if (quantity === 0) {
        cartItems.value = cartItems.value.filter(i => i.cartId !== cartId)
      } else {
        const item = cartItems.value.find(i => i.cartId === cartId)
        if (item) {
          item.quantity = quantity
          item.subtotal = item.price * quantity
        }
      }

      return res.data
    } catch (err) {
      error.value = 'Lỗi cập nhật giỏ hàng'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Xóa 1 sản phẩm
  const removeFromCart = async (cartId) => {
    loading.value = true
    error.value = null
    try {
      await api.delete(`/cart/${cartId}`)
      cartItems.value = cartItems.value.filter(i => i.cartId !== cartId)
    } catch (err) {
      error.value = 'Lỗi xóa sản phẩm'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Xóa toàn bộ
  const clearCart = async () => {
    loading.value = true
    error.value = null
    try {
      await api.delete('/cart')
      cartItems.value = []
    } catch (err) {
      error.value = 'Lỗi xóa giỏ hàng'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Tăng số lượng
  const increaseQuantity = async (cartId) => {
    const item = cartItems.value.find(i => i.cartId === cartId)
    if (item) await updateCartItem(cartId, item.quantity + 1)
  }

  // Giảm số lượng
  const decreaseQuantity = async (cartId) => {
    const item = cartItems.value.find(i => i.cartId === cartId)
    if (!item) return

    if (item.quantity > 1) {
      await updateCartItem(cartId, item.quantity - 1)
    } else {
      await removeFromCart(cartId)
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    cartItems,
    loading,
    error,
    cartCount,
    cartSubtotal,
    isEmpty,
    fetchCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart,
    increaseQuantity,
    decreaseQuantity,
    clearError
  }
})
