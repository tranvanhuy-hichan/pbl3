<template>
  <div class="flex-grow flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-bg-light relative overflow-hidden">
    <!-- Decorative background elements -->
    <div class="absolute -top-24 -left-24 w-96 h-96 bg-brand-100 rounded-full blur-3xl opacity-50 pointer-events-none"></div>
    <div class="absolute -bottom-24 -right-24 w-80 h-80 bg-brand-200 rounded-full blur-3xl opacity-40 pointer-events-none"></div>

    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl overflow-hidden z-10 border border-slate-100 relative">
      <div class="bg-brand-500 px-6 py-8 text-center text-white">
        <h2 class="text-3xl font-extrabold tracking-tight">Chào mừng trở lại</h2>
        <p class="mt-2 text-brand-100">Đăng nhập để sử dụng dịch vụ</p>
      </div>
      
      <div class="p-8">
        <!-- Error Message Alert -->
        <div v-if="error" class="mb-4 bg-red-50 border border-red-200 text-red-600 p-3 rounded-lg text-sm font-medium text-center shadow-sm">
          {{ error }}
        </div>

        <form class="space-y-6" @submit.prevent="login(form)">
          <div>
            <label for="username" class="block text-sm font-medium text-slate-700 mb-1">Tên đăng nhập</label>
            <input id="username" v-model="form.username" type="text" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="Enter your username" />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Mật khẩu</label>
            <input id="password" v-model="form.password" type="password" autocomplete="current-password" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="••••••••" />
          </div>

          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <input id="remember-me" name="remember-me" type="checkbox" class="h-4 w-4 text-brand-500 focus:ring-brand-500 border-slate-300 rounded cursor-pointer" />
              <label for="remember-me" class="ml-2 block text-sm text-slate-700 cursor-pointer">
                Ghi nhớ đăng nhập
              </label>
            </div>

            <div class="text-sm">
              <a href="#" class="font-semibold text-brand-600 hover:text-brand-500 transition-colors">
                Quên mật khẩu?
              </a>
            </div>
          </div>

          <div>
            <button type="submit" :disabled="isLoading"
                    class="w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-white bg-brand-500 hover:bg-brand-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 transition-all hover:shadow-md disabled:opacity-70 disabled:cursor-not-allowed">
              <svg v-if="isLoading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ isLoading ? 'Đang đăng nhập' : 'Đăng nhập' }}
            </button>
          </div>
        </form>

        <div class="mt-8 text-center">
          <p class="text-sm text-slate-600">
            Bạn chưa có tài khoản?
            <router-link to="/register" class="font-bold text-brand-600 hover:text-brand-500 transition-colors">
              Đăng ký ngay
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue';
import { useAuth } from '../../composables/useAuth';

const { login, isLoading, error } = useAuth();

const form = reactive({
  username: '',
  password: ''
});
</script>
