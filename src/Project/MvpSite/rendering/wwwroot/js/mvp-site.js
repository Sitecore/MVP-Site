$(document).ready(function () {
	
			//Check if user email cookie has been set, if not, make ajax request to set it
            var userEmail = getCookie('user_email');
            if (!userEmail) {
                $.ajax({
                    url: '/GetUserEmailClaim',
                    type: 'get',
                    crossDomain: true,
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    success: function (data) {
                        setCookie('user_email', data);
                    }
                });
            }
			
			//Only make this check if the application form exists on the page
			if ($('#application-form').length)
			{
				$.ajax({
					type: 'POST',
					url: 'https://mvp-cd.sc.localhost/Application/GetApplicationInfo',
					crossDomain: true,
					data: {"identifier":getCookie('user_email')},
					dataType: 'json',
					success: function(responseData, textStatus, jqXHR) {
						var value = responseData.someKey;
						alert(value);
					},
					error: function (responseData, textStatus, errorThrown) {
						alert('POST failed.');
					}
				});
			}



            var current_fs, next_fs, previous_fs; //fieldsets
            var opacity;
            var current = 1;
            var steps = $("fieldset").length;

            setProgressBar(current);

            $(".next").click(function () {

                current_fs = $(this).parent();
                next_fs = $(this).parent().next();

                //Add Class Active
                $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

                //show the next fieldset
                next_fs.show();
                //hide the current fieldset with style
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
                        // for making fielset appear animation
                        opacity = 1 - now;

                        current_fs.css({
                            'display': 'none',
                            'position': 'relative'
                        });
                        next_fs.css({ 'opacity': opacity });
                    },
                    duration: 500
                });
                setProgressBar(++current);
            });

            $(".previous").click(function () {

                current_fs = $(this).parent();
                previous_fs = $(this).parent().prev();

                //Remove class active
                $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

                //show the previous fieldset
                previous_fs.show();

                //hide the current fieldset with style
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
                        // for making fielset appear animation
                        opacity = 1 - now;

                        current_fs.css({
                            'display': 'none',
                            'position': 'relative'
                        });
                        previous_fs.css({ 'opacity': opacity });
                    },
                    duration: 500
                });
                setProgressBar(--current);
            });

            function setProgressBar(curStep) {
                var percent = parseFloat(100 / steps) * curStep;
                percent = percent.toFixed();
                $(".progress-bar")
                    .css("width", percent + "%")
            }


            //$(function () {
            //    //twitter bootstrap script
            //    $("button#btnWelcome").click(function () {
            //        $.ajax({
            //            type: "POST",
            //            url: "PastSurgicalCustomItem",
            //            data: $('form.form-horizontal').serialize(),
            //            success: function (msg) {
            //                alert(msg);
            //            },
            //            error: function () {
            //                alert("failure");
            //            }
            //        });
            //    });
            //});

            $("#btnWelcome").click(function (event) {

                event.preventDefault();
                var form = $('#welcomeForm');
                var method = form.attr('method');
                var url = form.attr('action');

                if ($('#welcomeForm')[0].checkValidity() === false) {
                    event.stopPropagation();
                    $('.form-check-input').addClass('is-invalid');
                } else {
                    var jsonData = {};
                    $.each($(form).serializeArray(), function () {
                        jsonData[this.name] = this.value;
                    });
                    var data = JSON.stringify(jsonData);
                    console.log(data);
                    ajaxCallRequest(method, url, data);
                }



            });

            function ajaxCallRequest(f_method, f_url, f_data) {
                $("#dataSent").val(unescape(f_data));
                var f_contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
                $.ajax({
                    url: f_url,
                    type: f_method,
                    contentType: f_contentType,
                    dataType: 'json',
                    data: f_data,
                    success: function (data) {
                        var jsonResult = JSON.stringify(data);
                        $("#results").val(unescape(jsonResult));
                    }
                });
            }

            function setCookie(key, value) {
                var expires = new Date();
                expires.setTime(expires.getTime() + 31536000000); //1 year  
                document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
            }

            function getCookie(key) {
                var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
                return keyValue ? keyValue[2] : null;
            }

        });