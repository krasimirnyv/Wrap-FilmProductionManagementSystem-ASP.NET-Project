$(document).ready(function() {
    // Character counter for biography
    const bioField = $('#biographyField');
    const counter = $('#bioCharCount');

    bioField.on('input', function() {
        counter.text($(this).val().length);
    });

    // Initialize counter
    counter.text(bioField.val().length);

    // Image preview
    $('#profileImageInput').on('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            // Validate file type
            if (!file.type.startsWith('image/')) {
                alert('Please select an image file.');
                this.value = '';
                return;
            }

            // Validate file size (5MB max)
            if (file.size > 5 * 1024 * 1024) {
                alert('File size must be less than 5MB.');
                this.value = '';
                return;
            }

            // Preview image
            const reader = new FileReader();
            reader.onload = function(e) {
                $('#profilePreview').attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });

    // Confirm before leaving if form has changes
    let formChanged = false;
    $('form :input').on('change', function() {
        formChanged = true;
    });

    window.addEventListener('beforeunload', function (e) {
        if (formChanged) {
            e.preventDefault();
            e.returnValue = '';
        }
    });

    // Don't warn on submit
    $('form').on('submit', function() {
        formChanged = false;
    });
});