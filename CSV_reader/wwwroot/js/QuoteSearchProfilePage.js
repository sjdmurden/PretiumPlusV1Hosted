document.addEventListener("DOMContentLoaded", function () {

    // select all buttons with the "quote-id-button" class
    const quoteButtons = document.querySelectorAll(".quote-id-button");

    quoteButtons.forEach(button => {
        button.addEventListener("click", async function () {

            // get the quote ID from the attribute "data-quote-id"
            const quoteIdInput = button.getAttribute("data-quote-id").trim();

            // Check if the input is valid ie is a number
            if (!/^[a-zA-Z0-9]+$/.test(quoteIdInput)) {
                alert("Please enter a valid quote ID with only letters and numbers.");
                return;
            }

            // Send an AJAX request to check if the quote ID exists
            try {
                const response = await fetch(`/Excel/CheckQuoteIdExists?quoteId=${quoteIdInput}`);
                const data = await response.json();

                if (data.exists) {
                    const params = new URLSearchParams();

                    // Use the batchId from server response
                    params.set("batchId", data.batchId);

                    // Add default parameters
                    params.set("selectedNumOfMonths", "12");
                    params.set("projYears", "3");
                    params.set("chargeCOIFee", "Yes");
                    params.set("pricingMetric", "CCPVY");
                    params.set("priceBy", "Experience");

                    // Redirect the user to the IndexCalculationsPlusClaims2 action method
                    window.location.href = `/Calculations/IndexCalculationsPlusClaims2?${params.toString()}`;
                } else {
                    alert("The selected Quote ID does not exist. Please try again.");
                }
            } catch (error) {
                console.error("Error checking the Quote ID:", error);
                alert("An error occurred while verifying the Quote ID. Please try again later.");
            }
        });
    });
});



