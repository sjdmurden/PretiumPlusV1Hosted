document.addEventListener("DOMContentLoaded", function () {

    document.getElementById("quoteSearchForm").addEventListener("submit", async function (e) {
        e.preventDefault();

        const quoteIdInput = document.getElementById("quoteIdToSearch").value.trim();

        // Check if the input is valid ie is a number
        if (!/^[a-zA-Z0-9]+$/.test(quoteIdInput)) {
            alert("Please enter a valid quote ID with only letters and numbers.");
            return;
        }

        // Send an AJAX request to check if the quote ID exists
        try {
            console.log("Inside try block - when 'Search' pressed");
            const response = await fetch(`/Excel/CheckQuoteIdExists?quoteId=${quoteIdInput}`);
            const data = await response.json();

            if (data.exists) {
                // Redirect with the quoteId if it exists
                const params = new URLSearchParams(window.location.search);

                // Set quoteId param
                params.set("batchId", data.batchId);

                // Add default parameters
                params.set("selectedNumOfMonths", "12")
                params.set("projYears", "3")
                params.set("chargeCOIFee", "Yes")
                params.set("pricingMetric", "CCPVY")
                params.set("priceBy", "Experience")

                console.log(`/Calculations/IndexCalculationsPlusClaims?${params.toString()}`);


                // Redirect the user to the IndexCalculationsPlusClaims2 action method
                window.location.href = `/Calculations/IndexCalculationsPlusClaims2?${params.toString()}`;
            } else {
               
                alert("The entered Quote ID does not exist. Please try again.");
            }
        } catch (error) {
            console.error("Error checking the Quote ID:", error);
            alert("An error occurred while verifying the Quote ID. Please try again later.");
        }
    });
})