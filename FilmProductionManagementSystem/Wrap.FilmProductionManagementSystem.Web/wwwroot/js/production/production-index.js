const searchInput = document.getElementById('searchInput');
const statusFilter = document.getElementById('statusFilter');
const countEl = document.getElementById('resultsCount');
const noResults = document.getElementById('noResults');
const topPaginationWrap = document.getElementById('topPaginationWrap');
const bottomPaginationWrap = document.getElementById('bottomPaginationWrap');

if (statusFilter) {
    statusFilter.addEventListener('change', function () {
        const url = new URL(window.location.href);
        const selectedValue = this.value;

        if (selectedValue) {
            url.searchParams.set('SelectedStatus', selectedValue);
        } else {
            url.searchParams.delete('SelectedStatus');
        }

        url.searchParams.set('PageNumber', '1');
        window.location.href = url.toString();
    });
}

function getItems() {
    return document.querySelectorAll('.prod-grid-item');
}

function productionLabel(n) {
    return n === 1 ? 'production' : 'productions';
}

function getServerTotalCount() {
    if (!countEl) return 0;
    const raw = countEl.getAttribute('data-total');
    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function applySearchOnly() {
    const query = (searchInput?.value ?? '').trim().toLowerCase();
    const items = getItems();

    let visible = 0;

    items.forEach(item => {
        const title = (item.dataset.title ?? '').toLowerCase();
        const show = title.includes(query);

        item.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const total = getServerTotalCount();

    if (countEl) {
        if (query.length > 0) {
            countEl.textContent = `${visible} of ${total} ${productionLabel(total)}`;
        } else {
            countEl.textContent = `${items.length} of ${total} ${productionLabel(total)}`;
        }
    }

    if (noResults) {
        const shouldShowNoResults = query.length > 0 && visible === 0;
        noResults.classList.toggle('d-none', !shouldShowNoResults);
    }

    if (topPaginationWrap) {
        topPaginationWrap.classList.toggle('d-none', query.length > 0);
    }
    
    if (bottomPaginationWrap) {
        bottomPaginationWrap.classList.toggle('d-none', query.length > 0);
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

searchInput?.addEventListener('input', applySearchOnly);

document.addEventListener('DOMContentLoaded', () => {
    runEntranceAnimation();
    applySearchOnly();
});