// src/services/axiosConfig.ts
import axios from 'axios';
import Cookies from 'js-cookie'; // Pastikan sudah mengimpor Cookies

// 1. Ambil URL mentah dari env atau fallback ke localhost
const rawUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100';

// 2. Bersihkan URL dari typo double-dot (..) atau trailing slash (/) di ujung string
const cleanUrl = rawUrl.replace(/\.\./g, '.').replace(/\/$/, '');

const api = axios.create({
  baseURL: `${cleanUrl}/api`,
  withCredentials: true, // WAJIB TRUE agar sinkron dengan .AllowCredentials() di backend .NET Anda!
  headers: {
    'Content-Type': 'application/json',
  },
});

// Menambahkan token ke header setiap kali ada request
api.interceptors.request.use(
  (config) => {
    // Konsisten menggunakan Cookies agar sinkron dengan api.ts
    const token = Cookies.get('token'); 
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default api;