document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const emailInput = document.getElementById("email-input");
    const passwordInput = document.getElementById("password-input");
    const errorMessage = document.getElementById("error-message");
    const emailError = document.querySelector("[data-valmsg-for='Email']");
    const passwordError = document.querySelector("[data-valmsg-for='Password']");
    const confirmPasswordInput = document.getElementById("confirm-password-input");
    const confirmPasswordError = document.querySelector("[data-valmsg-for='ConfirmPassword']");

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

    function validateEmail() {
        if (!emailRegex.test(emailInput.value)) {
            errorMessage.textContent = "Please enter a valid email address.";
            errorMessage.style.display = "block";
            return false;
        }
        return true;
    }

    function validatePassword() {
        if (!passwordRegex.test(passwordInput.value)) {
            errorMessage.textContent = "Password must be at least 8 characters long, include one uppercase letter, one lowercase letter, one number, and one special character.";
            errorMessage.style.display = "block";
            return false;
        }
        return true;
    }

    function hideErrorMessage() {
        errorMessage.style.display = "none";
        errorMessage.textContent = "";
    }

    function hideErrorMessageForField(errorElement) {
        if (errorElement) {
            errorElement.textContent = "";
            errorElement.style.display = "none";
        }
    }

    emailInput?.addEventListener("input", function () {
        if (errorMessage) {
            hideErrorMessage();
        }
        if (emailError) {
            hideErrorMessageForField(emailError);
        }
    });

    passwordInput?.addEventListener("input", function () {
        if (errorMessage) {
            hideErrorMessage();
        }
        if (passwordError) {
            hideErrorMessageForField(passwordError);
        }
    });

    confirmPasswordInput?.addEventListener("input", function () {
        if (errorMessage) {
            hideErrorMessage();
        }
        if (confirmPasswordError) {
            hideErrorMessageForField(confirmPasswordError);
        }
    });

    form?.addEventListener("submit", function (event) {
        hideErrorMessage();

        const isEmailValid = validateEmail();
        const isPasswordValid = validatePassword();

        if (!isEmailValid || !isPasswordValid) {
            event.preventDefault();
        }
    });
});