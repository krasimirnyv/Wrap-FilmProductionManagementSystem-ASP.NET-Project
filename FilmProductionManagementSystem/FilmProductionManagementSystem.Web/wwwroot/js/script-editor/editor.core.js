function isScriptBlock(el) {
    return el && el?.classList?.contains("script-block");
}

function placeCaretAtStart(el) {
    el.focus();
    const range = document.createRange();
    const selection = window.getSelection();

    range.setStart(el, 0);
    range.collapse(true);

    selection.removeAllRanges();
    selection.addRange(range);
}
