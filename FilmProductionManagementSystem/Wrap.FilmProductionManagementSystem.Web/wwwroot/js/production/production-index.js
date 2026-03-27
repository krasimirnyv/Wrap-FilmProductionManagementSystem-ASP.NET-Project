// ── Live search + phase filter ──────────────────────────────────────
// Phase-to-enum-values map mirrors ProductionStatusAbstraction on the client
// so the filter dropdown matches exactly what the service categorises.
const phaseMembers = {
    'Pre-production':  ['Concept','Development','Preproduction','Financing',
        'Casting','LocationScouting','Rehearsals'],
    'Production':      ['Production','OnHold','Reshoots'],
    'Post-production': ['PostProduction','PictureLock','SoundDesign',
        'ColorGrading','VisualEffects','MusicComposition'],
    'Distribution':    ['Marketing','Distribution','FestivalCircuit',
        'Released','Completed','Cancelled'],
};

const searchInput  = document.getElementById('searchInput');
const statusFilter = document.getElementById('statusFilter');
const items        = document.querySelectorAll('.prod-grid-item');
const countEl      = document.getElementById('resultsCount');
const noResults    = document.getElementById('noResults');

function applyFilters() {
    const q     = searchInput?.value.toLowerCase() ?? '';
    const phase = statusFilter?.value ?? '';
    let visible = 0;

    items.forEach(item => {
        const titleMatch = item.dataset.title.includes(q);
        const phaseMatch = !phase ||
            (phaseMembers[phase] ?? []).includes(item.dataset.status);
        const show = titleMatch && phaseMatch;
        item.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    if (countEl)   countEl.textContent = `${visible} production(s)`;
    if (noResults) noResults.classList.toggle('d-none', visible > 0);
}

searchInput?.addEventListener('input',   applyFilters);
statusFilter?.addEventListener('change', applyFilters);

// ── Staggered entrance animation ────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.prod-grid-item').forEach((el, i) => {
        el.style.opacity    = '0';
        el.style.transform  = 'translateY(20px)';
        el.style.transition = 'all 0.4s ease';
        setTimeout(() => {
            el.style.opacity   = '1';
            el.style.transform = 'translateY(0)';
        }, 80 * i);
    });
});