<template>
  <div class="min-h-screen flex items-center justify-center bg-sky-50 px-4">

    <div class="w-full max-w-2xl bg-white rounded-2xl shadow-xl overflow-hidden">

      <!-- HEADER -->
      <div class="bg-sky-700 text-white text-center p-6">
        <h1 class="text-2xl font-bold">Đăng ký tài khoản</h1>

        <div class="flex justify-center gap-2 mt-4">
          <div
            v-for="s in 3"
            :key="s"
            class="w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold"
            :class="step >= s ? 'bg-white text-sky-700' : 'bg-sky-400 text-white opacity-60'"
          >
            {{ s }}
          </div>
        </div>
      </div>

      <!-- BODY -->
      <div class="p-6 space-y-4">

        <div v-if="error" class="bg-red-50 text-red-600 p-3 rounded-lg text-sm">
          {{ error }}
        </div>

        <div v-if="successMsg" class="bg-green-50 text-green-600 p-3 rounded-lg text-sm">
          {{ successMsg }}
        </div>

        <!-- STEP 1 -->
        <div v-if="step === 1" class="space-y-4">

          <div>
            <label class="label">Họ và tên</label>
            <input v-model="form.fullName" placeholder="Ví dụ: Nguyễn Văn A" class="input" />
          </div>

          <div>
            <label class="label">Số điện thoại</label>
            <input v-model="form.phoneNumber" placeholder="Ví dụ: 09xxxxxxxx" class="input" />
          </div>

          <div>
            <label class="label">Email</label>
            <input v-model="form.email" placeholder="Ví dụ: abc@gmail.com" class="input" />
          </div>

          <div>
            <label class="label">Ngày sinh</label>
            <input v-model="form.dateOfBirth" type="date" class="input" />
          </div>

          <div>
            <label class="label">Địa chỉ</label>
            <input v-model="form.address" placeholder="Ví dụ: 123 Lê Lợi, Đà Nẵng" class="input" />
          </div>
        </div>

        <!-- STEP 2 -->
        <div v-if="step === 2" class="space-y-4">

          <div>
            <label class="label">Tên đăng nhập</label>
            <input v-model="form.username" placeholder="Ví dụ: nguyenvana123" class="input" />
          </div>

          <div>
            <label class="label">Mật khẩu</label>
            <input v-model="form.password" type="password" placeholder="••••••••" class="input" />
          </div>

          <div>
            <label class="label">Xác nhận mật khẩu</label>
            <input v-model="form.confirmPassword" type="password" placeholder="••••••••" class="input" />
          </div>

          <p class="text-xs text-gray-500">
            Mật khẩu ≥ 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt
          </p>
        </div>

        <!-- STEP 3 -->
        <div v-if="step === 3" class="space-y-3 text-sm text-gray-700">

          <p><b>Họ tên:</b> {{ form.fullName }}</p>
          <p><b>Email:</b> {{ form.email }}</p>
          <p><b>SĐT:</b> {{ form.phoneNumber }}</p>
          <p><b>Địa chỉ:</b> {{ form.address }}</p>
          <p><b>Username:</b> {{ form.username }}</p>

          <div class="flex items-center gap-2 mt-4">
            <input type="checkbox" v-model="terms" />
            <span>Tôi đồng ý điều khoản</span>
          </div>
        </div>

        <!-- BUTTONS -->
        <div class="flex gap-3 pt-4">

          <button v-if="step > 1" @click="prevStep" class="flex-1 py-2 border rounded-lg">
            Quay lại
          </button>

          <button v-if="step < 3" @click="nextStep" class="flex-1 py-2 bg-sky-600 text-white rounded-lg">
            Tiếp tục
          </button>

          <button
            v-if="step === 3"
            @click="handleRegister"
            :disabled="isLoading || !terms"
            class="flex-1 py-2 bg-green-600 text-white rounded-lg disabled:opacity-50"
          >
            {{ isLoading ? 'Đang tạo...' : 'Đăng ký' }}
          </button>

        </div>

        <!-- LOGIN -->
        <p class="text-center text-sm mt-4">
          Đã có tài khoản?
          <router-link to="/login" class="text-sky-700 font-bold">
            Đăng nhập
          </router-link>
        </p>

      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useAuth } from '@/composables/useAuth'

const { register, isLoading } = useAuth()

const step = ref(1)
const error = ref('')
const successMsg = ref('')
const terms = ref(false)

const form = reactive({
  fullName: '',
  phoneNumber: '',
  email: '',
  dateOfBirth: '',
  address: '',
  username: '',
  password: '',
  confirmPassword: ''
})

const isStrongPassword = (pwd) => {
  return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$/.test(pwd)
}

const nextStep = () => {
  error.value = ''

  if (step.value === 1) {
    if (!form.fullName || !form.phoneNumber || !form.email || !form.dateOfBirth || !form.address) {
      error.value = 'Vui lòng nhập đầy đủ thông tin'
      return
    }
  }

  if (step.value === 2) {
    if (!form.username || !form.password || !form.confirmPassword) {
      error.value = 'Vui lòng nhập đầy đủ tài khoản'
      return
    }

    if (!isStrongPassword(form.password)) {
      error.value = 'Mật khẩu yếu'
      return
    }

    if (form.password !== form.confirmPassword) {
      error.value = 'Mật khẩu không khớp'
      return
    }
  }

  step.value++
}

const prevStep = () => {
  if (step.value > 1) step.value--
}

const handleRegister = async () => {
  error.value = ''
  successMsg.value = ''

  if (!terms.value) {
    error.value = 'Bạn phải đồng ý điều khoản'
    return
  }

  try {
    await register({
      ...form
    })

    successMsg.value = 'Đăng ký thành công 🎉'

  } catch (e) {
    error.value = 'Đăng ký thất bại'
  }
}
</script>

<style scoped>
.input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 10px;
  outline: none;
  transition: 0.2s;
}

.input:focus {
  border-color: #0ea5e9;
  box-shadow: 0 0 0 2px rgba(14,165,233,0.2);
}

.label {
  font-size: 13px;
  font-weight: 600;
  color: #334155;
  margin-bottom: 4px;
  display: block;
}
</style>
