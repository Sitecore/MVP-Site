$(document).ready(function () {

    if (document.getElementById("application-form") == null) {
        return;
    }


    $(document).ajaxSend(function () {
        $("#overlay").fadeIn(300);
    });

    fillApplicationList();
    getApplicationInfo();

    var currentStepId = 1;
    //Comment/uncomment each of the steps if you need to review single screen for starting
    var currentStep = "#step_welcome";
    //var currentStep = "#step_category"; 
    //var currentStep = "#step_personal"; 
    //var currentStep = "#step_objectives"; 
    //var currentStep = "#step_socials"; 
    //var currentStep = "#step_socials"; 
    //var currentStep = "#step_contributions"; 
    //var currentStep = "#step_confirmation"; 

   // setStep(currentStep);



    $("#btnStep1").click(function (event) {
        'use strict'
        $("#btnStep1").attr("disabled", true);
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
                                if (data.success === true) {

                                    $('#applicationId').val(data.applicationItemId);

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

        $("#btnStep1").attr("disabled", false);
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
                        var _applicationId = $('#applicationId').val();
                        var _category =  $("#dllcategory").find("option:selected").val();

                        $.ajax({
                            url: '/submitStep2',
                            type: 'post',
                            data: { applicationId: _applicationId, category: _category },
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
                        var _applicationId = $('#applicationId').val().toString();
                        var _firstName = $('#firstName').val();
                        var _lastName = $('#lastName').val();
                        var _preferredName = $('#preferredName').val();
                        var _employmentStatus = $("#ddlEmploymentStatus").find("option:selected").val();
                        var _companyName = $('#companyName').val();
                        var _country = $("#ddlCountry").find("option:selected").val();
                        var _mentor = $('#mentor').val();

                        $.ajax({
                            url: '/submitStep3',
                            type: 'post',
                            data: { applicationId: _applicationId, firstName: _firstName, lastName: _lastName, preferredName: _preferredName, employmentStatus: _employmentStatus, companyName: _companyName, country: _country, state: '', mentor: _mentor },
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_objectives');
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
                        var _applicationId = $('#applicationId').val().toString();
                        var _eligibility = $('#eligibility').val();
                        var _objectives = $('#txtObjectives').val();


                        $.ajax({
                            url: '/submitStep4',
                            type: 'post',
                            data: { applicationId: _applicationId, eligibility: _eligibility, objectives: _objectives },
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_socials');
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
                        var _applicationId = $('#applicationId').val();
                        var _blog = $('#blog').val();
                        var _sitecoreCommunity = $('#customerCoreProfile').val();
                        var _customerCoreProfile = $('#customerCoreProfile').val();
                        var _stackExchange = $('#stackExchange').val();
                        var _gitHub = $('#gitHub').val();
                        var _twitter = $('#twitter').val();
                        var _others = $('#others').val();
                        var _agreeOnTerms = $('#chkTerms').prop("checked");

                        $.ajax({
                            url: '/submitStep5',
                            type: 'post',
                            data: { applicationId: _applicationId, blog: _blog, sitecoreCommunity: _sitecoreCommunity, customerCoreProfile: _customerCoreProfile, stackExchange: _stackExchange, gitHub: _gitHub, twitter: _twitter, others: _others, agreeOnTerms: _agreeOnTerms},
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_contributions');
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
                        var _applicationId = $('#applicationId').val();
                        var _onlineAcvitity = $('#onlineAcvitity').val();
                        var _offlineActivity = $('#offlineActivity').val();

                        $.ajax({
                            url: '/submitStep6',
                            type: 'post',
                            data: { applicationId: _applicationId, onlineAcvitity: _onlineAcvitity, offlineActivity: _offlineActivity, },
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    setStep('#step_confirmation');
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

    $("#btnStep7").click(function (event) {
        'use strict'
        var forms = document.querySelectorAll('#confirmationForm')

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
                        var _applicationId = $('#applicationId').val().toString();

                        $.ajax({
                            url: '/submitStep7',
                            type: 'post',
                            data: { applicationId: _applicationId},
                            dataType: 'json',
                            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                            success: function (data) {
                                if (data.success === true) {
                                    window.location.href = "/thank-you";
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

});


function updateinput(key, value) {

    var dropLowerCaseId = key.toLowerCase();
 
    if (dropLowerCaseId === 'agreeonterms') {
        $('#chkTerms').prop("checked", value );
    } else
    {
        $("input[asp-for='" + dropLowerCaseId + "']").val(value);
        $("textarea[asp-for='" + dropLowerCaseId + "']").val(value);
    
        if (value != null && typeof value.id !== 'undefined') {
        
            $("select[asp-for='" + dropLowerCaseId + "'] option[value=" + value.id + "]").prop('selected', true);
        }
    }
}

function fillDropLists(items, dropId, title) {
    var lists = '';
    var dropLowerCaseId = dropId.toLowerCase ();
    $("select[asp-for='" + dropLowerCaseId + "']").append("<option value=''>&nbsp;</option>");
   
    $.each(items, function (i, item) {
        
        if (typeof item.Active === 'undefined' || item.Active) {
            //lists += '<a class="dropdown-item" href="#">' + item[title] + '</a>';
           
            $("select[asp-for='" + dropLowerCaseId + "']").append("<option value='" + item['ID'] + "'>" + item[title] + "</option>");
        } 
    });

   // $("div[asp-for='" + dropId + "']").html(lists);
}

function getPrevStep() {

    if (currentStepId > 2) {
        $("#progressbar").find('[data-step="' + currentStepId + '"]').removeClass('active');

        currentStepId--;
        var stepIdid = $("div[data-step='" + currentStepId + "']").attr('id');

        setStep('#' + stepIdid);
    }
}
function getnextStep() {

}
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

    currentStepId = stepCount;
    setProgressBar(stepCount);
}

function setProgressBar(curStep) {
    var steps = $(".fieldSet").length;
    var percent = parseFloat(100 / steps) * curStep;
    percent = percent.toFixed();
    $(".progress-bar")
        .css("width", percent + "%")
}


function fillApplicationList() {
    $.ajax({
        type: "GET",
        url: "/Application/GetApplicationLists",
        data: {

        },
        success: function (data) {

            if (data.result) {
                //something is wrong 
                console.info(data.result);
            } else {
                var jsonData = JSON.parse(data);

                fillDropLists(jsonData.Country, 'Country', 'Name');
                fillDropLists(jsonData.EmploymentStatus, 'EmploymentStatus', 'Name');
                fillDropLists(jsonData.MVPCategory, 'mvpcategory', 'Name');

            }
            $("#overlay").fadeOut();
        },
        error: function (result) {

            console.error(result);
            $("#overlay").fadeOut();
        }
    });
}

function getApplicationInfo() {
	$.ajax({
		type: "GET",
		url: "/Application/GetApplicationInfo",
        success: function (data) {
            //console.info(data);
            if (!data.isLoggedIn) {
				//todo: redirec to login
				window.location = '/Application/Intro';
            }
            else if (data.applicationCompleted) {
                    
                window.location = '/thank-you';
            }
            else if (data.applicationAvailable) {
				//var jsonData = JSON.parse(data);
                $.each(data.result.application, function (k, v) {
					updateinput(k, v);
				});

				if (data.result.applicationStep.stepId) {
					setStep('#' + data.result.applicationStep.stepId);
				} else {
                    setStep('#step_welcome')
				}
			} else {
				//call 
				setStep('#step_welcome');
			}
			$("#overlay").fadeOut();
		},
		error: function (result) {

			console.error(result);
			$("#overlay").fadeOut();
		}
    });
}