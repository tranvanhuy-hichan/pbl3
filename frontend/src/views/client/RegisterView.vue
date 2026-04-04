<template>
  <div class="flex-grow flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-bg-light relative overflow-hidden">
    <!-- Decorative background elements -->
    <div class="absolute -top-32 right-10 w-96 h-96 bg-brand-100 rounded-full blur-3xl opacity-50 pointer-events-none"></div>
    <div class="absolute -bottom-10 left-10 w-80 h-80 bg-accent-500 rounded-full blur-3xl opacity-10 pointer-events-none"></div>

    <div class="max-w-2xl w-full bg-white rounded-2xl shadow-xl overflow-hidden z-10 border border-slate-100 relative">
      <div class="bg-brand-900 px-6 py-8 text-center text-white">
        <h2 class="text-3xl font-extrabold tracking-tight">Create Account</h2>
        <p class="mt-2 text-brand-200">Join ElectroShop today</p>
      </div>
      
      <div class="p-8">
        <!-- Error & Success Messages -->
        <div v-if="error" class="mb-5 bg-red-50 text-red-600 p-3 border border-red-200 rounded-lg text-sm font-medium text-center">
          {{ error }}
        </div>
        <div v-if="successMsg" class="mb-5 bg-emerald-50 text-emerald-600 border border-emerald-200 p-3 rounded-lg text-sm font-medium text-center">
          {{ successMsg }}
        </div>

        <form class="space-y-5" @submit.prevent="handleRegister">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
            <div>
              <label for="username" class="block text-sm font-medium text-slate-700 mb-1">Username</label>
              <input id="username" v-model="form.username" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="nguyenvana" />
            </div>

            <div>
              <label for="fullName" class="block text-sm font-medium text-slate-700 mb-1">Full Name</label>
              <input id="fullName" v-model="form.fullName" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="John Doe" />
            </div>

            <div>
              <label for="email" class="block text-sm font-medium text-slate-700 mb-1">Email address</label>
              <input id="email" v-model="form.email" type="email" autocomplete="email" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="you@email.com" />
            </div>

            <div>
              <label for="phoneNumber" class="block text-sm font-medium text-slate-700 mb-1">Phone Number</label>
              <input id="phoneNumber" v-model="form.phoneNumber" type="tel" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="0909090909" />
            </div>

            <div>
              <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Password</label>
              <input id="password" v-model="form.password" type="password" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="••••••••" />
            </div>
            
            <div>
              <label for="confirm-password" class="block text-sm font-medium text-slate-700 mb-1">Confirm Password</label>
              <input id="confirm-password" v-model="confirmPassword" type="password" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="••••••••" />
            </div>

            <div>
              <label for="address" class="block text-sm font-medium text-slate-700 mb-1">Address</label>
              <input id="address" v-model="form.address" type="text" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                     placeholder="123 Street Name" />
            </div>

            <div>
              <label for="dateOfBirth" class="block text-sm font-medium text-slate-700 mb-1">Date of Birth</label>
              <input id="dateOfBirth" v-model="form.dateOfBirth" type="date" required 
                     class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" />
            </div>
          </div>

          <div class="flex items-start mt-6">
            <div class="flex items-center h-5">
              <input id="terms" name="terms" type="checkbox" required class="h-4 w-4 text-brand-500 focus:ring-brand-500 border-slate-300 rounded cursor-pointer" />
            </div>
            <div class="ml-2 text-sm">
              <label for="terms" class="text-slate-600 cursor-pointer">
                I agree to the <a href="#" class="font-medium text-brand-600 hover:text-brand-500">Terms</a> and <a href="#" class="font-medium text-brand-600 hover:text-brand-500">Privacy Policy</a>.
              </label>
            </div>
          </div>

          <div class="pt-2">
            <button type="submit" :disabled="isLoading" 
                    class="w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-white bg-brand-900 hover:bg-brand-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-900 transition-all hover:shadow-md disabled:opacity-70 disabled:cursor-not-allowed">
              <svg v-if="isLoading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ isLoading ? 'Creating Account...' : 'Create Account' }}
            </button>
          </div>
        </form>

        <div class="mt-8 text-center">
          <p class="text-sm text-slate-600">
            Already have an account?
            <router-link to="/login" class="font-bold text-brand-600 hover:text-brand-500 transition-colors">
              Log in
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

const handleRegister = () => {
  error.value = null;

  if (form.password !== confirmPassword.value) {
    error.value = 'Mật khẩu xác nhận không khớp.';
    return;
  }

  // Gọi service composable đóng gói tất cả
  register(form);
}
</script>
