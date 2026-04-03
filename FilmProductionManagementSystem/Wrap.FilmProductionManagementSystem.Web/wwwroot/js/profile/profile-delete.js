(() => {
    const passwordInput = document.getElementById("passwordInput");
    const confirmCheck = document.getElementById("confirmCheck");
    const deleteButton = document.getElementById("deleteButton");
    const form = deleteButton?.closest("form");

    if (!passwordInput || !confirmCheck || !deleteButton) return;

    const updateState = () => {
        const hasPassword = passwordInput.value && passwordInput.value.trim().length > 0;
        const confirmed = confirmCheck.checked;
        deleteButton.disabled = !(hasPassword && confirmed);
    };

    passwordInput.addEventListener("input", updateState);
    confirmCheck.addEventListener("change", updateState);

    form?.addEventListener("submit", (e) => {
        updateState();
        if (deleteButton.disabled) e.preventDefault();
    });

    updateState();
})();