'use client';

import { useEffect, useState } from 'react';
import { userService } from '@/services/userService';
import { getHabits, createHabit, updateHabit, deleteHabit, completeHabit, getHabitStats, Habit } from '@/services/habitService';
import LogoutButton from '@/components/LogoutButton';
import MiniHeatmap from '@/components/MiniHeatmap';

export default function DashboardPage() {
  const [user, setUser] = useState<any>(null);
  const [habits, setHabits] = useState<Habit[]>([]);
  const [stats, setStats] = useState({ completedLast7Days: 0, totalHabits: 0, completionRate: 0 });
  
  const [form, setForm] = useState({ title: '', description: '', isCompleted: false, targetDate: '' });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState<number | null>(null);

  const formatDate = (dateString?: string) => {
    if (!dateString) return '';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: 'numeric' });
    } catch {
      return dateString;
    }
  };

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const [profileData, habitsData, statsData] = await Promise.all([
        userService.getProfile(),
        getHabits(),
        getHabitStats() 
      ]);
      setUser(profileData);
      setHabits(habitsData.data || []);
      setStats(statsData.data); 
    } catch (error) {
      console.error("Gagal memuat data", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboardData();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      ...form,
      targetDate: form.targetDate || new Date().toISOString().split('T')[0] 
    };

    try {
      if (editingId) {
        await updateHabit(editingId, payload);
      } else {
        await createHabit(payload);
      }
      setForm({ title: '', description: '', isCompleted: false, targetDate: '' });
      setEditingId(null);
      await loadDashboardData(); 
    } catch (error) {
      alert("Gagal menyimpan habit.");
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Yakin ingin menghapus habit ini?")) {
      try {
        await deleteHabit(id);
        await loadDashboardData();
      } catch (error) {
        alert("Gagal menghapus habit");
      }
    }
  };

  const handleToggleComplete = async (id: number) => {
    setProcessingId(id);
    try {
      await completeHabit(id);
      // Panggil ulang data agar heatmap terupdate
      await loadDashboardData();
    } catch (error) {
      alert("Gagal mengupdate.");
    } finally {
      setProcessingId(null);
    }
  };

  if (loading && habits.length === 0) return <div style={{ padding: '50px', textAlign: 'center' }}>Memuat...</div>;

  return (
    <div style={{ padding: '40px', maxWidth: '800px', margin: '0 auto' }}>
      <h1>Halo, {user?.username || 'User'}!</h1>
      <LogoutButton />

      {/* Stats Section */}
      <section style={{ marginTop: '20px', display: 'flex', gap: '20px' }}>
        <div style={{ background: '#e3f2fd', padding: '15px', borderRadius: '10px', flex: 1, textAlign: 'center' }}>
          <div style={{ fontSize: '0.8em', color: '#666' }}>Selesai (7 Hari Terakhir)</div>
          <div style={{ fontSize: '1.8em', fontWeight: 'bold', color: '#1976d2' }}>{stats.completedLast7Days}</div>
        </div>
        <div style={{ background: '#e8f5e9', padding: '15px', borderRadius: '10px', flex: 1, textAlign: 'center' }}>
          <div style={{ fontSize: '0.8em', color: '#666' }}>Rate Keberhasilan</div>
          <div style={{ fontSize: '1.8em', fontWeight: 'bold', color: '#388e3c' }}>{stats.completionRate.toFixed(1)}%</div>
        </div>
      </section>
      
      {/* Form Section */}
      <section style={{ marginTop: '30px' }}>
        <h2>{editingId ? 'Edit Habit' : 'Tambah Habit'}</h2>
        <form onSubmit={handleSubmit} style={{ marginBottom: '20px', display: 'flex', flexDirection: 'column', gap: '10px' }}>
          <div style={{ display: 'flex', gap: '10px' }}>
            <input 
              placeholder="Judul" 
              value={form.title} 
              onChange={e => setForm({...form, title: e.target.value})} 
              required 
              style={{ padding: '10px', flex: 2, borderRadius: '6px', border: '1px solid #ccc' }} 
            />
            <input 
              type="date" 
              value={form.targetDate} 
              onChange={e => setForm({...form, targetDate: e.target.value})} 
              style={{ padding: '10px', flex: 1, borderRadius: '6px', border: '1px solid #ccc' }} 
            />
          </div>
          <input 
            placeholder="Deskripsi" 
            value={form.description} 
            onChange={e => setForm({...form, description: e.target.value})} 
            style={{ padding: '10px', borderRadius: '6px', border: '1px solid #ccc' }} 
          />
          <button type="submit" style={{ padding: '10px', borderRadius: '6px', cursor: 'pointer', background: '#333', color: 'white' }}>
            {editingId ? 'Update' : 'Tambah'}
          </button>
        </form>

        {/* List Section */}
        <div style={{ display: 'grid', gap: '15px' }}>
          {habits.map((h) => (
            <div key={h.id} style={{ border: '1px solid #ddd', padding: '15px', borderRadius: '12px', backgroundColor: h.isCompleted ? '#f0fdf4' : 'white' }}>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                  <input 
                    type="checkbox" 
                    checked={h.isCompleted} 
                    disabled={h.isCompleted || processingId === h.id}
                    onChange={() => handleToggleComplete(h.id)} 
                    style={{ transform: 'scale(1.5)', cursor: h.isCompleted ? 'not-allowed' : 'pointer' }} 
                  />
                  <div>
                    <h3 style={{ margin: 0 }}>{h.title}</h3>
                    {h.description && <p style={{ fontSize: '0.9em', color: '#666', margin: '4px 0 0 0' }}>{h.description}</p>}
                    
                    {/* MiniHeatmap dengan proteksi || [] */}
                    <MiniHeatmap history={h.last7Days || []} />

                    {h.targetDate && (
                       <span style={{ fontSize: '0.8em', color: '#555', display: 'block', marginTop: '4px' }}>
                         📅 Target: {formatDate(h.targetDate)}
                       </span>
                    )}
                  </div>
                </div>
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '10px' }}>
                  <button onClick={() => handleDelete(h.id)} style={{ background: 'none', border: 'none', color: 'red', cursor: 'pointer', fontSize: '0.9em' }}>Hapus</button>
                  <div style={{ padding: '4px 10px', borderRadius: '20px', fontSize: '0.85em', fontWeight: 'bold', color: '#e65100', background: '#fff3e0' }}>
                      🔥 {h.streak || 0} Hari
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}