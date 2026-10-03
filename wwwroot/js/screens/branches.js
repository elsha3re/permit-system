window.SCREEN_branches = async function(){
  const c = document.getElementById('mainContent');
  c.innerHTML = `
    <div class="ph">
      <div><div class="pt">🏢 الفروع</div><div class="ps">إدارة فروع المجمع</div></div>
      <button class="btn bp" onclick="BRANCH_new()">➕ فرع جديد</button>
    </div>
    <div class="tbl-wrap">
      <table>
        <thead><tr><th>الاسم</th><th>الكود</th><th>المدينة</th><th>المدير</th><th>إجراءات</th></tr></thead>
        <tbody id="brBody"><tr><td colspan="5" style="text-align:center;padding:30px">⏳ جاري التحميل</td></tr></tbody>
      </table>
    </div>
  `;
  await BRANCH_load();
};

window.BRANCH_load = async function(){
  try{
    const res = await fetch('/api/branches');
    const list = await res.json();
    const t = document.getElementById('brBody');
    if(!list.length){
      t.innerHTML = '<tr><td colspan="5" style="text-align:center;padding:30px;color:#94a3b8">لا توجد فروع</td></tr>';
    } else {
      t.innerHTML = list.map(b => `
        <tr>
          <td><b>🏢 ${b.name}</b></td>
          <td><span class="badge bbl">${b.code}</span></td>
          <td>${b.city || '—'}</td>
          <td>${b.manager || '—'}</td>
          <td>
            <button class="btn bg2 bxs" onclick="BRANCH_edit(${b.id})">✏️</button>
            <button class="btn bred bxs" onclick="BRANCH_del(${b.id})">🗑</button>
          </td>
        </tr>`).join('');
    }
  } catch(e){
    document.getElementById('brBody').innerHTML = '<tr><td colspan="5" style="text-align:center;padding:30px;color:#dc2626">⛔ خطأ: '+e.message+'</td></tr>';
  }
};

window.BRANCH_new = function(){ BRANCH_form(null); };

window.BRANCH_edit = async function(id){
  const r = await fetch('/api/branches/'+id);
  const b = await r.json();
  BRANCH_form(b);
};

window.BRANCH_form = function(b){
  const isEdit = !!b;
  const html = `
    <div class="mover open" id="brModal" style="display:flex">
      <div class="modal">
        <div class="mh">
          <div class="mht">${isEdit?'✏️ تعديل':'➕ فرع جديد'}</div>
          <button class="mc" onclick="document.getElementById('brModal').remove()">✕</button>
        </div>
        <div class="mb">
          <div class="fg" style="margin-bottom:12px"><label>الاسم *</label><input id="brName" value="${b?.name||''}"></div>
          <div class="fg" style="margin-bottom:12px"><label>الكود *</label><input id="brCode" value="${b?.code||''}"></div>
          <div class="fg" style="margin-bottom:12px"><label>المدينة</label><input id="brCity" value="${b?.city||''}"></div>
          <div class="fg" style="margin-bottom:12px"><label>المدير</label><input id="brManager" value="${b?.manager||''}"></div>
          <div class="fg" style="margin-bottom:16px"><label>الهاتف</label><input id="brPhone" value="${b?.phone||''}"></div>
          <div class="bgrp">
            <button class="btn bg2" onclick="document.getElementById('brModal').remove()">إلغاء</button>
            <button class="btn bp" onclick="BRANCH_save(${b?.id||0})">💾 حفظ</button>
          </div>
        </div>
      </div>
    </div>`;
  const div = document.createElement('div');
  div.innerHTML = html;
  document.body.appendChild(div.firstElementChild);
};

window.BRANCH_save = async function(id){
  const dto = {
    name: document.getElementById('brName').value.trim(),
    code: document.getElementById('brCode').value.trim(),
    city: document.getElementById('brCity').value.trim(),
    manager: document.getElementById('brManager').value.trim(),
    phone: document.getElementById('brPhone').value.trim(),
    isActive: true
  };
  if(!dto.name || !dto.code){ toast('⚠️ الاسم والكود مطلوبان','err'); return; }
  const url = id ? '/api/branches/'+id : '/api/branches';
  const method = id ? 'PUT' : 'POST';
  const res = await fetch(url, {
    method,
    headers: {'Content-Type':'application/json'},
    body: JSON.stringify(dto)
  });
  if(!res.ok){
    const err = await res.json().catch(()=>({error:'فشل'}));
    toast('⛔ ' + (err.error||'فشل'), 'err');
    return;
  }
  document.getElementById('brModal').remove();
  toast('✅ تم الحفظ');
  await BRANCH_load();
};

window.BRANCH_del = async function(id){
  if(!confirm('حذف هذا الفرع؟')) return;
  const res = await fetch('/api/branches/'+id, {method:'DELETE'});
  if(!res.ok){
    const err = await res.json().catch(()=>({error:'فشل'}));
    toast('⛔ ' + (err.error||'فشل'), 'err');
    return;
  }
  toast('🗑 تم الحذف');
  await BRANCH_load();
};