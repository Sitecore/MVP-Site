$(document).ready(function () {
    (function () {
        'use strict'

        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.querySelectorAll('.needs-validation')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault()
                        event.stopPropagation()
                    }

                    form.classList.add('was-validated')
                }, false)
            })
    })()

    $("#btnWelcome").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "/Application/Welcome",
            data: {
                
            },
            success: function (result) {
                alert('ok');
            },
            error: function (result) {
                alert('error');
            }
        });
    });


});