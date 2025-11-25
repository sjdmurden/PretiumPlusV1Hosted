/*document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("generateQuotePDFButton").addEventListener("click", function () {
       

        let params = new URLSearchParams(document.location.search)
        let batchId = params.get("batchId")
        var quoteId = batchId.split("_")[0];

        let policyNumber = 0 //placeholder policy number for now

        let userEmail = document.querySelector("[data-user-email]")?.dataset.userEmail;

        let clientName = document.getElementById("clientName").textContent;

        fetch('/PDF/SaveDocumentToDB', {
            method: 'POST', 
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({

                quoteNumber: quoteId,
                policyNumber: policyNumber,
                batchId: batchId,
                clientName: clientName,
                userEmail: userEmail,
            })
        })
    })
})


document.addEventListener("DOMContentLoaded", function () {

    document.getElementById("generateBothDocsBtn").addEventListener("click", function () {

        saveDocument("Quote PDF");
        saveDocument("Policy PDF");
    });

    function saveDocument(documentType) {

        let params = new URLSearchParams(document.location.search);
        let batchId = params.get("batchId");
        let quoteId = batchId ? batchId.split("_")[0] : null;

        if (!batchId || !quoteId) {
            console.error("Missing batchId or quoteId in URL");
            return;
        }

        let userEmail = document.querySelector("[data-user-email]")?.dataset.userEmail || "";
        let clientName = document.getElementById("clientName")?.textContent || "";

        // Only used for policy PDF
        let policyNumber = 0;
        if (documentType === "Policy PDF") {
            var policyInput = document.getElementById("policyNumber");
            if (policyInput) {
                policyNumber = policyInput.value || 0;
            }
        }

        fetch('/PDF/SaveDocumentToDB', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                quoteNumber: quoteId,
                policyNumber: policyNumber,
                batchId: batchId,
                clientName: clientName,
                userEmail: userEmail,
                documentType: documentType
            })
        })
            .then(response => response.json())
            .then(result => {
                console.log(documentType + " save result:", result);

                if (result.success === false) {
                    console.error("Error saving " + documentType + ":", result.message);
                }
            })
            .catch(error => {
                console.error("Fetch error while saving " + documentType + ":", error);
            });
    }
});*/


document.addEventListener("DOMContentLoaded", function () {

    const generateBothBtn = document.getElementById("generateBothDocsBtn");

    generateBothBtn.addEventListener("click", function () {

        console.log("Generate Both button clicked!");

        const commonValues = collectCommonValues();
        if (!commonValues) return;

        // 1) Generate Quote PDF
        fetch('/PDF/GenerateQuoteDoc', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(commonValues)
        })
        .then(r => r.json())
        .then(data => {
            downloadPDF(data.pdfBase64, commonValues.batchId, "quote");

            // Save Quote PDF record to DB
            return saveDocumentToDB(commonValues, "Quote PDF");
        })
        .then(() => {
            // 2) Generate Policy PDF
            const policyValues = Object.assign({}, commonValues);

            // Generate policy number once, reuse it for both PDF + DB
            const policyNumber = Math.floor(Math.random() * 10000000);
            policyValues.policyNumber = policyNumber;
            commonValues.policyNumber = policyNumber; // this stores the newly created policy num back in the values dict

            return fetch('/PDF/GeneratePolicyDoc', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(policyValues)
            });
        })
        .then(r => r.json())
        .then(data => {
            downloadPDF(data.pdfBase64, commonValues.batchId, "policy");

            // Save Policy PDF record to DB
            return saveDocumentToDB(commonValues, "Policy PDF");
        })
        .catch(err => {
            console.error("Error in full generation:", err);
            alert("Failed to generate one or both documents.");
        });

    });
});


// this func simplifies the process by obtaining the values used in both the quote doc and policy doc without using the same 'getElementById' lines
// the readMoney func also simplifies by not having to use regex on every line
function collectCommonValues() {

    let params = new URLSearchParams(document.location.search)
    let batchId = params.get("batchId");
    if (!batchId) {
        alert("Missing batchId in URL.");
        return null;
    }

    let userEmail = document.querySelector("[data-user-email]")?.dataset.userEmail;
    let adjustmentNotes = document.getElementById("adjustmentNotes").value;

    return {
        userEmail: userEmail,
        batchId: batchId,
        claimsAmount: readMoney("ProjClaimsAmount"),
        largeLossFund: readMoney("ProjLLFund"),
        reinsuranceCosts: readMoney("ReinsuranceCosts"),
        claimsHandlingFee: readMoney("ProjClaimsHandlingFee"),
        levies: readMoney("Levies"),
        expenses: readMoney("PretiumExpenses"),
        profit: readMoney("Profit"),
        netPremium: readMoney("NetPremium"),
        commissions: readMoney("Commission"),
        grossPremium: readMoney("GrossPremium"),
        updatedGrossPremiumPlusIPT: readMoney("GrossPremiumPlusIPT"),
        adjustmentNotes: adjustmentNotes,
        FCDaysCOI: readMoney("FCDaysCOI"),
        FCDaysNonCOI: readMoney("FCDaysNonCOI"),
        FCTurnoverCOI: readMoney("FCTurnoverCOI"),
        FCTurnoverNonCOI: readMoney("FCTurnoverNonCOI"),
    };
}

function readMoney(id) {
    return parseFloat(document.getElementById(id).textContent.replace(/[£,]/g, ''));
}



function saveDocumentToDB(values, documentType) {
    return fetch('/PDF/SaveDocumentToDB', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            quoteNumber: values.batchId.split("_")[0],
            policyNumber: values.policyNumber || 0,
            batchId: values.batchId,
            clientName: document.getElementById("clientName")?.textContent || "",
            userEmail: values.userEmail,
            documentType: documentType
        })
    })
        .then(r => r.json())
        .then(result => console.log(documentType, "saved:", result))
}
