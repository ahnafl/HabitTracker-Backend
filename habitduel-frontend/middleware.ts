// src/middleware.ts
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const token = request.cookies.get('token')?.value;

  // 1. Jika mencoba akses /dashboard tapi tidak punya token
  if (request.nextUrl.pathname.startsWith('/dashboard') && !token) {
    return NextResponse.redirect(new URL('/auth/login', request.url));
  }

  // 2. Jika sudah punya token, jangan biarkan kembali ke login/register
  if ((request.nextUrl.pathname.startsWith('/auth/login') || 
       request.nextUrl.pathname.startsWith('/auth/register')) && token) {
    return NextResponse.redirect(new URL('/dashboard', request.url));
  }

  return NextResponse.next();
}

// Menentukan rute mana saja yang akan dipantau oleh middleware
export const config = {
  matcher: ['/dashboard/:path*', '/auth/login', '/auth/register'],
};