import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useProductStore = defineStore('products', () => {
    const products = ref([
        { id: 'P001', name: 'MacBook Pro 16" M3 Max', category: 'Laptops', price: 3499.00, stock: 42, image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=100&auto=format&fit=crop' },
        { id: 'P002', name: 'Sony WH-1000XM5', category: 'Audio', price: 398.00, stock: 15, image: 'https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?q=80&w=100&auto=format&fit=crop' },
        { id: 'P003', name: 'iPhone 15 Pro Max', category: 'Smartphones', price: 1199.00, stock: 0, image: 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?q=80&w=100&auto=format&fit=crop' },
        { id: 'P004', name: 'iPad Pro 13-inch', category: 'Tablets', price: 1299.00, stock: 5, image: 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?q=80&w=100&auto=format&fit=crop' },
    ])

    const categories = ref([
        'Laptops', 'Smartphones', 'Tablets', 'Audio', 'Monitors', 'Cameras', 'Accessories'
    ])

    // Getters
    const totalProducts = computed(() => products.value.length)
    const outOfStockCount = computed(() => products.value.filter(p => p.stock === 0).length)

    // Actions
    const addProduct = (product) => {
        // Generate a simple ID for mockup purposes
        const newId = `P${String(products.value.length + 1).padStart(3, '0')}`
        products.value.unshift({
            ...product,
            id: newId,
            // Default image if none provided
            image: product.image || 'https://via.placeholder.com/100x100?text=No+Image'
        })
    }

    const updateProduct = (id, updatedData) => {
        const index = products.value.findIndex(p => p.id === id)
        if (index !== -1) {
            products.value[index] = { ...products.value[index], ...updatedData }
        }
    }

    const deleteProduct = (id) => {
        products.value = products.value.filter(p => p.id !== id)
    }

    return {
        products,
        categories,
        totalProducts,
        outOfStockCount,
        addProduct,
        updateProduct,
        deleteProduct
    }
})
