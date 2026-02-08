/* =========================
   CREW SKILLS SELECTION
   Bubble button multi-select with department counters
   ========================= */

$(document).ready(function() {
    const selectedSkills = new Set();
    const form = $('#skillsForm');
    const submitBtn = $('#submitBtn');
    const selectedCountEl = $('#selectedCount');
    const hiddenInput = $('#hiddenSelectedSkills');

    // ===========================
    // BUBBLE BUTTON CLICK
    // ===========================
    
    $('.skill-bubble').on('click', function() {
        const bubble = $(this);
        const skillValue = bubble.data('skill').toString();
        const departmentId = bubble.data('department');

        if (bubble.hasClass('selected')) {
            // Deselect
            bubble.removeClass('selected');
            selectedSkills.delete(skillValue);
        } else {
            // Select
            bubble.addClass('selected');
            selectedSkills.add(skillValue);
        }

        updateDepartmentCount(departmentId);
        updateTotalCount();
        updateSubmitButton();
        updateHiddenInput();
    });

    // ===========================
    // UPDATE COUNTERS
    // ===========================

    function updateDepartmentCount(departmentId) {
        const department = $('#' + departmentId);
        const selectedInDepartment = department.find('.skill-bubble.selected').length;
        const counter = $('#count-' + departmentId);
        
        counter.text(selectedInDepartment);
        
        if (selectedInDepartment > 0) {
            counter.show();
        } else {
            counter.hide();
        }
    }

    function updateTotalCount() {
        selectedCountEl.text(selectedSkills.size);
    }

    function updateSubmitButton() {
        if (selectedSkills.size > 0) {
            submitBtn.prop('disabled', false);
        } else {
            submitBtn.prop('disabled', true);
        }
    }

    function updateHiddenInput() {
        // Convert Set to comma-separated string for form submission
        const skillsArray = Array.from(selectedSkills);
        
        // Clear existing hidden inputs
        $('input[name="SelectedSkills"]').not('#hiddenSelectedSkills').remove();
        
        // Add hidden input for each selected skill
        skillsArray.forEach(function(skill) {
            $('<input>')
                .attr('type', 'hidden')
                .attr('name', 'SelectedSkills')
                .val(skill)
                .appendTo(form);
        });
    }

    // ===========================
    // FORM SUBMISSION
    // ===========================

    form.on('submit', function(e) {
        if (selectedSkills.size === 0) {
            e.preventDefault();
            alert('Please select at least one skill');
            return false;
        }
    });

    // ===========================
    // SEARCH/FILTER (Optional Enhancement)
    // ===========================

    // Add search box functionality if needed
    const searchBox = $('#skillSearch');
    if (searchBox.length) {
        searchBox.on('input', function() {
            const query = $(this).val().toLowerCase();
            
            $('.skill-bubble').each(function() {
                const skillName = $(this).text().toLowerCase();
                if (skillName.includes(query)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });
    }

    // ===========================
    // KEYBOARD SHORTCUTS
    // ===========================

    $(document).on('keydown', function(e) {
        // Ctrl + A = Select all visible skills in current accordion
        if (e.ctrlKey && e.key === 'a') {
            const activeAccordion = $('.accordion-collapse.show');
            if (activeAccordion.length) {
                e.preventDefault();
                activeAccordion.find('.skill-bubble:visible').each(function() {
                    const bubble = $(this);
                    const skillValue = bubble.data('skill').toString();
                    const departmentId = bubble.data('department');
                    
                    if (!bubble.hasClass('selected')) {
                        bubble.addClass('selected');
                        selectedSkills.add(skillValue);
                    }
                });
                
                const departmentId = activeAccordion.attr('id');
                updateDepartmentCount(departmentId);
                updateTotalCount();
                updateSubmitButton();
                updateHiddenInput();
            }
        }
    });

    // ===========================
    // INITIALIZE
    // ===========================

    // Hide all department counters initially
    $('.department-count').hide();
    
    // If returning from validation error with previously selected skills
    // (can be implemented with TempData or ViewBag)
    // Example:
    // const preselected = @Html.Raw(Json.Serialize(Model.SelectedSkills ?? new List<int>()));
    // preselected.forEach(skill => {
    //     $('.skill-bubble[data-skill="' + skill + '"]').addClass('selected');
    //     selectedSkills.add(skill.toString());
    // });
});
