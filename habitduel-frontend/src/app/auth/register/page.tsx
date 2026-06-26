'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { authService } from '@/services/authService';

export default function RegisterPage() {
  const router = useRouter();
  // 1. Ubah 'name' menjadi 'username'
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setErrorMessage('');

    try {
      // 2. Pastikan objek yang dikirim sesuai dengan Swagger (username, email, password)
      await authService.register({ username, email, password });
      
      alert('Pendaftaran berhasil! Silakan login.');
      router.push('/auth/login');
    } catch (error: any) {
      setErrorMessage(error.response?.data?.message || 'Gagal mendaftar.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', backgroundColor: '#f4f4f9' }}>
      <div style={{ padding: '30px', maxWidth: '400px', width: '100%', backgroundColor: 'white', borderRadius: '10px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)' }}>
        <h1 style={{ textAlign: 'center', marginBottom: '20px' }}>Daftar Akun</h1>
        
        {errorMessage && (
          <div style={{ backgroundColor: '#fee2e2', color: '#b91c1c', padding: '10px', borderRadius: '5px', marginBottom: '15px', textAlign: 'center' }}>
            {errorMessage}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {/* 3. Ubah label dan input untuk Username */}
          <div style={{ marginBottom: '15px' }}>
            <label>Username</label>
            <input 
              type="text" 
              value={username} 
              onChange={(e) => setUsername(e.target.value)} 
              required 
              style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }} 
            />
          </div>
          <div style={{ marginBottom: '15px' }}>
            <label>Email</label>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }} />
          </div>
          <div style={{ marginBottom: '20px' }}>
            <label>Password</label>
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }} />
          </div>
          <button 
            type="submit" 
            disabled={isLoading}
            style={{ width: '100%', padding: '12px', backgroundColor: isLoading ? '#ccc' : '#28a745', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}
          >
            {isLoading ? 'Memproses...' : 'Daftar Sekarang'}
          </button>
        </form>
        
        <p style={{ textAlign: 'center', marginTop: '20px' }}>
          Sudah punya akun? <Link href="/auth/login" style={{ color: '#0070f3' }}>Login di sini</Link>
        </p>
      </div>
    </div>
  );
}