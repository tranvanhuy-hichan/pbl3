import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '@/services/apiClient'

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
  const outOfStockCount = computed(() => products.value.filter((p) => p.stockQuantity === 0).length)

  const fetchProducts = async () => {
    try {
      const params = {
        page: page.value,
        pageSize: pageSize.value,
      }

      if (searchQuery.value?.trim()) params.search = searchQuery.value
      if (selectedCategory.value) params.categoryId = selectedCategory.value
      if (selectedBrand.value) params.brandId = selectedBrand.value
      if (minPrice.value !== null) params.minPrice = minPrice.value
      if (maxPrice.value !== null) params.maxPrice = maxPrice.value

      const res = await apiClient.get('/products/admin', { params })
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

  const fetchDeletedProducts = async () => {
    try {
      const res = await apiClient.get('/products/deleted')
      return res
    } catch (error) {
      console.error('Lỗi khi lấy danh sách sản phẩm đã xóa:', error)
    }
  }

  const addProduct = async (formData) => {
    try {
      const res = await apiClient.post('/products', formData)
      page.value = 1
      return res
    } catch (error) {
      console.error('Lỗi khi thêm sản phẩm:', error)
      throw error
    }
  }

  const updateProduct = async (id, payload) => {
    try {
      const res = await apiClient.put(`/products/${id}`, payload)
      await fetchProducts()
      return res
    } catch (error) {
      console.error('Lỗi khi cập nhật sản phẩm:', error)
      throw error
    }
  }

  const uploadProductImage = async (id, file) => {
    const formData = new FormData()
    formData.append('file', file)

    return apiClient.post(`/products/${id}/image`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  }

  const deleteProduct = async (id) => {
    try {
      await apiClient.delete(`/products/${id}`)
      products.value = products.value.filter((p) => (p.productId || p.id) !== id)
      totalCount.value -= 1
    } catch (error) {
      console.error('Lỗi khi xóa sản phẩm:', error)
      throw error
    }
  }

  const restoreProduct = async (id) => {
    try {
      await apiClient.put(`/products/${id}/restore`)
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
    uploadProductImage,
    changePage,
    changePageSize,
  }
})
