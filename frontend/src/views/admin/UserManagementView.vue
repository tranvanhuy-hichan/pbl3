<template>
  <div class="space-y-6 bg-slate-50 p-6">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý người dùng</h2>
    </div>

    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col sm:flex-row gap-4 items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Tìm kiếm..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
      <div class="flex items-center gap-2 w-full sm:w-auto">
        <select v-model="selectedRole" class="w-full sm:w-auto border border-slate-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-brand-500 bg-white text-slate-700">
          <option value="">Tất cả</option>
          <option value="admin">Admin</option>
          <option value="customer">Customer</option>
        </select>
        <button
          @click="goLockedUsers"
          class="bg-red-400 hover:bg-slate-800 text-white px-5 py-2 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2"
        >
          Tài khoản đã khóa
        </button>

        <button
          @click="openAddModal"
          class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
          </svg>
          <span>Thêm</span>
        </button>
      </div>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Người dùng</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Liên hệ</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider">Vai trò</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase tracking-wider text-right">Trạng thái</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="user in filteredUsers" :key="user.userId" @click="openEditModal(user)" class="hover:bg-slate-50 transition-colors cursor-pointer">
              <td class="py-4 px-6">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-brand-100 flex items-center justify-center text-brand-700 font-bold border border-brand-200 uppercase">
                    {{ (user.fullName || user.username || 'U').charAt(0) }}
                  </div>
                  <div>
                    <div class="font-bold text-slate-800">{{ user.fullName || 'N/A' }}</div>
                    <div class="text-xs text-slate-400">ID: {{ user.userId }}</div>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6">
                <div class="text-sm text-slate-800 font-medium">{{ user.email }}</div>
                <div class="text-xs text-slate-500">{{ user.phoneNumber || 'Không có SĐT' }}</div>
              </td>
              <td class="py-4 px-6">
                <span class="px-2 py-0.5 rounded text-[10px] font-bold bg-slate-100 text-slate-600 uppercase">
                  {{ user.role == 'Admin' ? 'Admin' : 'Khách hàng' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right">
                <span :class="[
                  'px-2.5 py-1 text-xs font-semibold rounded-full',
                  user.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                ]">
                  {{ user.isActive ? 'Hoạt động' : 'Đã khóa' }}
                </span>
              </td>
            </tr>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="5" class="py-12 text-center text-slate-400 italic">
                Không tìm thấy dữ liệu người dùng nào.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- PAGINATION -->
      <div class="px-6 py-4 bg-slate-50 border-t border-blue-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="text-sm text-blue-700 font-medium">
          Hiển thị
          <span class="text-blue-900 font-bold">{{ startEntry }}</span> -
          <span class="text-blue-900 font-bold">{{ endEntry }}</span>
          trong tổng số
          <span class="text-blue-600 font-bold">{{ userStore.totalCount }}</span> người dùng
        </div>
        <div class="flex items-center gap-6">
          <div class="flex items-center gap-2">
            <span class="text-xs text-blue-400 font-bold uppercase">Mỗi trang:</span>
            <select :value="userStore.pageSize" @change="onPageSizeChange" class="bg-white border border-blue-200 rounded-lg px-2 py-1 text-sm font-bold text-blue-700 focus:outline-none">
              <option :value="5">5</option>
              <option :value="10">10</option>
              <option :value="20">20</option>
            </select>
          </div>
          <div class="flex items-center bg-white border border-blue-200 rounded-xl p-1 shadow-sm">
            <button @click="goToPage(userStore.page - 1)" :disabled="userStore.page === 1" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path></svg>
            </button>
            <div class="flex items-center px-2 gap-1">
              <span class="px-3 py-1 bg-slate-50 text-blue-600 rounded-lg text-sm font-bold">{{ userStore.page }}</span>
              <span class="text-sm font-bold text-blue-400">/ {{ totalPages }}</span>
            </div>
            <button @click="goToPage(userStore.page + 1)" :disabled="userStore.page >= totalPages" class="p-2 rounded-lg hover:bg-slate-50 disabled:opacity-30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL THÊM / SỬA -->
    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-lg overflow-hidden">
        <form @submit.prevent="saveUser">
          <div class="px-6 py-4 border-b flex justify-between items-center">
            <h3 class="text-xl font-bold text-slate-800">{{ isEditing ? 'Chi tiết tài khoản' : 'Thêm tài khoản mới' }}</h3>
            <button type="button" @click="closeModal" class="text-slate-400 hover:text-slate-600">✕</button>
          </div>
          <div class="p-6 space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Tên đăng nhập *</label>
                <input v-model="formData.username" type="text" :disabled="isEditing" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Họ và tên</label>
                <input v-model="formData.fullName" type="text" class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Email *</label>
                <input v-model="formData.email" type="email" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Số điện thoại</label>
                <input v-model="formData.phoneNumber" type="text" class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Vai trò</label>
                <select v-model="formData.role" class="w-full px-3 py-2 border rounded-lg bg-white">
                  <option value="Customer">Customer</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Trạng thái</label>
                <select v-model="formData.isActive" :disabled="isEditing" class="w-full px-3 py-2 border rounded-lg bg-white">
                  <option :value="true">Active</option>
                  <option :value="false">Inactive</option>
                </select>
              </div>
            </div>
            <div v-if="!isEditing">
              <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Password *</label>
              <input v-model="formData.password" type="password" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
            </div>
          </div>
          <div class="px-6 py-4 bg-slate-50 flex justify-end gap-3">
            <button type="button" @click="closeModal" class="px-4 py-2 text-slate-600 font-bold hover:bg-slate-200 rounded-lg">Hủy</button>
            <button v-if="isEditing" type="button" @click="confirmDelete(formData)" class="px-4 py-2 bg-red-50 text-red-600 font-bold rounded-lg hover:bg-red-100">Khóa tài khoản</button>
            <button type="submit" class="px-6 py-2 bg-brand-600 text-white font-bold rounded-lg hover:bg-brand-700 shadow-md">Lưu</button>
          </div>
        </form>
      </div>
    </div>

    <!-- DIALOG XÁC NHẬN KHÓA -->
    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50">
      <div class="bg-white p-6 rounded-xl max-w-sm w-full shadow-2xl text-center">
        <div class="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mx-auto mb-4 text-2xl">⚠️</div>
        <h3 class="text-lg font-bold mb-2">Khóa người dùng?</h3>
        <p class="text-slate-500 mb-6">Xác nhận khóa tài khoản <span class="font-bold text-slate-800">@{{ userToDelete?.username }}</span></p>
        <div class="flex gap-3">
          <button @click="showDeleteDialog = false" class="flex-1 px-4 py-2 bg-slate-100 font-bold rounded-lg">Hủy</button>
          <button @click="executeDelete" class="flex-1 px-4 py-2 bg-red-600 text-white font-bold rounded-lg">Xác nhận khóa</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const router = useRouter()
const userStore = useUserStore()

const searchQuery = ref('')
const selectedRole = ref('')
const showModal = ref(false)
const isEditing = ref(false)
const showDeleteDialog = ref(false)
const userToDelete = ref(null)

const formData = ref({
  userID: 0,
  username: '',
  password: '',
  fullName: '',
  email: '',
  phoneNumber: '',
  role: 'Customer',
  isActive: true
})

// ===== THÔNG BÁO =====
const notify = (msg) => {
  alert(msg)
}

// ===== LOAD DATA =====
onMounted(() => {
  userStore.fetchUsers()
})

// ===== FILTER (client-side search/role trên dữ liệu trang hiện tại) =====
const filteredUsers = computed(() => {
  if (!Array.isArray(userStore.users)) return []
  return userStore.users.filter(u => {
    const name = (u.fullName || u.username || '').toLowerCase()
    const email = (u.email || '').toLowerCase()
    const search = searchQuery.value.toLowerCase()
    const role = (u.role || '').toLowerCase()
    const matchesSearch = name.includes(search) || email.includes(search)
    const matchesRole =
      selectedRole.value === '' ||
      role === selectedRole.value.toLowerCase()
    return matchesSearch && matchesRole
  })
})

// ===== PAGINATION =====
const totalPages = computed(() => {
  return Math.ceil(userStore.totalCount / userStore.pageSize) || 1
})

const startEntry = computed(() => {
  return ((userStore.page - 1) * userStore.pageSize) + 1
})

const endEntry = computed(() => {
  return Math.min(userStore.page * userStore.pageSize, userStore.totalCount)
})

const goToPage = (page) => {
  if (page < 1 || page > totalPages.value) return
  userStore.page = page
  userStore.fetchUsers()
}

const onPageSizeChange = (e) => {
  userStore.pageSize = Number(e.target.value)
  userStore.page = 1
  userStore.fetchUsers()
}

// reset về trang 1 khi search/filter thay đổi
watch([searchQuery, selectedRole], () => {
  userStore.page = 1
  userStore.fetchUsers()
})

// ===== MODAL ACTIONS =====
const openAddModal = () => {
  isEditing.value = false
  formData.value = {
    userID: 0,
    username: '',
    password: '',
    fullName: '',
    email: '',
    phoneNumber: '',
    role: 'Customer',
    isActive: true
  }
  showModal.value = true
}

const openEditModal = (u) => {
  isEditing.value = true
  formData.value = {
    userID: u.userId || u.userID,
    username: u.username || '',
    password: '',
    fullName: u.fullName || '',
    email: u.email || '',
    phoneNumber: u.phoneNumber || '',
    role: u.role || 'Customer',
    isActive: u.isActive ?? true
  }
  showModal.value = true
}

const closeModal = () => (showModal.value = false)

// ===== SAVE =====
const saveUser = async () => {
  try {
    if (isEditing.value) {
      await userStore.updateUser(formData.value.userID, formData.value)
      notify('Cập nhật người dùng thành công')
    } else {
      await userStore.createUser(formData.value)
      notify('Thêm người dùng thành công')
    }
    closeModal()
    userStore.fetchUsers()
  } catch (err) {
    notify(err.response?.data?.message || err.message || 'Có lỗi xảy ra!')
  }
}

// ===== DELETE / LOCK =====
const confirmDelete = (u) => {
  userToDelete.value = u
  showDeleteDialog.value = true
}

const executeDelete = async () => {
  try {
    const id = userToDelete.value.userId || userToDelete.value.userID
    await userStore.toggleActiveUser(id)
    notify('Khóa người dùng thành công')
    showDeleteDialog.value = false
    closeModal()
    userStore.fetchUsers()
  } catch (err) {
    notify('Thất bại!')
  }
}

const goLockedUsers = () => {
  router.push('/admin/users/trash')
}
</script>