/* ═══════════════════════════════════════════════════════════
   PermitSystem — Main JS
   ═══════════════════════════════════════════════════════════ */
(function(){
  'use strict';

  const BS = window.__BOOTSTRAP__ || {};
  window.SYSTEM = BS.system || {};
  window.THEME = BS.theme || {};
  window.CU = BS.user || null;
  window.SCREENS = BS.screens || [];
  window.LK = BS.lookups || {};
  window.BR = BS.branches || [];
  window.BL = BS.buildings || [];
  window.CB = window.BR[0]?.Id || null;

  // Apply theme
  if(THEME.colors){
    const root = document.documentElement;
    Object.entries(THEME.colors).forEach(([k,v]) => { if(v) root.style.setProperty('--' + k, v); });
  }

  // Build topbar
  function buildTopbarNav(){
    const el = document.getElementById('topbarNav');
    if(!el) return;
    const top = SCREENS.filter(s => s.Category === 'main').slice(0, 5);
    el.innerHTML = top.map((s, i) =>
      `<button class="tnav ${i===0?'active':''}" data-screen="${s.Key}" onclick="goto('${s.Key}',this)">${s.Icon} ${s.Title}</button>`
    ).join('');
  }

  // Build sidebar
  function buildSidebar(){
    const el = document.getElementById('mainSidebar');
    if(!el) return;
    const groups = SCREENS.reduce((acc, s) => {
      const g = s.GroupName || 'أخرى';
      (acc[g] = acc[g] || []).push(s);
      return acc;
    }, {});
    let html = '';
    Object.entries(groups).forEach(([groupName, items], idx) => {
      html += `<div class="sb-group ${idx===0?'':'collapsed'}">
        <div class="sb-group-header" onclick="this.parentElement.classList.toggle('collapsed')">
          <span>${items[0].Icon} ${groupName}</span><span>▾</span>
        </div>
        <div class="sb-group-body">
          ${items.map(s => `<div class="sb-item" data-screen="${s.Key}" onclick="goto('${s.Key}',null);setSb(this)"><span class="sb-ico">${s.Icon}</span>${s.Title}</div>`).join('')}
        </div>
      </div>`;
    });
    el.innerHTML = html;
  }

  // Update topbar info
  function updateTopbar(){
    if(CU){
      const uname = document.getElementById('topUserName');
      if(uname) uname.textContent = CU.Name || '—';
      const av = document.getElementById('topUserAvatar');
      if(av) av.textContent = (CU.Name||'؟').split(' ').map(w=>w[0]).slice(0,2).join('');
      const rl = document.getElementById('topUserRoleLabel');
      if(rl) rl.textContent = CU.Role || '';
    }
  }

  // Navigation
  window.goto = function(screenKey, btn){
    const content = document.getElementById('mainContent');
    if(!content) return;
    document.querySelectorAll('.tnav').forEach(x => x.classList.remove('active'));
    if(btn) btn.classList.add('active');
    document.querySelectorAll('.sb-item').forEach(s => s.classList.remove('active'));
    const sbItem = document.querySelector(`.sb-item[data-screen="${screenKey}"]`);
    if(sbItem) sbItem.classList.add('active');

    content.innerHTML = '<div style="padding:60px;text-align:center;color:#94a3b8"><div style="font-size:48px;margin-bottom:12px">⏳</div>جاري التحميل...</div>';

    // For now, only dashboard is available
    if(screenKey === 'dashboard'){
      renderDashboard();
    } else {
      content.innerHTML = `<div class="ph"><div><div class="pt">🚧 ${screenKey}</div><div class="ps">قيد التطوير</div></div></div>
        <div class="card" style="padding:60px;text-align:center;color:#94a3b8">
          <div style="font-size:60px;margin-bottom:14px">🚧</div>
          <div style="font-size:16px;font-weight:700;color:#475569">هذه الشاشة قيد التطوير</div>
        </div>`;
    }
  };

  window.setSb = function(el){
    document.querySelectorAll('.sb-item').forEach(s => s.classList.remove('active'));
    if(el) el.classList.add('active');
  };

  // Dashboard render
  async function renderDashboard(){
    const content = document.getElementById('mainContent');
    content.innerHTML = `
      <div class="ph">
        <div>
          <div class="pt">🏠 مرحباً ${CU?.Name || ''}</div>
          <div class="ps">لوحة التحكم — نظرة عامة</div>
        </div>
      </div>
      <div class="grid4" style="margin-bottom:20px">
        <div class="stat-box">
          <div class="stat-ico">🏢</div>
          <div class="stat-val">${BR.length}</div>
          <div class="stat-lbl">الفروع</div>
        </div>
        <div class="stat-box">
          <div class="stat-ico">🏬</div>
          <div class="stat-val">${BL.length}</div>
          <div class="stat-lbl">المباني</div>
        </div>
        <div class="stat-box">
          <div class="stat-ico">📋</div>
          <div class="stat-val">${SCREENS.length}</div>
          <div class="stat-lbl">الشاشات</div>
        </div>
        <div class="stat-box">
          <div class="stat-ico">📚</div>
          <div class="stat-val">${Object.keys(LK).length}</div>
          <div class="stat-lbl">القوائم</div>
        </div>
      </div>
      <div class="card" style="padding:30px">
        <div style="font-family:'Tajawal';font-size:18px;font-weight:900;color:var(--gm);margin-bottom:14px">
          ✅ النظام يعمل بنجاح
        </div>
        <p style="font-size:13px;color:var(--g600);line-height:1.9">
          تم إنشاء المشروع بنجاح مع:
        </p>
        <ul style="font-size:12.5px;color:var(--g700);line-height:2;margin-top:10px;padding-right:20px">
          <li>✅ 22 كيان (Entity) في قاعدة البيانات</li>
          <li>✅ Entity Framework Core + SQLite</li>
          <li>✅ ASP.NET Core Identity (Users, Roles)</li>
          <li>✅ ${SCREENS.length} شاشة مسجّلة في قاعدة البيانات</li>
          <li>✅ ${Object.keys(LK).length} قائمة اختيار (Lookups)</li>
          <li>✅ الثيمات والإعدادات</li>
        </ul>
      </div>
    `;
  }

  // Toast
  window.toast = function(msg, type){
    let el = document.getElementById('toast');
    if(!el){
      el = document.createElement('div');
      el.id = 'toast'; el.className = 'toast';
      document.body.appendChild(el);
    }
    el.textContent = msg;
    el.className = 'toast ' + (type === 'err' ? 'err' : 'ok') + ' show';
    clearTimeout(window._tt);
    window._tt = setTimeout(() => el.classList.remove('show'), 3000);
  };

  // Init
  document.addEventListener('DOMContentLoaded', function(){
    buildTopbarNav();
    buildSidebar();
    updateTopbar();
    if(SCREENS.length){
      const first = SCREENS[0];
      window.goto(first.Key, document.querySelector('.tnav'));
    }
  });

  console.log('✅ PermitSystem initialized', { user: CU, screens: SCREENS.length, lookups: Object.keys(LK).length });
})();