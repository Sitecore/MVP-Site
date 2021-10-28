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

    $("#btnStep2").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#categoryForm')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        // get data from the form
                        var _category = $("#dllcategory").find("option:selected").text();

                        $.ajax({
                            url: '/Application/Category',
                            type: 'post',
                            data: JSON.stringify({ category: _category }),
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_personal', 3);
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

    $("#btnStep3").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#personalForm')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        // get data from the form
                        var _firstName = $('#firstName').val();
                        var _lastName = $('#lastName').val();
                        var _preferredName = $('#preferredName').val();
                        var _employmentStatus = $("#ddlEmploymentStatus").find("option:selected").text();
                        var _companyName = $('#companyName').val();
                        var _country = $("#dllCountry").find("option:selected").text();
                        var _mentor = $('#mentor').val();


                        $.ajax({
                            url: '/Application/PersonalInformation',
                            type: 'post',
                            data: JSON.stringify({ firstName: _firstName, lastName: _lastName, preferredName: _preferredName, employmentStatus: _employmentStatus, companyName: _companyName, country: _country, state: '', mentor: _mentor }),
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_objectives', 4);
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

    $("#btnStep4").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#objectivesForm')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        // get data from the form
                        var _eligibility = $('#eligibility').val();
                        var _objectives = $('#objectives').val();


                        $.ajax({
                            url: '/Application/ObjectivesandMotivation',
                            type: 'post',
                            data: JSON.stringify({ eligibility: _eligibility, objectives: _objectives}),
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_socials', 5);
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

    $("#btnStep5").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#socialForm')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        // get data from the form
                        var _blog = $('#blog').val();
                        var _sitecoreCommunity = $('#customerCoreProfile').val();
                        var _customerCoreProfile = $('#customerCoreProfile').val();
                        var _stackExchange = $('#stackExchange').val();
                        var _gitHub = $('#gitHub').val();
                        var _twitter = $('#twitter').val();
                        var _others = $('#others').val();


                        $.ajax({
                            url: '/Application/Socials',
                            type: 'post',
                            data: JSON.stringify({ blog: _blog, sitecoreCommunity: _sitecoreCommunity, customerCoreProfile: _customerCoreProfile, stackExchange: _stackExchange, gitHub: _gitHub, twitter: _twitter, others: _others }),
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_contributions', 6);
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

    $("#btnStep6").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#contributionForm')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    event.preventDefault()
                    if (!form.checkValidity()) {
                        event.stopPropagation()
                    }
                    else {
                        // get data from the form
                        var _onlineAcvitity = $('#onlineAcvitity').val();
                        var _offlineActivity = $('#offlineActivity').val();

                        $.ajax({
                            url: '/Application/NotableCurrentYearContributions',
                            type: 'post',
                            data: JSON.stringify({ onlineAcvitity: _onlineAcvitity, offlineActivity: _offlineActivity, }),
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_confirmation', 7);
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