const DownTimeApp = (function() {
    'use strict';
    let downtimeEntries = [];
    let nonProdGroups = [];
    let debounceTimer;
    const elements = {};

    function init() {
        cacheElements();
        initSelect2();
        initClock();
        initEventListeners();
        loadMachines();
        setDefaultDate();
    }

    function cacheElements() {
        elements.machineSelect = document.getElementById('machineDropdown');
        elements.jobInput = document.getElementById('jobInput');
        elements.stockInput = document.getElementById('stockCodeInput');
        elements.tableBody = document.getElementById('downtimeTableBody');
        elements.entryCount = document.getElementById('entryCount');
        elements.totalDowntime = document.getElementById('totalDowntime');
        elements.runTimeInput = document.getElementById('runTimeInput');
        elements.modal = document.getElementById('addDowntimeModal');
        elements.jobLoader = document.getElementById('jobLoader');
        elements.workCentreHidden = document.getElementById('workCentreHidden');
        elements.wcDisplay = document.getElementById('wcDisplay');
        elements.groupDescDisplay = document.getElementById('groupDescDisplay');
        elements.codeDescDisplay = document.getElementById('codeDescDisplay');
        elements.downtimeMinutesInput = document.getElementById('downtimeMinutesInput');
        elements.remarkInput = document.getElementById('remarkInput');
        elements.charCount = document.getElementById('charCount');
        elements.addDowntimeBtn = document.getElementById('addDowntimeBtn');
        elements.currentTime = document.getElementById('currentTime');
        elements.jobDate = document.getElementById('jobDate');
        elements.groupCodeSelect = $('#groupCodeSelect');
        elements.codeSelect = $('#codeSelect');
    }

    function initSelect2() {
        elements.groupCodeSelect.select2({ placeholder: 'Select Group...', allowClear: true, width: '100%', dropdownParent: $('#addDowntimeModal') });
        elements.codeSelect.select2({ placeholder: 'Select Code...', allowClear: true, width: '100%', dropdownParent: $('#addDowntimeModal'), disabled: true });
    }

    function initClock() { updateClock(); setInterval(updateClock, 1000); }
    function updateClock() { const now = new Date(); if (elements.currentTime) elements.currentTime.textContent = now.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }); }
    function setDefaultDate() { if (elements.jobDate) elements.jobDate.valueAsDate = new Date(); }

    function initEventListeners() {
        if (elements.machineSelect) elements.machineSelect.addEventListener('change', onMachineChange);
        if (elements.jobInput) elements.jobInput.addEventListener('input', onJobInput);
        elements.groupCodeSelect.on('change', onGroupCodeChange);
        elements.codeSelect.on('change', onCodeChange);
        if (elements.remarkInput) elements.remarkInput.addEventListener('input', onRemarkInput);
        if (elements.addDowntimeBtn) elements.addDowntimeBtn.addEventListener('click', openModal);
    }

    function onMachineChange(e) { const wc = e.target.options[e.target.selectedIndex]?.dataset.wc || ''; if (elements.workCentreHidden) elements.workCentreHidden.value = wc; if (elements.wcDisplay) elements.wcDisplay.textContent = wc || '-'; }

    function onJobInput() {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            const job = elements.jobInput.value.trim();
            if (!job) { if (elements.stockInput) elements.stockInput.value = ''; return; }
            if (elements.jobLoader) elements.jobLoader.classList.remove('hidden');
            fetch(`/Bom/GetJobInfo?jobNumber=${encodeURIComponent(job)}`).then(res => res.json()).then(json => {
                if (elements.jobLoader) elements.jobLoader.classList.add('hidden');
                if (json.success && json.data && elements.stockInput) elements.stockInput.value = json.data.stockCode;
            }).catch(() => { if (elements.jobLoader) elements.jobLoader.classList.add('hidden'); });
        }, 400);
    }

    async function loadMachines() {
        try {
            const res = await fetch('/Bom/GetMachines');
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const data = await res.json();
            if (elements.machineSelect && data.success && data.data?.length) {
                elements.machineSelect.innerHTML = '<option value="">-- Select Machines --</option>';
                data.data.forEach(m => { const opt = document.createElement('option'); opt.value = m.machine; opt.textContent = m.displayText; opt.dataset.wc = m.workCentre; elements.machineSelect.appendChild(opt); });
            }
        } catch (err) { console.error('Error loading machines:', err); }
    }

    async function loadNonProdGroups() {
        try {
            const res = await fetch('/Bom/GetNonProdGroups');
            const json = await res.json();
            if (json.success) {
                nonProdGroups = json.data;
                elements.groupCodeSelect.empty().append('<option value="">Select Group</option>');
                nonProdGroups.forEach(g => { const opt = new Option(g.grpCode, g.grpCode, false, false); $(opt).attr('data-desc', g.grpDescription); elements.groupCodeSelect.append(opt); });
                elements.groupCodeSelect.trigger('change');
            }
        } catch (err) { console.error('Error loading groups:', err); }
    }

    async function onGroupCodeChange() {
        const groupCode = this.value;
        const selectedOpt = this.options[this.selectedIndex];
        if (elements.groupDescDisplay) elements.groupDescDisplay.value = selectedOpt ? $(selectedOpt).data('desc') || '' : '';
        elements.codeSelect.empty().append('<option value="">Loading...</option>').prop('disabled', true).trigger('change');
        if (elements.codeDescDisplay) elements.codeDescDisplay.value = '';
        if (!groupCode) { elements.codeSelect.empty().append('<option value="">Select Code</option>').trigger('change'); return; }
        try {
            const res = await fetch(`/Bom/GetNonProdCodes?groupCode=${encodeURIComponent(groupCode)}`);
            if (!res.ok || !res.headers.get("content-type")?.includes("application/json")) throw new Error(`HTTP ${res.status}`);
            const json = await res.json();
            elements.codeSelect.empty().append('<option value="">Select Code</option>');
            if (json.success && json.data?.length) {
                json.data.forEach(c => { const opt = new Option(`${c.code} - ${c.description}`, c.code, false, false); $(opt).attr('data-desc', c.description); elements.codeSelect.append(opt); });
                elements.codeSelect.prop('disabled', false);
            }
        } catch (err) { console.error('Error loading codes:', err); elements.codeSelect.empty().append(`<option value="">Error</option>`).prop('disabled', true).trigger('change'); }
    }

    function onCodeChange() { const selectedOpt = this.options[this.selectedIndex]; if (elements.codeDescDisplay) elements.codeDescDisplay.value = selectedOpt ? $(selectedOpt).data('desc') || '' : ''; }
    function onRemarkInput(e) { if (elements.charCount) elements.charCount.textContent = e.target.value.length; }

    function openModal() {
        if (!elements.machineSelect?.value || !elements.jobInput?.value.trim()) { alert('Pilih Machine dan Job dulu!'); return; }
        if (elements.modal) elements.modal.classList.remove('hidden');
        loadNonProdGroups();
    }

    function closeModal() {
        if (elements.modal) elements.modal.classList.add('hidden');
        elements.groupCodeSelect.val(null).trigger('change');
        elements.codeSelect.val(null).trigger('change').prop('disabled', true);
        if (elements.groupDescDisplay) elements.groupDescDisplay.value = '';
        if (elements.codeDescDisplay) elements.codeDescDisplay.value = '';
        if (elements.downtimeMinutesInput) elements.downtimeMinutesInput.value = '';
        if (elements.remarkInput) elements.remarkInput.value = '';
        if (elements.charCount) elements.charCount.textContent = '0';
    }

    function addDowntimeEntry() {
        const gc = elements.groupCodeSelect.val();
        const c = elements.codeSelect.val();
        const dt = parseInt(elements.downtimeMinutesInput?.value) || 0;
        if (!gc || !c || dt <= 0) { alert('Lengkapi Group, Code, dan Downtime!'); return; }
        downtimeEntries.push({ groupCode: gc, groupDescription: elements.groupDescDisplay?.value || '', code: c, codeDescription: elements.codeDescDisplay?.value || '', downtimeMinutes: dt, remark: elements.remarkInput?.value || '' });
        updateTable();
        closeModal();
    }

    function updateTable() {
        if (!elements.tableBody || !elements.entryCount) return;
        if (downtimeEntries.length === 0) {
            elements.tableBody.innerHTML = `<tr><td colspan="5" class="px-6 py-16 text-center"><svg class="mx-auto h-12 w-12 mb-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" style="color: #cbd5e1;"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg><p class="font-medium" style="color: var(--color-text-secondary);">Belum ada downtime entry</p></td></tr>`;
            elements.entryCount.textContent = '0 entries';
        } else {
            elements.tableBody.innerHTML = downtimeEntries.map((e, i) => `<tr class="border-b hover:bg-gray-50"><td class="px-6 py-3"><button onclick="DownTimeApp.deleteEntry(${i})" class="bg-red-100 hover:bg-red-200 text-red-700 font-medium text-xs px-3 py-1.5 rounded-lg transition">Delete</button></td><td class="px-6 py-3 font-semibold">${e.groupCode}</td><td class="px-6 py-3 text-gray-600">${e.code}</td><td class="px-6 py-3 text-gray-700">${e.codeDescription || '-'}</td><td class="px-6 py-3 text-right font-bold text-red-600">${e.downtimeMinutes} min</td></tr>`).join('');
            elements.entryCount.textContent = `${downtimeEntries.length} entr${downtimeEntries.length === 1 ? 'y' : 'ies'}`;
        }
        const total = downtimeEntries.reduce((s, e) => s + e.downtimeMinutes, 0);
        if (elements.totalDowntime) elements.totalDowntime.innerHTML = `${total.toFixed(2)} <span class="text-sm font-normal" style="color: var(--color-text-secondary);">min</span>`;
        if (elements.runTimeInput) elements.runTimeInput.value = Math.max(0, 8 - (total / 60)).toFixed(2);
    }

    async function postDownTimeToServer() {
        if (downtimeEntries.length === 0) { alert('Tidak ada downtime entry untuk disimpan!'); return; }
        const machine = elements.machineSelect?.value;
        const workCentre = elements.workCentreHidden?.value || '';
        const jobNumber = elements.jobInput?.value.trim();
        const stockCode = elements.stockInput?.value || '';
        const entryDate = elements.jobDate?.value;
        const shift = document.querySelector('input[name="Shift"]:checked')?.value || '1';
        if (!machine || !jobNumber) { alert('Pilih Machine dan isi Job Number dulu!'); return; }
        const postBtn = document.querySelector('button[onclick="DownTimeApp.postDownTimeToServer()"]');
        const originalBtnText = postBtn?.innerHTML || 'Post';
        if (postBtn) { postBtn.disabled = true; postBtn.innerHTML = '⏳ Saving...'; }
        try {
            const payload = { machine, workCentre, jobNumber, stockCode, entryDate: entryDate ? new Date(entryDate) : new Date(), shift, entries: downtimeEntries.map(e => ({ groupCode: e.groupCode, code: e.code, description: e.codeDescription, downtimeMinutes: e.downtimeMinutes, remark: e.remark })) };
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
            const response = await fetch('/Bom/PostDownTime', { method: 'POST', headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token }, body: JSON.stringify(payload) });
            const result = await response.json();
            if (result.success) {
                alert('✅ Downtime berhasil disimpan!');
                downtimeEntries = [];
                updateTable();
                if (elements.machineSelect) elements.machineSelect.value = '';
                if (elements.workCentreHidden) elements.workCentreHidden.value = '';
                if (elements.wcDisplay) elements.wcDisplay.textContent = '-';
                if (elements.jobInput) elements.jobInput.value = '';
                if (elements.stockInput) elements.stockInput.value = '';
                if (elements.jobDate) elements.jobDate.valueAsDate = new Date();
            } else { alert(`❌ Gagal: ${result.message}`); }
        } catch (error) { console.error('Error:', error); alert('⚠️ Terjadi kesalahan.'); }
        finally { if (postBtn) { postBtn.disabled = false; postBtn.innerHTML = originalBtnText; } }
    }

    function deleteEntry(index) { if (confirm('Hapus entry ini?')) { downtimeEntries.splice(index, 1); updateTable(); } }

    return { init, closeModal, addDowntimeEntry, postDownTimeToServer, deleteEntry, updateTable };
})();

$(document).ready(function() { DownTimeApp.init(); });