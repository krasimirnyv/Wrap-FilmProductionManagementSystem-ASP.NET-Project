/* =========================
   EXPORT MODULE
   Export script as JSON
   ========================= */

function exportScriptAsJSON() {
    const scriptEditor = document.querySelector(".script-editor");
    const titlePage = document.querySelector(".script-title-page");

    const scriptData = {
        metadata: {
            scriptId: scriptEditor?.dataset.scriptId,
            productionId: scriptEditor?.dataset.productionId,
            exportedAt: new Date().toISOString(),
            version: scriptEditor?.dataset.version
        },
        titlePage: {
            title: titlePage?.querySelector(".title")?.textContent || "",
            writtenBy: titlePage?.querySelector(".author")?.textContent || "",
            basedOn: titlePage?.querySelector(".based-on")?.textContent || null,
            contactInfo: titlePage?.querySelector(".contact")?.textContent || null
        },
        blocks: Array.from(document.querySelectorAll(".script-block")).map((block, index) => ({
            id: block.dataset.blockId || null,
            orderIndex: index,
            blockType: block.dataset.blockType,
            content: block.innerHTML,
            textContent: block.textContent.trim(),
            metadata: block.dataset.metadata || null
        })),
        statistics: {
            totalBlocks: document.querySelectorAll(".script-block").length,
            totalPages: document.querySelectorAll(".script-page").length,
            characterCount: Array.from(document.querySelectorAll(".script-block"))
                .reduce((sum, b) => sum + b.textContent.length, 0),
            sceneCount: document.querySelectorAll('.script-block[data-block-type="SceneHeading"]').length,
            dialogueBlocks: document.querySelectorAll('.script-block[data-block-type="Dialogue"]').length
        }
    };

    const jsonString = JSON.stringify(scriptData, null, 2);
    const blob = new Blob([jsonString], { type: "application/json" });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = `script-${scriptData.metadata.scriptId || "export"}-${Date.now()}.json`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);

    console.log("Script exported as JSON");
}

// Bind to export button
document.getElementById("btnExport")?.addEventListener("click", exportScriptAsJSON);

// Also add Ctrl+S to save
document.addEventListener("keydown", (e) => {
    if (e.ctrlKey && e.key === "s") {
        e.preventDefault();
        if (typeof saveFullScript === "function") {
            saveFullScript();
        }
    }
});
