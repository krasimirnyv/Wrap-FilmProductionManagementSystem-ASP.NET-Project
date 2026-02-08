/* =========================
   AUTOSAVE MODULE (FIXED)
   Supports both new blocks (no ID) and existing blocks (with Guid)
   ========================= */

const AUTOSAVE_DELAY = 500;
const autosaveTimers = new Map();

function scheduleAutosave(block) {
    // Use data-block-id if exists, otherwise use a temp key
    const blockKey = block.dataset.blockId || `temp-${block.dataset.orderIndex || Date.now()}`;

    // Visual indicator
    setSaveStatus("saving");

    // Clear old timer for this block
    if (autosaveTimers.has(blockKey)) {
        clearTimeout(autosaveTimers.get(blockKey));
    }

    const timer = setTimeout(() => {
        autosaveBlock(block);
    }, AUTOSAVE_DELAY);

    autosaveTimers.set(blockKey, timer);
}

async function autosaveBlock(block) {
    const scriptId = document.querySelector(".script-editor")?.dataset.scriptId;
    if (!scriptId) {
        console.error("Script ID not found");
        return;
    }

    const payload = {
        scriptId: scriptId,
        blockId: block.dataset.blockId || null, // null for new blocks
        orderIndex: parseInt(block.dataset.orderIndex) || 0,
        blockType: block.dataset.blockType,
        content: block.innerHTML, // Preserve HTML formatting
        metadata: block.dataset.metadata || null
    };

    try {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        const response = await fetch("/Scripts/AutosaveBlock", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`Autosave failed: ${response.status}`);
        }

        const result = await response.json();

        // Update block ID if this was a new block
        if (result.blockId && !block.dataset.blockId) {
            block.dataset.blockId = result.blockId;
        }

        setSaveStatus("saved");
    } catch (err) {
        console.error("Autosave error:", err);
        setSaveStatus("error");
    }
}

function setSaveStatus(state) {
    const el = document.getElementById("scriptStatus");
    if (!el) return;

    el.className = "script-status " + state;

    if (state === "saving") el.textContent = "Saving...";
    if (state === "saved") {
        el.textContent = "Saved";
        // Auto-hide after 2s
        setTimeout(() => {
            if (el.textContent === "Saved") {
                el.textContent = "";
            }
        }, 2000);
    }
    if (state === "error") el.textContent = "⚠ Error saving";
}

// Full script save (on demand)
async function saveFullScript() {
    const scriptId = document.querySelector(".script-editor")?.dataset.scriptId;
    if (!scriptId) return;

    setSaveStatus("saving");

    const blocks = Array.from(document.querySelectorAll(".script-block")).map((block, index) => ({
        id: block.dataset.blockId || null,
        orderIndex: index,
        blockType: block.dataset.blockType,
        content: block.innerHTML,
        metadata: block.dataset.metadata || null
    }));

    const payload = {
        scriptId: scriptId,
        blocks: blocks
    };

    try {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        const response = await fetch("/Scripts/SaveScript", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error("Full save failed");
        }

        const result = await response.json();

        // Update all block IDs
        result.blocks?.forEach((savedBlock, idx) => {
            const block = document.querySelectorAll(".script-block")[idx];
            if (block) {
                block.dataset.blockId = savedBlock.id;
            }
        });

        setSaveStatus("saved");
    } catch (err) {
        console.error("Full save error:", err);
        setSaveStatus("error");
    }
}

// Auto-save on visibility change (user leaves tab)
document.addEventListener("visibilitychange", () => {
    if (document.hidden) {
        saveFullScript();
    }
});

// Save before unload
window.addEventListener("beforeunload", (e) => {
    const unsaved = autosaveTimers.size > 0;
    if (unsaved) {
        e.preventDefault();
        e.returnValue = "You have unsaved changes. Are you sure you want to leave?";
    }
});
