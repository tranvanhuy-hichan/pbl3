<template>
  <div class="max-w-3xl mx-auto p-6 space-y-6 ">

    <h2 class="text-2xl font-bold text-blue-700">Thông tin cá nhân</h2>

    <!-- 🔔 Toast -->
    <transition name="fade">
      <div 
        v-if="message"
        :class="[
          'fixed top-5 right-5 px-4 py-3 rounded-lg shadow-lg text-white z-50',
          messageType === 'success' ? 'bg-green-500' : 'bg-red-500'
        ]"
      >
        {{ message }}
      </div>
    </transition>

    <!-- Card -->
    <div class="bg-white p-6 rounded-xl shadow space-y-4">

      <div v-if="profileStore.loading" class="text-center text-gray-500">
        Đang tải dữ liệu...
      </div>

      <template v-else>

        <!-- FULL NAME -->
        <div>
          <label class="text-sm font-medium text-gray-600">Họ tên</label>
          <input v-model="form.fullName" placeholder="Họ tên" class="input" />
        </div>

        <!-- EMAIL -->
        <div>
          <label class="text-sm font-medium text-gray-600">Email</label>
          <input v-model="form.email" placeholder="Email" class="input" />
        </div>

        <!-- PHONE -->
        <div>
          <label class="text-sm font-medium text-gray-600">Số điện thoại</label>
          <input v-model="form.phoneNumber" placeholder="SĐT" class="input" />
        </div>

        <!-- ADDRESS -->
        <div>
          <label class="text-sm font-medium text-gray-600">Địa chỉ</label>
          <input v-model="form.address" placeholder="Địa chỉ" class="input" />
        </div>

        <!-- DOB -->
        <div>
          <label class="text-sm font-medium text-gray-600">Ngày sinh</label>
          <input v-model="form.dateOfBirth" type="date" class="input" />
        </div>

        <button @click="updateProfile" class="btn-primary w-full">
          {{ profileStore.loading ? 'Đang lưu...' : 'Cập nhật' }}
        </button>

      </template>

    </div>

    <!-- Actions -->
    <div class="flex gap-4">
      <button @click="openModal" class="btn-warning flex-1">
        Đổi mật khẩu
      </button>

      <button @click="handleLogout" class="btn-danger flex-1">
        Đăng xuất
      </button>
    </div>

    <!-- Modal -->
    <transition name="fade">
      <div v-if="showModal" class="fixed inset-0 bg-black/40 flex items-center justify-center z-40">

        <div class="bg-white p-6 rounded-xl w-96 space-y-4 shadow-lg">

          <h3 class="text-lg font-bold text-gray-700 text-center">
            Đổi mật khẩu
          </h3>

          <div>
            <label class="text-sm text-gray-600">Mật khẩu hiện tại</label>
            <input v-model="password.current" type="password" class="input" />
          </div>

          <div>
            <label class="text-sm text-gray-600">Mật khẩu mới</label>
            <input v-model="password.new" type="password" class="input" />
          </div>

          <div>
            <label class="text-sm text-gray-600">Xác nhận mật khẩu</label>
            <input v-model="password.confirm" type="password" class="input" />
          </div>

          <p v-if="error" class="text-red-500 text-sm text-center">
            {{ error }}
          </p>

          <div class="flex gap-3 pt-2">
            <button 
              @click="showModal = false"
              class="flex-1 border border-gray-300 rounded-lg py-2 hover:bg-gray-100">
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

    // 🔔 toast
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

    // 🔐 password regex
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
    .input {
    width: 100%;
    border: 1px solid #ddd;
    padding: 10px;
    border-radius: 10px;
    transition: 0.2s;
    }

    .input:focus {
    border-color: #2563eb;
    box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
    outline: none;
    }

    .btn-primary {
    background: #2563eb;
    color: white;
    padding: 10px;
    border-radius: 10px;
    }

    .btn-primary:hover {
    background: #1e40af;
    }

    .btn-danger {
    background: #dc2626;
    color: white;
    padding: 10px;
    border-radius: 10px;
    }

    .btn-danger:hover {
    background: #b91c1c;
    }

    .btn-warning {
    background: #f59e0b;
    color: white;
    padding: 10px;
    border-radius: 10px;
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
    ```
