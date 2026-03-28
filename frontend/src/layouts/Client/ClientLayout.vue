<template>
  <div class="flex flex-col min-h-screen font-sans bg-white text-slate-700">
    <Header />
    <main class="flex-grow">
      <RouterView />
    </main>
    <Footer />
  </div>
</template>

<script setup>
import { RouterView } from 'vue-router'
import Header from './Header.vue'
import Footer from './Footer.vue'
</script>
