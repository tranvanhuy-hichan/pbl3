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
    console.error('[Brands API Error]', err.response?.status, err.response?.data)
    return Promise.reject(err)
  }
)

// Helper function để lấy token từ localStorage
const getAuthHeader = () => {
  const token = localStorage.getItem('token')
  return token ? { Authorization: `Bearer ${token}` } : {}
}

export const useBrandStore = defineStore('brands', () => {
  // State
  const brands = ref([])
  const currentBrand = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Computed
  const totalBrands = computed(() => brands.value.length)
  const activeBrands = computed(() => brands.value.filter(b => b.isActive !== false))

  // Actions

  /**
   * Lấy tất cả thương hiệu (Public)
   */
  const fetchBrands = async () => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get('/brands')
      brands.value = res.data.data || res.data
      return brands.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Lỗi khi lấy danh sách thương hiệu'
      console.error('Lỗi khi lấy danh sách thương hiệu:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Lấy chi tiết thương hiệu theo ID (Public)
   * @param {number} id - Brand ID
   */
  const fetchBrandById = async (id) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.get(`/brands/${id}`)
      currentBrand.value = res.data
      return res.data
    } catch (err) {
      if (err.response?.status === 404) {
        error.value = 'Không tìm thấy thương hiệu'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi lấy chi tiết thương hiệu'
      }
      console.error('Lỗi khi lấy chi tiết thương hiệu:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Thêm thương hiệu mới (Admin - Bearer Token required)
   * @param {Object} brandData - { brandName: string, logoURL?: string }
   */
  const addBrand = async (brandData) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.post('/brands', brandData, {
        headers: getAuthHeader()
      })
      // Refresh danh sách sau khi thêm
      await fetchBrands()
      return res.data
    } catch (err) {
      if (err.response?.status === 400) {
        error.value = 'Dữ liệu không hợp lệ'
      } else if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi thêm thương hiệu'
      }
      console.error('Lỗi khi thêm thương hiệu:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Cập nhật thương hiệu (Admin - Bearer Token required)
   * @param {number} id - Brand ID
   * @param {Object} brandData - { brandName?: string, logoURL?: string }
   */
  const updateBrand = async (id, brandData) => {
    loading.value = true
    error.value = null
    try {
      const res = await api.put(`/brands/${id}`, brandData, {
        headers: getAuthHeader()
      })
      // Refresh danh sách sau khi cập nhật
      await fetchBrands()
      return res.data
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy thương hiệu'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi cập nhật thương hiệu'
      }
      console.error('Lỗi khi cập nhật thương hiệu:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * Xóa thương hiệu (Admin - Bearer Token required)
   * @param {number} id - Brand ID
   */
  const deleteBrand = async (id) => {
    loading.value = true
    error.value = null
    try {
      await api.delete(`/brands/${id}`, {
        headers: getAuthHeader()
      })
      // Cập nhật state local thay vì fetch lại
      brands.value = brands.value.filter(b => (b.brandId || b.id) !== id)
      return { message: 'Xóa thương hiệu thành công' }
    } catch (err) {
      if (err.response?.status === 401) {
        error.value = 'Bạn cần đăng nhập để thực hiện thao tác này'
      } else if (err.response?.status === 403) {
        error.value = 'Bạn không có quyền thực hiện thao tác này'
      } else if (err.response?.status === 404) {
        error.value = 'Không tìm thấy thương hiệu'
      } else {
        error.value = err.response?.data?.message || 'Lỗi khi xóa thương hiệu'
      }
      console.error('Lỗi khi xóa thương hiệu:', err)
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
    brands,
    currentBrand,
    loading,
    error,
    // Computed
    totalBrands,
    activeBrands,
    // Actions
    fetchBrands,
    fetchBrandById,
    addBrand,
    updateBrand,
    deleteBrand,
    clearError
  }
})
