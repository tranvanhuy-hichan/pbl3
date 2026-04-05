import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

const api = axios.create({
    baseURL: 'https://localhost:7010/api',
    headers: { 'Content-Type': 'application/json' }
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

export const useCheckoutStore = defineStore('checkout', () => {
    const shippingAddress = ref('')
    const note = ref('')
    const paymentMethod = ref('COD') // 'cod' hoặc 'banktransfer'
    const shippingFee = ref(0) 

    const reset = () => {
        shippingAddress.value = ''
        note.value = ''
        paymentMethod.value = 'COD'
    }

    const checkout = async () => {
        try {
            const pramas = {
                shippingAddress: shippingAddress.value,
                note: note.value,
                paymentMethod: paymentMethod.value,
                shippingFee: shippingFee.value
            }
            const res = await api.post('/orders', pramas)
            return res.data
        } catch (error) {
            console.error('Checkout failed:', error)
            throw error
        }
    }

    return { shippingAddress, note, paymentMethod, shippingFee, reset, checkout }
})
