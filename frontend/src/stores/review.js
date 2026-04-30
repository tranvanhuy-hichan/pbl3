import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/apiClient'

export const useReviewStore = defineStore('review', () => {
    const reviews = ref([])
    const myReview = ref(null)
    const loading = ref(false)
    const error = ref(null)
    const totalCount = ref(0)
    const currentPage = ref(1)
    const pageSize = ref(5)
    const averageRating = ref(0)

    const fetchReviews = async (productId, page = 1, size = 5) => {
        loading.value = true
        error.value = null
        try {
            const res = await apiClient.get(`/reviews/${productId}`, {
                params: { page, pageSize: size }
            })

            reviews.value = res.items || []
            totalCount.value = res.totalCount || 0
            currentPage.value = res.page || page
            pageSize.value = res.pageSize || size

            if (reviews.value.length > 0) {
                const sum = reviews.value.reduce((s, r) => s + (r.rating || 0), 0)
                averageRating.value = sum / reviews.value.length
            } else {
                averageRating.value = 0
            }
        } catch (err) {
            error.value = 'Không thể tải đánh giá.'
            console.error(err)
            reviews.value = []
            averageRating.value = 0
        } finally {
            loading.value = false
        }
    }

    const fetchMyReview = async (productId) => {
        try {
            const res = await apiClient.get(`/reviews/${productId}/my-review`)
            myReview.value = res
        } catch {
            myReview.value = null
        }
    }

    const addReview = async (productId, rating, comment) => {
        try {
            await apiClient.post('/reviews', { productId, rating, comment })
            await fetchReviews(productId, 1, pageSize.value)
            await fetchMyReview(productId)
            return { success: true, message: 'Đánh giá thành công!' }
        } catch (err) {
            const message = err?.response?.data?.message || 'Đánh giá thất bại.'
            return { success: false, message }
        }
    }

    const reset = () => {
        reviews.value = []
        myReview.value = null
        error.value = null
        totalCount.value = 0
        currentPage.value = 1
        averageRating.value = 0
    }

    return {
        reviews,
        myReview,
        loading,
        error,
        totalCount,
        currentPage,
        pageSize,
        averageRating,
        fetchReviews,
        fetchMyReview,
        addReview,
        reset
    }
})