// ------------------- ADJUST ULTIMATE COSTS ----------------------------

document.addEventListener("DOMContentLoaded", function () {

    const adjustValuesButton = document.getElementById("adjustValuesButton");
    const resetAdjustmentButton = document.getElementById("resetValuesButton");

    const netPremium = document.getElementById("NetPremium");
    const grossPremium = document.getElementById("GrossPremium");
    const grossPremiumPlusIPT = document.getElementById("GrossPremiumPlusIPT");
    const commission = document.getElementById("Commission");

    const initialNetVal = netPremium.textContent;
    const initialCommission = commission.textContent;
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

            // Adjust net premium
            const currentNetPrem = parseFloat(netPremium.textContent.replace(/[£,]/g, ""));
            const newNetPrem = currentNetPrem + adjustmentAmount;
            netPremium.textContent = `£${newNetPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            netPremium.style = "color: #65aacb;";

            // Adjust Commission
            const commissionValue = parseFloat(commission.textContent.replace(/[£,]/g, ""));
            const newCommission = newNetPrem * 0.08;
            commission.textContent = `£${newCommission.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            commission.style = "color: #65aacb;";

            // Adjust gross premium
            // commissionValue = parseFloat(commission.textContent.replace(/[£,]/g, ""));
            const newGrossPrem = newNetPrem + commissionValue;
            grossPremium.textContent = `£${newGrossPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremium.style = "color: #65aacb;";

            // Adjust gross premium plus IPT
            const newGrossPremPlusIPT = newGrossPrem + newGrossPrem * (12 / 100);
            grossPremiumPlusIPT.textContent = `£${newGrossPremPlusIPT.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            grossPremiumPlusIPT.style = "color: #65aacb;";

            adjustmentMessage = `Net Premium adjusted by £${adjustmentAmount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

        } else if (!isNaN(adjustmentPercentage)) {
            // Adjust using percentage
            const multiplier = 1 + adjustmentPercentage / 100;

            const currentNetPrem = parseFloat(netPremium.textContent.replace(/[£,]/g, ""));
            const newNetPrem = currentNetPrem * multiplier;
            netPremium.textContent = `£${newNetPrem.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            netPremium.style = "color: blue;";

            const commissionValue = parseFloat(commission.textContent.replace(/[£,]/g, ""));
            const newGrossPrem = newNetPrem + commissionValue;
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
        commission.textContent = initialCommission;
        grossPremium.textContent = initialGrossVal;
        grossPremiumPlusIPT.textContent = initialGrossPlusIPT;

        // Reset styles
        const valuesArray = [netPremium, commission, grossPremium, grossPremiumPlusIPT];
        for (var x of valuesArray) {
            x.style.color = "#7dd3fc;";
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
    });

});