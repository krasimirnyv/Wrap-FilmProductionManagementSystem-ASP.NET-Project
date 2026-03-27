document.addEventListener('DOMContentLoaded', () => {

    // ── Thumbnail preview ─────────────────────────────────────────
    const thumbInput   = document.getElementById('thumbnailInput');
    const thumbPreview = document.getElementById('thumbnailPreview');
    const thumbImg     = document.getElementById('thumbnailImage');
    const uploadLabel  = document.getElementById('uploadLabel');
    const clearBtn     = document.getElementById('clearThumbnail');

    thumbInput?.addEventListener('change', e => {
        const file = e.target.files[0];
        if (!file) return;
        if (!file.type.startsWith('image/'))  { alert('Please select an image file.'); thumbInput.value = ''; return; }
        if (file.size > 10 * 1024 * 1024)     { alert('Max file size is 10 MB.');      thumbInput.value = ''; return; }
        const reader = new FileReader();
        reader.onload = ev => {
            thumbImg.src = ev.target.result;
            thumbPreview.style.display = '';
            uploadLabel.style.display  = 'none';
        };
        reader.readAsDataURL(file);
    });

    clearBtn?.addEventListener('click', () => {
        thumbInput.value           = '';
        thumbPreview.style.display = 'none';
        uploadLabel.style.display  = '';
    });

    // ── Description counter ───────────────────────────────────────
    const descField   = document.getElementById('descriptionTextarea');
    const descCounter = document.getElementById('descriptionCounter');
    descField?.addEventListener('input', () =>
        descCounter.textContent = descField.value.length);

    // ── Default start date ────────────────────────────────────────
    const startDate = document.querySelector('[name="StatusStartDate"]');
    if (startDate && !startDate.value)
        startDate.value = new Date().toISOString().split('T')[0];

    // ── Cascading phase → status select ───────────────────────────
    const phaseSelect  = document.getElementById('phaseSelect');
    const statusSelect = document.getElementById('statusTypeSelect');
    const optgroups    = statusSelect.querySelectorAll('optgroup[data-phase]');

    phaseSelect?.addEventListener('change', () => {
        const chosen = phaseSelect.value;

        // Instead of toggling optgroup display (not reliable across browsers),
        // show/hide individual <option> elements. This ensures only options
        // relevant to the selected phase appear in the dropdown.
        const placeholder = statusSelect.querySelector('option[value=""]');

        // Iterate optgroups and their options
        optgroups.forEach(og => {
            const match = og.dataset.phase === chosen;
            og.querySelectorAll('option').forEach(opt => {
                // Keep the placeholder option visible regardless
                if (opt.value === '') return;

                // Use `hidden` and `disabled` so options are removed from the list
                // and cannot be selected in browsers that don't respect optgroup styling.
                opt.hidden = !match;
                opt.disabled = !match;
            });
        });

        if (!chosen) {
            // No phase chosen → disable status select and clear its value
            statusSelect.value = '';
            statusSelect.disabled = true;
            if (placeholder) placeholder.textContent = '— Select Phase first —';
        } else {
            statusSelect.disabled = false;
            statusSelect.value = ''; // reset to "please choose" state
            if (placeholder) placeholder.textContent = '— Select Status —';
        }
    });

    // ── Unsaved changes warning ───────────────────────────────────
    let dirty = false;
    document.querySelectorAll('#createProductionForm input, #createProductionForm textarea, #createProductionForm select')
        .forEach(el => el.addEventListener('change', () => dirty = true));
    window.addEventListener('beforeunload', e => { if (dirty) { e.preventDefault(); e.returnValue = ''; } });
    document.getElementById('createProductionForm')
        ?.addEventListener('submit', () => dirty = false);
});