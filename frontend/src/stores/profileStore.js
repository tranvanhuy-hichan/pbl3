import { defineStore } from 'pinia'
import axios from 'axios'

const api = axios.create({
    baseURL: 'https://localhost:7010/api',
    headers: {
        'Content-Type': 'application/json'
    },
    timeout: 10000
})

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('access_token')

    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }

    return config
})

api.interceptors.response.use(
    (res) => res.data,
    (err) => {
        if (err.response?.status === 401) {
            localStorage.removeItem('access_token')
            window.location.href = '/login'
        }
        return Promise.reject(err)
    }
)

export const useProfileStore = defineStore('profile', {
    state: () => ({
        user: null,
        loading: false,
        error: null
    }),

    actions: {
        async fetchProfile() {
            this.loading = true
            this.error = null
            const user = JSON.parse(localStorage.getItem('user'))
            const userId = user?.userId
            try {
                this.user = await api.get(`/users/${userId}`)
            } catch (err) {
                this.error = err.response?.data?.message || 'Lỗi load profile'
            } finally {
                this.loading = false
            }
        },

        async updateProfile(data) {
            this.loading = true

            try {
                const user = JSON.parse(localStorage.getItem('user'))
                const userId = user?.userId
                const res = await api.put(`/users/${userId}`, data)
                this.user = res

                return { success: true, message: 'Cập nhật thành công' }
            } catch (err) {
                return {
                    success: false,
                    message: err.response?.data?.message || 'Cập nhật thất bại'
                }
            } finally {
                this.loading = false
            }
        },

        async changePassword(payload) {
            this.loading = true

            try {
                await api.put('/auth/change-password', payload)

                return { success: true, message: 'Đổi mật khẩu thành công' }
            } catch (err) {
                return {
                    success: false,
                    message: err.response?.data?.message || 'Đổi mật khẩu thất bại'
                }
            } finally {
                this.loading = false
            }
        }
    }
})
