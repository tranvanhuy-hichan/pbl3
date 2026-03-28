<template>
  <div class="flex-grow flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-bg-light relative overflow-hidden">
    <!-- Decorative background elements -->
    <div class="absolute -top-24 -left-24 w-96 h-96 bg-brand-100 rounded-full blur-3xl opacity-50 pointer-events-none"></div>
    <div class="absolute -bottom-24 -right-24 w-80 h-80 bg-brand-200 rounded-full blur-3xl opacity-40 pointer-events-none"></div>

    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl overflow-hidden z-10 border border-slate-100 relative">
      <div class="bg-brand-500 px-6 py-8 text-center text-white">
        <h2 class="text-3xl font-extrabold tracking-tight">Welcome Back</h2>
        <p class="mt-2 text-brand-100">Log in to your ElectroShop account</p>
      </div>
      
      <div class="p-8">
        <form class="space-y-6" @submit.prevent="handleLogin">
          <div>
            <label for="email" class="block text-sm font-medium text-slate-700 mb-1">Email address</label>
            <input id="email" name="email" type="email" autocomplete="email" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="you@example.com" />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Password</label>
            <input id="password" name="password" type="password" autocomplete="current-password" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="••••••••" />
          </div>

          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <input id="remember-me" name="remember-me" type="checkbox" class="h-4 w-4 text-brand-500 focus:ring-brand-500 border-slate-300 rounded cursor-pointer" />
              <label for="remember-me" class="ml-2 block text-sm text-slate-700 cursor-pointer">
                Remember me
              </label>
            </div>

            <div class="text-sm">
              <a href="#" class="font-semibold text-brand-600 hover:text-brand-500 transition-colors">
                Forgot your password?
              </a>
            </div>
          </div>

          <div>
            <button type="submit" class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-white bg-brand-500 hover:bg-brand-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-500 transition-all hover:shadow-md">
              Sign in
            </button>
          </div>
        </form>

        <div class="mt-8 text-center">
          <p class="text-sm text-slate-600">
            Don't have an account?
            <router-link to="/register" class="font-bold text-brand-600 hover:text-brand-500 transition-colors">
              Sign up now
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const handleLogin = () => {
  console.log('Login attempt');
}
</script>
