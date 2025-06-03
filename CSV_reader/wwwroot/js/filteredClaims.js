// ------------------------------- FILTERED CLAIMS --------------------------------------------

// this function updates the calculations after excluding claims by sending an AJAX request
// it runs the GetFilteredClaims method in the Calculations controller

document.addEventListener("DOMContentLoaded", function () {
    $("#updateCalculationsBtn").click(function () {
        let selectedClaims = [];
        $(".claim-checkbox:checked").each(function () {
            selectedClaims.push($(this).data("claim-ref"));
        });

        if (selectedClaims.length === 0) {
            alert("Zero claims included.");
            return
        }

        let batchId = $("#batchIdHidden").val();

        const selectedNumOfMonthsDropdown = document.getElementById("selectedNumOfMonthsDropdown");
        const projYearsDropdown = document.getElementById("projYearsDropdown");
        const COIFeeDropdown = document.getElementById("COIFeeDropdown");
        const pricingMetricDropdown = document.getElementById("pricingMetricDropdown");
        const priceByDropdown = document.getElementById("priceByDropdown");

        const selectedNumOfMonths = selectedNumOfMonthsDropdown.value;
        const selectedProjYears = projYearsDropdown.value;
        const selectedCOIFee = COIFeeDropdown.value;
        const selectedPricingMetric = pricingMetricDropdown.value;
        const selectedPriceBy = priceByDropdown.value;

        console.log("Batch ID:", batchId);
        console.log("Selected Num of Months:", selectedNumOfMonths);
        console.log("Projection Years:", selectedProjYears);
        console.log("COI Fee:", selectedCOIFee);
        console.log("Pricing Metric:", selectedPricingMetric);
        console.log("Price By:", selectedPriceBy);
        console.log("Selected Claims:", selectedClaims);

        let requestData = {
            batchId: batchId,
            selectedNumOfMonths: selectedNumOfMonths,
            projYears: selectedProjYears,
            chargeCOIFee: selectedCOIFee,
            pricingMetric: selectedPricingMetric,
            priceBy: selectedPriceBy,
            selectedClaims: selectedClaims
        };

        console.log("request data: ", requestData);

        const loadingIndicator = document.getElementById("loadingIndicator");
        loadingIndicator.style.display = "flex";

        $.ajax({
            url: "/Calculations/GetFilteredClaims",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(requestData),
            dataType: "html",
            success: function (response) {
                $("#calculationsTablesContainer").html(response);

                setTimeout(() => {
                    loadingIndicator.style.display = "none";
                }, 3000);

                // Trigger an event so the tab switching logic gets reapplied
                document.dispatchEvent(new Event("ajaxComplete"));
            },
            error: function (xhr, status, error) {
                console.log("Error:", error);
                console.log("Response:", xhr.responseText);
                setTimeout(() => {
                    loadingIndicator.style.display = "none";
                }, 500);
            }
        });

    });

});