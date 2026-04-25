import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '@/services/apiClient'

export const useClientProductStore = defineStore('clientProducts', () => {
    const products = ref([])
    const categories = ref([])
    const brands = ref([])

    const suggestedCategories = ref(null)
    const suggestedProducts = ref([])

    const totalCount = ref(0)
    const page = ref(1)
    const pageSize = ref(25)

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

            const res = await apiClient.get('/products', { params })
            products.value = res.data
            totalCount.value = res.totalCount
        } catch (error) {
            console.error('Lỗi khi lấy danh sách sản phẩm:', error)
        }
    }

    const fetchProductById = async (id) => {
        try {
            const res = await apiClient.get(`/products/${id}`)
            return res
        } catch (error) {
            console.error('Lỗi khi lấy chi tiết sản phẩm:', error)
            throw error
        }
    }

    const fetchCategories = async () => {
        try {
            const res = await apiClient.get('/categories')
            categories.value = res
        } catch (error) {
            console.error('Lỗi khi lấy danh mục:', error)
        }
    }

    const fetchBrands = async () => {
        try {
            const res = await apiClient.get('/brands')
            brands.value = res
        } catch (error) {
            console.error('Lỗi khi lấy nhãn hàng:', error)
        }
    }

    const fetchSuggestedProducts = async () => {
        try {
            const params = {
                page: 1,
                pageSize: 12
            }

            if (suggestedCategories.value) params.categoryId = suggestedCategories.value

            const res = await apiClient.get('/products', { params })
            suggestedProducts.value = res.data
        } catch (error) {
            console.error('Lỗi khi lấy danh sách sản phẩm:', error)
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
        suggestedProducts,
        suggestedCategories,
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
        fetchSuggestedProducts,
        fetchProductById,
        fetchCategories,
        fetchBrands,
        changePage,
        changePageSize
    }
})