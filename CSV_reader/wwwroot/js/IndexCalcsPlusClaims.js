
// This js file

document.addEventListener("DOMContentLoaded", function () {


    // ------------------ LOADING INDICATOR ---------------------------------
    const loadingDiv = document.getElementById("loadingIndicator");
    const mainContent = document.getElementById("mainContent");

    // Show the loading indicator
    loadingDiv.style.display = "flex";

    setTimeout(() => {
        // after 3 seconds, hide the loading indicator and show main content
        loadingDiv.style.display = "none";
        mainContent.style.display = "block";
    }, 1000);



    // -------------- HIDE/SHOW CLAIMS TABLE ----------------------

    const toggleTableBtn = document.getElementById("toggleTableBtn");
    const table = document.getElementById("claimsLists");

    // Add initial hidden state and transition
    table.classList.add("claims-list-hidden");
    table.classList.add("claims-list-transition");

    toggleTableBtn.addEventListener("click", function () {
        if (table.classList.contains("claims-list-hidden")) {
            table.classList.remove("claims-list-hidden");
            table.classList.add("claims-list-visible");
            this.textContent = "Hide Claims";
        } else {
            table.classList.remove("claims-list-visible");
            // Force a reflow to trigger the transition
            void table.offsetWidth;
            table.classList.add("claims-list-hidden");
            this.textContent = "Show Claims";
        }
    });



    // -------------- CLAIMS FILTERS -------------------------
    function applyFilters() {
        let selectedYear = document.getElementById("policyYearFilter").value;
        let selectedTotalIncurred = parseFloat(document.getElementById("totalIncurredFilter").value);
        let selectedVehType = document.getElementById("vehicleTypeFilter").value;

        let rows = document.querySelectorAll("#claimsTable tbody tr");

        rows.forEach(row => {
            let policyYear = row.cells[0].textContent.trim();
            let totalIncurredText = row.cells[17].textContent.trim().replace(/[^0-9.-]+/g, '');
            let totalIncurred = parseFloat(totalIncurredText) || 0;
            let vehicleType = row.cells[5].textContent.trim()

            // Apply filters together
            let yearMatch = (selectedYear === "" || policyYear === selectedYear);
            let incurredMatch = (isNaN(selectedTotalIncurred) || totalIncurred >= selectedTotalIncurred);
            let vehicleTypeMatch = (selectedVehType === "" || vehicleType === selectedVehType)

            if (yearMatch && incurredMatch && vehicleTypeMatch) {
                row.style.display = "";  // Show row
            } else {
                row.style.display = "none";  // Hide row
            }
        });
    }
    // Attach the filters to both dropdowns
    document.getElementById("policyYearFilter").addEventListener("change", applyFilters);
    document.getElementById("totalIncurredFilter").addEventListener("change", applyFilters);
    document.getElementById("vehicleTypeFilter").addEventListener("change", applyFilters);



    // ---------------------------- CHECKBOXES -----------------------------

    const selectAllCheckbox = document.getElementById("selectAllCheckbox");
    const claimCheckboxes = document.querySelectorAll(".claim-checkbox");
    const excludedClaimsTableBody = document.querySelector("#excludedClaimsTable tbody");

    // disable updateCalulations at first
    //toggleUpdateButton();

    // Handling select all checkbox functionality
    selectAllCheckbox.addEventListener("change", function () {
        claimCheckboxes.forEach(checkbox => {
            checkbox.checked = selectAllCheckbox.checked;
            updateClaimSelection(checkbox);
        });
        //toggleUpdateButton();
    });

    // Handling indiv claim checkbos selection
    claimCheckboxes.forEach(checkbox => {
        checkbox.addEventListener("change", function () {
            updateClaimSelection(checkbox);
            //toggleUpdateButton();

            // if any checkbox is unchecked, the select all checkbox is then unchecked
            // if all checkboxes are checked, then the select all checkbox is checked
            if (!this.checked) {
                selectAllCheckbox.checked = false;
            } else if ([...claimCheckboxes].every(cb => cb.checked)) {
                selectAllCheckbox.checked = true;
            }
        });
    });



    // Get claim data to display in excluded claims list
    function updateClaimSelection(checkbox) {

        let claimRef = checkbox.getAttribute("data-claim-ref");
        let policyYear = checkbox.getAttribute("data-policy-year");
        let totalIncurred = checkbox.getAttribute("data-total-incurred");
        let isChecked = checkbox.checked;

        let row = document.querySelector(`#claimsTable tbody tr[data-claim-ref="${claimRef}"]`);

        if (!isChecked) {

            // if claim is unchecked, add it to table
            addExcludedClaim(claimRef, policyYear, totalIncurred);
            if (row) row.classList.add("highlight");
        } else {
            // if claim is checked, remove from table
            removeExcludedClaim(claimRef);
            if (row) row.classList.remove("highlight");
        }
    }

    // create new row in excluded claims table and add info if it isnt already there
    function addExcludedClaim(claimRef, policyYear, totalIncurred) {

        // get the row by the claim ref
        let existingRow = document.querySelector(`#excludedClaimsTable tbody tr[data-claim-ref="${claimRef}"]`);

        if (!existingRow) {
            let newRow = document.createElement("tr");
            newRow.className = "hover:bg-blue-100  text-sm border-b border-blue-200";
            newRow.setAttribute("data-claim-ref", claimRef);
            newRow.innerHTML = `
                                <td class="p-3 text-sm">${policyYear}</td>
                                <td class="p-3 text-sm">${claimRef}</td>
                                <td class="p-3 text-sm">£${totalIncurred.toLocaleString("en-GB", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                            `;
            excludedClaimsTableBody.appendChild(newRow);
        }
    }

    // removes claim from excluded claims table
    function removeExcludedClaim(claimRef) {

        // get the row via the claim ref
        let row = document.querySelector(`#excludedClaimsTable tbody tr[data-claim-ref="${claimRef}"]`);
        if (row) {
            row.remove();
        }
    }




    /*// ------------------------------- FILTERED CLAIMS --------------------------------------------

    // this function updates the calculations after excluding claims by sending an AJAX request
    // it runs the GetFilteredClaims method in the Calculations controller
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

    });*/



    /*// ------------------- ADJUST ULTIMATE COSTS ----------------------------
    const adjustValuesButton = document.getElementById("adjustValuesButton");
    const resetAdjustmentButton = document.getElementById("resetValuesButton");

    const netPremium = document.getElementById("NetPremium");
    const grossPremium = document.getElementById("GrossPremium");
    const grossPremiumPlusIPT = document.getElementById("GrossPremiumPlusIPT");
    const commissions = document.getElementById("Commission");

    const initialNetVal = netPremium.textContent;
    const initialGrossVal = grossPremium.textContent;
    const initialGrossPlusIPT = grossPremiumPlusIPT.textContent;



    adjustValuesButton.addEventListener("click", function () {
        const adjustmentPercentage = parseFloat(document.getElementById("adjustmentPercentage").value);
        const adjustmentAmount = parseFloat(document.getElementById("adjustmentAmount").value);

        // Validation
        if (isNaN(adjustmentPercentage) && isNaN(adjustmentAmount)) {
            alert("Please enter a valid percentage or amount.");
            return;
        }

        let adjustmentMessage = "";

        if (!isNaN(adjustmentAmount)) {
            // Adjust using fixed amount
            const currentNetPrem = parseFloat(netPremium.textContent.replace(/[£,]/g, ""));
            const newNetPrem = currentNetPrem + adjustmentAmount;
            netPremium.textContent = `£${newNetPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            netPremium.style = "color: blue;";

            // Adjust gross premium
            const commissionsValue = parseFloat(commissions.textContent.replace(/[£,]/g, ""));
            const newGrossPrem = newNetPrem + commissionsValue;
            grossPremium.textContent = `£${newGrossPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremium.style = "color: blue;";

            // Adjust gross premium plus IPT
            const newGrossPremPlusIPT = newGrossPrem + newGrossPrem * (12 / 100);
            grossPremiumPlusIPT.textContent = `£${newGrossPremPlusIPT.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremiumPlusIPT.style = "color: blue;";

            adjustmentMessage = `Net Premium adjusted by £${adjustmentAmount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

        } else if (!isNaN(adjustmentPercentage)) {
            // Adjust using percentage
            const multiplier = 1 + adjustmentPercentage / 100;

            const currentNetPrem = parseFloat(netPremium.textContent.replace(/[£,]/g, ""));
            const newNetPrem = currentNetPrem * multiplier;
            netPremium.textContent = `£${newNetPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            netPremium.style = "color: blue;";

            const commissionsValue = parseFloat(commissions.textContent.replace(/[£,]/g, ""));
            const newGrossPrem = newNetPrem + commissionsValue;
            grossPremium.textContent = `£${newGrossPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremium.style = "color: blue;";

            const newGrossPremPlusIPT = newGrossPrem + newGrossPrem * (12 / 100);
            grossPremiumPlusIPT.textContent = `£${newGrossPremPlusIPT.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremiumPlusIPT.style = "color: blue;";

            adjustmentMessage = `${adjustmentPercentage}% Adjustment for Net Premium`;
        }


        // Update adjustment message
        const adjustmentDiv = document.getElementById("resultAdjustment");
        adjustmentDiv.textContent = adjustmentMessage;

        // Disable the Adjust button
        adjustValuesButton.disabled = true;
        adjustValuesButton.style = "background: lightblue;";

    });

    // --------------- RESET ADJUSTMENT ---------------------

    resetAdjustmentButton.addEventListener("click", function () {
        // Reset values
        netPremium.textContent = initialNetVal;
        grossPremium.textContent = initialGrossVal;
        grossPremiumPlusIPT.textContent = initialGrossPlusIPT;

        // Reset styles
        const valuesArray = [netPremium, grossPremium, grossPremiumPlusIPT];
        for (var x of valuesArray) {
            x.style.color = "";
        }

        // Re-enable the Adjust Values button
        adjustValuesButton.disabled = false;
        adjustValuesButton.style.background = "";

        // Reset input fields
        document.getElementById("adjustmentPercentage").value = "";
        document.getElementById("adjustmentAmount").value = "";

        // Clear adjustment message
        const adjustmentDiv = document.getElementById("resultAdjustment");
        adjustmentDiv.textContent = "";
    });*/




/*    // ------------------- Generate pdf button ----------------------

    document.getElementById("generatePdfButton").addEventListener("click", function () {
        let params = new URLSearchParams(document.location.search)
        let batchId = params.get("batchId")
        var quoteId = batchId.split("_")[0];

        let adjustmentNotes = document.getElementById("adjustmentNotes").value

        let claimsAmountValue = parseFloat(document.getElementById("ProjClaimsAmount").textContent.replace(/[£,]/g, ''));
        let largeLossValue = parseFloat(document.getElementById("ProjLLFund").textContent.replace(/[£,]/g, ''));
        let reinsuranceCostsValue = parseFloat(document.getElementById("ReinsuranceCosts").textContent.replace(/[£,]/g, ''));
        let claimsHandlingFeeValue = parseFloat(document.getElementById("ProjClaimsHandlingFee").textContent.replace(/[£,]/g, ''));
        let leviesValue = parseFloat(document.getElementById("Levies").textContent.replace(/[£,]/g, ''));
        let expensesValue = parseFloat(document.getElementById("PretiumExpenses").textContent.replace(/[£,]/g, ''));
        let profitValue = parseFloat(document.getElementById("Profit").textContent.replace(/[£,]/g, ''));
        let netPremiumValue = parseFloat(document.getElementById("NetPremium").textContent.replace(/[£,]/g, ''));
        let commissionsValue = parseFloat(document.getElementById("Commission").textContent.replace(/[£,]/g, ''))
        let grossPremiumValue = parseFloat(document.getElementById("GrossPremium").textContent.replace(/[£,]/g, ''))
        let formattedGrossPremiumPlusIPT = parseFloat(document.getElementById("GrossPremiumPlusIPT").textContent.replace(/[£,]/g, ''))

        let FCDaysCOIValue = parseFloat(document.getElementById("FCDaysCOI").textContent.replace(/[£,]/g, ''))
        let FCDaysNonCOIValue = parseFloat(document.getElementById("FCDaysNonCOI").textContent.replace(/[£,]/g, ''))
        let FCTurnoverCOIValue = parseFloat(document.getElementById("FCTurnoverCOI").textContent.replace(/[£,]/g, ''))
        let FCTurnoverNonCOIValue = parseFloat(document.getElementById("FCTurnoverNonCOI").textContent.replace(/[£,]/g, ''))

        console.log(`adjustmentNotes: ${adjustmentNotes}`)
        console.log(`quoteId: ${quoteId}`)
        console.log(`updatedGrossPremiumPlusIPT: ${formattedGrossPremiumPlusIPT}`)

        fetch('/PDF/GeneratePDF', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                batchId: batchId,

                claimsAmount: claimsAmountValue,
                largeLossFund: largeLossValue,
                reinsuranceCosts: reinsuranceCostsValue,
                claimsHandlingFee: claimsHandlingFeeValue,
                levies: leviesValue,
                expenses: expensesValue,
                profit: profitValue,
                netPremium: netPremiumValue,
                commissions: commissionsValue,
                grossPremium: grossPremiumValue,
                updatedGrossPremiumPlusIPT: formattedGrossPremiumPlusIPT,
                adjustmentNotes: adjustmentNotes,
                FCDaysCOI: FCDaysCOIValue,
                FCDaysNonCOI: FCDaysNonCOIValue,
                FCTurnoverCOI: FCTurnoverCOIValue,
                FCTurnoverNonCOI: FCTurnoverNonCOIValue

            }),
        })
            .then(response => {
                console.log("Response received:", response);
                if (!response.ok) {
                    throw new Error('Failed to generate PDF');
                }
                return response.json();
            })
            .then(data => {
                console.log("pdf data: ", data)
                if (!data.pdfBase64) {
                    throw new Error('Missing PDF data in response');
                }
                const pdfBase64 = data.pdfBase64; // Extract the Base64 string from the response
                downloadPDF(pdfBase64); // Call download method
            })
            .catch(error => {
                console.error('Error generating PDF:', error);
                alert('Failed to generate PDF. Please try again.');
            });
    });*/



    // ------------------------- FORM PARAMETERS ----------------------------

    const combinedForm = document.getElementById("combinedForm");
    const goButton = document.getElementById("goButton");
    const selectedNumOfMonthsDropdown = document.getElementById("selectedNumOfMonthsDropdown");
    const projYearsDropdown = document.getElementById("projYearsDropdown");
    const COIFeeDropdown = document.getElementById("COIFeeDropdown");
    const pricingMetricDropdown = document.getElementById("pricingMetricDropdown");
    const priceByDropdown = document.getElementById("priceByDropdown");

    combinedForm.addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent form from submitting normally

        const selectedNumOfMonths = selectedNumOfMonthsDropdown.value;
        const selectedProjYears = projYearsDropdown.value;
        const selectedCOIFee = COIFeeDropdown.value;
        const selectedPricingMetric = pricingMetricDropdown.value;
        const selectedPriceBy = priceByDropdown.value;

        console.log("selectedPriceBy: ", selectedPriceBy);

        // Update the URL parameters without reloading the page
        const url = new URL(window.location);
        url.searchParams.set('selectedNumOfMonths', selectedNumOfMonths);
        url.searchParams.set('projYears', selectedProjYears);
        url.searchParams.set('chargeCOIFee', selectedCOIFee);
        url.searchParams.set('pricingMetric', selectedPricingMetric);
        url.searchParams.set('priceBy', selectedPriceBy);
        window.history.pushState({}, '', url);

        // Disable the Go button to prevent multiple submissions
        goButton.disabled = true;

        // Reload the page to apply the new parameters
        window.location.reload();
    });

    window.addEventListener("DOMContentLoaded", () => {
        const urlParams = new URLSearchParams(window.location.search);

        // Set the dropdown values based on URL parameters
        selectedNumOfMonthsDropdown.value = urlParams.get('selectedNumOfMonths') || selectedNumOfMonthsDropdown.value;
        projYearsDropdown.value = urlParams.get('projYears') || projYearsDropdown.value;
        COIFeeDropdown.value = urlParams.get('chargeCOIFee') || COIFeeDropdown.value;
        pricingMetricDropdown.value = urlParams.get('pricingMetric') || pricingMetricDropdown.value;
        priceByDropdown.value = urlParams.get('priceBy') || priceByDropdown.value;
    });






    // ---------------- COI Contingent --------------------

    const COI_ContingentText = document.getElementById("COI_Contingent"); // gets the text
    const COI_ContingentValue = parseFloat(COI_ContingentText.textContent.replace(/[£,]/g, "")); // extracts the monetary value

    const FCDaysCOIText = document.getElementById("FCDaysCOI");
    const FCDaysCOIValue = parseFloat(FCDaysCOIText?.textContent.replace(/,/g, "").trim()) || 0;
    console.log("Forecast Days COI: ", FCDaysCOIValue);



    const contingentCalculation = document.getElementById("contingentCalculation")

    function formatCurrency(amount) {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: "GBP",
            minimumFractionDigits: 2
        }).format(amount);
    }

    document.getElementById("COI_ContingentFixedFigureInput").addEventListener("input", function () {
        let enteredValue = parseFloat(this.value) || 0;
        console.log("Updated COI Contingent Fixed Figure: £" + enteredValue.toFixed(2));

        let updatedTotalCOIContingent = FCDaysCOIValue * enteredValue
        contingentCalculation.textContent = formatCurrency(updatedTotalCOIContingent)
    });


});