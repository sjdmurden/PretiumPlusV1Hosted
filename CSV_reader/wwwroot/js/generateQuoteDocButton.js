// ------------------- Generate quote pdf button ----------------------

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("generateQuotePDFButton").addEventListener("click", function () {
        let params = new URLSearchParams(document.location.search)
        let batchId = params.get("batchId")
        var quoteId = batchId.split("_")[0];

        let adjustmentNotes = document.getElementById("adjustmentNotes").value

        let userEmail = document.querySelector("[data-user-email]")?.dataset.userEmail;

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

        fetch('/PDF/GenerateQuoteDoc', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({

                userEmail: userEmail,
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
                const docType = "quote";
                downloadPDF(pdfBase64, batchId, docType); // Call download method
            })
            .catch(error => { 
                console.error('Error generating PDF:', error);
                alert('Failed to generate PDF. Please try again.');
            });
    });

})





