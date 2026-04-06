import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthService } from '../services/auth/auth.service';
import { useAuthStore } from '../stores/authStore';

export function useAuth() {
  const authStore = useAuthStore();
  const router = useRouter(); // Bơm Router vào Composable
  
  // Trạng thái dùng để bind lên giao diện báo loading hoặc lỗi
  const isLoading = ref(false);
  const error = ref(null);
  const successMsg = ref(''); // Quản lý thông báo thành công luôn ở logic

  /**
   * Gọi API đăng nhập, tự động lưu store và định tuyến
   */
  const login = async (credentials) => {
    isLoading.value = true;
    error.value = null; // Reset error
    
    try {
      const response = await AuthService.login(credentials);
      
      // Response trả về có dạng: { token, role, userId, fullName } 
      const userInfo = {
        userId: response.userId,
        fullName: response.fullName,
        role: response.role
      };
      
      // Lưu vào Pinia và Local Storage
      authStore.setAuth(response.token, userInfo);
      
      // LOGIC CHUYỂN TRANG
      if (response.role === 'Admin') {
        router.push('/admin');
      } else {
        router.push('/');
      }
      return response;
    } catch (err) {
      // Đọc errorMessage từ object lỗi, nếu không có thì show mặc định
      error.value = err?.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.';
    } finally {
      isLoading.value = false;
    }
  };

  /**
   * Gọi API đăng ký tài khoản
   */
  const register = async (userData) => {
    isLoading.value = true;
    error.value = null;
    successMsg.value = '';
    
    try {
      const response = await AuthService.register(userData);
      
      // LOGIC CHUYỂN TRANG
      successMsg.value = response.message || 'Đăng ký thành công! Đang chuyển hướng về trang đăng nhập...';
      setTimeout(() => {
        router.push('/login');
      }, 2000);

      return response;
    } catch (err) {
      error.value = err?.message || 'Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.';
    } finally {
      isLoading.value = false;
    }
  };

  /**
   * Đăng xuất User
   */
  const logout = () => {
    authStore.clearAuth();
    router.push('/');
  };

  return {
    isLoading,
    error,
    successMsg,
    login,
    register,
    logout
  };
}
