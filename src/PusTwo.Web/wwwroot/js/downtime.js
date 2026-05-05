let downtimeEntries = [];
let entryCounter = 0;

$(document).ready(function() {
    updateClock();
    setInterval(updateClock, 1000);
    loadMachines();
    loadNonProdGroups();
    
    $('#machineDropdown').on('change', function() {
        const wc = $(this).find('option:selected').data('workcentre') || '-';
        $('#workCentreHidden').val(wc);
        $('#wcDisplay').text(wc);
    });
    
    $('#jobInput').on('blur', function() {
        const job = $(this).val().trim();
        if (job) loadJobInfo(job);
    });
    
    $('#addDowntimeBtn').on('click', openModal);
    
    $('#groupCodeSelect').on('change', function() {
        const group = $(this).val();
        if (group) loadNonProdCodes(group);
        else $('#codeSelect').prop('disabled', true).html('<option value="">-- Select Group First --</option>');
    });
    
    $('#downtimeForm').on('submit', function(e) {
        e.preventDefault();
        submitForm();
    });
});

function updateClock() {
    $('#currentTime').text(new Date().toLocaleTimeString('id-ID', { hour: '2-digit', minute: '2-digit' }));
}

async function loadMachines() {
    const res = await $.get('/Bom/GetMachines');
    if (res.success) {
        const opts = res.data.map(m => `<option value="${m.machine}" data-workcentre="${m.workCentre}">${m.displayText}</option>`).join('');
        $('#machineDropdown').append(opts).select2({ placeholder: 'Select Machine', width: '100%' });
    }
}

async function loadJobInfo(job) {
    $('#jobLoader').removeClass('hidden');
    try {
        const res = await $.get('/Bom/GetJobInfo', { jobNumber: job });
        if (res.success && res.data) {
            $('#stockCodeInput').val(res.data.stockCode);
        } else {
            Swal.fire({ icon: 'warning', title: 'Job Not Found', text: res.message, confirmButtonColor: '#1e40af' });
            $('#stockCodeInput').val('');
        }
    } catch (err) {
        Swal.fire({ icon: 'error', title: 'Error', text: 'Gagal mengambil informasi job.', confirmButtonColor: '#1e40af' });
    } finally {
        $('#jobLoader').addClass('hidden');
    }
}

async function loadNonProdGroups() {
    const res = await $.get('/Bom/GetNonProdGroups');
    if (res.success) {
        const opts = res.data.map(g => `<option value="${g.grpCode}">${g.displayText}</option>`).join('');
        $('#groupCodeSelect').append(opts).select2({ placeholder: 'Select Group', width: '100%' });
    }
}

async function loadNonProdCodes(group) {
    const res = await $.get('/Bom/GetNonProdCodes', { groupCode: group });
    if (res.success) {
        const opts = res.data.map(c => `<option value="${c.code}">${c.code} - ${c.description}</option>`).join('');
        $('#codeSelect').html('<option value="">-- Select Code --</option>' + opts).prop('disabled', false).select2({ placeholder: 'Select Code', width: '100%' });
    }
}

function openModal() {
    $('#addDowntimeModal').removeClass('hidden');
    $('#groupCodeSelect').val('').trigger('change');
    $('#codeSelect').val('').prop('disabled', true).html('<option value="">-- Select Group First --</option>');
    $('#downtimeMinutesInput, #remarkInput').val('');
}

function closeModal() {
    $('#addDowntimeModal').addClass('hidden');
}

async function addDowntimeEntry() {
    const groupCode = $('#groupCodeSelect').val();
    const code = $('#codeSelect').val();
    const minutes = parseInt($('#downtimeMinutesInput').val());
    const remark = $('#remarkInput').val().trim();

    if (!groupCode || !code || !minutes || minutes < 1) {
        Swal.fire({ icon: 'warning', title: 'Incomplete Data', text: 'Mohon lengkapi semua field.', confirmButtonColor: '#1e40af' });
        return;
    }

    entryCounter++;
    const entry = { id: entryCounter, groupCode, code, minutes, remark };
    downtimeEntries.push(entry);

    try {
        const html = await $.ajax({
            url: '/Bom/RenderDowntimeRow',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(entry)
        });
        
        $('#downtimeTableBody tr[id="emptyStateRow"]').remove();
        $(`tr[data-entry-id="${entry.id}"]`).remove();
        $('#downtimeTableBody').append(html);
        closeModal();
        updateTotal();
    } catch (err) {
        console.error('Error rendering row:', err);
        Swal.fire({ icon: 'error', title: 'Error', text: 'Gagal render entry.', confirmButtonColor: '#dc2626' });
    }
}

function removeEntry(id) {
    downtimeEntries = downtimeEntries.filter(e => e.id !== id);
    $(`tr[data-entry-id="${id}"]`).fadeOut(200, function() { $(this).remove(); updateTotal(); });
}

async function updateTotal() {
    const total = downtimeEntries.reduce((sum, e) => sum + e.minutes, 0);
    $('#totalDowntime').html(`${total} <span class="text-sm font-normal" style="color: var(--text-sec);">min</span>`);
    $('#entryCount').text(`${downtimeEntries.length} entri${downtimeEntries.length > 0 ? ` (${total} min)` : ''}`);
    
    if (downtimeEntries.length === 0) {
        try {
            const html = await $.ajax({
                url: '/Bom/RenderDowntimeTable',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify([])
            });
            $('#downtimeTableBody').html(html);
        } catch (err) {
            console.error('Error rendering table:', err);
        }
    } else {
        $('#downtimeTableBody tr[id="emptyStateRow"]').remove();
    }
}

async function submitForm() {
    if (downtimeEntries.length === 0) {
        Swal.fire({ icon: 'warning', title: 'No Entries', text: 'Tambahkan minimal satu entry.', confirmButtonColor: '#1e40af' });
        return;
    }

    const machine = $('#machineDropdown').val();
    const jobNumber = $('#jobInput').val().trim();
    
    if (!machine || !jobNumber) {
        Swal.fire({ icon: 'warning', title: 'Incomplete Data', text: 'Pilih Machine dan Job Number.', confirmButtonColor: '#1e40af' });
        return;
    }

    const payload = {
        Machine: machine,
        WorkCentre: $('#workCentreHidden').val(),
        JobNumber: jobNumber,
        StockCode: $('#stockCodeInput').val(),
        EntryDate: $('#entryDate').val(),
        Shift: $('input[name="Shift"]:checked').val(),
        Entries: downtimeEntries.map(e => ({
            GroupCode: e.groupCode,
            Code: e.code,
            DowntimeMinutes: e.minutes,
            Remark: e.remark
        }))
    };

    const token = $('input[name="__RequestVerificationToken"]').val();
    
    try {
        const res = await $.ajax({
            url: '/Bom/PostDownTime',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            headers: token ? { 'RequestVerificationToken': token } : {}
        });

        if (res.success) {
            Swal.fire({ 
                icon: 'success', 
                title: 'Success!', 
                text: res.message, 
                showConfirmButton: false, 
                timer: 2000, 
                timerProgressBar: true 
            }).then(() => resetForm());
        } else {
            Swal.fire({ icon: 'error', title: 'Error', text: res.message, confirmButtonColor: '#dc2626' });
        }
    } catch (error) {
        let msg = 'Terjadi kesalahan.';
        if (error.responseJSON && error.responseJSON.message) msg = error.responseJSON.message;
        else if (error.status === 400) msg = 'Data tidak valid (400)';
        else if (error.status === 500) msg = 'Internal server error (500)';
        
        Swal.fire({ icon: 'error', title: 'Error', text: msg, confirmButtonColor: '#dc2626', width: 600 });
    }
}

function resetForm() {
    downtimeEntries = [];
    entryCounter = 0;
    $('#downtimeForm')[0].reset();
    $('#machineDropdown').val('').trigger('change');
    $('#stockCodeInput, #jobInput').val('');
    $('#wcDisplay').text('-');
    $('#workCentreHidden').val('');
    
    $.ajax({
        url: '/Bom/RenderDowntimeTable',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify([])
    }).done(function(html) {
        $('#downtimeTableBody').html(html);
    });
    
    updateTotal();
    closeModal();
}