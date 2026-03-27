document.addEventListener('DOMContentLoaded', () => {
    const confirmInput = document.getElementById('confirmInput');
    const deleteButton = document.getElementById('deleteButton');

    // Enable delete button only when user types "DELETE"
    confirmInput?.addEventListener('input', () => {
        const ready = confirmInput.value.trim() === 'DELETE';
        deleteButton.disabled = !ready;
        confirmInput.classList.toggle('border-danger', !ready && confirmInput.value.length > 0);
        confirmInput.classList.toggle('border-success', ready);
    });

    // Extra JS-level confirmation on submit
    deleteButton?.closest('form')?.addEventListener('submit', e => {
        if (confirmInput.value.trim() !== 'DELETE') {
            e.preventDefault();
            return;
        }
        if (!confirm('Final check: Are you absolutely sure you want to delete this production?')) {
            e.preventDefault();
        }
    });

    // Entrance animation
    document.querySelectorAll('.production-header-card, .section-card, .danger-zone-card')
        .forEach((el, i) => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(20px)';
            el.style.transition = 'all 0.4s ease';
            setTimeout(() => {
                el.style.opacity = '1';
                el.style.transform = 'translateY(0)';
            }, i * 100);
        });
});