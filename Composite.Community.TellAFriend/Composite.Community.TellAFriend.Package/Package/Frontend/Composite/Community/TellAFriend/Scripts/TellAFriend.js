$(document).ready(function () {

    jQuery.fn.fadeToggle = function (speed, easing, callback) {
        return this.animate({ opacity: 'toggle' }, speed, easing, callback);
    };

	$('#TellAFriend').hide();
	$('a.email, #TellAFriend a.close').click(function () {
		$("#TellAFriend").fadeToggle('slow');
	});

	$('#submitTellAFriend').click(function () {

		$.post("/TellAFriend.asmx/Send", { 'fromName': $('#fromName').val(), 'fromEmail': $('#fromEmail').val(), 'toName': $('#toName').val(), 'toEmail': $('#toEmail').val(), 'description': $('#description').val(), 'captcha': $('#captcha').val(), 'captchaEncryptedValue': $('#captchaEncryptedValue').val(), 'website': $('#website').val(), 'url': $('#url').val(), 'culture': $('#culture').val() }, function (xml) {
			AjaxSucceeded(xml);
		});

		return false;
	});
});


function AjaxSucceeded(xml) {
	var errorBox = '';
	var i = 0;
	$(":input").removeClass('FieldError');
	$(xml).find("Error").each(function () {
		var fieldname = $(this).attr("Fieldname");
		var errorDescription = $(this).attr("ErrorDescription");
		errorBox += '<li>' + errorDescription + '</li>';
		$('#' + fieldname).addClass('FieldError');
		i++;
	});

	$('#ErrorBox').html(errorBox);
	if (i==0) {
		$("#TellAFriend").fadeToggle('slow');
	}
}
