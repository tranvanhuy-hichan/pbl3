<template>
  <div class="flex-grow flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-sky-50 relative overflow-hidden">
    <!-- Các phần tử nền trang trí -->
    <div class="absolute -top-32 right-10 w-96 h-96 bg-sky-200 rounded-full blur-3xl opacity-50 pointer-events-none"></div>
    <div class="absolute -bottom-10 left-10 w-80 h-80 bg-blue-300 rounded-full blur-3xl opacity-10 pointer-events-none"></div>

    <div class="max-w-2xl w-full bg-white rounded-3xl shadow-xl overflow-hidden z-10 border border-sky-100 relative">
      <div class="bg-sky-700 px-6 py-8 text-center text-white">
        <h2 class="text-3xl font-extrabold tracking-tight">Tạo Tài Khoản</h2>
        <p class="mt-2 text-sky-200">Tham gia ngay hôm nay</p>
      </div>
      
      <div class="p-8">
        <!-- Thông báo lỗi & thành công -->
        <div v-if="error" class="mb-5 bg-red-50 text-red-600 p-3 border border-red-200 rounded-lg text-sm font-medium text-center shadow-sm">
          {{ error }}
        </div>
        <div v-if="successMsg" class="mb-5 bg-emerald-50 text-emerald-600 border border-emerald-200 p-3 rounded-lg text-sm font-medium text-center shadow-sm">
          {{ successMsg }}
        </div>

        <form class="space-y-5" @submit.prevent="handleRegister">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
            <div>
              <label for="username" class="block text-sm font-medium text-slate-700 mb-1">Tên đăng nhập</label>
              <input id="username" v-model="form.username" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="nguyenvana" />
            </div>

            <div>
              <label for="fullName" class="block text-sm font-medium text-slate-700 mb-1">Họ và tên</label>
              <input id="fullName" v-model="form.fullName" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="Nguyễn Văn A" />
            </div>

            <div>
              <label for="email" class="block text-sm font-medium text-slate-700 mb-1">Địa chỉ Email</label>
              <input id="email" v-model="form.email" type="email" autocomplete="email" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="ban@email.com" />
            </div>

            <div>
              <label for="phoneNumber" class="block text-sm font-medium text-slate-700 mb-1">Số điện thoại</label>
              <input id="phoneNumber" v-model="form.phoneNumber" type="tel" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="09xxxxxxxx" />
            </div>

            <div>
              <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Mật khẩu</label>
              <input id="password" v-model="form.password" type="password" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="••••••••" />
            </div>
            
            <div>
              <label for="confirm-password" class="block text-sm font-medium text-slate-700 mb-1">Xác nhận mật khẩu</label>
              <input id="confirm-password" v-model="confirmPassword" type="password" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="••••••••" />
            </div>

            <div>
              <label for="address" class="block text-sm font-medium text-slate-700 mb-1">Địa chỉ</label>
              <input id="address" v-model="form.address" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out" 
                     placeholder="123 Tên đường" />
            </div>

            <div>
              <label for="dateOfBirth" class="block text-sm font-medium text-slate-700 mb-1">Ngày sinh</label>
              <input id="dateOfBirth" v-model="form.dateOfBirth" type="date" required 
                     class="appearance-none block w-full px-4 py-3 border border-sky-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition-all duration-200 ease-in-out text-slate-700" />
            </div>
          </div>

          <div class="flex items-start mt-6">
            <div class="flex items-center h-5">
              <!-- Liên kết v-model với biến termsAccepted -->
              <input id="terms" name="terms" type="checkbox" v-model="termsAccepted" class="h-4 w-4 text-sky-600 focus:ring-sky-500 border-slate-300 rounded cursor-pointer" />
            </div>
            <div class="ml-2 text-sm">
              <label for="terms" class="text-slate-600 cursor-pointer">
                Tôi đồng ý với <a href="#" class="font-medium text-sky-700 hover:text-sky-600 transition-colors">Điều khoản</a> và <a href="#" class="font-medium text-sky-700 hover:text-sky-600 transition-colors">Chính sách bảo mật</a>.
              </label>
            </div>
          </div>

          <div class="pt-2">
            <button type="submit" 
                    :disabled="isLoading || !termsAccepted" 
                    class="w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-lg shadow-lg text-sm font-bold text-white bg-sky-700 hover:bg-sky-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-sky-700 transition-all duration-300 ease-in-out disabled:opacity-70 disabled:cursor-not-allowed">
              <svg v-if="isLoading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ isLoading ? 'Đang tạo tài khoản...' : 'Tạo Tài Khoản' }}
            </button>
          </div>
        </form>

        <div class="mt-8 text-center">
          <p class="text-sm text-slate-600">
            Đã có tài khoản?
            <router-link to="/login" class="font-bold text-sky-700 hover:text-sky-600 transition-colors">
              Đăng nhập
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { useAuth } from '../../composables/useAuth';

const { register, isLoading, error, successMsg } = useAuth();

const form = reactive({
  username: '',
  password: '',
  fullName: '',
  email: '',
  phoneNumber: '',
  address: '',
  dateOfBirth: ''
});
const confirmPassword = ref('');
const termsAccepted = ref(false); 

const handleRegister = () => {
  error.value = null;

  if (!termsAccepted.value) {
    error.value = 'Bạn phải đồng ý với Điều khoản và Chính sách bảo mật.';
    return;
  }

  if (form.password !== confirmPassword.value) {
    error.value = 'Mật khẩu xác nhận không khớp. Vui lòng thử lại.';
    return;
  }

  register(form);
}
</script>

<style scoped>

input[type="date"] {
  color: #334155;
}
input[type="date"]:focus {
  border-color: #0ea5e9; 
  box-shadow: 0 0 0 2px rgba(14, 165, 233, 0.2); 
}
</style>