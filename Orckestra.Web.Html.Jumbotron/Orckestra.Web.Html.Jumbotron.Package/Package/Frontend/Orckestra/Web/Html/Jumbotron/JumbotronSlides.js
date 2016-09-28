$(document).ready(function () {
	$('#jumbotronSlides').cycle();

	var firstSlideElements = $(".slide-0").find("[data-appear-effect]");
	appearSlideElements(firstSlideElements);

	$('#jumbotronSlides').on('cycle-before', function (e, optionHash, outgoingSlideEl, incomingSlideEl, forwardFlag) {
		var incomingSlideEls = $(incomingSlideEl).find("[data-appear-effect]");
		if ((incomingSlideEls.length > 0) && !Modernizr.touch) {
			appearSlideElements(incomingSlideEls);
		};

		var outgoingSlideEls = $(outgoingSlideEl).find("[data-disappear-effect]");
		if (outgoingSlideEls.length > 0) {
			outgoingSlideEls.each(function () {
				var $this = $(this),
                appearEffect = $this.attr("data-appear-effect"),
                hideEffect = $this.attr("data-disappear-effect");
				if (Modernizr.mq('only all and (min-width: 768px)') && Modernizr.csstransitions) {
					$this.removeClass(appearEffect).removeClass("object-non-visible").removeClass("object-visible");
					$this.addClass(hideEffect);
				}
			});
		};
		setTimeout(function () {
			outgoingSlideEls.attr("class", "").addClass("object-non-visible");
		}, 600);

	});

	function appearSlideElements(elements) {
		elements.each(function () {
			var $this = $(this),
            animationEffect = $this.attr("data-appear-effect");
			if (Modernizr.mq('only all and (min-width: 768px)') && Modernizr.csstransitions) {

				var delay = ($this.attr("data-effect-delay") ? $this.attr("data-effect-delay") : 1);
				if (delay > 1) $this.css("effect-delay", delay + "ms");

				setTimeout(function () {
					$this.addClass('animated object-visible ' + animationEffect);
				}, delay);

			} else {
				$this.addClass('object-visible');
			}
		});
	}
});