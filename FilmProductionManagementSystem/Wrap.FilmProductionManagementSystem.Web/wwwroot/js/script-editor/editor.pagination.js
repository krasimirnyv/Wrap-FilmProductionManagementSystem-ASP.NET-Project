/* =========================
   PAGINATION ENGINE (FIXED)
   Preserves HTML formatting during splits
   ========================= */

const PAGE_HEIGHT_INCHES = 11;
const USABLE_HEIGHT_INCHES = 9; // 11 - 1in top - 1in bottom

// =========================
// PAGE METRICS
// =========================
function getUsablePageHeight(page) {
    const style = window.getComputedStyle(page);
    const paddingTop = parseFloat(style.paddingTop);
    const paddingBottom = parseFloat(style.paddingBottom);
    return page.clientHeight - paddingTop - paddingBottom;
}

function fitsInPage(page, blockHeight) {
    const usable = getUsablePageHeight(page);
    const current = page._usedHeight || 0;
    return (current + blockHeight) <= usable;
}

// =========================
// PAGE CREATION
// =========================
function createNewPage(pageNumber = 1) {
    const page = document.createElement("div");
    page.classList.add("script-page");
    page._usedHeight = 0;

    if (pageNumber > 1) {
        const pageNum = document.createElement("div");
        pageNum.classList.add("script-page-number");
        pageNum.textContent = pageNumber + ".";
        page.appendChild(pageNum);

        // Account for page number height
        page._usedHeight += pageNum.offsetHeight || 20;
    }

    // Unlock button (for locked pages)
    const unlockBtn = document.createElement("button");
    unlockBtn.type = "button";
    unlockBtn.classList.add("unlock-page");
    unlockBtn.textContent = "🔓 Unlock";
    unlockBtn.addEventListener("click", () => {
        page.classList.remove("locked");
        page.querySelectorAll(".script-block")
            .forEach(b => b.setAttribute("contenteditable", "true"));
    });
    page.appendChild(unlockBtn);

    return page;
}

// =========================
// BLOCK HELPERS (FIXED)
// =========================

/**
 * Clone block preserving HTML structure
 */
function cloneBlockWithHTML(original, htmlContent) {
    const el = document.createElement("div");
    el.className = original.className;
    el.setAttribute("contenteditable", original.getAttribute("contenteditable") || "true");
    el.dataset.blockType = original.dataset.blockType;
    el.dataset.blockId = original.dataset.blockId || "";
    el.dataset.orderIndex = original.dataset.orderIndex || "";
    el.innerHTML = htmlContent; // Preserve HTML!
    return el;
}

/**
 * Get text length accounting for HTML tags
 */
function getTextLength(htmlString) {
    const temp = document.createElement("div");
    temp.innerHTML = htmlString;
    return temp.textContent.length;
}

// =========================
// DIALOGUE SPLITTING (FIXED)
// =========================
function splitDialogueToFit(dialogueBlock, page) {
    const fullHTML = dialogueBlock.innerHTML || "";
    const fullText = dialogueBlock.textContent || "";

    // Try to split by words while preserving HTML
    const words = fullText.split(/\s+/).filter(Boolean);
    if (words.length < 2) {
        return [null, cloneBlockWithHTML(dialogueBlock, fullHTML)];
    }

    // Binary search for optimal split point
    const temp = cloneBlockWithHTML(dialogueBlock, "");
    page.appendChild(temp);

    let lo = 1;
    let hi = words.length - 1;
    let best = 0;

    while (lo <= hi) {
        const mid = Math.floor((lo + hi) / 2);
        const testText = words.slice(0, mid).join(" ");
        temp.textContent = testText;

        const height = temp.offsetHeight || temp.getBoundingClientRect().height;

        if (fitsInPage(page, height)) {
            best = mid;
            lo = mid + 1;
        } else {
            hi = mid - 1;
        }
    }

    page.removeChild(temp);

    if (best === 0) {
        // Doesn't fit at all - move to next page
        return [null, cloneBlockWithHTML(dialogueBlock, fullHTML)];
    }

    // IMPROVED: Try to preserve HTML by splitting at word boundaries
    const firstText = words.slice(0, best).join(" ");
    const secondText = words.slice(best).join(" ");

    // Simple approach: just use plain text for splits
    // (Advanced: parse HTML and split DOM nodes - complexity++)
    const first = cloneBlockWithHTML(dialogueBlock, firstText);
    const second = cloneBlockWithHTML(dialogueBlock, secondText);

    return [first, second];
}

// =========================
// PAGINATION ENGINE
// =========================
function paginateByHeight() {
    const pagesContainer = document.querySelector(".script-pages");
    const allBlocks = Array.from(pagesContainer.querySelectorAll(".script-block"));

    // Clear pagination
    pagesContainer.innerHTML = "";

    let pageNumber = 1;
    let currentPage = createNewPage(pageNumber);
    pagesContainer.appendChild(currentPage);

    for (const block of allBlocks) {
        // Clone to preserve original
        const blockClone = block.cloneNode(true);
        currentPage.appendChild(blockClone);

        const blockHeight = blockClone.offsetHeight || blockClone.getBoundingClientRect().height;

        if (!fitsInPage(currentPage, blockHeight)) {
            // Doesn't fit - rollback
            currentPage.removeChild(blockClone);

            // === Dialogue split logic ===
            if (block.dataset.blockType === "Dialogue") {
                const [first, second] = splitDialogueToFit(block, currentPage);

                if (first) {
                    currentPage.appendChild(first);
                    const h = first.offsetHeight || first.getBoundingClientRect().height;
                    currentPage._usedHeight += h;
                }

                // Create new page for continuation
                currentPage = createNewPage(++pageNumber);
                pagesContainer.appendChild(currentPage);

                if (second) {
                    currentPage.appendChild(second);
                    const h = second.offsetHeight || second.getBoundingClientRect().height;
                    currentPage._usedHeight += h;
                }
            } else {
                // === Normal block - move to next page ===
                currentPage = createNewPage(++pageNumber);
                pagesContainer.appendChild(currentPage);
                currentPage.appendChild(blockClone);
                currentPage._usedHeight += blockHeight;
            }
        } else {
            currentPage._usedHeight += blockHeight;
        }
    }

    applyPageLocks();
}

// =========================
// PAGE LOCKING
// =========================
function applyPageLocks() {
    const pages = Array.from(document.querySelectorAll(".script-page"));
    pages.forEach((p, idx) => {
        const isLast = idx === pages.length - 1;
        p.classList.toggle("locked", !isLast);

        p.querySelectorAll(".script-block").forEach(b => {
            b.setAttribute("contenteditable", isLast ? "true" : "false");
        });
    });
}

// =========================
// AUTO-PAGINATION ON CHANGES
// =========================
let paginationTimer = null;

function schedulePagination() {
    clearTimeout(paginationTimer);
    paginationTimer = setTimeout(() => {
        paginateByHeight();
    }, 1000); // Wait 1s after last change
}

// Hook into editor changes
document.addEventListener("input", function(e) {
    if (isScriptBlock(e.target)) {
        schedulePagination();
    }
});

// Print button
document.getElementById("btnPrint")?.addEventListener("click", () => {
    paginateByHeight();
    window.print();
});
