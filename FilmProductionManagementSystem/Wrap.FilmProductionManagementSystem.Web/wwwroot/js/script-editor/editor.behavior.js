/* =========================
   EDITOR BEHAVIOR (FIXED)
   Enhanced with order index management
   ========================= */

/* =========================
   HELPERS
   ========================= */

function getBlockType(block) {
    return block.dataset.blockType;
}

function setBlockType(block, type) {
    block.dataset.blockType = type;
    block.className = `script-block ${type.toLowerCase()}`;
}

function onInput(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    handleAutoUppercase(block);

    // Schedule autosave (from autosave module)
    if (typeof scheduleAutosave === "function") {
        scheduleAutosave(block);
    }
}

function onPaste(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    handlePaste(e, block);
}

function onFocusOut(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    // Immediate save on blur
    if (typeof autosaveBlock === "function") {
        autosaveBlock(block);
    }
}

function isUndo(e) {
    return e.ctrlKey && e.key === "z";
}

function isRedo(e) {
    return e.ctrlKey && (e.key === "y" || (e.shiftKey && e.key === "Z"));
}

/* =========================
   AUTO UPPERCASE
   ========================= */

function shouldAutoUppercase(type) {
    return type === "SceneHeading" || type === "Character" || type === "Transition";
}

function handleAutoUppercase(block) {
    const type = getBlockType(block);
    if (!shouldAutoUppercase(type)) return;

    const selection = window.getSelection();
    if (!selection.rangeCount) return;

    const range = selection.getRangeAt(0);
    const cursorPosition = range.startOffset;

    if (block.textContent === block.textContent.toUpperCase()) return;

    block.textContent = block.textContent.toUpperCase();

    const textNode = block.firstChild;
    if (textNode && textNode.nodeType === Node.TEXT_NODE) {
        range.setStart(textNode, Math.min(cursorPosition, textNode.length));
        range.collapse(true);
        selection.removeAllRanges();
        selection.addRange(range);
    }
}

/* =========================
   PASTE HANDLING
   ========================= */

function handlePaste(e, block) {
    const type = getBlockType(block);
    if (!shouldAutoUppercase(type)) return;

    e.preventDefault();

    const text = (e.clipboardData || window.clipboardData)
        .getData("text")
        .toUpperCase();

    document.execCommand("insertText", false, text);
}

/* =========================
   ENTER KEY LOGIC
   ========================= */

const enterFlow = {
    SceneHeading: "Action",
    Subheader: "Action",
    Action: "Action",
    Character: "Dialogue",
    Extension: "Dialogue",
    Parenthetical: "Dialogue",
    Dialogue: "Action",  // After dialogue, default to Action (user can type Character)
    Transition: "SceneHeading"
};

function handleEnterKey(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    if (e.shiftKey) return; // soft line break

    e.preventDefault();

    const currentType = getBlockType(block);
    const nextType = enterFlow[currentType] || "Action";

    const newBlock = createBlock(nextType);

    // Set order index (current + 1)
    const currentOrder = parseInt(block.dataset.orderIndex) || 0;
    newBlock.dataset.orderIndex = (currentOrder + 1).toString();

    // Insert after current block
    block.after(newBlock);

    // Reindex all blocks after insertion
    reindexBlocks();

    placeCaretAtStart(newBlock);

    // Schedule pagination update
    if (typeof schedulePagination === "function") {
        schedulePagination();
    }
}

/* =========================
   TAB KEY LOGIC
   ========================= */

const tabFlow = {
    Action: "Character",
    Character: "Extension",
    Extension: "Parenthetical",
    Parenthetical: "Dialogue"
};

function handleTabKey(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    const currentType = getBlockType(block);
    const nextType = tabFlow[currentType];

    if (!nextType) return;

    e.preventDefault();
    setBlockType(block, nextType);

    // Auto-uppercase if needed
    if (shouldAutoUppercase(nextType)) {
        handleAutoUppercase(block);
    }
}

/* =========================
   BLOCK CREATION
   ========================= */

function createBlock(type) {
    const block = document.createElement("div");
    block.classList.add("script-block", type.toLowerCase());
    block.setAttribute("contenteditable", "true");
    block.dataset.blockType = type;
    block.dataset.blockId = ""; // Empty until saved
    block.innerHTML = "";
    return block;
}

/* =========================
   ORDER INDEX MANAGEMENT
   ========================= */

function reindexBlocks() {
    const blocks = Array.from(document.querySelectorAll(".script-block"));
    blocks.forEach((block, index) => {
        block.dataset.orderIndex = index.toString();
    });
}

function insertBlockAtIndex(block, index) {
    const allBlocks = Array.from(document.querySelectorAll(".script-block"));

    if (index >= allBlocks.length) {
        // Append at end
        document.querySelector(".script-pages").appendChild(block);
    } else {
        // Insert before block at index
        allBlocks[index].before(block);
    }

    reindexBlocks();
}

/* =========================
   BLOCK DELETION
   ========================= */

function deleteBlock(block) {
    const blockId = block.dataset.blockId;

    // Remove from DOM
    block.remove();

    // Reindex remaining blocks
    reindexBlocks();

    // If block had ID, mark for deletion on server
    if (blockId && blockId !== "") {
        // TODO: Add to deletion queue and sync on next save
        console.log("Marked for deletion:", blockId);
    }

    // Schedule pagination update
    if (typeof schedulePagination === "function") {
        schedulePagination();
    }
}

// Delete with Backspace on empty block
document.addEventListener("keydown", function(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    if (e.key === "Backspace" && block.textContent.trim() === "") {
        e.preventDefault();

        const prevBlock = block.previousElementSibling;
        if (prevBlock && isScriptBlock(prevBlock)) {
            deleteBlock(block);
            placeCaretAtEnd(prevBlock);
        }
    }
});

/* =========================
   TOOLBAR
   ========================= */

function bindToolbarButtons() {
    document.querySelectorAll(".script-toolbar .btn-block").forEach(btn => {
        btn.addEventListener("click", () => {
            const type = btn.dataset.block;
            const block = createBlock(type);

            // Find last block and insert after
            const lastBlock = document.querySelector(".script-block:last-of-type");
            if (lastBlock) {
                lastBlock.after(block);
            } else {
                document.querySelector(".script-pages").appendChild(block);
            }

            reindexBlocks();
            placeCaretAtStart(block);

            if (typeof schedulePagination === "function") {
                schedulePagination();
            }
        });
    });

    // Undo/Redo buttons
    document.getElementById("btnUndo")?.addEventListener("click", () => {
        if (typeof undo === "function") undo();
    });

    document.getElementById("btnRedo")?.addEventListener("click", () => {
        if (typeof redo === "function") redo();
    });
}

/* =========================
   GLOBAL EVENT BINDINGS
   ========================= */

document.addEventListener("input", onInput);
document.addEventListener("paste", onPaste);
document.addEventListener("focusout", onFocusOut);

document.addEventListener("keydown", function (e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    // Schedule undo snapshot (from history module)
    if (typeof scheduleUndoSnapshot === "function") {
        scheduleUndoSnapshot();
    }

    if (isUndo(e)) {
        e.preventDefault();
        if (typeof undo === "function") undo();
        return;
    }

    if (isRedo(e)) {
        e.preventDefault();
        if (typeof redo === "function") redo();
        return;
    }

    if (e.key === "Enter") {
        handleEnterKey(e);
        return;
    }

    if (e.key === "Tab") {
        handleTabKey(e);
        return;
    }
});

// Initialize
document.addEventListener("DOMContentLoaded", function() {
    bindToolbarButtons();
    reindexBlocks(); // Ensure all blocks have correct indices

    console.log("Editor behavior initialized");
});
