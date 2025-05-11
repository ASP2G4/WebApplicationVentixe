document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");

    const fields = [
        { input: document.getElementById("firstname"), error: document.querySelector("[data-valmsg-for='FirstName']") },
        { input: document.getElementById("lastname"), error: document.querySelector("[data-valmsg-for='LastName']") },
        { input: document.getElementById("phonenumber"), error: document.querySelector("[data-valmsg-for='PhoneNumber']") },
        { input: document.getElementById("streetname"), error: document.querySelector("[data-valmsg-for='StreetName']") },
        { input: document.getElementById("postalcode"), error: document.querySelector("[data-valmsg-for='PostalCode']") },
        { input: document.getElementById("city"), error: document.querySelector("[data-valmsg-for='City']") }
    ];

    function hideErrorMessageForField(errorElement) {
        if (errorElement) {
            errorElement.textContent = "";
            errorElement.style.display = "none";
        }
    }

    fields.forEach(field => {
        if (field.input && field.error) {
            field.input.addEventListener("input", function () {
                hideErrorMessageForField(field.error);
            });
        } 
    });
});