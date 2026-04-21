import { defineStore } from 'pinia'
import apiClient from '@/services/apiClient'

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
                this.user = await apiClient.get(`/users/${userId}`)
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
                const res = await apiClient.put(`/users/${userId}`, data)
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
                await apiClient.put('/auth/change-password', payload)

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
