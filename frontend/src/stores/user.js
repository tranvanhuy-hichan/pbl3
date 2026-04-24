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

    const page = ref(1)
    const pageSize = ref(10)
    const totalCount = ref(0)

    const fetchUsers = async () => {
        loading.value = true
        try {
            const res = await api.get(`/users?page=${page.value}&pageSize=${pageSize.value}`)
            const data = res.data

            users.value =
                data.items ||
                data.data ||
                data.Data ||
                []

            totalCount.value =
                data.totalCount ||
                data.TotalCount ||
                users.value.length

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
        await api.post('/users', payload)
    }

    const updateUser = async (id, payload) => {
        await api.put(`/users/${id}`, payload)
    }

    const toggleActiveUser = async (id) => {
        await api.patch(`/users/${id}/toggle-active`)
    }

    return {
        deletedUsers,
        users,
        loading,
        error,

        // ✅ QUAN TRỌNG
        page,
        pageSize,
        totalCount,

        fetchUsers,
        fetchDeletedUsers,
        createUser,
        updateUser,
        toggleActiveUser
    }
})
