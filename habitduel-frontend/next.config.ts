/** @type {import('next').NextConfig} */
const nextConfig = {
  eslint: {
    // Ini akan mengabaikan error linting saat proses build
    ignoreDuringBuilds: true,
  },
};

module.exports = nextConfig;
