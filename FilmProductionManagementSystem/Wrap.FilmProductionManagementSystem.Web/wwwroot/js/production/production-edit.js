document.addEventListener('DOMContentLoaded', () => {

    // ── Thumbnail preview ─────────────────────────────────────────
    const thumbInput    = document.getElementById('thumbnailInput');
    const newPreviewWrap= document.getElementById('newPreviewWrap');
    const newPreviewImg = document.getElementById('newPreviewImg');
    const clearBtn      = document.getElementById('clearThumbnail');

    thumbInput?.addEventListener('change', e => {
        const file = e.target.files[0];
        if (!file) return;
        if (!file.type.startsWith('image/'))  { alert('Please select an image file.'); thumbInput.value = ''; return; }
        if (file.size > 10 * 1024 * 1024)     { alert('Max file size is 10 MB.');      thumbInput.value = ''; return; }
        const reader = new FileReader();
        reader.onload = ev => {
            newPreviewImg.src            = ev.target.result;
            newPreviewWrap.style.display = '';
        };
        reader.readAsDataURL(file);
    });

    clearBtn?.addEventListener('click', () => {
        thumbInput.value             = '';
        newPreviewWrap.style.display = 'none';
    });

    // ── Description counter ───────────────────────────────────────
    const descField   = document.getElementById('descriptionTextarea');
    const descCounter = document.getElementById('descriptionCounter');
    descField?.addEventListener('input', () =>
        descCounter.textContent = descField.value.length);

    // ── Cascading phase → status select ───────────────────────────
    const phaseSelect  = document.getElementById('phaseSelect');
    const statusSelect = document.getElementById('statusTypeSelect');
    const optgroups    = statusSelect.querySelectorAll('optgroup[data-phase]');

    // Apply showing/hiding by manipulating option.hidden & option.disabled for cross-browser reliability
    function applyPhaseSelection(chosen) {
        const placeholder = statusSelect.querySelector('option[value=""]');

        optgroups.forEach(og => {
            const match = og.dataset.phase === chosen;
            og.querySelectorAll('option').forEach(opt => {
                if (opt.value === '') return; // keep placeholder
                opt.hidden = !match;
                opt.disabled = !match;
            });
        });

        if (!chosen) {
            statusSelect.value = '';
            statusSelect.disabled = true;
            if (placeholder) placeholder.textContent = '— Select Phase first —';
        } else {
            statusSelect.disabled = false;
            // Only reset selection if it doesn't belong to the chosen phase
            const currentOpt = statusSelect.querySelector(`option[value="${statusSelect.value}"]`);
            const currentPhaseOfOpt = currentOpt?.closest('optgroup')?.dataset.phase;
            if (!currentOpt || currentPhaseOfOpt !== chosen) {
                statusSelect.value = '';
            }
            if (placeholder) placeholder.textContent = '— Select Status —';
        }
    }

    // Listen for user changes
    phaseSelect?.addEventListener('change', () => applyPhaseSelection(phaseSelect.value));

    // Run once on load to enforce the initial pre-selected phase/status state
    applyPhaseSelection(phaseSelect?.value || '');

    // ── Unsaved changes warning ───────────────────────────────────
    let dirty = false;
    document.querySelectorAll('#editProductionForm input, #editProductionForm textarea, #editProductionForm select')
        .forEach(el => el.addEventListener('change', () => dirty = true));
    window.addEventListener('beforeunload', e => { if (dirty) { e.preventDefault(); e.returnValue = ''; } });
    document.getElementById('editProductionForm')
        ?.addEventListener('submit', () => dirty = false);
});