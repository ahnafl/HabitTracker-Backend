// lib/screens/register_screen.dart

import 'package:flutter/material.dart';
import '../services/auth_service.dart';

class RegisterScreen extends StatefulWidget {
  @override
  _RegisterScreenState createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final AuthService _authService = AuthService();
  final _usernameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  Future<void> _handleRegister() async {
    if (_usernameController.text.isEmpty || _emailController.text.isEmpty || _passwordController.text.isEmpty) {
      _showSnackBar("Semua field wajib diisi!");
      return;
    }

    setState(() => _isLoading = true);
    final sukses = await _authService.register(
      _usernameController.text.trim(),
      _emailController.text.trim(),
      _passwordController.text,
    );
    setState(() => _isLoading = false);

    if (sukses) {
      _showSnackBar("Registrasi berhasil! Silakan login.");
      Navigator.pop(context); // Kembali ke halaman Login
    } else {
      _showSnackBar("Registrasi gagal. Username/Email mungkin sudah terpakai.");
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
              Text("Buat Akun Baru", style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold), textAlign: TextAlign.center), // <-- Diubah ke TextAlign.center
              SizedBox(height: 30),
              TextField(controller: _usernameController, decoration: InputDecoration(labelText: "Username", border: OutlineInputBorder())),
              SizedBox(height: 16),
              TextField(controller: _emailController, decoration: InputDecoration(labelText: "Email", border: OutlineInputBorder())),
              SizedBox(height: 16),
              TextField(controller: _passwordController, obscureText: true, decoration: InputDecoration(labelText: "Password", border: OutlineInputBorder())),
              SizedBox(height: 24),
              ElevatedButton(
                onPressed: _isLoading ? null : _handleRegister,
                style: ElevatedButton.styleFrom(backgroundColor: Colors.blue.shade700, foregroundColor: Colors.white, padding: EdgeInsets.symmetric(vertical: 16)),
                child: _isLoading ? CircularProgressIndicator(color: Colors.white) : Text("Daftar", style: TextStyle(fontSize: 16)),
              ),
              SizedBox(height: 16),
              TextButton(
                onPressed: () => Navigator.pop(context),
                child: Text("Sudah punya akun? Login di sini"),
              )
            ],
          ),
        ),
      ),
    );
  }
}