'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Cookies from 'js-cookie';

export default function LogoutButton() {
  const router = useRouter();
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  const handleLogout = () => {
    setIsLoggingOut(true); // Aktifkan status loading
    
    // Hapus token
    Cookies.remove('token');
    
    // Arahkan kembali ke login
    router.push('/auth/login');
  };

  return (
    <button 
      onClick={handleLogout}
      disabled={isLoggingOut}
      style={{ 
        marginTop: '20px', 
        padding: '10px 20px', 
        backgroundColor: isLoggingOut ? '#ccc' : '#dc3545', 
        color: 'white', 
        border: 'none', 
        borderRadius: '5px', 
        cursor: isLoggingOut ? 'not-allowed' : 'pointer',
        fontWeight: 'bold'
      }}
    >
      {isLoggingOut ? 'Memproses...' : 'Logout'}
    </button>
  );
}