document.addEventListener("DOMContentLoaded", function () {
    const boxes = document.querySelectorAll(".verification-code-input");
    const hiddenInput = document.querySelector("input[name='Code']");
    const errorMessage = document.getElementById("error-message");

    function hideErrorMessage() {
        if (errorMessage) {
            errorMessage.style.display = "none";
            errorMessage.textContent = "";
        }
    }

    boxes[0].focus();

    boxes.forEach((box, index) => {
        box.addEventListener("input", function () {
            hideErrorMessage(); // Hide error message when input is entered
            box.value = box.value.replace(/[^0-9]/g, '');
            if (box.value.length === 1 && index < boxes.length - 1) {
                boxes[index + 1].focus();
            }
            hiddenInput.value = Array.from(boxes).map(b => b.value).join('');
        });

        box.addEventListener("keydown", function (e) {
            if (e.key === "Backspace" && index > 0 && box.value.length === 0) {
                boxes[index - 1].focus();
            }
        });
    });
});