// Character counter for biography
$(document).ready(function() {
    const bioField = $('#Biography');
    const counter = $('#bioCharCount');

    bioField.on('input', function() {
        counter.text($(this).val().length);
    });

    // Initialize
    counter.text(bioField.val().length);
});

document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById("profileImageInput");
    const img = document.getElementById("previewImg");
    const preview = document.getElementById("imagePreview");
    const placeholder = preview?.querySelector(".placeholder-text");

    if (!input || !img || !preview) return;

    input.addEventListener("change", () => {
        const file = input.files && input.files[0];
        if (!file) {
            img.style.display = "none";
            preview.classList.remove("has-image");
            if (placeholder) placeholder.style.display = "";
            return;
        }

        if (file.size > 10 * 1024 * 1024) {
            alert("File size must be less than 10MB.");
            input.value = "";
            return;
        }

        const url = URL.createObjectURL(file);
        img.src = url;
        img.style.display = "block";
        preview.classList.add("has-image");
        if (placeholder) placeholder.style.display = "none";
    });
});
