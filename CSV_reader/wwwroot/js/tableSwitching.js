document.addEventListener("DOMContentLoaded", function () {

    // ------------- JavaScript for table switching DESKTOP ------------------

    const tableSelectDesktop = document.getElementById("tableSelectDesktop");

    function updateActiveSectionDesktop() {
        const selectedValue = tableSelectDesktop.value;
        const sections = document.querySelectorAll(".tab-content");

        sections.forEach(section => {
            section.classList.remove("active");
        });

        if (selectedValue) {
            const selectedSection = document.getElementById(selectedValue);
            if (selectedSection) {
                selectedSection.classList.add("active");
            }
        }
    }

    tableSelectDesktop.addEventListener("change", updateActiveSectionDesktop);

    // Ensure only the first section is visible when the page loads
    updateActiveSectionDesktop();

    // Reapply table switching after AJAX request
    document.addEventListener("ajaxComplete", function () {
        updateActiveSectionDesktop();
    });



    // ------------- JavaScript for table switching MOBILE ------------------

    const tableSelectMobile = document.getElementById("tableSelectMobile");

    function updateActiveSectionMobile() {
        const selectedValue = tableSelectMobile.value;
        const sections = document.querySelectorAll(".tab-content");

        sections.forEach(section => {
            section.classList.remove("active");
        });

        if (selectedValue) {
            const selectedSection = document.getElementById(selectedValue);
            if (selectedSection) {
                selectedSection.classList.add("active");
            }
        }
    }

    tableSelectMobile.addEventListener("change", updateActiveSectionMobile);

    // Ensure only the first section is visible when the page loads
    updateActiveSectionMobile();
     
    // Reapply table switching after AJAX request
    document.addEventListener("ajaxComplete", function () {
        updateActiveSectionMobile();
    });

});