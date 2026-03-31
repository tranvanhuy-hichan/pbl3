// src/stores/products.js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

// Cấu hình API instance
const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  // QUAN TRỌNG: Không để Content-Type là application/json ở đây 
  // để Axios tự động nhận diện khi nào gửi JSON, khi nào gửi FormData (ảnh)
  headers: {
    'Accept': 'application/json'
  }
})

export const useProductStore = defineStore('products', () => {
  // --- STATE ---
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

  // --- GETTERS ---
  const totalProducts = computed(() => totalCount.value)
  const outOfStockCount = computed(() => products.value.filter(p => p.stockQuantity === 0).length)

  // --- ACTIONS ---

  // 1. Lấy danh sách sản phẩm (có filter và phân trang)
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

      const res = await api.get('/products', { params })
      
      // API của bạn trả về { data: [...], totalCount: ... }
      products.value = res.data.data
      totalCount.value = res.data.totalCount
    } catch (error) {
      console.error('Lỗi khi lấy danh sách sản phẩm:', error)
    }
  }

  // 2. Lấy chi tiết 1 sản phẩm
  const fetchProductById = async (id) => {
    try {
      const res = await api.get(`/products/${id}`)
      return res.data 
    } catch (error) {
      console.error('Lỗi khi lấy chi tiết sản phẩm:', error)
      throw error
    }
  }

  // 3. Lấy danh mục
  const fetchCategories = async () => {
    try {
      const res = await api.get('/categories')
      categories.value = res.data.data || res.data
    } catch (error) {
      console.error('Lỗi khi lấy danh mục:', error)
    }
  }

  // 4. Lấy nhãn hàng
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
      products.value = res.data.data || res.data
      return products.value 
    } catch (error) {
      console.error('Lỗi khi lấy sản phẩm đã xóa:', error)
    }
  }


  // 5. Thêm sản phẩm mới (Hỗ trợ upload ảnh)
  const addProduct = async (formData) => {
    try {
      // Khi gửi FormData, Axios sẽ tự động thêm boundary và Content-Type phù hợp
      const res = await api.post('/products', formData)
      
      // Sau khi thêm thành công, nên load lại trang đầu để thấy sp mới
      page.value = 1
      await fetchProducts()
      return res.data
    } catch (error) {
      console.error('Lỗi khi thêm sản phẩm:', error)
      throw error 
    }
  }

  // 6. Cập nhật sản phẩm (Hỗ trợ upload ảnh mới)
  const updateProduct = async (id, formData) => {
    try {
      const res = await api.put(`/products/${id}`, formData)
      
      // Load lại danh sách để cập nhật UI
      await fetchProducts()
      return res.data
    } catch (error) {
      console.error('Lỗi khi cập nhật sản phẩm:', error)
      throw error 
    }
  }

  // 7. Xóa sản phẩm (Soft delete theo logic backend của bạn)
  const deleteProduct = async (id) => {
    try {
      await api.delete(`/products/${id}`)
      // Xóa trực tiếp trên mảng hiện tại để UI phản hồi nhanh
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
      // Thêm lại sản phẩm vào mảng để UI phản hồi nhanh
      const restoredProduct = await fetchProductById(id)
      products.value.push(restoredProduct)
      totalCount.value += 1
    } catch (error) {
      console.error('Lỗi khi xóa sản phẩm:', error)
      throw error
    }
  }

  // 8. Điều hướng trang
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
    changePageSize,
    restoreProduct,
    fetchDeletedProducts
  }
})