import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
})

// Interceptor để log lỗi
api.interceptors.response.use(
  res => res,
  err => {
    console.error('[Categories API Error]', err.response?.status, err.response?.data)
    return Promise.reject(err)
  }
)

// Helper function để lấy token từ localStorage
const getAuthHeader = () => {
  const token = localStorage.getItem('token')
  return token ? { Authorization: `Bearer ${token}` } : {}
}

export const useCategoryStore = defineStore('categories', () => {
  // State
  const categories = ref([])
  const currentCategory = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Computed
  const totalCategories = computed(() => categories.value.length)
  const activeCategories = computed(() => categories.value.filter(c => c.isActive !== false))

  // Actions

  /**
   * Lấy tất cả danh mục (Public)
   */
  const fetchCategories = async () => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get('/categories')
      categories.value = res.data.data || res.data
      return categories.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Lỗi khi lấy danh sách danh mục'
      console.error('Lỗi khi lấy danh sách danh mục:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Lấy chi tiết danh mục theo ID (Public)
   * @param {number} id - Category ID
   */
  const fetchCategoryById = async (id) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get(`/categories/${id}`)
      currentCategory.value = res.data
      return res.data
    } catch (err) {
      if (err.response?.status === 404) {
        error.value = 'Không tìm thấy danh mục'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi lấy chi tiết danh mục'
      }
      console.error('Lỗi khi lấy chi tiết danh mục:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Thêm danh mục mới (Admin - Bearer Token required)
   * @param {Object} categoryData - { categoryName: string, description?: string }
   */
  const addCategory = async (categoryData) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.post('/categories', categoryData, {
        headers: getAuthHeader()
      })
      // Refresh danh sách sau khi thêm
      await fetchCategories()
      return res.data
    } catch (err) {
      if (err.response?.status === 400) {
        error.value = 'Dữ liệu không hợp lệ'
      } else if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi thêm danh mục'
      }
      console.error('Lỗi khi thêm danh mục:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Cập nhật danh mục (Admin - Bearer Token required)
   * @param {number} id - Category ID
   * @param {Object} categoryData - { categoryName?: string, description?: string }
   */
  const updateCategory = async (id, categoryData) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.put(`/categories/${id}`, categoryData, {
        headers: getAuthHeader()
      })
      // Refresh danh sách sau khi cập nhật
      await fetchCategories()
      return res.data
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy danh mục'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi cập nhật danh mục'
      }
      console.error('Lỗi khi cập nhật danh mục:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Xóa danh mục (Admin - Bearer Token required)
   * @param {number} id - Category ID
   */
  const deleteCategory = async (id) => {
    loading.value = true
    error.value = null
    try {
      await api.delete(`/categories/${id}`, {
        headers: getAuthHeader()
      })
      // Cập nhật state local thay vì fetch lại
      categories.value = categories.value.filter(c => (c.categoryId || c.id) !== id)
      return { message: 'Xóa danh mục thành công' }
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy danh mục'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi xóa danh mục'
      }
      console.error('Lỗi khi xóa danh mục:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Reset error state
   */
  const clearError = () => {
    error.value = null
  }

  return {
    // State
    categories,
    currentCategory,
    loading,
    error,
    // Computed
    totalCategories,
    activeCategories,
    // Actions
    fetchCategories,
    fetchCategoryById,
    addCategory,
    updateCategory,
    deleteCategory,
    clearError
  }
})
