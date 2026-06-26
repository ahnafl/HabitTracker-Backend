import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Menambahkan konfigurasi untuk mengabaikan linting saat build
  eslint: {
    ignoreDuringBuilds: true,
  },
};

export default nextConfig;
