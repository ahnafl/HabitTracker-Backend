// lib/services/habit_service.dart

import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/habit_model.dart';

class HabitService {
  static const String baseUrl = 'http://localhost:5100/api';

  // 1. GET ALL HABITS (Sudah Diperbaiki & Anti-Crash)
  Future<List<Habit>> getHabits(String token) async {
    final response = await http.get(
      Uri.parse('$baseUrl/habits'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );

    if (response.statusCode == 200) {
      final decodedData = json.decode(response.body);
      List<dynamic> habitsJson = [];

      // Validasi struktur JSON secara dinamis agar COCOK dengan segala jenis backend
      if (decodedData is List) {
        // Jika backend langsung mengembalikan array: [{}, {}]
        habitsJson = decodedData;
      } else if (decodedData is Map<String, dynamic>) {
        // Jika backend membungkus di dalam object: {"data": []} atau {"habits": []}
        habitsJson = decodedData['data'] ?? decodedData['habits'] ?? [];
      }

      return habitsJson.map((json) => Habit.fromJson(json)).toList();
    } else {
      throw Exception('Gagal memuat data habit');
    }
  }

  // 2. TOGGLE COMPLETE HABIT
  Future<bool> completeHabit(dynamic id, String token) async {
    final response = await http.post(
      Uri.parse('$baseUrl/habits/$id/complete'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );
    return response.statusCode == 200;
  }

  // 3. CREATE HABIT
  Future<bool> createHabit(String title, String description, String targetDate, String token) async {
    final response = await http.post(
      Uri.parse('$baseUrl/habits'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
      body: json.encode({
        'title': title,
        'description': description,
        'targetDate': targetDate, 
      }),
    );
    return response.statusCode == 201 || response.statusCode == 200;
  }

  // 4. DELETE HABIT
  Future<bool> deleteHabit(dynamic id, String token) async {
    final response = await http.delete(
      Uri.parse('$baseUrl/habits/$id'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );
    return response.statusCode == 200;
  }
}