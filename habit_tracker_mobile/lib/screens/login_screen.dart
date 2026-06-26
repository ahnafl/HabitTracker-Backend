// lib/screens/login_screen.dart

import 'package:flutter/material.dart';
import '../services/auth_service.dart';
import 'dashboard_screen.dart';
import 'register_screen.dart';

class LoginScreen extends StatefulWidget {
  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final AuthService _authService = AuthService();
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  Future<void> _handleLogin() async {
    if (_usernameController.text.isEmpty || _passwordController.text.isEmpty) {
      _showSnackBar("Username dan Password wajib diisi!");
      return;
    }

    setState(() => _isLoading = true);

    try {
      final token = await _authService.login(
        _usernameController.text.trim(),
        _passwordController.text,
      );

      if (token != null) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => DashboardScreen(token: token)),
        );
      } else {
        _showSnackBar("Login gagal. Periksa kembali username dan password.");
      }
    } catch (e) {
      // Menangkap error jika server mati atau terkena CORS
      _showSnackBar("Terjadi kesalahan: $e");
    } finally {
      // Memastikan loading berhenti apa pun yang terjadi
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  void _showSnackBar(String pesan) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(pesan)));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
  child: Container(
    constraints: const BoxConstraints(maxWidth: 400), // <--- Perbaikan di sini
    padding: const EdgeInsets.all(24.0),
    child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text("HabitDuel Mobile", style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold, color: Colors.blue.shade700), textAlign: TextAlign.center), // <-- Diubah ke TextAlign.center
SizedBox(height: 8),
Text("Masuk untuk melanjutkan track habitmu", style: TextStyle(color: Colors.grey), textAlign: TextAlign.center), // <-- Diubah ke TextAlign.center
              SizedBox(height: 30),
              TextField(controller: _usernameController, decoration: InputDecoration(labelText: "Username", border: OutlineInputBorder())),
              SizedBox(height: 16),
              TextField(controller: _passwordController, obscureText: true, decoration: InputDecoration(labelText: "Password", border: OutlineInputBorder())),
              SizedBox(height: 24),
              ElevatedButton(
                onPressed: _isLoading ? null : _handleLogin,
                style: ElevatedButton.styleFrom(backgroundColor: Colors.blue.shade700, foregroundColor: Colors.white, padding: EdgeInsets.symmetric(vertical: 16)),
                child: _isLoading ? CircularProgressIndicator(color: Colors.white) : Text("Masuk", style: TextStyle(fontSize: 16)),
              ),
              SizedBox(height: 16),
              TextButton(
                onPressed: () {
                  Navigator.push(context, MaterialPageRoute(builder: (context) => RegisterScreen()));
                },
                child: Text("Belum punya akun? Daftar sekarang"),
              )
            ],
          ),
        ),
      ),
    );
  }
}