/* =========================
   EDITOR PRINT MODULE
   ========================= */

let __printSnapshot = null;

/* Entry point */
function printScript() {
    prepareForPrint();
    window.print();
}

/* =========================
   PREPARE
   ========================= */

function prepareForPrint() {
    // Save current editor state
    __printSnapshot = serializeEditorForPrint();

    // Force clean pagination
    if (typeof paginateByHeight === "function") {
        paginateByHeight();
    }

    // Lock editor (read-only)
    lockEditorForPrint();

    // Hide UI elements
    togglePrintUI(true);

    // Hook restore after print
    window.addEventListener("afterprint", restoreAfterPrint, { once: true });
}

/* =========================
   RESTORE
   ========================= */

function restoreAfterPrint() {
    // Restore DOM
    restoreEditorFromPrint(__printSnapshot);

    // Unlock editor
    unlockEditorAfterPrint();

    // Show UI back
    togglePrintUI(false);

    __printSnapshot = null;
}

/* =========================
   SNAPSHOT
   ========================= */

function serializeEditorForPrint() {
    return Array.from(document.querySelectorAll(".script-block")).map(block => ({
        type: block.dataset.blockType,
        html: block.innerHTML
    }));
}

function restoreEditorFromPrint(snapshot) {
    const container = document.querySelector(".script-pages");
    container.innerHTML = "";

    const fragment = document.createDocumentFragment();

    snapshot.forEach(b => {
        const el = document.createElement("div");
        el.className = `script-block ${b.type.toLowerCase()}`;
        el.dataset.blockType = b.type;
        el.contentEditable = "true";
        el.innerHTML = b.html;
        fragment.appendChild(el);
    });

    container.appendChild(fragment);

    if (typeof paginateByHeight === "function") {
        paginateByHeight();
    }
}

/* =========================
   LOCKING
   ========================= */

function lockEditorForPrint() {
    document.querySelectorAll(".script-block").forEach(b => {
        b.setAttribute("contenteditable", "false");
    });
}

function unlockEditorAfterPrint() {
    document.querySelectorAll(".script-block").forEach(b => {
        b.setAttribute("contenteditable", "true");
    });
}

/* =========================
   UI VISIBILITY
   ========================= */

function togglePrintUI(isPrinting) {
    document.body.classList.toggle("print-mode", isPrinting);

    // toolbar
    document.querySelector(".script-toolbar")?.classList.toggle("hidden", isPrinting);

    // unlock buttons
    document.querySelectorAll(".unlock-page").forEach(btn => {
        btn.style.display = isPrinting ? "none" : "";
    });
}