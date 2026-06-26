'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { authService } from '@/services/authService'; 
import Cookies from 'js-cookie'; 

export default function LoginPage() {
  const router = useRouter();
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setErrorMessage('');

    try {
      console.log("Mengirim data login:", { username, password }); // Debug point 1

      // Memanggil API
      const response = await authService.login({ username, password });

      if (response && response.token) {
        Cookies.set('token', response.token, { expires: 7, secure: true });
        router.push('/dashboard');
      } else {
        throw new Error('Respon tidak mengandung token');
      }
      
    } catch (error: any) {
      // DEBUGGING PENTING: Melihat apa yang sebenarnya dikirim backend
      console.error("Error Detail dari Backend:", error.response?.data); 
      
      // Menangkap pesan spesifik dari backend jika ada
      const message = error.response?.data?.message || 
                      error.response?.data?.title || // Kadang .NET menggunakan 'title' untuk error 400
                      'Username atau password salah. Silakan coba lagi.';
      
      setErrorMessage(message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', backgroundColor: '#f4f4f9' }}>
      <div style={{ padding: '30px', maxWidth: '400px', width: '100%', backgroundColor: 'white', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', borderRadius: '10px' }}>
        <h1 style={{ textAlign: 'center', marginBottom: '20px', color: '#333' }}>Login</h1>
        
        {errorMessage && (
          <div style={{ backgroundColor: '#fee2e2', color: '#b91c1c', padding: '10px', borderRadius: '5px', marginBottom: '15px', textAlign: 'center', fontSize: '14px' }}>
            {errorMessage}
          </div>
        )}
        
        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '15px' }}>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Username</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              style={{ width: '100%', padding: '10px', borderRadius: '5px', border: '1px solid #ddd' }}
              placeholder="Masukkan username Anda"
              required
            />
          </div>
          
          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              style={{ width: '100%', padding: '10px', borderRadius: '5px', border: '1px solid #ddd' }}
              placeholder="Masukkan password"
              required
            />
          </div>
          
          <button 
            type="submit" 
            disabled={isLoading}
            style={{ width: '100%', padding: '12px', backgroundColor: isLoading ? '#ccc' : '#0070f3', color: 'white', border: 'none', borderRadius: '5px', cursor: isLoading ? 'not-allowed' : 'pointer', fontWeight: 'bold' }}
          >
            {isLoading ? 'Memproses...' : 'Masuk'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: '20px', fontSize: '14px' }}>
          Belum punya akun? <Link href="/auth/register" style={{ color: '#0070f3', textDecoration: 'none', fontWeight: 'bold' }}>Daftar di sini</Link>
        </p>
      </div>
    </div>
  );
}