<template>
  <div class="space-y-6 bg-slate-50 p-6">

    <!-- HEADER -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800 tracking-tight">
        Tài khoản đã xóa
      </h2>

      <button
        @click="goBack"
        class="bg-slate-700 hover:bg-slate-800 text-white px-5 py-2 rounded-lg font-semibold shadow-sm transition-all"
      >
        ← Quay lại
      </button>
    </div>
    <!-- TABLE -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">

          <thead>
            <tr class="bg-slate-50 border-b border-slate-200">
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase">Người dùng</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase">Email</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase">Vai trò</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase">Trạng thái</th>
              <th class="py-4 px-6 text-xs font-bold text-blue-700 uppercase text-right">Thao tác</th>
            </tr>
          </thead>

          <tbody class="divide-y divide-slate-100">
            <tr
              v-for="u in filteredDeletedUsers"
              :key="u.userId"
              class="hover:bg-slate-50 transition-colors"
            >
              <td class="py-4 px-6 flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-red-100 text-red-600 flex items-center justify-center font-bold uppercase">
                  {{ (u.fullName || u.username || 'U').charAt(0) }}
                </div>

                <div>
                  <div class="font-bold text-slate-800">
                    {{ u.fullName || 'N/A' }}
                  </div>
                  <div class="text-xs text-slate-400">
                    ID: {{ u.userId }}
                  </div>
                </div>
              </td>

              <td class="py-4 px-6 text-slate-700">
                {{ u.email }}
              </td>

              <td class="py-4 px-6">
                <span class="px-2 py-0.5 rounded text-[10px] font-bold bg-slate-100 text-slate-600 uppercase">
                  {{ u.role }}
                </span>
              </td>

              <td class="py-4 px-6">
                <span class="px-3 py-1 text-xs font-bold rounded-full bg-red-100 text-red-600">
                  Đã khóa 
                </span>
              </td>

              <td class="py-4 px-6 text-right">
                <button
                    @click="unlockUser(u)"
                    class="px-3 py-1 text-xs font-bold rounded-lg bg-green-100 text-green-700 hover:bg-green-200 transition"
                >
                    Mở khóa
                </button>
                </td>

            </tr>

            <tr v-if="filteredDeletedUsers.length === 0">
              <td colspan="4" class="py-12 text-center text-slate-400 italic">
                Không có tài khoản nào đã xóa
              </td>
            </tr>

          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const router = useRouter()
const userStore = useUserStore()

const searchQuery = ref('')

onMounted(() => {
  userStore.fetchDeletedUsers?.() 
})

const deletedUsers = computed(() => userStore.deletedUsers || [])

const filteredDeletedUsers = computed(() => {
  const search = searchQuery.value.toLowerCase()

  return deletedUsers.value.filter(u => {
    return (
      (u.fullName || '').toLowerCase().includes(search) ||
      (u.email || '').toLowerCase().includes(search)
    )
  })
})

const unlockUser = async (u) => {
  try {
    await userStore.toggleActiveUser(u.userId)
    await userStore.fetchDeletedUsers()
  } catch (err) {
    console.log(err)
  }
}

const goBack = () => {
  router.push('/admin/users')
}
</script>
