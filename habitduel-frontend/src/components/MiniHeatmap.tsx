export default function MiniHeatmap({ history }: { history?: boolean[] }) {
  // Jika history undefined atau null, gunakan array 7 false (abu-abu)
  const displayHistory = (history && Array.isArray(history)) ? history : Array(7).fill(false);

  return (
    <div style={{ display: 'flex', gap: '6px', alignItems: 'center', marginTop: '8px' }}>
      {displayHistory.map((isDone, index) => (
        <div
          key={index}
          style={{
            width: '12px',
            height: '12px',
            borderRadius: '3px',
            backgroundColor: isDone === true ? '#4caf50' : '#e0e0e0', // Hijau jika true, Abu-abu jika false
            transition: 'background-color 0.3s'
          }}
          title={isDone ? "Selesai" : "Belum selesai"}
        />
      ))}
    </div>
  );
}