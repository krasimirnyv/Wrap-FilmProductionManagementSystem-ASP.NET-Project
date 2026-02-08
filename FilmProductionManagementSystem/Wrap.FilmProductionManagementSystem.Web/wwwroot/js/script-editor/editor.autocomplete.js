/* =========================
   CHARACTER AUTOCOMPLETE
   ========================= */

const characterNamesSet = new Set();
const AUTOCOMPLETE_DELAY = 300;
let autocompleteTimer = null;

// Initialize from datalist
function initAutocomplete() {
    const datalist = document.getElementById("characterNames");
    if (datalist) {
        datalist.querySelectorAll("option").forEach(opt => {
            if (opt.value) characterNamesSet.add(opt.value.toUpperCase());
        });
    }
}

// Extract character names from script
function extractCharacterNames() {
    document.querySelectorAll('.script-block[data-block-type="Character"]').forEach(block => {
        const name = block.textContent.trim().toUpperCase();
        if (name && name.length > 0) {
            characterNamesSet.add(name);
        }
    });
    updateDatalist();
}

// Update the datalist element
function updateDatalist() {
    const datalist = document.getElementById("characterNames");
    if (!datalist) return;

    datalist.innerHTML = "";
    Array.from(characterNamesSet).sort().forEach(name => {
        const option = document.createElement("option");
        option.value = name;
        datalist.appendChild(option);
    });
}

// Show autocomplete suggestions
function showAutocompleteSuggestions(block) {
    if (block.dataset.blockType !== "Character") return;

    const text = block.textContent.trim().toUpperCase();
    if (text.length < 2) return;

    const matches = Array.from(characterNamesSet).filter(name => 
        name.startsWith(text) && name !== text
    );

    if (matches.length === 0) return;

    // Create/update suggestion box
    let suggestionBox = document.getElementById("autocomplete-suggestions");
    if (!suggestionBox) {
        suggestionBox = document.createElement("div");
        suggestionBox.id = "autocomplete-suggestions";
        suggestionBox.className = "autocomplete-suggestions";
        document.body.appendChild(suggestionBox);
    }

    // Position below block
    const rect = block.getBoundingClientRect();
    suggestionBox.style.top = `${rect.bottom + window.scrollY}px`;
    suggestionBox.style.left = `${rect.left + window.scrollX}px`;

    // Populate suggestions
    suggestionBox.innerHTML = "";
    matches.slice(0, 5).forEach(name => {
        const item = document.createElement("div");
        item.className = "autocomplete-item";
        item.textContent = name;
        item.addEventListener("click", () => {
            block.textContent = name;
            hideAutocompleteSuggestions();
            placeCaretAtEnd(block);
        });
        suggestionBox.appendChild(item);
    });

    suggestionBox.style.display = "block";
}

function hideAutocompleteSuggestions() {
    const box = document.getElementById("autocomplete-suggestions");
    if (box) box.style.display = "none";
}

function placeCaretAtEnd(el) {
    el.focus();
    const range = document.createRange();
    const sel = window.getSelection();
    if (el.childNodes.length > 0) {
        range.setStart(el.childNodes[0], el.textContent.length);
    } else {
        range.setStart(el, 0);
    }
    range.collapse(true);
    sel.removeAllRanges();
    sel.addRange(range);
}

// Schedule autocomplete check
function scheduleAutocomplete(block) {
    clearTimeout(autocompleteTimer);
    autocompleteTimer = setTimeout(() => {
        showAutocompleteSuggestions(block);
    }, AUTOCOMPLETE_DELAY);
}

// Hook into input events
document.addEventListener("input", function(e) {
    const block = e.target;
    if (!isScriptBlock(block)) return;

    if (block.dataset.blockType === "Character") {
        scheduleAutocomplete(block);
        
        // Add to set when user finishes typing
        clearTimeout(autocompleteTimer);
        autocompleteTimer = setTimeout(() => {
            const name = block.textContent.trim().toUpperCase();
            if (name.length > 0) {
                characterNamesSet.add(name);
                updateDatalist();
            }
        }, 1000);
    }
});

// Hide on blur
document.addEventListener("focusout", function(e) {
    setTimeout(() => {
        if (!document.querySelector(".autocomplete-item:hover")) {
            hideAutocompleteSuggestions();
        }
    }, 200);
});

// Initialize on load
initAutocomplete();
extractCharacterNames();
