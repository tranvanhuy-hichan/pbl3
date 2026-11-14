import apiClient from '../apiClient';

export const AuthService = {
  /**
   * Đăng nhập
   * @param {Object} data { username, password }
   * @returns Promise
   */
  login(data) {
    return apiClient.post('/auth/login', data);
  },

  /**
   * Đăng ký tài khoản
   * @param {Object} data { username, password, fullName, email, phoneNumber, address, dateOfBirth }
   * @returns Promise
   */
  register(data) {
    return apiClient.post('/auth/register', data);
  }
};
