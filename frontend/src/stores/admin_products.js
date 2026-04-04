import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
    baseURL: 'https://localhost:7010/api',
    headers: {
        'Accept': 'application/json'
    }
})
api.interceptors.response.use(
    res => res,
    err => {
        console.error('[API Error]', err.response?.status, err.response?.data)
        return Promise.reject(err)
    }
)
export const useAdminProductStore = defineStore('adminProducts', () => {
    const products = ref([])
    const categories = ref([])
    const brands = ref([])

    const totalCount = ref(0)
    const page = ref(1)
    const pageSize = ref(20)

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

            if (searchQuery.value?.trim()) params.search = searchQuery.value
            if (selectedCategory.value) params.categoryId = selectedCategory.value
            if (selectedBrand.value) params.brandId = selectedBrand.value
            if (minPrice.value !== null) params.minPrice = minPrice.value
            if (maxPrice.value !== null) params.maxPrice = maxPrice.value

            const res = await api.get('/products/admin', { params })
            products.value = res.data.data
            totalCount.value = res.data.totalCount
        } catch (error) {
            console.error('Lỗi khi lấy danh sách sản phẩm:', error)
        }
    }

    const fetchProductById = async (id) => {
        try {
            const res = await api.get(`/products/${id}`)
            return res.data
        } catch (error) {
            console.error('Lỗi khi lấy chi tiết sản phẩm:', error)
            throw error
        }
    }

    const fetchCategories = async () => {
        try {
            const res = await api.get('/categories')
            categories.value = res.data.data || res.data
        } catch (error) {
            console.error('Lỗi khi lấy danh mục:', error)
        }
    }

    const fetchBrands = async () => {
        try {
            const res = await api.get('/brands')
            brands.value = res.data.data || res.data
        } catch (error) {
            console.error('Lỗi khi lấy nhãn hàng:', error)
        }
    }

    const fetchDeletedProducts = async () => {
        try {
            const res = await api.get('/products/deleted')
            return res.data.data || res.data
        } catch (error) {
            console.error('Lỗi khi lấy danh sách sản phẩm đã xóa:', error)
        }
    }

    const addProduct = async (formData) => {
        try {
            const res = await api.post('/products', formData)
            page.value = 1
            return res.data
        } catch (error) {
            console.error('Lỗi khi thêm sản phẩm:', error)
            throw error
        }
    }

    const updateProduct = async (id, formData) => {
        try {
            const res = await api.put(`/products/${id}`, formData)
            await fetchProducts()
            return res.data
        } catch (error) {
            console.error('Lỗi khi cập nhật sản phẩm:', error)
            throw error
        }
    }

    const deleteProduct = async (id) => {
        try {
            await api.delete(`/products/${id}`)
            products.value = products.value.filter(p => (p.productId || p.id) !== id)
            totalCount.value -= 1
        } catch (error) {
            console.error('Lỗi khi xóa sản phẩm:', error)
            throw error
        }
    }

    const restoreProduct = async (id) => {
        try {
            await api.put(`/products/${id}/restore`)
            const restoredProduct = await fetchProductById(id)
            products.value.push(restoredProduct)
            totalCount.value += 1
        } catch (error) {
            console.error('Lỗi khi xóa sản phẩm:', error)
            throw error
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

    return {
        products,
        categories,
        brands,
        totalProducts,
        totalCount,
        outOfStockCount,
        page,
        pageSize,
        searchQuery,
        selectedCategory,
        selectedBrand,
        minPrice,
        maxPrice,
        fetchProducts,
        fetchProductById,
        fetchCategories,
        fetchBrands,
        fetchDeletedProducts,
        addProduct,
        updateProduct,
        deleteProduct,
        restoreProduct,
        changePage,
        changePageSize
    }
})