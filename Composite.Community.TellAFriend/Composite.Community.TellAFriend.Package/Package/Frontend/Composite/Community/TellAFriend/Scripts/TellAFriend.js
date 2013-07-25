$(document).ready(function () {

    $.fn.alignCenter = function () {
        var marginLeft = Math.max(40, parseInt($(window).width() / 2 - $(this).width() / 2 + $(window).scrollLeft())) + 'px';
        var marginTop = Math.max(40, parseInt($(window).height() / 2 - $(this).height() / 2 + $(window).scrollTop())) + 'px';
        return $(this).css({ 'margin-left': marginLeft, 'margin-top': marginTop, 'left': 0, 'top': 0 });
    };

    jQuery.fn.fadeToggle = function (speed, easing, callback) {
        if (!$(this).is(':visible')) this.alignCenter();
        return $(this).animate({ opacity: 'toggle' }, speed, easing, callback);
    };

    $('#TellAFriend').hide();
    $('a.email, #TellAFriend a.close').click(function (event) {
        event.preventDefault();
        $("#TellAFriend").fadeToggle('slow');
    });

    $('#submitTellAFriend').click(function () {

        $.post("/TellAFriend.asmx/Send", { 'fromName': $('#fromName').val(), 'fromEmail': $('#fromEmail').val(), 'toName': $('#toName').val(), 'toEmail': $('#toEmail').val(), 'description': $('#description').val(), 'captcha': $('#captcha').val(), 'captchaEncryptedValue': $('#captchaEncryptedValue').val(), 'useCaptcha': $('#useCaptcha').val(), 'website': $('#website').val(), 'url': $('#url').val(), 'culture': $('#culture').val() }, function (xml) {
            if (AjaxSucceeded(xml)) {
                $('#fromName').val('');
                $('#fromEmail').val('');
                $('#toName').val('');
                $('#toEmail').val('');
                $('#description').val('');
                $('#captcha').val('');
            }
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
    if (i == 0) {
        $("#TellAFriend").fadeToggle('slow');
    }

    if ($(xml).find("Error").length) {
        return false;
    } else {
        return true;
    }

}
