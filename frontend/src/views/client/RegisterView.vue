<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-50 via-white to-blue-50 px-4 relative overflow-hidden">

    <!-- background blobs -->
    <div class="absolute w-[500px] h-[500px] bg-sky-300/30 blur-[120px] rounded-full -top-40 -left-40"></div>
    <div class="absolute w-[450px] h-[450px] bg-blue-300/30 blur-[120px] rounded-full -bottom-40 -right-40"></div>

    <!-- CARD -->
    <div class="w-full max-w-2xl bg-white/80 backdrop-blur-xl border border-white/40 rounded-3xl shadow-2xl overflow-hidden z-10">

      <!-- HEADER -->
      <div class="bg-gradient-to-r from-sky-500 to-blue-500 text-white text-center p-6">
        <h1 class="text-2xl font-bold">Đăng ký tài khoản mới</h1>
        <p class="text-white/70 text-sm mt-1">Chỉ 3 bước để bắt đầu sử dụng hệ thống</p>

        <div class="flex justify-center gap-3 mt-5">
          <div
            v-for="s in 3"
            :key="s"
            class="w-9 h-9 rounded-full flex items-center justify-center text-sm font-bold transition-all"
            :class="step >= s
              ? 'bg-white text-sky-600 shadow-md'
              : 'bg-white/20 text-white/70'"
          >
            {{ s }}
          </div>
        </div>
      </div>

      <!-- BODY -->
      <div class="p-6 space-y-5">

        <!-- ERROR / SUCCESS -->
        <div v-if="error" class="bg-red-50 border border-red-200 text-red-600 text-sm p-3 rounded-xl">
          {{ error }}
        </div>

        <div v-if="successMsg" class="bg-green-50 border border-green-200 text-green-600 text-sm p-3 rounded-xl">
          {{ successMsg }}
        </div>

        <!-- STEP 1 -->
        <div v-if="step === 1" class="space-y-4">

          <div>
            <label class="label">Họ và tên</label>
            <input v-model="form.fullName" class="input" placeholder="Nguyễn Văn A" />
          </div>

          <div>
            <label class="label">Số điện thoại</label>
            <input v-model="form.phoneNumber" class="input" placeholder="09xxxxxxxx" />
          </div>

          <div>
            <label class="label">Email</label>
            <input v-model="form.email" class="input" placeholder="abc@gmail.com" />
          </div>

          <div>
            <label class="label">Ngày sinh</label>
            <input v-model="form.dateOfBirth" type="date" class="input" />
          </div>

          <div>
            <label class="label">Địa chỉ</label>
            <input v-model="form.address" class="input" placeholder="Đà Nẵng..." />
          </div>

        </div>

        <!-- STEP 2 -->
        <div v-if="step === 2" class="space-y-4">

          <div>
            <label class="label">Tên đăng nhập</label>
            <input v-model="form.username" class="input" placeholder="username" />
          </div>

          <div>
            <label class="label">Mật khẩu</label>
            <input v-model="form.password" type="password" class="input" placeholder="••••••••" />
          </div>

          <div>
            <label class="label">Xác nhận mật khẩu</label>
            <input v-model="form.confirmPassword" type="password" class="input" placeholder="••••••••" />
          </div>

          <p class="text-xs text-slate-500">
            Mật khẩu ≥ 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt
          </p>

        </div>

        <!-- STEP 3 -->
        <div v-if="step === 3" class="bg-slate-50 p-4 rounded-xl text-sm text-slate-700 space-y-2">

          <p><b>👤 Họ tên:</b> {{ form.fullName }}</p>
          <p><b>📧 Email:</b> {{ form.email }}</p>
          <p><b>📱 SĐT:</b> {{ form.phoneNumber }}</p>
          <p><b>📍 Địa chỉ:</b> {{ form.address }}</p>
          <p><b>🔑 Username:</b> {{ form.username }}</p>

          <div class="flex items-center gap-2 mt-3">
            <input type="checkbox" v-model="terms" class="accent-sky-500" />
            <span>Tôi đồng ý điều khoản sử dụng</span>
          </div>

        </div>

        <!-- BUTTONS -->
        <div class="flex gap-3 pt-2">

          <button
            v-if="step > 1"
            @click="prevStep"
            class="flex-1 py-3 rounded-xl border font-semibold hover:bg-slate-50 transition"
          >
            Quay lại
          </button>

          <button
            v-if="step < 3"
            @click="nextStep"
            class="flex-1 py-3 rounded-xl font-semibold text-white
                   bg-gradient-to-r from-sky-500 to-blue-500
                   hover:shadow-lg transition active:scale-[0.98]"
          >
            Tiếp tục
          </button>

          <button
            v-if="step === 3"
            @click="handleRegister"
            :disabled="isLoading || !terms"
            class="flex-1 py-3 rounded-xl font-semibold text-white
                   bg-gradient-to-r from-emerald-500 to-green-500
                   hover:shadow-lg transition disabled:opacity-50"
          >
            {{ isLoading ? 'Đang tạo...' : 'Hoàn tất đăng ký' }}
          </button>

        </div>

        <!-- LOGIN LINK -->
        <p class="text-center text-sm text-slate-500">
          Đã có tài khoản?
          <router-link to="/login" class="text-sky-600 font-bold hover:underline">
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
    const res = await register({ ...form })

    if (res?.status === 201) {
      successMsg.value = 'Đăng ký thành công 🎉'
    }

  } catch (e) {
    error.value = e.response?.data?.message || 'Đăng ký thất bại'
  }
}
</script>

<style scoped>
.input {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid #e0f2fe;
  border-radius: 14px;
  outline: none;
  transition: 0.2s;
  background: white;
}

.input:focus {
  border-color: #38bdf8;
  box-shadow: 0 0 0 3px rgba(56,189,248,0.2);
}

.label {
  font-size: 13px;
  font-weight: 600;
  color: #334155;
  margin-bottom: 4px;
  display: block;
}
</style>
