import api from './api';

export const userService = {
  getProfile: async () => {
    // Interceptor akan otomatis menambahkan token di sini
    const response = await api.get('/user/profile'); 
    return response.data;
  }
};