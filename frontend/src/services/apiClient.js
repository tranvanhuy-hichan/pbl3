import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:7010/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor: tự động đính kèm Access Token vào mọi HTTP Request
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('access_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor: chuẩn hóa payload trả về và bắt lỗi 401
apiClient.interceptors.response.use(
  (response) => {
    // Thường API axios trả response trong property .data
    // Mình trả thẳng response.data để caller khỏi phải gọi res.data.data
    return response.data;
  },
  (error) => {
    if (error.response?.status === 401) {
      // Hết hạn token 8 tiếng, clear local storage
      localStorage.removeItem('access_token');
      localStorage.removeItem('user_info');
      // Nếu không dùng Vue Router trực tiếp ở file .js này, ta dùng window.location (hoặc cấu hình event bus)
      // Tùy theo cấu trúc router frontend mà path login là gì. Ví dụ:
      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
