<template>
  <div class="flex-grow flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-bg-light relative overflow-hidden">
    <!-- Decorative background elements -->
    <div class="absolute -top-32 right-10 w-96 h-96 bg-brand-100 rounded-full blur-3xl opacity-50 pointer-events-none"></div>
    <div class="absolute -bottom-10 left-10 w-80 h-80 bg-accent-500 rounded-full blur-3xl opacity-10 pointer-events-none"></div>

    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl overflow-hidden z-10 border border-slate-100 relative">
      <div class="bg-brand-900 px-6 py-8 text-center text-white">
        <h2 class="text-3xl font-extrabold tracking-tight">Create Account</h2>
        <p class="mt-2 text-brand-200">Join ElectroShop today</p>
      </div>
      
      <div class="p-8">
        <form class="space-y-5" @submit.prevent="handleRegister">
          <div>
            <label for="name" class="block text-sm font-medium text-slate-700 mb-1">Full Name</label>
            <input id="name" name="name" type="text" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="John Doe" />
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-slate-700 mb-1">Email address</label>
            <input id="email" name="email" type="email" autocomplete="email" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="you@example.com" />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Password</label>
            <input id="password" name="password" type="password" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="••••••••" />
          </div>
          
          <div>
            <label for="confirm-password" class="block text-sm font-medium text-slate-700 mb-1">Confirm Password</label>
            <input id="confirm-password" name="confirm-password" type="password" required 
                   class="appearance-none block w-full px-4 py-3 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 transition-colors" 
                   placeholder="••••••••" />
          </div>

          <div class="flex items-start mt-4">
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
            <button type="submit" class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-white bg-brand-900 hover:bg-brand-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-brand-900 transition-all hover:shadow-md">
              Create Account
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
const handleRegister = () => {
  console.log('Register attempt');
}
</script>
