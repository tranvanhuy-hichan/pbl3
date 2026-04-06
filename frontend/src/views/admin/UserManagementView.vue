<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">User management</h2>
      <button @click="openAddModal" class="bg-brand-500 hover:bg-brand-600 text-white px-5 py-2.5 rounded-lg font-semibold shadow-sm transition-all flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        <span>Add account</span>
      </button>
    </div>

    <div class="bg-white p-4 rounded-xl shadow-sm border border-slate-100 flex flex-col sm:flex-row gap-4 items-center justify-between">
      <div class="relative w-full max-w-md">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search by name, email, phone number..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-brand-500"
        />
        <svg class="absolute left-3 top-2.5 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
      </div>
      <div class="flex items-center gap-2 w-full sm:w-auto">
        <select v-model="selectedRole" class="w-full sm:w-auto border border-slate-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-brand-500 bg-white text-slate-700">
          <option value="">All roles</option>
          <option value="admin">Admin</option>
          <option value="customer">Customer</option>
        </select>
      </div>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">User</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Contact</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Role</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
              <th class="py-3 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Operation</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-for="user in filteredUsers" :key="user.userID" class="hover:bg-slate-50 transition-colors">
              <td class="py-4 px-6">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-brand-100 flex items-center justify-center text-brand-700 font-bold border border-brand-200 uppercase">
                    {{ (user.fullName || user.username || 'U').charAt(0) }}
                  </div>
                  <div>
                    <div class="font-bold text-slate-800">{{ user.fullName || 'N/A' }}</div>
                    <div class="text-xs text-slate-400">ID: {{ user.userID }} | @{{ user.username }}</div>
                  </div>
                </div>
              </td>
              <td class="py-4 px-6">
                <div class="text-sm text-slate-800 font-medium">{{ user.email }}</div>
                <div class="text-xs text-slate-500">{{ user.phoneNumber || 'Không có SĐT' }}</div>
              </td>
              <td class="py-4 px-6">
                <span class="px-2 py-0.5 rounded text-[10px] font-bold bg-slate-100 text-slate-600 uppercase">
                  {{ user.role }}
                </span>
              </td>
              <td class="py-4 px-6">
                <span :class="[
                  'px-2.5 py-1 text-xs font-semibold rounded-full',
                  user.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                ]">
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right flex justify-end gap-2">
                <button @click="openEditModal(user)" class="p-1.5 text-brand-600 hover:bg-brand-50 rounded-lg transition-colors">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg>
                </button>
                <button @click="confirmDelete(user)" class="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                </button>
              </td>
            </tr>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="5" class="py-12 text-center text-slate-400 italic">
                Không tìm thấy dữ liệu người dùng nào.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-lg overflow-hidden">
        <form @submit.prevent="saveUser">
          <div class="px-6 py-4 border-b flex justify-between items-center">
            <h3 class="text-xl font-bold text-slate-800">{{ isEditing ? 'Update Account' : 'Add Account' }}</h3>
            <button type="button" @click="closeModal" class="text-slate-400 hover:text-slate-600">✕</button>
          </div>
          <div class="p-6 space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Username *</label>
                <input v-model="formData.username" type="text" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Full Name</label>
                <input v-model="formData.fullName" type="text" class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Email *</label>
                <input v-model="formData.email" type="email" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Phone Number</label>
                <input v-model="formData.phoneNumber" type="text" class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Role</label>
                <select v-model="formData.role" class="w-full px-3 py-2 border rounded-lg bg-white">
                  <option value="Customer">Customer</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>
              <div>
                <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Status</label>
                <select v-model="formData.isActive" class="w-full px-3 py-2 border rounded-lg bg-white">
                  <option :value="true">Active</option>
                  <option :value="false">Inactive</option>
                </select>
              </div>
            </div>
            <div v-if="!isEditing">
              <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Password *</label>
              <input v-model="formData.password" type="password" required class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 outline-none">
            </div>
          </div>
          <div class="px-6 py-4 bg-slate-50 flex justify-end gap-3">
            <button type="button" @click="closeModal" class="px-4 py-2 text-slate-600 font-bold hover:bg-slate-200 rounded-lg">Hủy</button>
            <button type="submit" class="px-6 py-2 bg-brand-600 text-white font-bold rounded-lg hover:bg-brand-700 shadow-md">Lưu</button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="showDeleteDialog" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50">
      <div class="bg-white p-6 rounded-xl max-w-sm w-full shadow-2xl text-center">
        <div class="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mx-auto mb-4 text-2xl">⚠️</div>
        <h3 class="text-lg font-bold mb-2">Xóa người dùng?</h3>
        <p class="text-slate-500 mb-6">Xác nhận xóa tài khoản <span class="font-bold text-slate-800">@{{ userToDelete?.username }}</span>. Thao tác này không thể hoàn tác.</p>
        <div class="flex gap-3">
          <button @click="showDeleteDialog = false" class="flex-1 px-4 py-2 bg-slate-100 font-bold rounded-lg">Hủy</button>
          <button @click="executeDelete" class="flex-1 px-4 py-2 bg-red-600 text-white font-bold rounded-lg">Xác nhận xóa</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const users = ref([])
const searchQuery = ref('')
const selectedRole = ref('')
const showModal = ref(false)
const isEditing = ref(false)
const showDeleteDialog = ref(false)
const userToDelete = ref(null)

// CHÚ Ý: Đã bổ sung param phân trang cho giống hệt Swagger của bạn
const FETCH_URL = 'https://localhost:7205/api/users?page=1&pageSize=30';
const API_URL = 'https://localhost:7205/api/users';

const formData = ref({
  userID: 0, username: '', password: '', fullName: '', email: '', phoneNumber: '', role: 'Customer', isActive: true
})

const fetchUsers = async () => {
  try {
    const res = await fetch(FETCH_URL);
    const result = await res.json();
    console.log("📦 Dữ liệu gốc C# ném sang:", result);
    
    // Quét mọi trường hợp C# có thể trả về
    if (Array.isArray(result)) {
      users.value = result; 
    } else if (result && Array.isArray(result.data)) {
      users.value = result.data; // Chữ d thường
    } else if (result && Array.isArray(result.Data)) {
      users.value = result.Data; // Chữ D viết Hoa
    } else if (result && Array.isArray(result.items)) {
      users.value = result.items; // Kiểu phân trang
    } else {
      users.value = [];
      alert("⚠️ C# có phản hồi nhưng Vue không tìm thấy danh sách! Hãy bấm F12 xem Console.");
    }
    console.log("✅ Danh sách User sau khi gỡ vỏ:", users.value);
  } catch (err) {
    console.error("Lỗi gọi API:", err);
  }
}

onMounted(fetchUsers);

const filteredUsers = computed(() => {
  if (!Array.isArray(users.value)) return [];

  return users.value.filter(u => {
    const name = (u.fullName || u.FullName || u.username || u.Username || '').toLowerCase();
    const email = (u.email || u.Email || '').toLowerCase();
    const search = searchQuery.value.toLowerCase();
    const role = (u.role || u.Role || '').toLowerCase();

    const matchesSearch = name.includes(search) || email.includes(search);
    const matchesRole = selectedRole.value === '' || role === selectedRole.value.toLowerCase();

    return matchesSearch && matchesRole;
  });
})

const openAddModal = () => {
  isEditing.value = false;
  formData.value = { userID: 0, username: '', password: '', fullName: '', email: '', phoneNumber: '', role: 'Customer', isActive: true };
  showModal.value = true;
}

const openEditModal = (u) => {
  isEditing.value = true;
  formData.value = {
    userID: u.userID || u.UserID,
    username: u.username || u.Username || '',
    password: '', 
    fullName: u.fullName || u.FullName || '',
    email: u.email || u.Email || '',
    phoneNumber: u.phoneNumber || u.PhoneNumber || '',
    role: u.role || u.Role || 'Customer',
    isActive: u.isActive !== undefined ? u.isActive : true
  };
  showModal.value = true;
}

const closeModal = () => showModal.value = false;

const saveUser = async () => {
  const method = isEditing.value ? 'PUT' : 'POST';
  const id = formData.value.userID || formData.value.UserID;
  const url = isEditing.value ? `${API_URL}/${id}` : API_URL;

  try {
    const res = await fetch(url, {
      method: method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData.value)
    });

    if (res.ok) {
      alert("Lưu thành công!");
      await fetchUsers(); // Tự động load lại bảng
      closeModal();
    } else {
      const msg = await res.text();
      alert("Lỗi: " + msg);
    }
  } catch (err) {
    console.error(err);
  }
}

const confirmDelete = (u) => {
  userToDelete.value = u;
  showDeleteDialog.value = true;
}

const executeDelete = async () => {
  if (!userToDelete.value) return;
  // Bắt đúng ID kể cả khi C# viết Hoa hay Thường
  const id = userToDelete.value.userID || userToDelete.value.UserID || userToDelete.value.id;
  
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    if (res.ok) {
      alert("Xóa thành công!");
      await fetchUsers(); // Tự động làm mới dữ liệu
      showDeleteDialog.value = false;
    } else {
      alert("Lỗi xóa từ Backend!");
    }
  } catch (err) {
    console.error(err);
  }
}
</script>