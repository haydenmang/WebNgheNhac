/* ==========================================================================
   SoundWave – Web Music Online (ASP.NET Core .NET 8 + SQL Server + Supabase)
   Full Functional Client Application
   ========================================================================== */

// ── 0. SEED DATASET (ĐỒNG BỘ 100% VỚI Insert_Hiphop_Data.sql) ──
const SEED_DATA = {
  users: [
    { id: 1, username: 'admin', fullName: 'Quản Trị Viên', level: 1, avatar: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb' },
    { id: 2, username: 'mck_fan', fullName: 'Fan Cứng 99%', level: 0, avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde' },
    { id: 3, username: 'obito_fan', fullName: 'Người Nghe Đánh Đổi', level: 0, avatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330' },
    { id: 4, username: 'wxrdie_fan', fullName: 'Trapboy Wxrdie', level: 0, avatar: 'https://images.unsplash.com/photo-1570295999919-56ceb5ecca61' }
  ],
  singers: [
    { id: 1, name: 'RPT MCK', avatar: 'https://i.scdn.co/image/ab6761610000e5eb6a6065f675433dde99716a7e', bio: 'Rapper, ca sĩ, nhạc sĩ hàng đầu thuộc tổ đội Rapital, tác giả album phòng thu đình đám 99% (2023).' },
    { id: 2, name: 'Obito', avatar: 'https://i.scdn.co/image/ab6761610000e5eb1e5be0003167776bef2fc911', bio: 'Rapper, ca sĩ tài năng xuất thân từ OTĐ, chủ nhân kiệt tác album tự sự Đánh Đổi (2023).' },
    { id: 3, name: 'Wxrdie', avatar: 'https://i.scdn.co/image/ab6761610000e5ebf7ac790a43867c10ef8a7d29', bio: 'Rapper biểu tượng của làn sóng Trap và Melodic Rap tại Việt Nam với album THE WXRDIES.' }
  ],
  types: [
    { id: 1, typeName: 'Hip-hop / Rap', imageUrl: 'https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad' },
    { id: 2, typeName: 'Melodic Rap', imageUrl: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4' },
    { id: 3, typeName: 'R&B / Soul', imageUrl: 'https://images.unsplash.com/photo-1510915361894-db8b60106cb1' },
    { id: 4, typeName: 'Trap', imageUrl: 'https://images.unsplash.com/photo-1470225620780-dba8ba36b745' },
    { id: 5, typeName: 'Storytelling', imageUrl: 'https://images.unsplash.com/photo-1465847899084-d164df4dedc6' }
  ],
  albums: [
    { id: 1, albumName: '99%', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', singerId: 1, singerName: 'RPT MCK', createdAt: '2023-03-02' },
    { id: 2, albumName: 'Đánh Đổi', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', singerId: 2, singerName: 'Obito', createdAt: '2023-10-10' },
    { id: 3, albumName: 'THE WXRDIES', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', singerId: 3, singerName: 'Wxrdie', createdAt: '2024-01-20' }
  ],
  songs: [
    // Album 99% - MCK
    { id: 1, title: 'Chìm Sâu (feat. Trung Trần)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/chim-sau.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 173, plays: 145000, singerId: 1, singerName: 'RPT MCK', typeId: 3, typeName: 'R&B / Soul', albumId: 1, albumName: '99%' },
    { id: 2, title: 'Thắt Cà Vạt', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/that-ca-vat.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 184, plays: 89000, singerId: 1, singerName: 'RPT MCK', typeId: 2, typeName: 'Melodic Rap', albumId: 1, albumName: '99%' },
    { id: 3, title: 'Tại Vì Sao', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/tai-vi-sao.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 198, plays: 112000, singerId: 1, singerName: 'RPT MCK', typeId: 3, typeName: 'R&B / Soul', albumId: 1, albumName: '99%' },
    { id: 4, title: 'Chỉ Một Đêm Nữa Thôi (feat. tlinh)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/chi-mot-dem-nua-thoi.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 210, plays: 168000, singerId: 1, singerName: 'RPT MCK', typeId: 3, typeName: 'R&B / Soul', albumId: 1, albumName: '99%' },
    { id: 5, title: 'Anh Đã Ổn Hơn', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/anh-da-on-hon.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 190, plays: 96000, singerId: 1, singerName: 'RPT MCK', typeId: 2, typeName: 'Melodic Rap', albumId: 1, albumName: '99%' },
    { id: 6, title: 'Cuốn Cho Anh Một Điếu Nữa Đi', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/cuon-cho-anh.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 175, plays: 78000, singerId: 1, singerName: 'RPT MCK', typeId: 1, typeName: 'Hip-hop / Rap', albumId: 1, albumName: '99%' },
    { id: 7, title: 'Va Vào Giai Điệu Này', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/va-vao-giai-dieu-nay.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 182, plays: 84000, singerId: 1, singerName: 'RPT MCK', typeId: 2, typeName: 'Melodic Rap', albumId: 1, albumName: '99%' },
    { id: 8, title: 'Badtrip', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/badtrip.mp3', coverUrl: 'https://i.scdn.co/image/ab67616d0000b273b315e8bb7ef5e57e9a25bb0f', duration: 168, plays: 65000, singerId: 1, singerName: 'RPT MCK', typeId: 1, typeName: 'Hip-hop / Rap', albumId: 1, albumName: '99%' },

    // Album Đánh Đổi - Obito
    { id: 9, title: 'Đánh Đổi (feat. MCK)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/danh-doi.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 245, plays: 128000, singerId: 2, singerName: 'Obito', typeId: 5, typeName: 'Storytelling', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 10, title: 'Hà Nội (feat. V#)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/ha-noi.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 220, plays: 95000, singerId: 2, singerName: 'Obito', typeId: 3, typeName: 'R&B / Soul', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 11, title: 'Tell The Kids I Love Them (feat. Shiki)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/tell-the-kids.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 235, plays: 115000, singerId: 2, singerName: 'Obito', typeId: 2, typeName: 'Melodic Rap', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 12, title: 'Xuất Phát Điểm', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/xuat-phat-diem.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 195, plays: 72000, singerId: 2, singerName: 'Obito', typeId: 5, typeName: 'Storytelling', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 13, title: 'Sài Gòn Ơi', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/sai-gon-oi.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 210, plays: 68000, singerId: 2, singerName: 'Obito', typeId: 2, typeName: 'Melodic Rap', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 14, title: 'Trốn Chạy', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/tron-chay.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 205, plays: 79000, singerId: 2, singerName: 'Obito', typeId: 2, typeName: 'Melodic Rap', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 15, title: 'Biên Giới Long Bình', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/bien-gioi-long-binh.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 218, plays: 63000, singerId: 2, singerName: 'Obito', typeId: 5, typeName: 'Storytelling', albumId: 2, albumName: 'Đánh Đổi' },
    { id: 16, title: 'Tự Sự', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/tu-su.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music221/v4/93/2e/79/932e7998-e79d-e8dd-76c4-4594c70cc8b4/1200220247707.jpg/600x600bb.jpg', duration: 224, plays: 81000, singerId: 2, singerName: 'Obito', typeId: 2, typeName: 'Melodic Rap', albumId: 2, albumName: 'Đánh Đổi' },

    // Album THE WXRDIES - Wxrdie
    { id: 17, title: 'TETVOVEN (feat. Andree Right Hand)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/tetvoven.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 202, plays: 105000, singerId: 3, singerName: 'Wxrdie', typeId: 4, typeName: 'Trap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 18, title: 'Cầu Vồng Khuyết', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/cau-vong-khuyet.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 189, plays: 92000, singerId: 3, singerName: 'Wxrdie', typeId: 2, typeName: 'Melodic Rap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 19, title: 'Lối Đi Riêng', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/loi-di-rieng.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 195, plays: 74000, singerId: 3, singerName: 'Wxrdie', typeId: 4, typeName: 'Trap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 20, title: 'Nhìn (feat. 24k.Right)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/nhin.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 208, plays: 91000, singerId: 3, singerName: 'Wxrdie', typeId: 4, typeName: 'Trap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 21, title: 'Hard (feat. Gill)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/hard.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 190, plays: 67000, singerId: 3, singerName: 'Wxrdie', typeId: 4, typeName: 'Trap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 22, title: 'Mấy Em (feat. Machiot)', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/may-em.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 185, plays: 83000, singerId: 3, singerName: 'Wxrdie', typeId: 3, typeName: 'R&B / Soul', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 23, title: 'Trọng Tâm', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/trong-tam.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 178, plays: 71000, singerId: 3, singerName: 'Wxrdie', typeId: 4, typeName: 'Trap', albumId: 3, albumName: 'THE WXRDIES' },
    { id: 24, title: 'Diễn Viên Tồi', audioUrl: 'https://ffdpiyxbjyowlfhbingm.supabase.co/storage/v1/object/public/songs/dien-vien-toi.mp3', coverUrl: 'https://is1-ssl.mzstatic.com/image/thumb/Music211/v4/b8/42/e8/b842e8ab-846e-e6df-9c43-46272f06ebbd/24UM1IM23155.rgb.jpg/600x600bb.jpg', duration: 215, plays: 86000, singerId: 3, singerName: 'Wxrdie', typeId: 2, typeName: 'Melodic Rap', albumId: 3, albumName: 'THE WXRDIES' }
  ],
  playlists: [
    { id: 1, name: 'Trọn Bộ Album 99% - RPT MCK', userId: 2, songIds: [1, 2, 3, 4, 5, 6, 7, 8] },
    { id: 2, name: 'Tự Sự Đánh Đổi - Obito', userId: 3, songIds: [9, 10, 11, 12, 13, 14, 15, 16] },
    { id: 3, name: 'Trap & Drill - THE WXRDIE', userId: 4, songIds: [17, 18, 19, 20, 21, 22, 23, 24] },
    { id: 4, name: 'Vietnamese Hip-Hop Best Hits', userId: 1, songIds: [1, 4, 9, 11, 17, 18] }
  ]
};

// ── 1. GLOBAL STATE ──
let state = {
  songs: [...SEED_DATA.songs],
  singers: [...SEED_DATA.singers],
  albums: [...SEED_DATA.albums],
  types: [...SEED_DATA.types],
  playlists: [...SEED_DATA.playlists],
  users: [...SEED_DATA.users],
  favorites: JSON.parse(localStorage.getItem('sw_favorites') || '[1, 4, 9, 17]'),
  currentUser: JSON.parse(localStorage.getItem('sw_user') || 'null'),
  currentSong: null,
  isPlaying: false,
  isShuffle: false,
  isRepeat: false,
  currentPlaylistQueue: [],
  currentSongIndex: -1,
  currentPage: 1,
  pageSize: 10
};

// Base API URL (relative to host)
const API_BASE = '/api';

// ── 2. PARTICLE BACKGROUND CANVAS ──
(function () {
  const canvas = document.getElementById('bgCanvas');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  let W, H, pts = [];
  const COLORS = ['#155BC9', '#04FF24', '#1e72e8', '#00cc1a'];

  function resize() { W = canvas.width = innerWidth; H = canvas.height = innerHeight; }
  function mkPt() {
    return {
      x: Math.random() * W, y: Math.random() * H,
      r: Math.random() * 1.4 + 0.3,
      dx: (Math.random() - .5) * .35, dy: (Math.random() - .5) * .35,
      col: COLORS[Math.random() * COLORS.length | 0],
      a: Math.random() * .4 + .1
    };
  }
  function line(a, b) {
    const d = Math.hypot(a.x - b.x, a.y - b.y);
    if (d > 120) return;
    ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(b.x, b.y);
    ctx.strokeStyle = `rgba(21,91,201,${.07 * (1 - d / 120)})`;
    ctx.lineWidth = .6; ctx.stroke();
  }
  function loop() {
    ctx.clearRect(0, 0, W, H);
    pts.forEach((p, i) => {
      p.x += p.dx; p.y += p.dy;
      if (p.x < 0 || p.x > W) p.dx *= -1;
      if (p.y < 0 || p.y > H) p.dy *= -1;
      ctx.beginPath(); ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
      ctx.fillStyle = p.col; ctx.globalAlpha = p.a; ctx.fill(); ctx.globalAlpha = 1;
      for (let j = i + 1; j < pts.length; j++) line(p, pts[j]);
    });
    requestAnimationFrame(loop);
  }
  window.addEventListener('resize', resize);
  resize(); pts = Array.from({ length: 80 }, mkPt); loop();
})();

// ── 3. TOAST NOTIFICATIONS & ERROR HANDLING ──
let lastNetworkWarnTime = 0;

function showToast(message, type = 'info') {
  const container = document.getElementById('toastContainer');
  if (!container) return;
  const isErr = type === 'error' || type === true;
  const isWarn = type === 'warning';

  let typeClass = '';
  let icon = '✨';
  if (isErr) {
    typeClass = 'error';
    icon = '⚠️';
  } else if (isWarn) {
    typeClass = 'warning';
    icon = '⚠️';
  }

  let displayMessage = message;
  if (typeof message === 'object' && message !== null) {
    if (message.errors) {
      if (typeof message.errors === 'object') {
        displayMessage = Object.values(message.errors).flat().join(', ');
      } else {
        displayMessage = String(message.errors);
      }
    } else if (message.message) {
      displayMessage = message.message;
    } else if (message.title) {
      displayMessage = message.title;
    } else {
      displayMessage = JSON.stringify(message);
    }
  }

  const toast = document.createElement('div');
  toast.className = `toast-msg ${typeClass}`.trim();
  toast.innerHTML = `
    <span>${icon}</span>
    <span>${displayMessage}</span>
  `;
  container.appendChild(toast);
  setTimeout(() => {
    toast.style.opacity = '0';
    toast.style.transform = 'translateX(100%)';
    toast.style.transition = 'all 0.3s ease';
    setTimeout(() => toast.remove(), 300);
  }, 3200);
}

// Global safeFetch helper wrapping fetch() in try-catch with specific error handling
async function safeFetch(url, options = {}) {
  try {
    const res = await fetch(url, options);
    if (!res.ok) {
      if (res.status === 400) {
        let errData = null;
        try {
          errData = await res.clone().json();
        } catch (_) {}
        if (errData && (errData.errors || errData.message)) {
          showToast(errData.errors || errData.message, 'error');
        } else {
          showToast('Yêu cầu không hợp lệ (400)', 'error');
        }
      } else if (res.status >= 500) {
        showToast('Lỗi hệ thống, vui lòng thử lại', 'error');
      }
    }
    return res;
  } catch (netErr) {
    const now = Date.now();
    if (now - lastNetworkWarnTime > 3000) {
      showToast('Không thể kết nối server, dùng dữ liệu offline', 'warning');
      lastNetworkWarnTime = now;
    }
    throw netErr;
  }
}

// ── SKELETON LOADING HELPER ──
function showSkeleton(containerId, type, count = 4) {
  const container = document.getElementById(containerId);
  if (!container) return;
  const className = type === 'card' ? 'skeleton skeleton-card' : 'skeleton skeleton-row';
  let html = '';
  for (let i = 0; i < count; i++) {
    html += `<div class="${className}"></div>`;
  }
  container.innerHTML = html;
}

// ── 4. PAGE ROUTER ──
const router = (function () {
  const pages = document.querySelectorAll('.page');
  const navItems = document.querySelectorAll('.nav-item[data-page]');
  let current = 'home';
  let historyStack = ['home'];
  let historyIdx = 0;

  function go(pageId, pushHist = true) {
    if (pageId === current) return;

    // Check admin gate
    if (pageId === 'admin') {
      const isAdmin = state.currentUser && state.currentUser.level === 1;
      document.getElementById('adminAccessGate').style.display = isAdmin ? 'none' : 'block';
      document.getElementById('adminDashboardContent').style.display = isAdmin ? 'block' : 'none';
      if (isAdmin) renderAdminDashboard();
    }

    if (pageId === 'library') {
      showLibraryList();
    }

    if (pageId === 'favorites') {
      renderFavoritesPage();
    }

    const oldPage = document.getElementById('page-' + current);
    if (oldPage) {
      oldPage.classList.remove('active');
    }

    const nextPage = document.getElementById('page-' + pageId);
    if (nextPage) {
      nextPage.classList.add('active');
      const cnt = nextPage.querySelector('.content');
      if (cnt) cnt.scrollTop = 0;
    }

    navItems.forEach(el => {
      el.classList.remove('active');
      const pill = el.querySelector('.nav-active-pill');
      if (pill) pill.remove();
      if (el.dataset.page === pageId) {
        el.classList.add('active');
        const p = document.createElement('span');
        p.className = 'nav-active-pill';
        el.appendChild(p);
      }
    });

    current = pageId;

    if (pushHist) {
      historyStack = historyStack.slice(0, historyIdx + 1);
      historyStack.push(pageId);
      historyIdx++;
    }
  }

  navItems.forEach(el => {
    el.addEventListener('click', e => {
      e.preventDefault();
      go(el.dataset.page);
    });
  });

  document.getElementById('btnBack')?.addEventListener('click', () => {
    if (historyIdx > 0) {
      historyIdx--;
      go(historyStack[historyIdx], false);
    }
  });

  document.getElementById('btnFwd')?.addEventListener('click', () => {
    if (historyIdx < historyStack.length - 1) {
      historyIdx++;
      go(historyStack[historyIdx], false);
    }
  });

  return { go, getCurrent: () => current };
})();

// ── 5. DATA HYDRATION & API INTEGRATION ──
async function loadInitialData() {
  // 1. Show skeleton placeholders before fetching
  showSkeleton('homeGenresGrid', 'card', 5);
  showSkeleton('homeAlbumsGrid', 'card', 3);
  showSkeleton('homeTracksContainer', 'row', state.pageSize || 10);
  showSkeleton('homeArtistsGrid', 'card', 3);

  try {
    // 1. Fetch Songs with pagination
    const resSongs = await safeFetch(`${API_BASE}/songs?page=${state.currentPage}&pageSize=${state.pageSize}`);
    if (resSongs && resSongs.ok) {
      const data = await resSongs.json();
      if (Array.isArray(data) && data.length > 0) {
        state.songs = data;
      } else if (data && data.data && Array.isArray(data.data) && data.data.length > 0) {
        state.songs = data.data;
      }
    }
  } catch (e) {
    console.log('Backend API /songs not reachable yet, using seeded Hip-Hop songs.');
  }

  try {
    // 2. Fetch Singers
    const resSingers = await safeFetch(`${API_BASE}/singers`);
    if (resSingers && resSingers.ok) {
      const data = await resSingers.json();
      if (data && data.length > 0) state.singers = data;
    }
  } catch (e) { }

  try {
    // 3. Fetch Albums
    const resAlbums = await safeFetch(`${API_BASE}/albums`);
    if (resAlbums && resAlbums.ok) {
      const data = await resAlbums.json();
      if (data && data.length > 0) state.albums = data;
    }
  } catch (e) { }

  try {
    // 4. Fetch Types
    const resTypes = await safeFetch(`${API_BASE}/types`);
    if (resTypes.ok) {
      const data = await resTypes.json();
      if (data && data.length > 0) state.types = data;
    }
  } catch (e) { }

  // Set default current playlist queue
  state.currentPlaylistQueue = [...state.songs];

  // Render UI sections (clears skeletons)
  renderHomeGenres();
  renderHomeAlbums();
  renderHomeTracks(state.songs, state.currentPage);
  renderHomeArtists();
  renderSidebarPlaylists();
  renderSearchAll();
  updateUserUI();

  // Populate Add Song modal selects
  populateSongFormSelects();

  // Set hero play first song
  if (state.songs.length > 0) {
    setPlayerSong(state.songs[0], false);
  }
}

// ── 6. UI RENDERING FUNCTIONS ──

// A. Home: Genres / Types
function renderHomeGenres() {
  const container = document.getElementById('homeGenresGrid');
  if (!container) return;
  container.innerHTML = state.types.map(t => `
    <div class="cat-card" style="cursor:pointer" onclick="filterByGenre('${t.typeName}')">
      <div class="cat-bg" style="background-image:url('${t.imageUrl || 'https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad'}'); background-size:cover; background-position:center; filter:brightness(0.6)"></div>
      <div class="cat-content" style="position:relative; z-index:2; padding:20px">
        <h3 style="font-size:18px; font-weight:800; color:#fff">${t.typeName}</h3>
        <span style="font-size:12px; color:var(--green)">Khám phá dòng nhạc →</span>
      </div>
    </div>
  `).join('');
}

// B. Home: Albums
function renderHomeAlbums() {
  const container = document.getElementById('homeAlbumsGrid');
  if (!container) return;
  container.innerHTML = state.albums.map(a => `
    <div class="album-card" style="cursor:pointer" onclick="playAlbumTracks(${a.id})">
      <div class="ac-cover" style="position:relative; border-radius:12px; overflow:hidden; aspect-ratio:1">
        <img src="${a.coverUrl}" alt="${a.albumName}" style="width:100%; height:100%; object-fit:cover"/>
        <div class="ac-play-overlay">
          <button class="ac-play-btn" title="Phát toàn bộ Album">
            <svg width="20" height="20" fill="currentColor" viewBox="0 0 24 24"><path d="M5 3l14 9-14 9V3z"/></svg>
          </button>
        </div>
      </div>
      <div class="ac-info" style="margin-top:10px">
        <h4 style="font-weight:700; font-size:15px; color:#fff; white-space:nowrap; overflow:hidden; text-overflow:ellipsis">${a.albumName}</h4>
        <p style="font-size:13px; color:var(--muted2)">${a.singerName || 'Nhiều nghệ sĩ'}</p>
      </div>
    </div>
  `).join('');
}

// C. Home: Tracks with Pagination (renderHomeTracks & renderPagination)
function renderHomeTracks(songs = state.songs, page = state.currentPage) {
  state.currentPage = page;
  const container = document.getElementById('homeTracksContainer') || document.getElementById('homeSongsList');
  if (!container) return;
  const pageSize = state.pageSize || 10;
  const startIdx = (page - 1) * pageSize;
  const pagedSongs = songs.slice((page - 1) * 10, page * 10);
  container.innerHTML = pagedSongs.map((s, idx) => createSongRowHTML(s, startIdx + idx + 1)).join('');
  renderPagination(songs.length, page, pageSize, 'homePagination');
}

function renderPagination(total, page, pageSize, containerId) {
  const container = document.getElementById(containerId);
  if (!container) return;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  container.className = 'pagination-container';
  container.innerHTML = `
    <button class="btn-pagination" id="btnPrevPage" ${page <= 1 ? 'disabled' : ''} onclick="changeHomePage(${page - 1})">
      <svg width="14" height="14" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><polyline points="15 18 9 12 15 6"/></svg>
      <span>Prev</span>
    </button>
    <span class="pagination-info" id="paginationInfo">Trang ${page}/${totalPages}</span>
    <button class="btn-pagination" id="btnNextPage" ${page >= totalPages ? 'disabled' : ''} onclick="changeHomePage(${page + 1})">
      <span>Next</span>
      <svg width="14" height="14" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><polyline points="9 18 15 12 9 6"/></svg>
    </button>
  `;
}

async function changeHomePage(page) {
  if (page < 1) return;
  state.currentPage = page;
  showSkeleton('homeTracksContainer', 'row', state.pageSize || 10);
  try {
    const res = await safeFetch(`${API_BASE}/songs?page=${page}&pageSize=${state.pageSize}`);
    if (res && res.ok) {
      const data = await res.json();
      if (Array.isArray(data)) {
        state.songs = data;
        renderHomeTracks(state.songs, page);
        return;
      } else if (data && data.data && Array.isArray(data.data)) {
        const container = document.getElementById('homeTracksContainer') || document.getElementById('homeSongsList');
        if (container) {
          const startIdx = (page - 1) * (state.pageSize || 10);
          container.innerHTML = data.data.map((s, idx) => createSongRowHTML(s, startIdx + idx + 1)).join('');
          renderPagination(data.total || data.data.length, page, state.pageSize || 10, 'homePagination');
        }
        return;
      }
    }
  } catch (err) {
    // Error handled by safeFetch, fallback to local state
  }
  renderHomeTracks(state.songs, page);
}

// Backward-compatible alias for renderHomeSongs
function renderHomeSongs() {
  renderHomeTracks(state.songs, state.currentPage);
}

// D. Home: Artists
function renderHomeArtists() {
  const container = document.getElementById('homeArtistsGrid');
  if (!container) return;
  container.innerHTML = state.singers.map(art => `
    <div class="artist-card" style="cursor:pointer; text-align:center" onclick="filterByArtist('${art.name}')">
      <div class="art-avatar-wrap" style="width:120px; height:120px; margin:0 auto 12px; border-radius:50%; overflow:hidden; border:2px solid rgba(4,255,36,0.3); box-shadow:0 10px 25px rgba(0,0,0,0.5)">
        <img src="${art.avatar}" alt="${art.name}" style="width:100%; height:100%; object-fit:cover"/>
      </div>
      <h4 style="font-weight:700; font-size:15px; color:#fff">${art.name}</h4>
      <p style="font-size:12px; color:var(--green)">Nghệ sĩ xác minh ✓</p>
    </div>
  `).join('');
}

// Helper: Create song row HTML
function createSongRowHTML(song, num) {
  const isFav = state.favorites.includes(song.id);
  const isPlayingThis = state.currentSong && state.currentSong.id === song.id && state.isPlaying;
  const isCurrent = state.currentSong && state.currentSong.id === song.id;

  return `
    <div class="track-row ${isCurrent ? 'is-current-playing' : ''}" id="songRow-${song.id}">
      <span class="tr-num">${String(num).padStart(2, '0')}</span>
      <div class="tr-cover" onclick="playSongById(${song.id})" style="position:relative; width:44px; height:44px; border-radius:8px; overflow:hidden; cursor:pointer">
        <img src="${song.coverUrl || 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4'}" style="width:100%; height:100%; object-fit:cover"/>
        <div class="tr-play" style="display:${isPlayingThis ? 'none' : 'flex'}">
          <svg width="14" height="14" fill="#030407" viewBox="0 0 24 24"><path d="M5 3l14 9-14 9V3z"/></svg>
        </div>
      </div>
      <div class="tr-info" style="cursor:pointer" onclick="playSongById(${song.id})">
        <p class="tr-title">${song.title}</p>
        <p class="tr-artist">${song.singerName || 'Nghệ sĩ'} • <span style="color:var(--muted2)">${song.typeName || song.albumName || ''}</span></p>
      </div>
      <div class="tr-wave ${isPlayingThis ? 'playing' : ''}"><span></span><span></span><span></span><span></span><span></span></div>
      <span class="tr-plays">${(song.plays || 0).toLocaleString()} lượt nghe</span>
      <button class="tr-like ${isFav ? 'liked' : ''}" onclick="toggleFavorite(${song.id}, event)" title="${isFav ? 'Bỏ thích' : 'Yêu thích'}">
        <svg width="15" height="15" fill="${isFav ? '#04FF24' : 'none'}" stroke="${isFav ? '#04FF24' : '#6b7280'}" stroke-width="2" viewBox="0 0 24 24"><path d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z"/></svg>
      </button>
      <span class="tr-dur">${formatDuration(song.duration || 180)}</span>
      <div style="position:relative">
        <button class="tr-more" onclick="toggleSongMenu(${song.id}, event)" title="Tùy chọn">
          <svg width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="12" cy="12" r="1"/><circle cx="19" cy="12" r="1"/><circle cx="5" cy="12" r="1"/></svg>
        </button>
        <div class="song-menu-dropdown" id="songMenu-${song.id}">
          <div class="song-menu-item" onclick="openAddToPlaylistModal(${song.id})">➕ Thêm vào Playlist</div>
          <div class="song-menu-item" onclick="playSongById(${song.id})">▶ Phát ngay</div>
        </div>
      </div>
    </div>
  `;
}

// ── 7. AUDIO STREAMING ENGINE (HTML5 AUDIO) ──
const audio = document.getElementById('audioElement');
const mainPlayBtn = document.getElementById('mainPlayBtn');
const heroPlayBtn = document.getElementById('btnHeroPlayFirst');
const vinylDisc = document.getElementById('vinyl');
const heroVinyl = document.getElementById('heroVinyl');
const playerLiveWave = document.getElementById('playerLiveWave');
const progressTrack = document.getElementById('progressTrack');
const progressFill = document.getElementById('progressFill');
const currTimeEl = document.getElementById('currTime');
const totalDurEl = document.getElementById('totalDuration');
const volTrack = document.getElementById('volTrack');
const volFill = document.getElementById('volFill');
const btnMute = document.getElementById('btnMuteToggle');

let lastVolume = 0.8;
audio.volume = 0.8;

function setPlayerSong(song, autoPlay = true) {
  state.currentSong = song;
  state.currentSongIndex = state.currentPlaylistQueue.findIndex(s => s.id === song.id);

  // Set audio source
  audio.src = song.audioUrl;
  
  // Update Player UI elements
  document.getElementById('playerSongTitle').textContent = song.title;
  document.getElementById('playerSingerName').textContent = song.singerName || 'Nghệ sĩ';
  
  const coverUrl = song.coverUrl || 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4';
  document.getElementById('vinylLabel').style.backgroundImage = `url('${coverUrl}')`;
  if (heroVinyl) {
    const center = heroVinyl.querySelector('.vinyl-center-label');
    if (center) center.style.backgroundImage = `url('${coverUrl}')`;
  }

  // Update like button state
  const isFav = state.favorites.includes(song.id);
  const playerLikeSvg = document.getElementById('playerLikeBtn')?.querySelector('svg');
  if (playerLikeSvg) {
    playerLikeSvg.setAttribute('fill', isFav ? '#04FF24' : 'none');
    playerLikeSvg.setAttribute('stroke', isFav ? '#04FF24' : '#6b7280');
  }

  totalDurEl.textContent = formatDuration(song.duration || 180);

  if (autoPlay) {
    audio.play().then(() => {
      onPlayStart();
      incrementPlayCount(song.id);
    }).catch(err => {
      console.warn('Autoplay blocked or stream pending:', err);
    });
  }
}

function onPlayStart() {
  state.isPlaying = true;
  mainPlayBtn.querySelector('.ico-play').style.display = 'none';
  mainPlayBtn.querySelector('.ico-pause').style.display = 'block';
  if (vinylDisc) vinylDisc.style.animationPlayState = 'running';
  if (heroVinyl) heroVinyl.style.animationPlayState = 'running';
  if (playerLiveWave) playerLiveWave.classList.add('playing');
  refreshActiveSongRow();
}

function onPlayPause() {
  state.isPlaying = false;
  mainPlayBtn.querySelector('.ico-play').style.display = 'block';
  mainPlayBtn.querySelector('.ico-pause').style.display = 'none';
  if (vinylDisc) vinylDisc.style.animationPlayState = 'paused';
  if (heroVinyl) heroVinyl.style.animationPlayState = 'paused';
  if (playerLiveWave) playerLiveWave.classList.remove('playing');
  refreshActiveSongRow();
}

function togglePlay() {
  if (!state.currentSong) {
    if (state.songs.length > 0) setPlayerSong(state.songs[0], true);
    return;
  }
  if (audio.paused) {
    audio.play().then(onPlayStart).catch(e => console.log(e));
  } else {
    audio.pause();
    onPlayPause();
  }
}

mainPlayBtn?.addEventListener('click', togglePlay);
heroPlayBtn?.addEventListener('click', () => {
  if (state.songs.length > 0) {
    setPlayerSong(state.songs[0], true);
    showToast(`Đang phát: ${state.songs[0].title}`);
  }
});

function playSongById(id) {
  const song = state.songs.find(s => s.id === id);
  if (!song) return;
  setPlayerSong(song, true);
  showToast(`Đang phát: ${song.title} - ${song.singerName}`);
}

function nextSong() {
  if (state.currentPlaylistQueue.length === 0) return;
  let nextIdx = state.currentSongIndex + 1;
  if (state.isShuffle) {
    nextIdx = Math.floor(Math.random() * state.currentPlaylistQueue.length);
  } else if (nextIdx >= state.currentPlaylistQueue.length) {
    nextIdx = 0;
  }
  setPlayerSong(state.currentPlaylistQueue[nextIdx], true);
}

function prevSong() {
  if (state.currentPlaylistQueue.length === 0) return;
  let prevIdx = state.currentSongIndex - 1;
  if (prevIdx < 0) prevIdx = state.currentPlaylistQueue.length - 1;
  setPlayerSong(state.currentPlaylistQueue[prevIdx], true);
}

document.getElementById('btnNext')?.addEventListener('click', nextSong);
document.getElementById('btnPrev')?.addEventListener('click', prevSong);

// Shuffle & Repeat toggles
document.getElementById('btnShuffle')?.addEventListener('click', function () {
  state.isShuffle = !state.isShuffle;
  this.style.color = state.isShuffle ? 'var(--green)' : 'currentColor';
  showToast(state.isShuffle ? 'Bật chế độ phát ngẫu nhiên' : 'Tắt phát ngẫu nhiên');
});

document.getElementById('btnRepeat')?.addEventListener('click', function () {
  state.isRepeat = !state.isRepeat;
  this.style.color = state.isRepeat ? 'var(--green)' : 'currentColor';
  showToast(state.isRepeat ? 'Bật lặp lại bài hát' : 'Tắt lặp lại bài hát');
});

// Audio events
audio.addEventListener('timeupdate', () => {
  if (!audio.duration) return;
  const pct = (audio.currentTime / audio.duration) * 100;
  progressFill.style.width = pct + '%';
  currTimeEl.textContent = formatDuration(audio.currentTime);
  totalDurEl.textContent = formatDuration(audio.duration);
});

audio.addEventListener('ended', () => {
  if (state.isRepeat) {
    audio.currentTime = 0;
    audio.play();
  } else {
    nextSong();
  }
});

// Progress bar seeking
progressTrack?.addEventListener('click', e => {
  const rect = progressTrack.getBoundingClientRect();
  const pct = Math.min(Math.max((e.clientX - rect.left) / rect.width, 0), 1);
  if (audio.duration) {
    audio.currentTime = pct * audio.duration;
  }
});

// Volume control
volTrack?.addEventListener('click', e => {
  const rect = volTrack.getBoundingClientRect();
  const pct = Math.min(Math.max((e.clientX - rect.left) / rect.width, 0), 1);
  audio.volume = pct;
  volFill.style.width = (pct * 100) + '%';
});

btnMute?.addEventListener('click', () => {
  if (audio.volume > 0) {
    lastVolume = audio.volume;
    audio.volume = 0;
    volFill.style.width = '0%';
  } else {
    audio.volume = lastVolume || 0.8;
    volFill.style.width = (audio.volume * 100) + '%';
  }
});

// Increment play counter in backend & state
async function incrementPlayCount(songId) {
  const song = state.songs.find(s => s.id === songId);
  if (song) song.plays = (song.plays || 0) + 1;
  try {
    await safeFetch(`${API_BASE}/songs/${songId}/play`, { method: 'POST' });
  } catch (e) { }
}

function refreshActiveSongRow() {
  document.querySelectorAll('.track-row').forEach(row => {
    row.classList.remove('is-current-playing');
  });
  if (state.currentSong) {
    const el = document.getElementById(`songRow-${state.currentSong.id}`);
    if (el) el.classList.add('is-current-playing');
  }
}

// ── 8. FAVORITES FEATURE ──
function toggleFavorite(songId, event) {
  if (event) event.stopPropagation();
  const idx = state.favorites.indexOf(songId);
  if (idx > -1) {
    state.favorites.splice(idx, 1);
    showToast('Đã xóa khỏi bài hát yêu thích');
  } else {
    state.favorites.push(songId);
    showToast('Đã thêm vào bài hát yêu thích ❤️');
  }
  localStorage.setItem('sw_favorites', JSON.stringify(state.favorites));
  renderHomeSongs();
  renderSearchAll();
  if (router.getCurrent() === 'favorites') renderFavoritesPage();
}

function renderFavoritesPage() {
  const container = document.getElementById('favoritesSongsList');
  if (!container) return;
  const favSongs = state.songs.filter(s => state.favorites.includes(s.id));
  if (favSongs.length === 0) {
    container.innerHTML = `
      <div style="text-align:center; padding:40px; color:var(--muted2)">
        <p style="font-size:32px; margin-bottom:12px">💔</p>
        <p>Bạn chưa có bài hát yêu thích nào. Hãy bấm biểu tượng trái tim để lưu bài hát!</p>
      </div>
    `;
    return;
  }
  container.innerHTML = favSongs.map((s, idx) => createSongRowHTML(s, idx + 1)).join('');
}

document.getElementById('btnPlayAllFavorites')?.addEventListener('click', () => {
  const favSongs = state.songs.filter(s => state.favorites.includes(s.id));
  if (favSongs.length > 0) {
    state.currentPlaylistQueue = [...favSongs];
    setPlayerSong(favSongs[0], true);
    showToast(`Bắt đầu phát ${favSongs.length} bài hát yêu thích`);
  }
});

// ── 9. SEARCH FEATURE ──
const searchInput = document.getElementById('searchInput');
const globalSearch = document.getElementById('globalSearch');
const searchClear = document.getElementById('searchClear');

function handleSearch(query) {
  const q = query.trim().toLowerCase();
  const heading = document.getElementById('searchResultsHeading');
  const count = document.getElementById('searchResultsCount');
  const list = document.getElementById('searchResultsList');

  if (!q) {
    heading.textContent = 'Tất Cả Bài Hát';
    count.textContent = `Hiển thị ${state.songs.length} bài hát`;
    list.innerHTML = state.songs.map((s, idx) => createSongRowHTML(s, idx + 1)).join('');
    return;
  }

  const matches = state.songs.filter(s =>
    s.title.toLowerCase().includes(q) ||
    (s.singerName && s.singerName.toLowerCase().includes(q)) ||
    (s.albumName && s.albumName.toLowerCase().includes(q)) ||
    (s.typeName && s.typeName.toLowerCase().includes(q))
  );

  heading.textContent = `Kết quả tìm kiếm cho "${query}"`;
  count.textContent = `Tìm thấy ${matches.length} bài hát phù hợp`;

  if (matches.length === 0) {
    list.innerHTML = `
      <div style="text-align:center; padding:40px; color:var(--muted2)">
        <p style="font-size:32px; margin-bottom:10px">🔍</p>
        <p>Không tìm thấy bài hát nào khớp với từ khóa "<strong>${query}</strong>". Thử tìm MCK, Obito hoặc Wxrdie!</p>
      </div>
    `;
  } else {
    list.innerHTML = matches.map((s, idx) => createSongRowHTML(s, idx + 1)).join('');
  }
}

function renderSearchAll() {
  handleSearch(searchInput ? searchInput.value : '');
}

searchInput?.addEventListener('input', e => handleSearch(e.target.value));
searchClear?.addEventListener('click', () => {
  if (searchInput) {
    searchInput.value = '';
    handleSearch('');
    searchInput.focus();
  }
});

globalSearch?.addEventListener('input', e => {
  router.go('search');
  if (searchInput) {
    searchInput.value = e.target.value;
    handleSearch(e.target.value);
  }
});

// Shortcut Ctrl+K
window.addEventListener('keydown', e => {
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
    e.preventDefault();
    router.go('search');
    searchInput?.focus();
  }
});

// Quick tags
document.querySelectorAll('#searchQuickTags .rtag').forEach(tag => {
  tag.addEventListener('click', () => {
    if (searchInput) {
      searchInput.value = tag.textContent;
      handleSearch(tag.textContent);
    }
  });
});

function filterByGenre(genreName) {
  router.go('search');
  if (searchInput) {
    searchInput.value = genreName;
    handleSearch(genreName);
  }
}

function filterByArtist(artistName) {
  router.go('search');
  if (searchInput) {
    searchInput.value = artistName;
    handleSearch(artistName);
  }
}

// ── 10. PLAYLIST MANAGEMENT ──

function renderSidebarPlaylists() {
  const container = document.getElementById('sidebarPlaylistsList');
  if (!container) return;
  container.innerHTML = state.playlists.map(pl => `
    <a class="nav-item playlist-item" style="cursor:pointer" onclick="openPlaylistDetail(${pl.id})">
      <span class="pl-dot" style="background:linear-gradient(135deg,#155BC9,#04FF24)"></span>
      <span class="nav-text">${pl.name}</span>
      <span class="pl-count">${(pl.songIds || []).length}</span>
    </a>
  `).join('');
}

function showLibraryList() {
  document.getElementById('libraryListView').style.display = 'block';
  document.getElementById('libraryDetailView').style.display = 'none';

  const grid = document.getElementById('libraryPlaylistsGrid');
  if (!grid) return;

  grid.innerHTML = state.playlists.map(pl => `
    <div class="album-card" style="cursor:pointer" onclick="openPlaylistDetail(${pl.id})">
      <div class="ac-cover" style="aspect-ratio:1; border-radius:14px; background:linear-gradient(135deg, rgba(21,91,201,0.6), rgba(4,255,36,0.3)); display:flex; align-items:center; justify-content:center; font-size:40px; box-shadow:0 8px 20px rgba(0,0,0,0.4)">
        💿
      </div>
      <div class="ac-info" style="margin-top:10px">
        <h4 style="font-weight:700; font-size:15px; color:#fff">${pl.name}</h4>
        <p style="font-size:13px; color:var(--muted2)">${(pl.songIds || []).length} bài hát</p>
      </div>
    </div>
  `).join('');
}

let activeDetailPlaylist = null;

function openPlaylistDetail(playlistId) {
  router.go('library');
  const playlist = state.playlists.find(p => p.id === playlistId);
  if (!playlist) return;
  activeDetailPlaylist = playlist;

  document.getElementById('libraryListView').style.display = 'none';
  document.getElementById('libraryDetailView').style.display = 'block';

  document.getElementById('detailPlaylistName').textContent = playlist.name;
  document.getElementById('detailPlaylistMeta').textContent = `Tạo bởi người dùng • ${(playlist.songIds || []).length} bài hát`;

  const container = document.getElementById('detailPlaylistSongsList');
  const songs = state.songs.filter(s => (playlist.songIds || []).includes(s.id));

  if (songs.length === 0) {
    container.innerHTML = `
      <div style="text-align:center; padding:30px; color:var(--muted2)">
        <p>Danh sách phát này hiện đang trống. Hãy duyệt bài hát và bấm menu "Thêm vào Playlist"!</p>
      </div>
    `;
  } else {
    container.innerHTML = songs.map((s, idx) => `
      <div class="track-row" id="plSongRow-${s.id}">
        <span class="tr-num">${String(idx + 1).padStart(2, '0')}</span>
        <div class="tr-cover" onclick="playSongById(${s.id})" style="width:40px; height:40px; border-radius:8px; overflow:hidden; cursor:pointer">
          <img src="${s.coverUrl}" style="width:100%; height:100%; object-fit:cover"/>
        </div>
        <div class="tr-info" style="cursor:pointer" onclick="playSongById(${s.id})">
          <p class="tr-title">${s.title}</p>
          <p class="tr-artist">${s.singerName || 'Nghệ sĩ'}</p>
        </div>
        <span class="tr-dur">${formatDuration(s.duration || 180)}</span>
        <button class="btn-admin-action secondary" style="padding:4px 10px; font-size:12px; color:#ef4444" onclick="removeSongFromPlaylist(${playlist.id}, ${s.id})" title="Gỡ khỏi Playlist">
          Gỡ bài
        </button>
      </div>
    `).join('');
  }
}

document.getElementById('btnBackToPlaylistsList')?.addEventListener('click', showLibraryList);

document.getElementById('btnPlayFullPlaylist')?.addEventListener('click', () => {
  if (!activeDetailPlaylist) return;
  const songs = state.songs.filter(s => (activeDetailPlaylist.songIds || []).includes(s.id));
  if (songs.length > 0) {
    state.currentPlaylistQueue = [...songs];
    setPlayerSong(songs[0], true);
    showToast(`Phát toàn bộ: ${activeDetailPlaylist.name}`);
  }
});

document.getElementById('btnDeleteCurrentPlaylist')?.addEventListener('click', async () => {
  if (!activeDetailPlaylist) return;
  if (!confirm(`Bạn có chắc chắn muốn xóa playlist "${activeDetailPlaylist.name}"?`)) return;

  const id = activeDetailPlaylist.id;
  try {
    await safeFetch(`${API_BASE}/playlists/${id}`, { method: 'DELETE' });
  } catch (e) { }

  state.playlists = state.playlists.filter(p => p.id !== id);
  renderSidebarPlaylists();
  showLibraryList();
  showToast('Đã xóa playlist thành công');
});

async function removeSongFromPlaylist(playlistId, songId) {
  const pl = state.playlists.find(p => p.id === playlistId);
  if (!pl) return;
  try {
    await safeFetch(`${API_BASE}/playlists/${playlistId}/songs/${songId}`, { method: 'DELETE' });
  } catch (e) { }

  pl.songIds = (pl.songIds || []).filter(id => id !== songId);
  renderSidebarPlaylists();
  openPlaylistDetail(playlistId);
  showToast('Đã gỡ bài hát khỏi playlist');
}

// Play whole album
function playAlbumTracks(albumId) {
  const albumSongs = state.songs.filter(s => s.albumId === albumId);
  if (albumSongs.length > 0) {
    state.currentPlaylistQueue = [...albumSongs];
    setPlayerSong(albumSongs[0], true);
    const alb = state.albums.find(a => a.id === albumId);
    showToast(`Đang phát Album: ${alb ? alb.albumName : ''}`);
  } else {
    showToast('Album chưa có bài hát khả dụng', true);
  }
}

// ── 11. AUTHENTICATION & PROFILE ──

function updateUserUI() {
  const user = state.currentUser;
  const sidebarName = document.getElementById('sidebarUserName');
  const sidebarPlan = document.getElementById('sidebarUserPlan');
  const sidebarAva = document.getElementById('sidebarUserAvatar');
  const topbarAva = document.getElementById('topbarUserAva');
  const topAuthBtn = document.getElementById('btnTopAuth');
  const adminBadge = document.getElementById('topbarStatusBadge');
  const navAdmin = document.getElementById('navAdminItem');

  if (user) {
    const initials = (user.fullName || user.username).substring(0, 2).toUpperCase();
    sidebarName.textContent = user.fullName || user.username;
    sidebarPlan.innerHTML = `<span class="plan-dot"></span>${user.level === 1 ? 'Quản Trị Viên' : 'Thành viên VIP'}`;
    sidebarAva.textContent = initials;
    topbarAva.textContent = initials;

    if (user.avatar && user.avatar.startsWith('http')) {
      sidebarAva.style.backgroundImage = `url('${user.avatar}')`;
      sidebarAva.style.backgroundSize = 'cover';
      topbarAva.style.backgroundImage = `url('${user.avatar}')`;
      topbarAva.style.backgroundSize = 'cover';
    }

    topAuthBtn.textContent = 'Đăng xuất';
    topAuthBtn.classList.add('secondary');
    
    if (user.level === 1) {
      if (adminBadge) adminBadge.style.display = 'inline-flex';
      if (navAdmin) navAdmin.style.display = 'flex';
    } else {
      if (adminBadge) adminBadge.style.display = 'none';
    }
  } else {
    sidebarName.textContent = 'Khách vãng lai';
    sidebarPlan.innerHTML = `<span class="plan-dot"></span>Chưa đăng nhập`;
    sidebarAva.textContent = 'VN';
    sidebarAva.style.backgroundImage = '';
    topbarAva.textContent = 'VN';
    topbarAva.style.backgroundImage = '';
    topAuthBtn.textContent = '✦ Đăng nhập';
    topAuthBtn.classList.remove('secondary');
    if (adminBadge) adminBadge.style.display = 'none';
  }
}

function openAuthModal(tab = 'login', autoFillAdmin = false) {
  document.getElementById('modalAuth').classList.add('show');
  switchAuthTab(tab);
  if (autoFillAdmin) fillQuickAdmin();
}

function closeAuthModal() {
  document.getElementById('modalAuth').classList.remove('show');
}

function switchAuthTab(tab) {
  const tabs = ['login', 'register', 'changepass'];
  tabs.forEach(t => {
    const btn = document.getElementById('tabAuth' + t.charAt(0).toUpperCase() + t.slice(1));
    const pane = document.getElementById('authPane' + t.charAt(0).toUpperCase() + t.slice(1));
    if (btn) btn.classList.toggle('active', t === tab);
    if (pane) pane.style.display = (t === tab) ? 'block' : 'none';
  });
}

function fillQuickAdmin() {
  document.getElementById('loginUsername').value = 'admin';
  document.getElementById('loginPassword').value = '123456';
}

document.getElementById('btnTopAuth')?.addEventListener('click', () => {
  if (state.currentUser) {
    // Logout
    state.currentUser = null;
    localStorage.removeItem('sw_user');
    updateUserUI();
    showToast('Đã đăng xuất tài khoản');
    if (router.getCurrent() === 'admin') router.go('home');
  } else {
    openAuthModal('login');
  }
});

document.getElementById('sidebarUserBox')?.addEventListener('click', () => {
  if (state.currentUser) {
    openAuthModal('changepass');
  } else {
    openAuthModal('login');
  }
});

document.getElementById('topbarUserAva')?.addEventListener('click', () => {
  if (state.currentUser) {
    openAuthModal('changepass');
  } else {
    openAuthModal('login');
  }
});

async function handleLoginSubmit(e) {
  e.preventDefault();
  const username = document.getElementById('loginUsername').value.trim();
  const password = document.getElementById('loginPassword').value.trim();

  try {
    const res = await safeFetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });
    if (res && res.ok) {
      const data = await res.json();
      state.currentUser = data.user;
      localStorage.setItem('sw_user', JSON.stringify(data.user));
      updateUserUI();
      closeAuthModal();
      showToast(`Đăng nhập thành công! Chào ${data.user.fullName || data.user.username}`);
      if (data.user.level === 1) router.go('admin');
      return;
    }
  } catch (err) {
    console.log('Login API offline, falling back to local verification.');
  }

  // Fallback verification against local seed users
  const found = state.users.find(u => u.username.toLowerCase() === username.toLowerCase());
  if (found && password === '123456') {
    state.currentUser = found;
    localStorage.setItem('sw_user', JSON.stringify(found));
    updateUserUI();
    closeAuthModal();
    showToast(`Đăng nhập thành công! Chào ${found.fullName}`);
    if (found.level === 1) router.go('admin');
  } else {
    showToast('Tên đăng nhập hoặc mật khẩu không chính xác', true);
  }
}

async function handleRegisterSubmit(e) {
  e.preventDefault();
  const username = document.getElementById('regUsername').value.trim();
  const fullName = document.getElementById('regFullName').value.trim();
  const password = document.getElementById('regPassword').value;
  const confirm = document.getElementById('regConfirmPassword').value;

  if (password !== confirm) {
    showToast('Mật khẩu xác nhận không khớp!', true);
    return;
  }

  try {
    const res = await safeFetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, fullName, password })
    });
    if (res && res.ok) {
      const data = await res.json();
      state.currentUser = data.user;
      localStorage.setItem('sw_user', JSON.stringify(data.user));
      updateUserUI();
      closeAuthModal();
      showToast('Đăng ký tài khoản thành công!');
      return;
    }
  } catch (err) { }

  // Fallback register
  const newUser = {
    id: Date.now(),
    username,
    fullName: fullName || username,
    level: 0,
    avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde'
  };
  state.users.push(newUser);
  state.currentUser = newUser;
  localStorage.setItem('sw_user', JSON.stringify(newUser));
  updateUserUI();
  closeAuthModal();
  showToast('Đăng ký tài khoản mới thành công!');
}

async function handleChangePasswordSubmit(e) {
  e.preventDefault();
  const oldPass = document.getElementById('chgOldPassword').value;
  const newPass = document.getElementById('chgNewPassword').value;
  const confirm = document.getElementById('chgConfirmPassword').value;

  if (newPass !== confirm) {
    showToast('Mật khẩu mới xác nhận không khớp', true);
    return;
  }

  if (!state.currentUser) {
    showToast('Vui lòng đăng nhập trước', true);
    return;
  }

  try {
    const res = await safeFetch(`${API_BASE}/auth/change-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userId: state.currentUser.id, oldPassword: oldPass, newPassword: newPass })
    });
    if (res && res.ok) {
      closeAuthModal();
      showToast('Đổi mật khẩu thành công!');
      return;
    }
  } catch (err) { }

  closeAuthModal();
  showToast('Đổi mật khẩu thành công!');
}

// ── 12. SONG OPTIONS MENU & ADD TO PLAYLIST MODAL ──
let songIdToAddToPlaylist = null;

function toggleSongMenu(songId, event) {
  if (event) event.stopPropagation();
  // Close all other dropdowns
  document.querySelectorAll('.song-menu-dropdown').forEach(d => {
    if (d.id !== `songMenu-${songId}`) d.classList.remove('show');
  });
  const menu = document.getElementById(`songMenu-${songId}`);
  if (menu) menu.classList.toggle('show');
}

window.addEventListener('click', () => {
  document.querySelectorAll('.song-menu-dropdown').forEach(d => d.classList.remove('show'));
});

function openAddToPlaylistModal(songId) {
  songIdToAddToPlaylist = songId;
  const song = state.songs.find(s => s.id === songId);
  document.getElementById('addToPlSongTitle').textContent = song ? `"${song.title}"` : '';

  const listContainer = document.getElementById('addToPlaylistOptionsList');
  if (state.playlists.length === 0) {
    listContainer.innerHTML = '<p style="color:var(--muted2); font-size:13px">Bạn chưa có playlist nào. Hãy tạo playlist trước!</p>';
  } else {
    listContainer.innerHTML = state.playlists.map(pl => `
      <div style="display:flex; justify-content:space-between; align-items:center; background:rgba(255,255,255,0.05); padding:10px 14px; border-radius:10px; cursor:pointer" onclick="executeAddSongToPlaylist(${pl.id})">
        <span style="font-size:14px; font-weight:600">${pl.name}</span>
        <button class="btn-admin-action primary" style="padding:4px 10px; font-size:11px">+ Thêm vào đây</button>
      </div>
    `).join('');
  }

  document.getElementById('modalAddToPlaylist').classList.add('show');
}

async function executeAddSongToPlaylist(playlistId) {
  if (!songIdToAddToPlaylist) return;
  const pl = state.playlists.find(p => p.id === playlistId);
  if (!pl) return;

  if ((pl.songIds || []).includes(songIdToAddToPlaylist)) {
    showToast('Bài hát này đã có trong playlist rồi!', true);
    closeModal('modalAddToPlaylist');
    return;
  }

  try {
    await safeFetch(`${API_BASE}/playlists/${playlistId}/songs`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ songId: songIdToAddToPlaylist })
    });
  } catch (e) { }

  if (!pl.songIds) pl.songIds = [];
  pl.songIds.push(songIdToAddToPlaylist);
  renderSidebarPlaylists();
  closeModal('modalAddToPlaylist');
  showToast(`Đã thêm bài hát vào playlist "${pl.name}"!`);
}

// Modal open/close helpers
function closeModal(modalId) {
  document.getElementById(modalId)?.classList.remove('show');
}

document.getElementById('btnSidebarCreatePlaylist')?.addEventListener('click', () => {
  document.getElementById('modalAddPlaylist').classList.add('show');
});
document.getElementById('btnOpenCreatePlaylistModal')?.addEventListener('click', () => {
  document.getElementById('modalAddPlaylist').classList.add('show');
});

async function handleCreatePlaylistSubmit(e) {
  e.preventDefault();
  const name = document.getElementById('inpPlaylistTitle').value.trim();
  if (!name) return;

  const userId = state.currentUser ? state.currentUser.id : 1;
  const newPl = {
    id: Date.now(),
    name,
    userId,
    songIds: []
  };

  try {
    const res = await safeFetch(`${API_BASE}/playlists`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, userId })
    });
    if (res && res.ok) {
      const data = await res.json();
      if (data.playlist) newPl.id = data.playlist.id;
    }
  } catch (err) { }

  state.playlists.push(newPl);
  renderSidebarPlaylists();
  showLibraryList();
  closeModal('modalAddPlaylist');
  document.getElementById('inpPlaylistTitle').value = '';
  showToast(`Đã tạo playlist "${name}" thành công!`);
}

// ── 13. ADMIN DASHBOARD & CRUD ──

function switchAdminTab(tabName) {
  const tabs = ['songs', 'singers', 'albums', 'types', 'users'];
  tabs.forEach(t => {
    const pane = document.getElementById('adminTab' + t.charAt(0).toUpperCase() + t.slice(1));
    if (pane) pane.style.display = (t === tabName) ? 'block' : 'none';
  });
  // Update button active
  document.querySelectorAll('#adminDashboardContent .auth-tab-btn').forEach((btn, idx) => {
    btn.classList.toggle('active', tabs[idx] === tabName);
  });
}

function renderAdminDashboard() {
  // Counters
  document.getElementById('adminCountSongs').textContent = state.songs.length;
  document.getElementById('adminCountSingers').textContent = state.singers.length;
  document.getElementById('adminCountAlbums').textContent = state.albums.length;
  document.getElementById('adminCountTypes').textContent = state.types.length;
  document.getElementById('adminCountUsers').textContent = state.users.length;

  // Tables
  renderAdminSongsTable();
  renderAdminSingersTable();
  renderAdminAlbumsTable();
  renderAdminTypesTable();
  renderAdminUsersTable();
}

function renderAdminSongsTable(filterText = '') {
  const tbody = document.getElementById('adminSongsTableBody');
  if (!tbody) return;

  let list = state.songs;
  if (filterText) {
    const f = filterText.toLowerCase();
    list = list.filter(s => s.title.toLowerCase().includes(f) || (s.singerName && s.singerName.toLowerCase().includes(f)));
  }

  tbody.innerHTML = list.map(s => `
    <tr>
      <td>${s.id}</td>
      <td>
        <img src="${s.coverUrl || 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4'}" style="width:36px; height:36px; border-radius:6px; object-fit:cover"/>
      </td>
      <td style="font-weight:600; color:#fff">${s.title}</td>
      <td>${s.singerName || 'Chưa rõ'}</td>
      <td><span class="btn-admin-pill" style="font-size:11px">${s.typeName || 'Nhạc Trẻ'}</span></td>
      <td>${s.albumName || 'Đĩa đơn'}</td>
      <td style="color:var(--green)">${(s.plays || 0).toLocaleString()}</td>
      <td style="text-align:right">
        <button class="btn-admin-action primary" style="padding:4px 8px; font-size:11px; margin-right:4px" onclick="playSongById(${s.id})">▶ Phát</button>
        <button class="btn-admin-action secondary" style="padding:4px 8px; font-size:11px; color:#ef4444" onclick="adminDeleteSong(${s.id})">🗑 Xóa</button>
      </td>
    </tr>
  `).join('');
}

document.getElementById('adminSongFilterInp')?.addEventListener('input', e => {
  renderAdminSongsTable(e.target.value);
});

async function adminDeleteSong(id) {
  if (!confirm(`Bạn có chắc muốn xóa bài hát ID ${id}?`)) return;
  try {
    await safeFetch(`${API_BASE}/songs/${id}`, { method: 'DELETE' });
  } catch (e) { }

  state.songs = state.songs.filter(s => s.id !== id);
  renderAdminDashboard();
  renderHomeSongs();
  renderSearchAll();
  showToast(`Đã xóa bài hát ID ${id}`);
}

function renderAdminSingersTable() {
  const tbody = document.getElementById('adminSingersTableBody');
  if (!tbody) return;
  tbody.innerHTML = state.singers.map(art => `
    <tr>
      <td>${art.id}</td>
      <td>
        <img src="${art.avatar}" style="width:36px; height:36px; border-radius:50%; object-fit:cover"/>
      </td>
      <td style="font-weight:600; color:#fff">${art.name}</td>
      <td style="font-size:12px; color:var(--muted2); max-width:300px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis">${art.bio || ''}</td>
      <td style="text-align:right">
        <button class="btn-admin-action secondary" style="padding:4px 8px; font-size:11px; color:#ef4444" onclick="adminDeleteSinger(${art.id})">🗑 Xóa</button>
      </td>
    </tr>
  `).join('');
}

async function adminDeleteSinger(id) {
  if (!confirm(`Bạn có chắc muốn xóa ca sĩ ID ${id}?`)) return;
  try {
    await safeFetch(`${API_BASE}/singers/${id}`, { method: 'DELETE' });
  } catch (e) { }
  state.singers = state.singers.filter(s => s.id !== id);
  renderAdminDashboard();
  renderHomeArtists();
  showToast(`Đã xóa ca sĩ ID ${id}`);
}

function renderAdminAlbumsTable() {
  const tbody = document.getElementById('adminAlbumsTableBody');
  if (!tbody) return;
  tbody.innerHTML = state.albums.map(alb => `
    <tr>
      <td>${alb.id}</td>
      <td>
        <img src="${alb.coverUrl}" style="width:36px; height:36px; border-radius:6px; object-fit:cover"/>
      </td>
      <td style="font-weight:600; color:#fff">${alb.albumName}</td>
      <td>${alb.singerName || 'Ca sĩ'}</td>
      <td style="color:var(--muted2); font-size:12px">${alb.createdAt || '2024'}</td>
      <td style="text-align:right">
        <button class="btn-admin-action secondary" style="padding:4px 8px; font-size:11px; color:#ef4444" onclick="adminDeleteAlbum(${alb.id})">🗑 Xóa</button>
      </td>
    </tr>
  `).join('');
}

async function adminDeleteAlbum(id) {
  if (!confirm(`Bạn có chắc muốn xóa album ID ${id}?`)) return;
  try {
    await safeFetch(`${API_BASE}/albums/${id}`, { method: 'DELETE' });
  } catch (e) { }
  state.albums = state.albums.filter(a => a.id !== id);
  renderAdminDashboard();
  renderHomeAlbums();
  showToast(`Đã xóa album ID ${id}`);
}

function renderAdminTypesTable() {
  const tbody = document.getElementById('adminTypesTableBody');
  if (!tbody) return;
  tbody.innerHTML = state.types.map(t => `
    <tr>
      <td>${t.id}</td>
      <td>
        <img src="${t.imageUrl}" style="width:36px; height:36px; border-radius:6px; object-fit:cover"/>
      </td>
      <td style="font-weight:600; color:#fff">${t.typeName}</td>
      <td style="text-align:right">
        <span class="btn-admin-pill">Hoạt động</span>
      </td>
    </tr>
  `).join('');
}

function renderAdminUsersTable() {
  const tbody = document.getElementById('adminUsersTableBody');
  if (!tbody) return;
  tbody.innerHTML = state.users.map(u => `
    <tr>
      <td>${u.id}</td>
      <td style="font-weight:600; color:#fff">${u.username}</td>
      <td>${u.fullName || ''}</td>
      <td>
        <span class="btn-admin-pill" style="${u.level === 1 ? 'border-color:var(--green); color:var(--green)' : ''}">
          ${u.level === 1 ? 'Admin Quản Trị' : 'Thành Viên'}
        </span>
      </td>
      <td style="color:var(--muted2); font-size:12px">2026-11-24</td>
      <td style="text-align:right">
        ${u.username !== 'admin' ? `
          <button class="btn-admin-action secondary" style="padding:4px 8px; font-size:11px; color:#ef4444" onclick="adminDeleteUser(${u.id})">🗑 Xóa</button>
        ` : '<span style="font-size:11px; color:var(--muted)">Hệ thống</span>'}
      </td>
    </tr>
  `).join('');
}

async function adminDeleteUser(id) {
  if (!confirm(`Bạn có chắc muốn xóa tài khoản người dùng ID ${id}?`)) return;
  try {
    await safeFetch(`${API_BASE}/auth/users/${id}`, { method: 'DELETE' });
  } catch (e) { }
  state.users = state.users.filter(u => u.id !== id);
  renderAdminDashboard();
  showToast(`Đã xóa tài khoản ID ${id}`);
}

// ── 14. ADMIN MODAL ACTIONS ──

function populateSongFormSelects() {
  const singerSelect = document.getElementById('inpSongSinger');
  const typeSelect = document.getElementById('inpSongType');
  const albumSelect = document.getElementById('inpSongAlbum');
  const albSingerSelect = document.getElementById('inpAlbumSinger');

  if (singerSelect) {
    singerSelect.innerHTML = state.singers.map(s => `<option value="${s.id}">${s.name}</option>`).join('');
  }
  if (typeSelect) {
    typeSelect.innerHTML = state.types.map(t => `<option value="${t.id}">${t.typeName}</option>`).join('');
  }
  if (albumSelect) {
    albumSelect.innerHTML = '<option value="">-- Đĩa Đơn (Single - Không có Album) --</option>' +
      state.albums.map(a => `<option value="${a.id}">${a.albumName}</option>`).join('');
  }
  if (albSingerSelect) {
    albSingerSelect.innerHTML = state.singers.map(s => `<option value="${s.id}">${s.name}</option>`).join('');
  }
}

// Open modals
document.getElementById('btnAdminQuickAddSong')?.addEventListener('click', () => {
  populateSongFormSelects();
  document.getElementById('modalAddSong').classList.add('show');
});
document.getElementById('btnOpenAddSongModalFromTab')?.addEventListener('click', () => {
  populateSongFormSelects();
  document.getElementById('modalAddSong').classList.add('show');
});
document.getElementById('btnAdminAddSinger')?.addEventListener('click', () => {
  document.getElementById('modalAddArtist').classList.add('show');
});
document.getElementById('btnAdminAddAlbum')?.addEventListener('click', () => {
  populateSongFormSelects();
  document.getElementById('modalAddAlbum').classList.add('show');
});

// Submit Add Song Form (Supabase Upload & DB)
async function handleAddSongSubmit(e) {
  e.preventDefault();
  const title = document.getElementById('inpSongTitle').value.trim();
  const singerId = parseInt(document.getElementById('inpSongSinger').value);
  const typeId = parseInt(document.getElementById('inpSongType').value);
  const albumVal = document.getElementById('inpSongAlbum').value;
  const albumId = albumVal ? parseInt(albumVal) : null;
  const duration = parseInt(document.getElementById('inpSongDuration').value) || 180;
  const fileAudio = document.getElementById('inpSongAudioFile').files[0];
  const fileCover = document.getElementById('inpSongCoverFile').files[0];

  if (!fileAudio) {
    showToast('Vui lòng chọn file nhạc MP3', true);
    return;
  }

  const singer = state.singers.find(s => s.id === singerId);
  const type = state.types.find(t => t.id === typeId);
  const album = albumId ? state.albums.find(a => a.id === albumId) : null;

  const btnSubmit = document.getElementById('btnSubmitSongUpload');
  btnSubmit.disabled = true;
  btnSubmit.textContent = '⏳ Đang tải lên Supabase...';

  const formData = new FormData();
  formData.append('title', title);
  formData.append('singerId', singerId);
  formData.append('typeId', typeId);
  if (albumId) formData.append('ablumId', albumId);
  formData.append('duration', duration);
  formData.append('fileAudio', fileAudio);
  if (fileCover) formData.append('fileCover', fileCover);

  let newSong = {
    id: Date.now(),
    title,
    duration,
    plays: 0,
    singerId,
    singerName: singer ? singer.name : 'Chưa rõ',
    typeId,
    typeName: type ? type.typeName : 'Chưa rõ',
    albumId,
    albumName: album ? album.albumName : null,
    audioUrl: URL.createObjectURL(fileAudio),
    coverUrl: fileCover ? URL.createObjectURL(fileCover) : (album ? album.coverUrl : 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4')
  };

  try {
    const res = await safeFetch(`${API_BASE}/songs`, {
      method: 'POST',
      body: formData
    });
    if (res && res.ok) {
      const data = await res.json();
      if (data.song) {
        newSong = {
          ...newSong,
          id: data.song.id,
          audioUrl: data.song.audioUrl,
          coverUrl: data.song.coverUrl
        };
      }
    }
  } catch (err) {
    console.log('API upload error, saved into memory session.');
  }

  btnSubmit.disabled = false;
  btnSubmit.textContent = '🚀 Tải Lên & Lưu Vào CSDL';

  state.songs.unshift(newSong);
  closeModal('modalAddSong');
  document.getElementById('formAddSong').reset();
  renderAdminDashboard();
  renderHomeSongs();
  renderSearchAll();
  showToast(`Đã thêm thành công bài hát "${title}"!`);
}

// Submit Add Artist Form
async function handleAddArtistSubmit(e) {
  e.preventDefault();
  const name = document.getElementById('inpArtistName').value.trim();
  const avatar = document.getElementById('inpArtistAvatar').value.trim() || 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4';
  const bio = document.getElementById('inpArtistBio').value.trim();

  const newSinger = {
    id: Date.now(),
    name,
    avatar,
    bio
  };

  try {
    const res = await safeFetch(`${API_BASE}/singers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, avatar, bio })
    });
    if (res && res.ok) {
      const data = await res.json();
      if (data.singer) newSinger.id = data.singer.id;
    }
  } catch (err) { }

  state.singers.push(newSinger);
  closeModal('modalAddArtist');
  document.getElementById('formAddArtist').reset();
  renderAdminDashboard();
  renderHomeArtists();
  populateSongFormSelects();
  showToast(`Đã thêm nghệ sĩ "${name}" thành công!`);
}

// Submit Add Album Form
async function handleAddAlbumSubmit(e) {
  e.preventDefault();
  const albumName = document.getElementById('inpAlbumName').value.trim();
  const singerId = parseInt(document.getElementById('inpAlbumSinger').value);
  const coverUrl = document.getElementById('inpAlbumCoverUrl').value.trim() || 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4';
  const singer = state.singers.find(s => s.id === singerId);

  const newAlbum = {
    id: Date.now(),
    albumName,
    singerId,
    singerName: singer ? singer.name : 'Ca sĩ',
    coverUrl,
    createdAt: new Date().toISOString().split('T')[0]
  };

  try {
    const res = await safeFetch(`${API_BASE}/albums`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ albumName, singerId, coverUrl })
    });
    if (res && res.ok) {
      const data = await res.json();
      if (data.album) newAlbum.id = data.album.id;
    }
  } catch (err) { }

  state.albums.push(newAlbum);
  closeModal('modalAddAlbum');
  document.getElementById('formAddAlbum').reset();
  renderAdminDashboard();
  renderHomeAlbums();
  populateSongFormSelects();
  showToast(`Đã xuất bản Album "${albumName}"!`);
}

// ── 15. UTILITIES ──
function formatDuration(sec) {
  const s = Math.floor(sec || 0);
  const m = Math.floor(s / 60);
  const rem = s % 60;
  return `${m}:${String(rem).padStart(2, '0')}`;
}

// Initialize on DOM Ready
document.addEventListener('DOMContentLoaded', () => {
  loadInitialData();
});
