// lib/screens/dashboard_screen.dart

import 'package:flutter/material.dart';
import '../models/habit_model.dart';
import '../services/habit_service.dart';

class DashboardScreen extends StatefulWidget {
  final String token;

  const DashboardScreen({Key? key, required this.token}) : super(key: key);

  @override
  _DashboardScreenState createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  final HabitService _habitService = HabitService();
  List<Habit> _habits = [];
  bool _isLoading = true;

  // Controller Input Form
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _descController = TextEditingController();
  DateTime? _selectedDate;

  // Warna Tema Utama (Menyelaraskan dengan Tema Login)
  final Color primaryColor = const Color(0xFF1E293B); // Dark Slate / Charcoal
  final Color accentColor = const Color(0xFF475569);  // Medium Slate
  final Color backgroundColor = const Color(0xFFF8FAFC); // Off-White Premium

  @override
  void initState() {
    super.initState();
    _fetchHabits();
  }

  @override
  void dispose() {
    _titleController.dispose();
    _descController.dispose();
    super.dispose();
  }

  // Ambil data Habit dari Backend
  Future<void> _fetchHabits() async {
    setState(() => _isLoading = true);
    try {
      final data = await _habitService.getHabits(widget.token);
      setState(() {
        _habits = data;
      });
    } catch (e) {
      _showSnackBar("Gagal memuat data: $e");
    } finally {
      setState(() => _isLoading = false);
    }
  }

  // Fungsi Tambah Habit
  Future<void> _handleCreateHabit() async {
    final judul = _titleController.text.trim();
    final deskripsi = _descController.text.trim();

    if (judul.isEmpty) {
      _showSnackBar("Judul tidak boleh kosong!");
      return;
    }

    DateTime tanggalTerpilih = _selectedDate ?? DateTime.now();
    String tanggalFormatted = "${tanggalTerpilih.year}-${tanggalTerpilih.month.toString().padLeft(2, '0')}-${tanggalTerpilih.day.toString().padLeft(2, '0')}";

    final sukses = await _habitService.createHabit(
      judul, 
      deskripsi, 
      tanggalFormatted, 
      widget.token
    );

    if (sukses) {
      _titleController.clear();
      _descController.clear();
      setState(() => _selectedDate = null);
      _fetchHabits(); 
      _showSnackBar("Habit berhasil ditambahkan!");
    } else {
      _showSnackBar("Gagal menambahkan habit.");
    }
  }

  // Fungsi Hapus Habit
  Future<void> _handleDeleteHabit(dynamic id) async {
    final sukses = await _habitService.deleteHabit(id, widget.token);
    if (sukses) {
      _fetchHabits();
      _showSnackBar("Habit berhasil dihapus.");
    } else {
      _showSnackBar("Gagal menghapus habit.");
    }
  }

  // Fungsi Checkbox Toggle
  Future<void> _handleToggleComplete(dynamic id) async {
    final sukses = await _habitService.completeHabit(id, widget.token);
    if (sukses) {
      _fetchHabits();
    } else {
      _showSnackBar("Gagal mengupdate habit.");
    }
  }

  void _showSnackBar(String pesan) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(pesan),
        backgroundColor: primaryColor,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      ),
    );
  }

  String _formatDisplayDate(DateTime? date) {
    if (date == null) return "dd/mm/yyyy";
    const months = ["Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"];
    return "${date.day} ${months[date.month - 1]} ${date.year}";
  }

  @override
  Widget build(BuildContext context) {
    // Kalkulasi Statistik Dinamis
    int selesai7Hari = _habits.where((h) => h.isCompleted).length;
    double rateKeberhasilan = _habits.isEmpty 
        ? 0.0 
        : (_habits.where((h) => h.isCompleted).length / _habits.length) * 100;

    return Scaffold(
      backgroundColor: backgroundColor,
      body: _isLoading
          ? Center(child: CircularProgressIndicator(valueColor: AlwaysStoppedAnimation<Color>(primaryColor)))
          : SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 50.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // ================= HEADER WELCOME & LOGOUT =================
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            "Halo, farel!",
                            style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold, color: primaryColor, letterSpacing: -0.5),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            "Pantau perkembangan habit harianmu.",
                            style: TextStyle(fontSize: 14, color: Colors.grey.shade600),
                          ),
                        ],
                      ),
                      IconButton(
                        onPressed: () => Navigator.pushReplacementNamed(context, '/'),
                        icon: const Icon(Icons.logout_rounded),
                        color: const Color(0xFFEF4444),
                        tooltip: "Logout",
                      )
                    ],
                  ),
                  const SizedBox(height: 28),

                  // ================= CARDS STATISTIK (ROW) =================
                  Row(
                    children: [
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.all(20),
                          decoration: BoxDecoration(
                            color: Colors.white,
                            borderRadius: BorderRadius.circular(16),
                            boxShadow: [
                              BoxShadow(color: Colors.black.withOpacity(0.03), blurRadius: 10, offset: const Offset(0, 4)),
                            ],
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text("Selesai (7 Hari)", style: TextStyle(color: Colors.grey.shade500, fontSize: 12, fontWeight: FontWeight.w500)),
                              const SizedBox(height: 8),
                              Text("$selesai7Hari", style: TextStyle(color: primaryColor, fontSize: 32, fontWeight: FontWeight.bold)),
                            ],
                          ),
                        ),
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.all(20),
                          decoration: BoxDecoration(
                            color: primaryColor,
                            borderRadius: BorderRadius.circular(16),
                            boxShadow: [
                              BoxShadow(color: primaryColor.withOpacity(0.15), blurRadius: 12, offset: const Offset(0, 6)),
                            ],
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text("Rate Keberhasilan", style: TextStyle(color: Colors.grey.shade300, fontSize: 12, fontWeight: FontWeight.w500)),
                              const SizedBox(height: 8),
                              Text("${rateKeberhasilan.toStringAsFixed(1)}%", style: const TextStyle(color: Colors.white, fontSize: 32, fontWeight: FontWeight.bold)),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 36),

                  // ================= FORM TAMBAH HABIT =================
                  Text("Tambah Habit Baru", style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: primaryColor)),
                  const SizedBox(height: 16),
                  
                  Container(
                    padding: const EdgeInsets.all(20),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: [
                        BoxShadow(color: Colors.black.withOpacity(0.02), blurRadius: 10, offset: const Offset(0, 4)),
                      ],
                    ),
                    child: Column(
                      children: [
                        Row(
                          children: [
                            Expanded(
                              flex: 3,
                              child: TextField(
                                controller: _titleController,
                                decoration: InputDecoration(
                                  hintText: "Nama Habit",
                                  filled: true,
                                  fillColor: backgroundColor,
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                                ),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              flex: 2,
                              child: InkWell(
                                onTap: () async {
                                  DateTime? picked = await showDatePicker(
                                    context: context,
                                    initialDate: DateTime.now(),
                                    firstDate: DateTime(2025),
                                    lastDate: DateTime(2030),
                                    builder: (context, child) {
                                      return Theme(
                                        data: Theme.of(context).copyWith(
                                          colorScheme: ColorScheme.light(primary: primaryColor, onPrimary: Colors.white, onSurface: primaryColor),
                                        ),
                                        child: child!,
                                      );
                                    },
                                  );
                                  if (picked != null) {
                                    setState(() => _selectedDate = picked);
                                  }
                                },
                                child: Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
                                  decoration: BoxDecoration(
                                    color: backgroundColor,
                                    borderRadius: BorderRadius.circular(12),
                                  ),
                                  child: Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      Expanded(
                                        child: Text(
                                          _selectedDate == null ? "Tanggal" : "${_selectedDate!.day}/${_selectedDate!.month}",
                                          style: TextStyle(fontSize: 13, color: _selectedDate == null ? Colors.grey.shade500 : primaryColor, fontWeight: FontWeight.w500),
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                      Icon(Icons.calendar_today_rounded, size: 16, color: accentColor),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 12),
                        TextField(
                          controller: _descController,
                          decoration: InputDecoration(
                            hintText: "Deskripsi Singkat",
                            filled: true,
                            fillColor: backgroundColor,
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
                            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                          ),
                        ),
                        const SizedBox(height: 16),
                        SizedBox(
                          width: double.infinity,
                          child: ElevatedButton(
                            onPressed: _handleCreateHabit,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: primaryColor,
                              foregroundColor: Colors.white,
                              elevation: 0,
                              padding: const EdgeInsets.symmetric(vertical: 16),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                            ),
                            child: const Text("Simpan Habit", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 36),

                  // ================= LIST HABITS =================
                  Text("Daftar Kegiatan", style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: primaryColor)),
                  const SizedBox(height: 16),

                  _habits.isEmpty
                      ? Center(
                          child: Padding(
                            padding: const EdgeInsets.all(40.0),
                            child: Column(
                              children: [
                                Icon(Icons.assignment_turned_in_outlined, size: 48, color: Colors.grey.shade400),
                                const SizedBox(height: 12),
                                Text("Belum ada habit. Yuk tambah baru!", style: TextStyle(color: Colors.grey.shade500, fontSize: 14)),
                              ],
                            ),
                          ),
                        )
                      : ListView.builder(
                          shrinkWrap: true,
                          physics: const NeverScrollableScrollPhysics(),
                          itemCount: _habits.length,
                          itemBuilder: (context, index) {
                            final habit = _habits[index];
                            
                            String targetDateText = _selectedDate != null 
                                ? _formatDisplayDate(_selectedDate) 
                                : "1 Juli 2026"; 

                            return Container(
                              margin: const EdgeInsets.only(bottom: 16),
                              padding: const EdgeInsets.all(16),
                              decoration: BoxDecoration(
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(16),
                                border: habit.isCompleted ? Border.all(color: Colors.green.shade200, width: 1.5) : null,
                                boxShadow: [
                                  BoxShadow(color: Colors.black.withOpacity(0.02), blurRadius: 10, offset: const Offset(0, 4)),
                                ],
                              ),
                              child: Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Transform.scale(
                                    scale: 1.1,
                                    child: Checkbox(
                                      value: habit.isCompleted,
                                      activeColor: Colors.green,
                                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(4)),
                                      onChanged: habit.isCompleted 
                                          ? null 
                                          : (val) => _handleToggleComplete(habit.id),
                                    ),
                                  ),
                                  const SizedBox(width: 8),
                                  
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          habit.title,
                                          style: TextStyle(
                                            fontSize: 16, 
                                            fontWeight: FontWeight.bold,
                                            color: habit.isCompleted ? Colors.grey.shade500 : primaryColor,
                                            decoration: habit.isCompleted ? TextDecoration.lineThrough : null
                                          ),
                                        ),
                                        const SizedBox(height: 4),
                                        if (habit.description.isNotEmpty)
                                          Text(
                                            habit.description, 
                                            style: TextStyle(color: Colors.grey.shade500, fontSize: 13, height: 1.3),
                                          ),
                                        const SizedBox(height: 12),
                                        Row(
                                          children: [
                                            Icon(Icons.calendar_month_rounded, size: 14, color: Colors.grey.shade400),
                                            const SizedBox(width: 4),
                                            Text("Target: $targetDateText", style: TextStyle(color: Colors.grey.shade500, fontSize: 12)),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ),
                                  
                                  Column(
                                    crossAxisAlignment: CrossAxisAlignment.end,
                                    children: [
                                      IconButton(
                                        onPressed: () => _handleDeleteHabit(habit.id),
                                        icon: const Icon(Icons.delete_outline_rounded, size: 20),
                                        color: Colors.red.shade400,
                                        constraints: const BoxConstraints(),
                                        padding: EdgeInsets.zero,
                                      ),
                                      const SizedBox(height: 16),
                                      Container(
                                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                                        decoration: BoxDecoration(
                                          color: const Color(0xFFFFF3E0),
                                          borderRadius: BorderRadius.circular(8),
                                        ),
                                        child: Row(
                                          children: [
                                            const Text("🔥 ", style: TextStyle(fontSize: 11)),
                                            Text(
                                              "${habit.streak} Hari", 
                                              style: const TextStyle(color: Color(0xFFE65100), fontSize: 11, fontWeight: FontWeight.bold)
                                            ),
                                          ],
                                        ),
                                      ),
                                    ],
                                  )
                                ],
                              ),
                            );
                          },
                        ),
                ],
              ),
            ),
    );
  }
}