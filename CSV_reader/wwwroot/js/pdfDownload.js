function downloadPDF(pdfBase64, batchId, docType) {
    /*const iframe = document.querySelector('iframe');

    const pdfBase64 = iframe.src.split(',')[1]  // Extract base64 part of iframe:src attribute*/

    /*
    When the generate PDF button is clicked, it sends a fetch request to '/PDF/GeneratePDF' which runs the GeneratePDF method in the PDF controller.
    Then this downloadPDF function is called.
    */

    // the Base64 string must be decoded back into raw bytes for the Blob constructor to interpret it correctly. This requires two steps:
    // 1. atob converts base64 string into binary string (byteCharacters)
    // 2. now convert this binary string into numeric binary data using charCodeAt() to create a Uint8Array (this is byteNumbers), so the Blob constructor can use it to create a proper PDF file

    const byteCharacters = atob(pdfBase64)

    const byteNumbers = new Array(byteCharacters.length)  // this array's length is equal to the length of the byteCharacters string
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i)  // this converts each character to its numeric byte value (UTF-16 code unit) and puts them into the byteNumbers array
    }

    // now need to convert byteNumbers array into a usable array that represents binary data for the Blob onstructor
    // byteArray now represents the PDF's raw binary data
    const byteArray = new Uint8Array(byteNumbers)
    const blob = new Blob([byteArray], { type: 'application/pdf' })
    // the Blob constructor creates a Blob object representing the PDF file. The "application/pdf" specifies the type of data


    const link = document.createElement('a')  // creates temp anchor tag

    // create a temp url variable that points to the blob object in browser's memory. Then assign it to the href attribute of the link anchor tag
    const url = URL.createObjectURL(blob);
    link.href = url;
    /*let batchId = params.get("batchId")*/

    console.log("pdfDownload.js file - batch ID: ", batchId);

    if (docType == "quote") {
        link.download = `FleetInsuranceQuote_${batchId}.pdf`;  // names the downloaded file
    }
    if (docType == "policy") {
        link.download = `PolicyDocument_${batchId}.pdf`;  // names the downloaded file
    }

    document.body.appendChild(link);  // this adds the anchor tag to the body

    link.click();  // simulates a click on the anchor tag 
    document.body.removeChild(link);  // removes anchor tag from document
    URL.revokeObjectURL(url);  // cleans up space by removing url from memory so the same pdf isn't downloaded again
}