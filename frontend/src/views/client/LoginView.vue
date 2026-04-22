<template>
  <div class="min-h-screen flex items-center justify-center bg-sky-50 px-4 relative overflow-hidden">

    <!-- background blur circles -->
    <div class="absolute w-[500px] h-[500px] bg-sky-300/30 blur-[120px] rounded-full -top-40 -left-40"></div>
    <div class="absolute w-[450px] h-[450px] bg-blue-300/30 blur-[120px] rounded-full -bottom-40 -right-40"></div>

    <!-- card -->
    <div class="w-full max-w-md z-10">

      <div class="bg-white/80 backdrop-blur-xl border border-sky-100 rounded-3xl shadow-xl overflow-hidden">

        <!-- header -->
        <div class="p-8 text-center bg-gradient-to-r from-sky-500 to-blue-500 text-white">
          <h1 class="text-3xl font-bold">Chào mừng trở lại</h1>
          <p class="text-sm text-white/80 mt-1">Đăng nhập để tiếp tục</p>
        </div>

        <!-- body -->
        <div class="p-8 space-y-5">

          <!-- error -->
          <div v-if="error"
            class="bg-red-50 border border-red-200 text-red-600 text-sm p-3 rounded-xl text-center">
            {{ error }}
          </div>

          <!-- username -->
          <div>
            <label class="text-sm font-semibold text-slate-600">Tên đăng nhập</label>
            <input
              v-model="form.username"
              type="text"
              placeholder="Nhập username"
              class="mt-1 w-full px-4 py-3 rounded-xl border border-sky-100
                     focus:outline-none focus:ring-2 focus:ring-sky-400
                     transition shadow-sm"
            />
          </div>

          <!-- password -->
          <div>
            <label class="text-sm font-semibold text-slate-600">Mật khẩu</label>
            <input
              v-model="form.password"
              type="password"
              placeholder="••••••••"
              class="mt-1 w-full px-4 py-3 rounded-xl border border-sky-100
                     focus:outline-none focus:ring-2 focus:ring-sky-400
                     transition shadow-sm"
            />
          </div>

          <!-- options -->
          <div class="flex items-center justify-between text-sm text-slate-500">
            <label class="flex items-center gap-2">
              <input type="checkbox" class="accent-sky-500" />
              Ghi nhớ đăng nhập
            </label>

            <a href="#" class="text-sky-600 hover:underline font-semibold">
              Quên mật khẩu?
            </a>
          </div>

          <!-- button -->
          <button
            @click="login(form)"
            :disabled="isLoading"
            class="w-full py-3 rounded-xl font-bold text-white
                   bg-gradient-to-r from-sky-500 to-blue-500
                   hover:from-sky-600 hover:to-blue-600
                   transition-all shadow-lg shadow-sky-200
                   active:scale-[0.98] disabled:opacity-50"
          >
            {{ isLoading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
          </button>

          <!-- register -->
          <p class="text-center text-sm text-slate-500">
            Chưa có tài khoản?
            <router-link to="/register" class="text-sky-600 font-bold hover:underline">
              Đăng ký ngay
            </router-link>
          </p>

        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useAuth } from '@/composables/useAuth'

const { login, isLoading, error } = useAuth()

const form = reactive({
  username: '',
  password: ''
})
</script>
