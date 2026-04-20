document.addEventListener('DOMContentLoaded', () => {
    // Optional debug fingerprint (can remove later)
    // console.log('production-create.js loaded', new Date().toISOString());

    // ── Thumbnail preview ─────────────────────────────────────────
    const thumbInput   = document.getElementById('thumbnailInput');
    const thumbPreview = document.getElementById('thumbnailPreview');
    const thumbImg     = document.getElementById('thumbnailImage');
    const uploadLabel  = document.getElementById('uploadLabel');
    const clearBtn     = document.getElementById('clearThumbnail');

    if (thumbInput && thumbPreview && thumbImg && uploadLabel) {
        thumbInput.addEventListener('change', (e) => {
            const input = /** @type {HTMLInputElement} */ (e.target);
            const file = input.files && input.files[0];
            if (!file) return;

            if (!file.type || !file.type.startsWith('image/')) {
                alert('Please select an image file.');
                input.value = '';
                return;
            }

            if (file.size > 10 * 1024 * 1024) {
                alert('Max file size is 10 MB.');
                input.value = '';
                return;
            }

            const reader = new FileReader();
            reader.onload = (ev) => {
                const result = ev.target && ev.target.result;
                if (!result) return;

                thumbImg.src = String(result);
                thumbPreview.style.display = '';
                uploadLabel.style.display = 'none';
            };
            reader.readAsDataURL(file);
        });

        if (clearBtn) {
            clearBtn.addEventListener('click', () => {
                thumbInput.value = '';
                thumbPreview.style.display = 'none';
                uploadLabel.style.display = '';
            });
        }
    }

    // ── Description counter ───────────────────────────────────────
    const descField   = document.getElementById('descriptionTextarea');
    const descCounter = document.getElementById('descriptionCounter');

    if (descField && descCounter) {
        descField.addEventListener('input', () => {
            descCounter.textContent = String(descField.value.length);
        });
        // init if server pre-filled
        descCounter.textContent = String(descField.value.length);
    }

    // ── Default start date ────────────────────────────────────────
    const startDate = document.querySelector('[name="StatusStartDate"]');
    if (startDate && !startDate.value) {
        // YYYY-MM-DD (works for <input type="date">)
        startDate.value = new Date().toISOString().split('T')[0];
    }

    // ── Cascading phase → status select ───────────────────────────
    const phaseSelect  = document.getElementById('phaseSelect');
    const statusSelect = document.getElementById('statusTypeSelect');

    if (!phaseSelect || !statusSelect) {
        console.warn('CreateProduction: missing selects. phaseSelect/statusTypeSelect not found.');
    } else {
        const optgroups = statusSelect.querySelectorAll('optgroup[data-phase]');

        const applyPhase = () => {
            const chosen = phaseSelect.value;
            const placeholder = statusSelect.querySelector('option[value=""]');

            // If there are no optgroups, something is off in the markup
            if (!optgroups || optgroups.length === 0) {
                console.warn('CreateProduction: no optgroups[data-phase] found under statusTypeSelect.');
            }

            optgroups.forEach((og) => {
                const match = og.dataset.phase === chosen;

                // IMPORTANT: don't rely on optgroup display toggling; hide/disable options instead
                og.querySelectorAll('option').forEach((opt) => {
                    if (opt.value === '') return; // keep placeholder visible
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
                statusSelect.value = ''; // reset selection on phase change
                if (placeholder) placeholder.textContent = '— Select Status —';
            }
        };

        phaseSelect.addEventListener('change', applyPhase);

        // Init state on load (handles browser autofill / preselected values)
        applyPhase();
    }

    // ── Unsaved changes warning ───────────────────────────────────
    const form = document.getElementById('createProductionForm');
    if (form) {
        let dirty = false;

        form.querySelectorAll('input, textarea, select').forEach((el) => {
            el.addEventListener('change', () => (dirty = true));
            el.addEventListener('input', () => (dirty = true));
        });

        window.addEventListener('beforeunload', (e) => {
            if (!dirty) return;
            e.preventDefault();
            e.returnValue = '';
        });

        form.addEventListener('submit', () => (dirty = false));
    }
});