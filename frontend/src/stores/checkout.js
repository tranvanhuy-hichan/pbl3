import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/apiClient'

export const useCheckoutStore = defineStore('checkout', () => {
    const shippingAddress = ref('')
    const note = ref('')
    const paymentMethod = ref('COD')
    const shippingFee = ref(0) 

    const reset = () => {
        shippingAddress.value = ''
        note.value = ''
        paymentMethod.value = 'COD'
    }

    const checkout = async (checkoutData) => {
        try {
            const res = await apiClient.post('/orders', checkoutData)
            return res
        } catch (error) {
            console.error('Checkout failed:', error)
            throw error
        }
    }

    return { shippingAddress, note, paymentMethod, shippingFee, reset, checkout }
})
