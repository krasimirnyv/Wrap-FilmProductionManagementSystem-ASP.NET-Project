const undoStack = [];
const redoStack = [];
const MAX_HISTORY = 50;

let undoTimer = null;

function deepClone(obj) {
    return JSON.parse(JSON.stringify(obj));
}

function serializeEditor() {
    return Array.from(document.querySelectorAll(".script-block"))
        .map(block => ({
            type: block.dataset.blockType,
            text: block.innerHTML
        }));
}

function restoreEditor(state) {
    const pagesContainer = document.querySelector(".script-pages");
    pagesContainer.innerHTML = "";

    const fragment = document.createDocumentFragment();

    state.forEach(block => {
        const el = document.createElement("div");
        el.className = `script-block ${block.type.toLowerCase()}`;
        el.contentEditable = "true";
        el.dataset.blockType = block.type;
        el.innerHTML = block.text;

        fragment.appendChild(el);
    });

    pagesContainer.appendChild(fragment);

    if (typeof paginateByHeight === "function") {
        paginateByHeight();
    }
}

function pushUndoState() {
    const snapshot = deepClone(serializeEditor());
    const last = undoStack[undoStack.length - 1];

    if (JSON.stringify(last) === JSON.stringify(snapshot))
        return;

    undoStack.push(snapshot);

    if (undoStack.length > MAX_HISTORY) {
        undoStack.shift();
    }

    redoStack.length = 0;
}

function scheduleUndoSnapshot(delay = 300) {
    clearTimeout(undoTimer);
    undoTimer = setTimeout(() => {
        pushUndoState();
    }, delay);
}

function undo() {
    if (!undoStack.length)
        return;

    redoStack.push(deepClone(serializeEditor()));
    restoreEditor(deepClone(undoStack.pop()));
}

function redo() {
    if (!redoStack.length)
        return;

    undoStack.push(deepClone(serializeEditor()));
    restoreEditor(deepClone(redoStack.pop()));
}
