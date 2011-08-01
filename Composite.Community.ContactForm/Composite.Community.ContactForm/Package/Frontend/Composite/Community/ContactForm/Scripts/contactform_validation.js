$(document).ready(function () {
	var submitbutton = $("#submitContactForm");
	var root = $("#contact_form");

	// validation logic

	submitbutton.click(function (event) {

		inputs = root.find(".required :input").removeClass("error");
		empty = inputs.filter(function () {
			return $(this).val().replace(/\s*/g, '') == '';
		});

		if (empty.length) { empty.addClass("error"); return false; }

	});


	$(".Email_Input").keyup(function () {
		var email = $(this).val();
		var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
		if (pattern.test(email)) {
			$(this).removeClass("error").addClass("ok");
		}
		else {
			$(this).removeClass("ok").addClass("error");
		}
	})


});