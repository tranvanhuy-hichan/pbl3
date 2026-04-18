import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthService } from '../services/auth/auth.service';
import { useAuthStore } from '../stores/authStore';

export function useAuth() {
  const authStore = useAuthStore();
  const router = useRouter(); 

  const isLoading = ref(false);
  const error = ref(null);
  const successMsg = ref('');


  const login = async (credentials) => {
    isLoading.value = true;
    error.value = null; 
    
    try {
      const response = await AuthService.login(credentials);

      const userInfo = {
        userId: response.userId,
        fullName: response.fullName,
        role: response.role
      };
      
      authStore.setAuth(response.token, userInfo);
      
      if (response.role === 'Admin') {
        router.push('/admin');
      } else {
        router.push('/');
      }
      return response;
    } catch (err) {
      error.value = err?.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.';
    } finally {
      isLoading.value = false;
    }
  };

  const register = async (userData) => {
    isLoading.value = true;
    error.value = null;
    successMsg.value = '';
    
    try {
      const response = await AuthService.register(userData);
      
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
