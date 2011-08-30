var win = $(window), options, compatibleOverlay, middle, centerWidth, centerHeight,
		ie6 = !window.XMLHttpRequest, hiddenElements = [], documentElement = document.documentElement;
// DOM elements
var overlay, center, htmlContent, sizer, bottomContainer, bottom, caption, number;


function showHtmlLightbox(o, _options) {
	options = $.extend({
		width: 400,
		height: 300,
		html: "",
		overlayOpacity: 0.8,
		overlayFadeDuration: 100,
		captionAnimationDuration: 400,
		resizeDuration: 700,
		resizeEasing: "swing",
		closeKeys: [27, 88, 67]
	}, _options);

	var link = $(o);
	var content = options.html;

	center.className = "htmlLoading";
	middle = win.scrollTop() + (win.height() / 2);
	centerWidth = options.width;
	centerHeight = options.height;
	$(center).css({ top: Math.max(0, middle - (centerHeight / 2)), width: centerWidth, height: 20, marginLeft: -centerWidth / 2 }).show();
	compatibleOverlay = ie6 || (overlay.currentStyle && (overlay.currentStyle.position != "fixed"));
	if (compatibleOverlay)
		overlay.style.position = "absolute";
	$(overlay).css("opacity", options.overlayOpacity).fadeIn(options.overlayFadeDuration);
	position();
	setup(1);

	$(htmlContent).children(":first").html(content);
	center.className = "";
	$(sizer).width(options.width);
	$(sizer).height(options.height);
	var top = Math.max(0, middle - (centerHeight / 2));

	$(center).animate({ height: centerHeight, top: top }, options.resizeDuration, options.resizeEasing);

	$(center).queue(function () {
		$(bottomContainer).css({ width: centerWidth, top: top + centerHeight, marginLeft: -centerWidth / 2, visibility: "hidden", display: "" });
		$(bottom).css("marginTop", -bottom.offsetHeight).animate({ marginTop: 0 }, options.captionAnimationDuration);
		bottomContainer.style.visibility = "";
	});

}


function stop() {
	$([center, bottom]).stop(true);
	$([bottomContainer]).hide();
}


function position() {
	var l = win.scrollLeft(), w = win.width();
	$([center, bottomContainer]).css("left", l + (w / 2));
	if (compatibleOverlay)
		$(overlay).css({ left: l, top: win.scrollTop(), width: w, height: win.height() });

}

function setup(open) {
	if (open) {
		$("object").add(ie6 ? "select" : "embed").each(function (index, el) {
			hiddenElements[index] = [el, el.style.visibility];
		el.style.visibility = "hidden";
		});
	} else {
		$.each(hiddenElements, function (index, el) {
			el[0].style.visibility = el[1];
		});
		hiddenElements = [];
	}
	var fn = open ? "bind" : "unbind";
	win[fn]("scroll resize", position);
	$(document)[fn]("keydown", keyDown);
}

function keyDown(event) {
	var code = event.keyCode, fn = $.inArray;
	// Prevent default keyboard action (like navigating inside the page)
	return (fn(code, options.closeKeys) >= 0) ? close() : false;
}

function close() {
	stop();
	$(center).hide();
	$(overlay).stop().fadeOut(options.overlayFadeDuration, setup);
	$(htmlContent).children(":first").html("");
	return false;
}

$(document).ready(function () {
	$("body").append(
			$([
				overlay = $('<div id="htmlOverlay" />')[0],
				center = $('<div id="htmlCenter" />')[0],
				bottomContainer = $('<div id="htmlBottomContainer" />')[0]
			]).css("display", "none")
		);

	htmlContent = $('<div id="htmlContent" />').appendTo(center).append(
			sizer = $('<div style="position: relative;" />')[0]
		)[0];

	bottom = $('<div id="htmlBottom" />').appendTo(bottomContainer).append([
			$('<a id="htmlCloseLink" href="#" />').add(overlay).click(close)[0],
			caption = $('<div id="htmlCaption" />')[0],
			number = $('<div id="htmlNumber" />')[0],
			$('<div style="clear: both;" />')[0]
		])[0];

});