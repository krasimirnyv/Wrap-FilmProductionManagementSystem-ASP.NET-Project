document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.production-header-card, .section-card').forEach((el, i) => {
        el.style.opacity    = '0';
        el.style.transform  = 'translateY(20px)';
        el.style.transition = 'all 0.45s ease';
        setTimeout(() => {
            el.style.opacity   = '1';
            el.style.transform = 'translateY(0)';
        }, i * 120);
    });
});