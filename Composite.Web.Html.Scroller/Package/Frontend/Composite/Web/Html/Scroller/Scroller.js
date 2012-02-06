(function ($) {
	$.fn.autoScrolledContent = function (options) {
		var defaults = {
			scrollspeed: 1,// SET SCROLLER SPEED 1 = SLOWEST
			speedjump: 30,// ADJUST SCROLL JUMPING = RANGE 20 TO 40
			startdelay: 2,// START SCROLLING DELAY IN SECONDS
			nextdelay: 0, // SECOND SCROLL DELAY IN SECONDS 0 = QUICKEST
			topspace: "2px"// TOP SPACING FIRST TIME SCROLLING
		}; //default options
		//inherit from provided configuration (if any)
		var options = $.extend(defaults, options);

		return this.each(function () {
			var $this = $(this);
			var id = $(this).attr("id");
			var $ScrollBox = $("#" + id + " .plnScrollBox");
			var $ScrollBox_content = $("#" + id + " .plnContent");
			var $ScrollBox_title = $("#" + id + " .plnTitle");
			var speed = options.scrollspeed;

			$ScrollBox_content.css("top", options.topspace);
			$ScrollBox_content.css("width", $this.width() - 20);
			$ScrollBox.css("height", $this.height() - $ScrollBox_title.height());

			var ScrollPanelHeight = $ScrollBox.height();

			$ScrollBox_content.mouseover(function () {options.scrollspeed = 0;})

			$ScrollBox_content.mouseout(function () {options.scrollspeed = speed;})

			$ScrollBox_content.mousewheel(function (event, delta) {
				ScrollContentTo((delta * 30 * (-1)));
				event.preventDefault();
			})

			var ScrollContentTo = function (scrollSpeed) {
				var top = parseInt($ScrollBox_content.css("top")) - parseInt(scrollSpeed);
				$ScrollBox_content.css("top", top);
			};

			var ScrollContent = function () {
				var top = parseInt($ScrollBox_content.css("top")) - parseInt(options.scrollspeed);
				$ScrollBox_content.css("top", top);
				if (top * (-1) > $ScrollBox_content.height()) {
					$ScrollBox_content.css("top", ScrollPanelHeight);
					setTimeout(function () { ScrollContent() }, (options.nextdelay * 1000));
				}
				else {
					setTimeout(function () { ScrollContent() }, options.speedjump);
				}
			};
			setTimeout(function () { ScrollContent() }, options.startdelay * 1000);
		});
	};
})(jQuery);