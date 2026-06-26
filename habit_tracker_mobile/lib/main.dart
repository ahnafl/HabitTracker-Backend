import 'package:flutter/material.dart';
import 'screens/login_screen.dart'; // Pastikan ini mengarah ke login_screen

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Habit Tracker Mobile',
      debugShowCheckedModeBanner: false,
      home: LoginScreen(), // Ini yang membuat halaman login muncul pertama kali
    );
  }
}