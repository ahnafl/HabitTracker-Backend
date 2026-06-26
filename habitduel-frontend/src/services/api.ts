// src/services/api.ts
import axios from 'axios';
import Cookies from 'js-cookie';

// Konfigurasi dasar
const api = axios.create({
  baseURL: 'http://localhost:5100/api', // Pastikan ini hanya sampai '/api'
  headers: {
    'Content-Type': 'application/json', // Penting agar ASP.NET Core membaca [FromBody] dengan benar
  },
});

// Interceptor: Menambahkan Token secara otomatis
api.interceptors.request.use(
  (config) => {
    const token = Cookies.get('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor: Menangani error global
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Mengecek apakah error berasal dari server (ada response)
    if (error.response) {
      const { status } = error.response;
      
      // Jika Unauthorized (401), hapus token dan tendang ke login
      if (status === 401) {
        Cookies.remove('token');
        window.location.href = '/auth/login';
      }
      
      // Bisa ditambahkan penanganan error lain (403 Forbidden, 500 Server Error, dll)
    } else {
      // Error jaringan (server mati/tidak bisa dijangkau)
      console.error("Network Error: Pastikan Backend .NET berjalan.");
    }
    
    return Promise.reject(error);
  }
);

export default api;