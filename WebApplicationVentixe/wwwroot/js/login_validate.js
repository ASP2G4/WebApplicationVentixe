document.addEventListener("DOMContentLoaded", function () {
    const emailInput = document.getElementById("email-input");
    const passwordInput = document.getElementById("password-input");
    const errorMessage = document.getElementById("error-message");
    const emailError = document.querySelector("[data-valmsg-for='Email']");
    const passwordError = document.querySelector("[data-valmsg-for='Password']");

    function hideErrorMessage() {
        if (errorMessage) {
            errorMessage.style.display = "none";
        }
    }

    function hideErrorMessageForField(errorElement) {
        if (errorElement) {
            errorElement.textContent = "";
            errorElement.style.display = "none";
        }
    }

    emailInput?.addEventListener("input", function () {
        hideErrorMessageForField(emailError);
        hideErrorMessage();
    });

    passwordInput?.addEventListener("input", function () {
        hideErrorMessageForField(passwordError);
        hideErrorMessage();
    });
});