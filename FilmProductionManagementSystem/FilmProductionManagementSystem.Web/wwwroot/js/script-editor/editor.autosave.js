const AUTOSAVE_DELAY = 500;
const autosaveTimers = new Map();

function scheduleAutosave(block) {
    const blockId = block.dataset.blockId;
    if (!blockId) return;

    // визуален индикатор
    setSaveStatus("saving");

    // чистим стария таймер за този block
    if (autosaveTimers.has(blockId)) {
        clearTimeout(autosaveTimers.get(blockId));
    }

    const timer = setTimeout(() => {
        autosaveBlock(block);
    }, AUTOSAVE_DELAY);

    autosaveTimers.set(blockId, timer);
}

async function autosaveBlock(block) {
    const blockId = block.dataset.blockId;
    if (!blockId) return;

    const payload = {
        blockId: blockId,
        blockType: block.dataset.blockType,
        content: block.innerHTML
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
            throw new Error("Autosave failed");
        }

        setSaveStatus("saved");
    } catch (err) {
        console.error(err);
        setSaveStatus("error");
    }
}

function setSaveStatus(state) {
    const el = document.getElementById("scriptStatus");
    if (!el) return;

    el.className = "script-status " + state;

    if (state === "saving") el.textContent = "Saving...";
    if (state === "saved") el.textContent = "Saved";
    if (state === "error") el.textContent = "Error saving";
}