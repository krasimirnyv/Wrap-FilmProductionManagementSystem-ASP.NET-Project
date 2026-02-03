// =========================
// PAGE METRICS
// =========================
function getUsablePageHeight(page) {
    const style = window.getComputedStyle(page);
    const paddingTop = parseFloat(style.paddingTop);
    const paddingBottom = parseFloat(style.paddingBottom);

    // clientHeight включва padding, затова го изваждаме
    return page.clientHeight - paddingTop - paddingBottom;
}

function fitsInPage(page, blockHeight) {
    const usable = getUsablePageHeight(page);
    return (page._usedHeight + blockHeight) <= usable;
}

// =========================
// PAGE CREATION
// =========================
function createNewPage(pageNumber = 1) {
    const page = document.createElement("div");
    page.classList.add("script-page");

    // incremental page height tracking
    page._usedHeight = 0;

    if (pageNumber > 1) {
        const pageNum = document.createElement("div");
        pageNum.classList.add("script-page-number");
        pageNum.textContent = pageNumber;
        page.appendChild(pageNum);
    }

    const unlockBtn = document.createElement("button");
    unlockBtn.type = "button";
    unlockBtn.classList.add("unlock-page");
    unlockBtn.textContent = "Unlock";
    unlockBtn.addEventListener("click", () => {
        page.classList.remove("locked");
        page.querySelectorAll(".script-block")
            .forEach(b => b.setAttribute("contenteditable", "true"));
    });

    page.appendChild(unlockBtn);

    return page;
}

// =========================
// BLOCK HELPERS
// =========================
function cloneBlockWithText(original, htmlText) {
    const el = document.createElement("div");
    el.className = original.className;
    el.setAttribute("contenteditable", "true");
    el.dataset.blockType = original.dataset.blockType;
    el.innerHTML = htmlText;
    el.style.whiteSpace = "pre-wrap";
    return el;
}

function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

// =========================
// DIALOGUE SPLITTING
// =========================
function splitDialogueToFit(dialogueBlock, page) {
    const fullHtml = dialogueBlock.innerHTML || "";
    const fullText = dialogueBlock.innerText || "";

    // Split по думи за по-естествено разделяне
    const words = fullText.split(/\s+/).filter(Boolean);
    if (words.length < 2) return [null, dialogueBlock]; // няма какво да split-нем

    // temp контейнер за измерване
    const temp = cloneBlockWithText(dialogueBlock, "");
    page.appendChild(temp);

    let lo = 1;
    let hi = words.length - 1;
    let best = 0;

    while (lo <= hi) {
        const mid = Math.floor((lo + hi) / 2);
        const leftText = words.slice(0, mid).join(" ");
        temp.textContent = leftText;

        if (fitsInPage(page, temp)) {
            best = mid;
            lo = mid + 1;
        } else {
            hi = mid - 1;
        }
    }

    page.removeChild(temp);

    if (best === 0) {
        // не се побира дори малко -> оставяме за следващата страница
        return [null, cloneBlockWithText(dialogueBlock, dialogueBlock.innerHTML)];
    }

    // губи се inline formatting, не е DOM-aware
    const first = cloneBlockWithText(dialogueBlock, escapeHtml(words.slice(0, best).join(" ")));
    const second = cloneBlockWithText(dialogueBlock, escapeHtml(words.slice(best).join(" ")));

    return [first, second];
}

// =========================
// PAGINATION ENGINE
// =========================
function paginateByHeight() {
    const pagesContainer = document.querySelector(".script-pages");
    const allBlocks = Array.from(document.querySelectorAll(".script-block"));

    pagesContainer.innerHTML = "";

    let pageNumber = 1;
    let currentPage = createNewPage(pageNumber);
    pagesContainer.appendChild(currentPage);

    for (const block of allBlocks) {
        currentPage.appendChild(block);

        const blockHeight = block.offsetHeight || block.getBoundingClientRect().height;

        if (!fitsInPage(currentPage, blockHeight)) {
            // rollback
            currentPage.removeChild(block);

            // === Dialogue split logic ===
            if (block.dataset.blockType === "Dialogue") {
                const [first, second] = splitDialogueToFit(block, currentPage);

                if (first) {
                    currentPage.appendChild(first);
                    currentPage._usedHeight +=
                        first.offsetHeight || first.getBoundingClientRect().height;
                }

                currentPage = createNewPage(++pageNumber);
                pagesContainer.appendChild(currentPage);

                if (second) {
                    currentPage.appendChild(second);
                    currentPage._usedHeight +=
                        second.offsetHeight || second.getBoundingClientRect().height;
                }
            } else {
                // === Normal block ===
                currentPage = createNewPage(++pageNumber);
                pagesContainer.appendChild(currentPage);
                currentPage.appendChild(block);
                currentPage._usedHeight += blockHeight;
            }
        } else {
            currentPage._usedHeight += blockHeight;
        }
    }

    applyPageLocks();
}

function applyPageLocks() {
    const pages = Array.from(document.querySelectorAll(".script-page"));
    pages.forEach((p, idx) => {
        const isLast = idx === pages.length - 1;
        p.classList.toggle("locked", !isLast);

        // contenteditable toggle
        p.querySelectorAll(".script-block").forEach(b => {
            b.setAttribute("contenteditable", isLast ? "true" : "false");
        });
    });
}

// =========================
// PRINT
// =========================
document.getElementById("btnPrint")?.addEventListener("click", () => {
    paginateByHeight();
    window.print();
});