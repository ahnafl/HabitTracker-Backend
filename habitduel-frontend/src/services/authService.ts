// src/services/authService.ts
import api from './api';

export const authService = {
  // Login menggunakan username sesuai kesepakatan Swagger
  login: async (credentials: { username: string; password: string }) => {
    const response = await api.post('/auth/login', credentials);
    return response.data;
  },
  
  // Register menggunakan username, email, dan password
  register: async (userData: { username: string; email: string; password: string }) => {
    const response = await api.post('/auth/register', userData);
    return response.data;
  }
};