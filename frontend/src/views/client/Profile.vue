<template>
  <div class="max-w-3xl mx-auto p-6 space-y-6">

    <div>
      <h2 class="text-2xl font-bold text-slate-800">Thông tin cá nhân</h2>
      <p class="text-sm text-slate-500">Quản lý thông tin tài khoản của bạn</p>
    </div>

    <transition name="fade">
      <div 
        v-if="message"
        :class="[
          'fixed top-5 right-5 px-5 py-3 rounded-xl shadow-lg text-white z-50 text-sm font-medium',
          messageType === 'success' ? 'bg-green-500' : 'bg-red-500'
        ]"
      >
        {{ message }}
      </div>
    </transition>

    <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 space-y-5">

      <div v-if="profileStore.loading" class="text-center text-gray-500">
        Đang tải dữ liệu...
      </div>

      <template v-else>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">

          <div>
            <label class="label">Họ tên</label>
            <input v-model="form.fullName" class="input" />
          </div>

          <div>
            <label class="label">Email</label>
            <input v-model="form.email" class="input" />
          </div>

          <div>
            <label class="label">Số điện thoại</label>
            <input v-model="form.phoneNumber" class="input" />
          </div>

          <div>
            <label class="label">Ngày sinh</label>
            <input v-model="form.dateOfBirth" type="date" class="input" />
          </div>

        </div>

        <div>
          <label class="label">Địa chỉ</label>
          <input v-model="form.address" class="input" />
        </div>

        <button @click="updateProfile" class="btn-primary w-full">
          {{ profileStore.loading ? 'Đang lưu...' : 'Cập nhật thông tin' }}
        </button>

      </template>
    </div>

    <div class="flex gap-4">
      <button @click="openModal" class="btn-warning flex-1">
        Đổi mật khẩu
      </button>

      <button @click="handleLogout" class="btn-danger flex-1">
        Đăng xuất
      </button>
    </div>

    <transition name="fade">
      <div v-if="showModal" class="fixed inset-0 bg-black/40 flex items-center justify-center z-40">

        <div class="bg-white p-6 rounded-2xl w-96 space-y-4 shadow-xl">

          <h3 class="text-lg font-bold text-center text-slate-800">
            Đổi mật khẩu
          </h3>

          <div>
            <label class="label">Mật khẩu hiện tại</label>
            <input v-model="password.current" type="password" class="input" />
          </div>

          <div>
            <label class="label">Mật khẩu mới</label>
            <input v-model="password.new" type="password" class="input" />
          </div>

          <div>
            <label class="label">Xác nhận mật khẩu</label>
            <input v-model="password.confirm" type="password" class="input" />
          </div>

          <p v-if="error" class="text-red-500 text-sm text-center">
            {{ error }}
          </p>

          <div class="flex gap-3 pt-2">
            <button 
              @click="showModal = false"
              class="flex-1 border border-slate-200 rounded-xl py-2 hover:bg-slate-100">
              Hủy
            </button>

            <button 
              @click="changePassword"
              class="flex-1 btn-primary">
              Xác nhận
            </button>
          </div>

        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useProfileStore } from '@/stores/profileStore'
import { useAuth } from '@/composables/useAuth'

const profileStore = useProfileStore()
const { logout } = useAuth()

const form = ref({})
const showModal = ref(false)
const error = ref('')

const message = ref('')
const messageType = ref('success')

const password = ref({
  current: '',
  new: '',
  confirm: ''
})

onMounted(async () => {
  await profileStore.fetchProfile()
  form.value = { ...profileStore.user }
})

const showToast = (msg, type = 'success') => {
  message.value = msg
  messageType.value = type

  setTimeout(() => {
    message.value = ''
  }, 3000)
}

const updateProfile = async () => {
  const res = await profileStore.updateProfile(form.value)
  showToast(res.message, res.success ? 'success' : 'error')
}

const openModal = () => {
  error.value = ''
  password.value = { current: '', new: '', confirm: '' }
  showModal.value = true
}

const isStrongPassword = (pwd) => {
  const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/
  return regex.test(pwd)
}

const changePassword = async () => {
  if (!password.value.current || !password.value.new || !password.value.confirm) {
    error.value = 'Vui lòng nhập đầy đủ'
    return
  }

  if (!isStrongPassword(password.value.new)) {
    error.value = 'Mật khẩu phải ≥ 8 ký tự, có hoa, thường, số và ký tự đặc biệt'
    return
  }

  if (password.value.new !== password.value.confirm) {
    error.value = 'Mật khẩu xác nhận không khớp'
    return
  }

  const res = await profileStore.changePassword({
    currentPassword: password.value.current,
    newPassword: password.value.new
  })

  if (res.success) {
    showToast(res.message, 'success')
    showModal.value = false
  } else {
    error.value = res.message
  }
}

const handleLogout = () => {
  logout()
}
</script>

<style scoped>
.label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #64748b;
  margin-bottom: 6px;
}

.input {
  width: 100%;
  border: 1px solid #e2e8f0;
  padding: 10px 12px;
  border-radius: 12px;
  transition: all 0.2s;
  background: #f8fafc;
}

.input:focus {
  border-color: #3b82f6;
  background: white;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
  outline: none;
}

.btn-primary {
  background: linear-gradient(to right, #3b82f6, #2563eb);
  color: white;
  padding: 11px;
  border-radius: 12px;
  font-weight: 600;
  transition: 0.2s;
}

.btn-primary:hover {
  opacity: 0.9;
}

.btn-danger {
  background: #ef4444;
  color: white;
  padding: 10px;
  border-radius: 12px;
  font-weight: 600;
}

.btn-danger:hover {
  background: #dc2626;
}

.btn-warning {
  background: #f59e0b;
  color: white;
  padding: 10px;
  border-radius: 12px;
  font-weight: 600;
}

.btn-warning:hover {
  background: #d97706;
}

.fade-enter-active, .fade-leave-active {
  transition: 0.2s;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
