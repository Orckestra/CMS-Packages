$.fn.extend({
	animateCss: function (animationName, refresh) {
		var $item = $(this);
		var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
		var delay = $item.data('delay');
		setTimeout(function () {
			$item.removeClass('object-non-visible').addClass('animated ' + animationName).one(animationEnd, function () {
				if (refresh) {
					$(this).removeClass('animated ' + animationName);
				}
			});
		}, delay);
	}
});

$(document).ready(function () {

	$('.animated-content').each(function () {
		var $item = $(this);
		var duration = $item.data('duration');
		var animation = $item.data('animation');
		var action = $item.data('animateaction');

		$item.css("-webkit-animation-duration", duration).css('animation-duration', duration);

		if (action === 'page-load') {
			$item.animateCss(animation);
		}

		if (action === 'hover') {
			$item.hover(function () {
				$(this).animateCss(animation, true);
			});
		}

		if (action === 'appear') {
			$item.appear();
			$item.on('appear', function () {
				$(this).animateCss(animation);
			});
		}

	});
});
