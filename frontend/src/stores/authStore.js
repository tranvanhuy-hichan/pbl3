import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  // Lấy giá trị khởi tạo từ local storage để F5 không bị mất phiên
  const token = ref(localStorage.getItem('access_token') || null)

  // Parse chuỗi JSON thành object, nếu không có thì trả về null
  let initialUser = null
  try {
    const userRef = localStorage.getItem('user')
    if (userRef) {
      initialUser = JSON.parse(userRef)
    }
  } catch (e) {
    console.error('Error parsing user from localStorage', e)
  }

  const user = ref(initialUser)

  /**
   * Lưu thông tin đăng nhập vào cả Store và Local Storage
   */
  const setAuth = (newToken, newUserInfo) => {
    token.value = newToken
    user.value = newUserInfo

    // access_token có hạn 8h (không refresh)
    localStorage.setItem('access_token', newToken)
    localStorage.setItem('user', JSON.stringify(newUserInfo))

    // Dọn dẹp keyũ có thể bị rác từ code trước
    localStorage.removeItem('user_info')
  }

  /**
   * Xoá phiên đăng nhập hiện tại
   */
  const clearAuth = () => {
    token.value = null
    user.value = null

    localStorage.removeItem('access_token')
    localStorage.removeItem('user')
    localStorage.removeItem('user_info')
  }

  /**
   * Getter kiểm tra xem user đã đăng nhập chưa
   */
  const isAuthenticated = () => {
    return !!token.value
  }

  return {
    token,
    user,
    setAuth,
    clearAuth,
    isAuthenticated,
  }
})
