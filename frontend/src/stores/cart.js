import { defineStore } from 'pinia'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('access_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

export const useCartStore = defineStore('cart', () => {

  const addToCart = async (productId, quantity) => {
    try {
      await api.post('/carts', {
        productId,
        quantity
      })
      alert('Đã thêm vào giỏ hàng!')
    } catch (err) {
      console.error('Add to cart failed:', err)
      throw err
    }
  }

  return { addToCart }
})
