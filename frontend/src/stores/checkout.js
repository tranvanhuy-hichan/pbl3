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

    const checkout = async () => {
        try {
            const pramas = {
                shippingAddress: shippingAddress.value,
                note: note.value,
                paymentMethod: paymentMethod.value,
                shippingFee: shippingFee.value
            }
            const res = await apiClient.post('/orders', pramas)
            return res
        } catch (error) {
            console.error('Checkout failed:', error)
            throw error
        }
    }

    return { shippingAddress, note, paymentMethod, shippingFee, reset, checkout }
})
