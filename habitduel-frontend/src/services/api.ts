// src/services/api.ts
import axios from 'axios';
import Cookies from 'js-cookie';

// 1. Ambil URL mentah dari env atau fallback ke localhost
const rawUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100';

// 2. Bersihkan URL dari typo double-dot (..) atau trailing slash (/) di ujung string
const cleanUrl = rawUrl.replace(/\.\./g, '.').replace(/\/$/, '');

// Konfigurasi dasar
const api = axios.create({
  baseURL: `${cleanUrl}/api`,
  withCredentials: true, // WAJIB TRUE agar sinkron dengan .AllowCredentials() di backend .NET Anda!
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
      console.error("Network Error: Pastikan Backend .NET berjalan dan CORS diizinkan.");
    }
    
    return Promise.reject(error);
  }
);

export default api;