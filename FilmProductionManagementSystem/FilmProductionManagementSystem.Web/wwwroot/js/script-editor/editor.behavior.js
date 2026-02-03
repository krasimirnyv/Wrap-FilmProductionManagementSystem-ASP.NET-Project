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
    scheduleAutosave(block); // ⬅ autosave hook
}

function onPaste(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    handlePaste(e, block);
}

function onFocusOut(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    autosaveBlock(block);
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

function handleAutoUppercase(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    const type = getBlockType(block);
    if (!shouldAutoUppercase(type)) return;

    const selection = window.getSelection();
    if (!selection.rangeCount) return;

    const range = selection.getRangeAt(0);
    const cursorPosition = range.startOffset;

    if (block.textContent === block.textContent.toUpperCase()) return;

    block.textContent = block.textContent.toUpperCase();

    const textNode = block.firstChild;
    if (textNode) {
        range.setStart(textNode, Math.min(cursorPosition, textNode.length));
        range.collapse(true);
        selection.removeAllRanges();
        selection.addRange(range);
    }
}

/* =========================
   PASTE HANDLING
   ========================= */

function handlePaste(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

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
    Action: "Action",
    Character: "Dialogue",
    Extension: "Dialogue",
    Parenthetical: "Dialogue",
    Dialogue: "Dialogue",
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
    block.after(newBlock);
    placeCaretAtStart(newBlock);
}

/* =========================
   TAB KEY LOGIC
   ========================= */

const tabFlow = {
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
}

/* =========================
   BLOCK CREATION
   ========================= */

function createBlock(type) {
    const block = document.createElement("div");
    block.classList.add("script-block", type.toLowerCase());
    block.setAttribute("contenteditable", "true");
    block.dataset.blockType = type;
    block.innerHTML = "";
    return block;
}

/* =========================
   TOOLBAR
   ========================= */

function bindToolbarButtons() {
    document.querySelectorAll(".script-toolbar button").forEach(btn => {
        btn.addEventListener("click", () => {
            const type = btn.dataset.block;
            const block = createBlock(type);

            document.querySelector(".script-pages").appendChild(block);
            placeCaretAtStart(block);
        });
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

    scheduleUndoSnapshot();

    if (isUndo(e)) {
        e.preventDefault();
        undo();
        return;
    }

    if (isRedo(e)) {
        e.preventDefault();
        redo();
        return;
    }

    if (e.key === "Enter") {
        handleEnterKey(e);
        return;
    }

    if (e.key === "Tab") {
        handleTabKey(e);
    }
});

bindToolbarButtons();