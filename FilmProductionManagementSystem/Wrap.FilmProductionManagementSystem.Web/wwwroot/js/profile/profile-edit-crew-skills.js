$(document).ready(function() {
    const selectedSkills = new Set();
    const hiddenInput = $('#hiddenSelectedSkills');

    // Initialize with current skills from hidden input
    const initialSkills = hiddenInput.val().split(',').filter(s => s);
    initialSkills.forEach(skill => selectedSkills.add(skill));

    // Update UI
    updateSelectedCount();
    updateAllDepartmentCounts();

    // ========================================
    // Skill bubble click handler
    // ========================================
    $('.skill-bubble').on('click', function() {
        const $bubble = $(this);
        // Convert to string for consistency
        const skillValue = String($bubble.data('skill'));
        const departmentId = $bubble.data('department');

        // Toggle selection
        if (selectedSkills.has(skillValue)) {
            // Remove skill
            selectedSkills.delete(skillValue);
            $bubble.removeClass('selected');
        } else {
            // Add skill
            selectedSkills.add(skillValue);
            $bubble.addClass('selected');
        }

        // Update UI
        updateSelectedCount();
        updateDepartmentCount(departmentId);
        updateHiddenInput();
    });

    // ========================================
    // Update total count
    // ========================================
    function updateSelectedCount() {
        $('#selectedCount').text(selectedSkills.size);
    }

    // ========================================
    // Update department badge count
    // ========================================
    function updateDepartmentCount(departmentId) {
        const count = $(`#${departmentId} .skill-bubble.selected`).length;
        $(`#count-${departmentId}`).text(count);
    }

    // ========================================
    // Update all department counts
    // ========================================
    function updateAllDepartmentCounts() {
        $('.accordion-item').each(function() {
            const departmentId = $(this).find('.accordion-collapse').attr('id');
            updateDepartmentCount(departmentId);
        });
    }

    // ========================================
    // Update hidden input for form submission
    // ========================================
    function updateHiddenInput() {
        hiddenInput.val(Array.from(selectedSkills).join(','));
    }

    // ========================================
    // Prevent form submission if no skills selected
    // ========================================
    $('#skillsForm').on('submit', function(e) {
        if (selectedSkills.size === 0) {
            e.preventDefault();
            alert('Please select at least one skill!');
            return false;
        }
    });
});