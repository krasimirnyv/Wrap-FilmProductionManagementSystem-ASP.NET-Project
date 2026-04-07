document.addEventListener('DOMContentLoaded', () => {
    const deptSelects = document.querySelectorAll('select[data-role-dept]');

    deptSelects.forEach(deptSelect => {
        const roleSelectId = deptSelect.getAttribute('data-role-dept');
        const roleSelect = document.getElementById(roleSelectId);
        if (!roleSelect) return;

        const optgroups = roleSelect.querySelectorAll('optgroup[data-phase]');

        const update = () => {
            const chosenDept = deptSelect.value;

            // Show/hide individual <option> for reliability across browsers
            const placeholder = roleSelect.querySelector('option[value=""]');

            optgroups.forEach(og => {
                const match = og.dataset.phase === chosenDept;
                og.querySelectorAll('option').forEach(opt => {
                    if (opt.value === '') return;
                    opt.hidden = !match;
                    opt.disabled = !match;
                });
            });

            if (!chosenDept) {
                roleSelect.value = '';
                roleSelect.disabled = true;
                if (placeholder) placeholder.textContent = '— Select dept first —';
            } else {
                roleSelect.disabled = false;
                roleSelect.value = '';
                if (placeholder) placeholder.textContent = '— Select role —';
            }
        };

        deptSelect.addEventListener('change', update);
        update();
    });
});