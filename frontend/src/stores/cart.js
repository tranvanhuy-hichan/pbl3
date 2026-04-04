import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
})

// Interceptor để log lỗi
api.interceptors.response.use(
  res => res,
  err => {
    console.error('[Cart API Error]', err.response?.status, err.response?.data)
    return Promise.reject(err)
  }
)

// Helper function để lấy token từ localStorage
const getAuthHeader = () => {
  const token = localStorage.getItem('token')
  return token ? { Authorization: `Bearer ${token}` } : {}
}

export const useCartStore = defineStore('cart', () => {
  // State
  const cartItems = ref([])
  const loading = ref(false)
  const error = ref(null)

  // Computed
  const cartCount = computed(() => cartItems.value.reduce((sum, item) => sum + item.quantity, 0))
  const cartSubtotal = computed(() => cartItems.value.reduce((sum, item) => sum + (item.subtotal || item.price * item.quantity), 0))
  const isEmpty = computed(() => cartItems.value.length === 0)

  // Actions

  /**
   * Lấy danh sách giỏ hàng (Customer - Bearer Token required)
   * Response: [{cartId, productId, productName, imageURL, price, quantity, subtotal}]
   */
  const fetchCart = async () => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get('/cart', {
        headers: getAuthHeader()
      })
      cartItems.value = res.data.data || res.data || []
      return cartItems.value
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để xem giỏ hàng'
        cartItems.value = []
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi lấy giỏ hàng'
      }
      console.error('Lỗi khi lấy giỏ hàng:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Thêm sản phẩm vào giỏ hàng (Customer - Bearer Token required)
   * @param {number} productId - Product ID
   * @param {number} quantity - Số lượng
   * Response: {message, cartItem}
   */
  const addToCart = async (productId, quantity = 1) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.post('/cart', { productId, quantity }, {
        headers: getAuthHeader()
      })
      // Refresh giỏ hàng sau khi thêm
      await fetchCart()
      return res.data
    } catch (err) {
      if (err.response?.status === 400) {
        error.value = err.response?.data?.message || 'Dữ liệu không hợp lệ'
      } else if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng'
      } else if (err.response?.status === 404) {
        error.value = 'Sản phẩm không tồn tại'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi thêm vào giỏ hàng'
      }
      console.error('Lỗi khi thêm vào giỏ hàng:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Cập nhật số lượng sản phẩm trong giỏ hàng (Customer - Bearer Token required)
   * @param {number} cartId - Cart Item ID
   * @param {number} quantity - Số lượng mới (quantity = 0 -> tự động xóa)
   * Response: {message, updatedItem}
   */
  const updateCartItem = async (cartId, quantity) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.put(`/cart/${cartId}`, { quantity }, {
        headers: getAuthHeader()
      })
      // Nếu quantity = 0, item sẽ bị xóa
      if (quantity === 0) {
        cartItems.value = cartItems.value.filter(item => item.cartId !== cartId)
      } else {
        // Cập nhật item trong state local
        const index = cartItems.value.findIndex(item => item.cartId === cartId)
        if (index !== -1) {
          cartItems.value[index].quantity = quantity
          cartItems.value[index].subtotal = cartItems.value[index].price * quantity
        }
      }
      return res.data
    } catch (err) {
      if (err.response?.status === 400) {
        error.value = 'Số lượng không hợp lệ'
      } else if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để cập nhật giỏ hàng'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy sản phẩm trong giỏ hàng'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi cập nhật giỏ hàng'
      }
      console.error('Lỗi khi cập nhật giỏ hàng:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Xóa một sản phẩm khỏi giỏ hàng (Customer - Bearer Token required)
   * @param {number} cartId - Cart Item ID
   * Response: {message}
   */
  const removeFromCart = async (cartId) => {
    loading.value = true
    error.value = null
    try {
      await api.delete(`/cart/${cartId}`, {
        headers: getAuthHeader()
      })
      // Cập nhật state local
      cartItems.value = cartItems.value.filter(item => item.cartId !== cartId)
      return { message: 'Đã xóa sản phẩm khỏi giỏ hàng' }
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để xóa sản phẩm'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy sản phẩm trong giỏ hàng'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi xóa sản phẩm'
      }
      console.error('Lỗi khi xóa sản phẩm khỏi giỏ hàng:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Xóa toàn bộ giỏ hàng (Customer - Bearer Token required)
   * Dùng sau khi đặt hàng thành công
   * Response: {message}
   */
  const clearCart = async () => {
    loading.value = true
    error.value = null
    try {
      await api.delete('/cart', {
        headers: getAuthHeader()
      })
      // Clear state local
      cartItems.value = []
      return { message: 'Đã xóa toàn bộ giỏ hàng' }
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để xóa giỏ hàng'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi xóa giỏ hàng'
      }
      console.error('Lỗi khi xóa giỏ hàng:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Tăng số lượng sản phẩm trong giỏ
   * @param {number} cartId - Cart Item ID
   */
  const increaseQuantity = async (cartId) => {
    const item = cartItems.value.find(i => i.cartId === cartId)
    if (item) {
      await updateCartItem(cartId, item.quantity + 1)
    }
  }

  /**
   * Giảm số lượng sản phẩm trong giỏ
   * @param {number} cartId - Cart Item ID
   */
  const decreaseQuantity = async (cartId) => {
    const item = cartItems.value.find(i => i.cartId === cartId)
    if (item && item.quantity > 1) {
      await updateCartItem(cartId, item.quantity - 1)
    } else if (item && item.quantity === 1) {
      // Nếu số lượng = 1, xóa luôn
      await removeFromCart(cartId)
    }
  }

  /**
   * Reset error state
   */
  const clearError = () => {
    error.value = null
  }

  return {
    // State
    cartItems,
    loading,
    error,
    // Computed
    cartCount,
    cartSubtotal,
    isEmpty,
    // Actions
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
