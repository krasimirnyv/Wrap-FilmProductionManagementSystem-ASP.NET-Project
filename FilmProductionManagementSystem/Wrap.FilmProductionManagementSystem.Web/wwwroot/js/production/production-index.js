// ── Live search + phase filter + pagination visibility ───────────────

const phaseMembers = {
    'Pre-production': ['Concept', 'Development', 'Preproduction', 'Financing', 'Casting', 'LocationScouting', 'Rehearsals'],
    'Production': ['Production', 'OnHold', 'Reshoots'],
    'Post-production': ['PostProduction', 'PictureLock', 'SoundDesign', 'ColorGrading', 'VisualEffects', 'MusicComposition'],
    'Distribution': ['Marketing', 'Distribution', 'FestivalCircuit', 'Released', 'Completed', 'Cancelled'],
};

const searchInput = document.getElementById('searchInput');
const statusFilter = document.getElementById('statusFilter');
const countEl = document.getElementById('resultsCount');
const noResults = document.getElementById('noResults');
const paginationWrap = document.getElementById('paginationWrap');

function getItems() {
    return document.querySelectorAll('.prod-grid-item');
}

function productionLabel(n) {
    return n === 1 ? 'production' : 'productions';
}

function getTotalCount() {
    if (!countEl) return 0;
    const raw = countEl.getAttribute('data-total');
    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function applyFilters() {
    const query = (searchInput?.value ?? '').trim().toLowerCase();
    const phase = (statusFilter?.value ?? '').trim();

    const hasFilter = query.length > 0 || phase.length > 0;

    let visible = 0;
    const items = getItems();

    const allowedStatusesForPhase = phaseMembers[phase] ?? [];
    const allowedSet = allowedStatusesForPhase.length ? new Set(allowedStatusesForPhase.map(s => s.toLowerCase())) : null;

    items.forEach(item => {
        const title = (item.dataset.title ?? '').toLowerCase();
        const status = (item.dataset.status ?? '').toLowerCase();

        const titleMatch = title.includes(query);

        const phaseMatch =
            !phase ||
            (allowedSet ? allowedSet.has(status) : false);

        const show = titleMatch && phaseMatch;

        item.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const total = getTotalCount();

    // X of Y productions
    if (countEl) {
        // ако total == 0 (няма data-total), fallback да не изглежда счупено
        if (total > 0) {
            countEl.textContent = `${visible} of ${total} ${productionLabel(total)}`;
        } else {
            countEl.textContent = `${visible} ${productionLabel(visible)}`;
        }
    }

    if (noResults) {
        noResults.classList.toggle('d-none', visible > 0);
    }

    // Hide pagination when filtering/searching
    if (paginationWrap) {
        paginationWrap.classList.toggle('d-none', hasFilter);
    }
}

function runEntranceAnimation() {
    const items = getItems();
    items.forEach((el, i) => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'all 0.4s ease';

        setTimeout(() => {
            el.style.opacity = '1';
            el.style.transform = 'translateY(0)';
        }, 80 * i);
    });
}

// Events
searchInput?.addEventListener('input', applyFilters);
statusFilter?.addEventListener('change', applyFilters);

// Init
document.addEventListener('DOMContentLoaded', () => {
    runEntranceAnimation();
    applyFilters();
});