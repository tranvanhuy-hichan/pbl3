import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const api = axios.create({
    baseURL: 'https://localhost:7010/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('access_token')
    if (token) config.headers.Authorization = `Bearer ${token}`
    return config
})

api.interceptors.response.use(
    res => res,
    err => {
        console.error('[User API Error]', err.response?.data || err.message)
        return Promise.reject(err)
    }
)

export const useUserStore = defineStore('user', () => {
    const users = ref([])
    const deletedUsers = ref([])
    const loading = ref(false)
    const error = ref(null)

    const fetchUsers = async () => {
        loading.value = true
        try {
            const res = await api.get('/users?page=1&pageSize=30')
            const data = res.data

            users.value =
                data?.data ||
                data?.Data ||
                data?.items ||
                data ||
                []
        } catch (err) {
            error.value = err.response?.data?.message || 'Lỗi lấy danh sách user'
            users.value = []
        } finally {
            loading.value = false
        }
    }

    const fetchDeletedUsers = async () => {
        loading.value = true
        try {
            const res = await api.get('/users/deleted?page=1&pageSize=30')
            const data = res.data

            deletedUsers.value =
                data?.data ||
                data?.Data ||
                data?.items ||
                data ||
                []
        } catch (err) {
            error.value = err.response?.data?.message || 'Lỗi lấy danh sách user'
            users.value = []
        } finally {
            loading.value = false
        }
    }

    const createUser = async (payload) => {
        try {
            await api.post('/users', payload)
        } catch (err) {
            throw err
        }
    }

    const updateUser = async (id, payload) => {
        try {
            await api.put(`/users/${id}`, payload)
        } catch (err) {
            showMsg(msg)
            throw err
        }
    }

    const toggleActiveUser = async (id) => {
        try {
            await api.patch(`/users/${id}/toggle-active`)
        } catch (err) {
            const msg =
                err.response?.data?.message ||
                err.response?.data?.errorDetail ||
                'Thất bại'
            throw err
        }
    }

    return {
        deletedUsers,
        users,
        loading,
        error,
        fetchUsers,
        fetchDeletedUsers,
        createUser,
        updateUser,
        toggleActiveUser
    }
})
