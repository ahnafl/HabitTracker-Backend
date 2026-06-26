// lib/services/auth_service.dart

import 'dart:convert';
import 'package:http/http.dart' as http;

class AuthService {
  static const String baseUrl = 'http://localhost:5100/api';

  // 1. FUNGSI REGISTER
  Future<bool> register(String username, String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/auth/register'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode({
        'username': username,
        'email': email,
        'password': password,
      }),
    );
    return response.statusCode == 201 || response.statusCode == 200;
  }

  // 2. FUNGSI LOGIN
  // Mengembalikan String token jika sukses, null jika gagal
  Future<String?> login(String username, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode({
        'username': username,
        'password': password,
      }),
    );

    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body);
      // Sesuaikan key token dengan response dari backend Anda (misal: data['token'] atau data['accessToken'])
      return data['token']; 
    }
    return null;
  }
}