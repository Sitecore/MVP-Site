$(document).ready(function () {

    //TODO
    //Naim - good entry point to call your ajax function and with the response you can set field values and then also set the current step for the user
    //Assuming in the response we will have application data which can be used to update values on the application
    //We will also have the current step for the user.  It will be an ID - so '#step_welcome'
    //Using that ID you can find the element on the page, get the data-step attribute and set the two variables below
    //current - comes from data-step attribute
    //currentStep - ID returned from Sitecore for the user
    var current = 1;
    var currentStep = "#step_personal";//"#step_welcome";
    var steps = $(".fieldSet").length;
    setStep(currentStep);

    $(document).ajaxSend(function () {
        $("#overlay").fadeIn(300);
    });

    function updateinput(key, value) {
        $("input[asp-for='"+key+"']").val(value);
    }

    function fillDropLists(items,dropId,title) {
        var lists = '';

        $.each(items, function (i, item) {
            
            if (typeof item.Active === 'undefined' || item.Active) {
                console.info(item.Name);
                lists += '<a class="dropdown-item" href="#">' + item[title] + '</a>';
            } 
        });
        
        $("div[asp-for='" + dropId + "']").html(lists);
    }
  
        $.ajax({
            type: "GET",
            url: "/Application/GetApplicationLists",
            data: {

            },
            success: function (data) {
                console.info(data);
                if (data.result) {
                    console.info(data.result);
                } else {
                    var jsonData = JSON.parse(data);

                    fillDropLists(jsonData.Countries, 'Countries', 'Name');
                    fillDropLists(jsonData.EmploymentStatus, 'EmploymentStatus', 'Name');
                    fillDropLists(jsonData.MVPCategories, 'MVPCategories', 'Name');

                    getApplicationInfo()
                }
                $("#overlay").fadeOut();
            },
            error: function (result) {
                
                console.info(result);
                $("#overlay").fadeOut();
            }
        });

    function getApplicationInfo() {
        $.ajax({
            type: "GET",
            url: "/Application/GetApplicationInfo",
            data: {

            },
            success: function (data) {
                console.info(data);
                if (data.result) {
                    
                } else {
                    var jsonData = JSON.parse(data);
                    setStep('#' + jsonData.ApplicationStep.StepId);

                    $.each(jsonData.Application, function (k, v) {
                        updateinput(k, v);
                    });
                }
                $("#overlay").fadeOut();
            },
            error: function (result) {

                console.error(result);
                $("#overlay").fadeOut();
            }
        });
    }

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
                                    setStep('#step_category');
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

    function setStep(stepId) {
        //hide all steps
        $('.appStep').attr("hidden", true);
        //show requested step
        $(stepId).attr("hidden", false);

        stepCount = $(stepId).attr('data-step');
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