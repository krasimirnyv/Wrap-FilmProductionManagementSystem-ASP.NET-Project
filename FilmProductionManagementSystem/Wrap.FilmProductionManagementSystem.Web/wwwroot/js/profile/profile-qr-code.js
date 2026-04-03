window.addEventListener("load", () => {
    const dataEl = document.getElementById("qrCodeData");
    const qrEl = document.getElementById("qrCode");

    if (!dataEl || !qrEl) return;

    const uri = dataEl.getAttribute("data-url");
    if (!uri) return;

    // clear previous QR if any
    qrEl.innerHTML = "";

    new QRCode(qrEl, {
        text: uri,
        width: 180,
        height: 180,
        correctLevel: QRCode.CorrectLevel.L
    });
});