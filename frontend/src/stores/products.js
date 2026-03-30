    // src/stores/products.js
    import { defineStore } from 'pinia'
    import { ref, computed } from 'vue'
    import axios from 'axios'

    const api = axios.create({
    baseURL: 'https://localhost:7010/api',
    headers: {
        'Content-Type': 'application/json'
    }
    })

    export const useProductStore = defineStore('products', () => {
    const products = ref([])
    const categories = ref([])
    const brands = ref([])

    const totalCount = ref(0)
    const page = ref(1)
    const pageSize = ref(12)

    const searchQuery = ref('')
    const selectedCategory = ref(null)
    const selectedBrand = ref(null)

    const minPrice = ref(null)
    const maxPrice = ref(null)

    const totalProducts = computed(() => totalCount.value)
    const outOfStockCount = computed(() => products.value.filter(p => p.stockQuantity === 0).length)

    const fetchProducts = async () => {
        try {
        const params = {
            page: page.value,
            pageSize: pageSize.value
        }

        if (searchQuery.value && searchQuery.value.trim() !== '') {
            params.search = searchQuery.value
        }

        if (selectedCategory.value !== null && selectedCategory.value !== undefined) {
            params.categoryId = selectedCategory.value
        }

        if (selectedBrand.value !== null && selectedBrand.value !== undefined) {
            params.brandId = selectedBrand.value
        }

        if (minPrice.value !== null) params.minPrice = minPrice.value
        if (maxPrice.value !== null) params.maxPrice = maxPrice.value

        const res = await api.get('/products', { params })
        products.value = res.data.data
        totalCount.value = res.data.totalCount

        } catch (error) {
        console.error('Failed to fetch products:', error)
        }
    }

    const fetchProductById = async (id) => {
    try {
        const response = await api.get(`/products/${id}`)
        return response.data 
    } catch (error) {
        console.error('Lỗi API:', error)
        throw error
    }
}

    const fetchCategories = async () => {
        try {
        const res = await api.get('/categories')
        categories.value = res.data.data || res.data
        } catch (error) {
        console.error('Failed to fetch categories:', error)
        }
    }

    const fetchBrands = async () => {
        try {
        const res = await api.get('/brands')
        brands.value = res.data.data || res.data
        } catch (error) {
        console.error('Failed to fetch brands:', error)
        }
    }

        const addProduct = async (product) => {
        try {
            const res = await api.post('/products', product)
            products.value.unshift(res.data)
            totalCount.value += 1
            return res.data
        } catch (error) {
            console.error('Failed to add product:', error)
            throw error 
        }
        }

        const updateProduct = async (id, updatedData) => {
        try {
            const res = await api.put(`/products/${id}`, updatedData)
            const index = products.value.findIndex(p => p.id === id)
            if (index !== -1) products.value[index] = res.data
            return res.data
        } catch (error) {
            console.error('Failed to update product:', error)
            throw error 
        }
        }

        const deleteProduct = async (id) => {
            try {
                await api.delete(`/products/${id}`)

                products.value = products.value.filter(p => (p.productId || p.id) !== id)
                
                totalCount.value -= 1;
            } catch (error) {
                console.error('Failed to delete product:', error)
                throw error;
            }
        }

    const changePage = (newPage) => {
        page.value = newPage
        fetchProducts()
    }

    const changePageSize = (newSize) => {
        pageSize.value = newSize
        page.value = 1
        fetchProducts()
    }

    const addToCart = async () => {
        if (!product.value) return

        try {
            await api.post('/cart/add', {
            productId: product.value.id,
            quantity: quantity.value
            })

            alert(`Đã thêm ${quantity.value} x ${product.value.name}`)
        } catch (err) {
            console.error(err)
        }
    }

    return {
        products,
        categories,
        brands,
        totalCount,
        page,
        pageSize,
        searchQuery,
        selectedCategory,
        selectedBrand,
        minPrice,
        maxPrice,
        totalProducts,
        outOfStockCount,
        fetchProducts,
        fetchProductById,
        fetchCategories,
        fetchBrands,
        addProduct,
        updateProduct,
        deleteProduct,
        changePage,
        changePageSize
    }
    })