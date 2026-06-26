// lib/models/habit_model.dart

class Habit {
  final int id;
  final String title;
  final String description;
  final String? targetDate;
  final bool isCompleted;
  final int streak;

  Habit({
    required this.id,
    required this.title,
    required this.description,
    this.targetDate,
    required this.isCompleted,
    required this.streak,
  });

  // Fungsi untuk mengubah JSON dari API menjadi Objek Dart (Pengganti otomatis di TS)
  factory Habit.fromJson(Map<String, dynamic> json) {
    return Habit(
      id: json['id'],
      title: json['title'] ?? '',
      description: json['description'] ?? '',
      targetDate: json['targetDate'],
      // Menangani jika backend mengirim boolean atau integer (0/1)
      isCompleted: json['isCompleted'] == true || json['isCompleted'] == 1,
      streak: json['streak'] ?? 0,
    );
  }

  // Fungsi jika kita ingin mengirim data objek ini kembali ke API dalam bentuk JSON
  Map<String, dynamic> toJson() {
    return {
      'title': title,
      'description': description,
      'targetDate': targetDate,
      'isCompleted': isCompleted,
    };
  }
}