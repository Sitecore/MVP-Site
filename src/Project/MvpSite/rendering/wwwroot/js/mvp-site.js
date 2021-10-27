$(document).ready(function () {

    //TODO
    //Naim - good entry point to call your ajax function and with the response you can set field values and then also set the current step for the user
    //Assuming in the response we will have application data which can be used to update values on the application
    //We will also have the current step for the user.  It will be an ID - so '#step_welcome'
    //Using that ID you can find the element on the page, get the data-step attribute and set the two variables below
    //current - comes from data-step attribute
    //currentStep - ID returned from Sitecore for the user
    var current = 1;
    var currentStep = "#step_welcome"
    var steps = $(".fieldSet").length;
    setStep(currentStep, current);

    $(document).ajaxSend(function () {
        $("#overlay").fadeIn(300);
    });

    $("#btnStep1").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#form_step1')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        $.ajax({
                            url: '/submitStep1',
                            type: 'post',
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success == true) {
                                    setStep('#step_category', 2);
                                }
                                else {
                                    alert(data.responseText);
                                }
                                
                            }
                        }).done(function () {
                            setTimeout(function () {
                                $("#overlay").fadeOut(300);
                            }, 500);
                        });
                    }

                    form.classList.add('was-validated')
                }, false)
            })
    });		

    function setStep(stepId, stepCount) {
        //hide all steps
        $('.appStep').attr("hidden", true);
        //show requested step
        $(stepId).attr("hidden", false);
        //update progress bar at the top 
        //$("#progressbar").find('[data-step="' + stepCount + '"]').addClass('active');
        for (var i = stepCount; i >= 1; i--) {
            $("#progressbar").find('[data-step="' + i + '"]').addClass('active');
        }
        setProgressBar(stepCount);
    }

    function setProgressBar(curStep) {
        var percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar")
            .css("width", percent + "%")
    }

});