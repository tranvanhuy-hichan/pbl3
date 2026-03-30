import { defineStore } from 'pinia'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export const useCartStore = defineStore('cart', () => {

  const addToCart = async (productId, quantity) => {
    try {
      await api.post('/carts', {
        productId,
        quantity
      })
    } catch (err) {
      console.error('Add to cart failed:', err)
      throw err
    }
  }

  return { addToCart }
})
