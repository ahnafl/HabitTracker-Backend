// src/services/habitService.ts
import api from './api';
import { AxiosResponse } from 'axios';

// Interface Habit yang sinkron dengan backend
export interface Habit {
  id: number; 
  title: string;
  description: string;
  targetDate?: string;
  isCompleted: boolean;
  streak: number;
  last7Days: boolean[];
}

const BASE_URL = '/habits';

// Mengambil daftar habit
export const getHabits = (): Promise<AxiosResponse<Habit[]>> => 
  api.get<Habit[]>(BASE_URL);

// Membuat habit baru
export const createHabit = (data: { title: string; description: string; targetDate?: string }) => 
  api.post(BASE_URL, data);

// Mengupdate habit
export const updateHabit = (id: number, data: Partial<Habit>) => 
  api.put(`${BASE_URL}/${id}`, data);

// Menghapus habit
export const deleteHabit = (id: number) => 
  api.delete(`${BASE_URL}/${id}`);

// FUNGSI INI ADALAH KUNCI UTAMA:
// Pastikan endpoint ini yang dipanggil saat checkbox di-klik
export const completeHabit = (id: number) => 
  api.post(`${BASE_URL}/${id}/complete`);

// Mendapatkan statistik
export const getHabitStats = () => api.get('/habits/stats');