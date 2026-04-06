import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('access_token')
    if (token) config.headers.Authorization = `Bearer ${token}`
    return config
  },
  (error) => Promise.reject(error)
)

export const useCartStore = defineStore('cart', () => {

  const items = ref([])  

  const addToCart = async (productId, quantity, userId) => {
    try {
      await api.post('/carts', { productId, quantity, userId })
      alert('Đã thêm vào giỏ hàng!')
      await fetchCart()
    } catch (err) {
      console.error('Add to cart failed:', err)
      throw err
    }
  }

  const fetchCart = async () => {
    try {
      const res = await api.get(`/carts`)
      items.value = res.data  
    } catch (err) {
      console.error('Fetch cart failed:', err)
      throw err
    }
  }

  const totalPrice = computed(() =>
    items.value.reduce((sum, i) => sum + i.price * i.quantity, 0)
  )

  return { items, addToCart, fetchCart, totalPrice }
})
