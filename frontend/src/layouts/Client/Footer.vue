<template>
  <footer class="bg-slate-900 text-slate-300 pt-16">
    <div class="container mx-auto px-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-12 mb-12">
        <div class="lg:col-span-2">
          <h3 class="text-white text-2xl font-bold mb-4">Cửa hàng thiết bị điện tử</h3>
          <p class="leading-relaxed mb-6">Điểm đến cao cấp của bạn cho các thiết bị điện tử, laptop, điện thoại thông minh và phụ kiện mới nhất. Trải nghiệm công nghệ ở mức tốt nhất.</p>
          <div class="flex gap-4">
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Facebook">FB</a>
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Twitter">TW</a>
            <a href="#" class="w-10 h-10 flex items-center justify-center bg-slate-800 text-white rounded-full hover:bg-blue-500 hover:-translate-y-1 transition-all" aria-label="Instagram">IG</a>
          </div>
        </div>
        
        <div>
          <h4 class="text-white text-lg font-semibold mb-5">Danh mục</h4>
          <ul class="space-y-3">
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Laptop</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Điện thoại thông minh</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Tablet</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Loa & Tai nghe</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Tivi</a></li>
          </ul>
        </div>
        
        <div>
          <h4 class="text-white text-lg font-semibold mb-5">Dịch vụ khách hàng</h4>
          <ul class="space-y-3">
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Liên hệ chúng tôi</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Vận chuyển & Giao hàng</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Hoàn trả & Đổi trả</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Câu hỏi thường gặp</a></li>
            <li><a href="#" class="text-slate-400 hover:text-blue-500 transition-colors">Theo dõi đơn hàng</a></li>
          </ul>
        </div>
        
        <div class="lg:col-span-1">
          <h4 class="text-white text-lg font-semibold mb-5">Bản tin</h4>
          <p class="mb-4 text-sm">Đăng ký để nhận các ưu đãi đặc biệt, quà tặng miễn phí và các chương trình khuyến mãi.</p>
          <form class="flex mt-4" @submit.prevent>
            <input type="email" placeholder="Enter your email" class="flex-1 w-full px-4 py-3 rounded-l text-slate-900 outline-none" required />
            <button type="submit" class="bg-blue-500 hover:bg-blue-600 text-white px-5 rounded-r font-semibold transition-colors">Đăng ký</button>
          </form>
        </div>
      </div>
      
      <div class="border-t border-slate-800 py-6 flex flex-col sm:flex-row justify-between items-center text-sm gap-4 text-center">
        <p>&copy; {{ new Date().getFullYear() }} PBL3</p>
        <div class="flex gap-6">
          <a href="#" class="text-slate-400 hover:text-white transition-colors">Chính sách bảo mật</a>
          <a href="#" class="text-slate-400 hover:text-white transition-colors">Điều khoản dịch vụ</a>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
</script>
