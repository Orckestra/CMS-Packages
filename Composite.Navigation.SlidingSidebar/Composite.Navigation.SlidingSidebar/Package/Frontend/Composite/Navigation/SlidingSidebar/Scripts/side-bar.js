var isExtended = 0;
var height = 450;
var width = 200;
var slideDuration = 900;
var opacityDuration = 1500;

function extendContract() {
	if (isExtended == 0) {
		$("#SlidingSidebar").animate({ "width": width }, slideDuration - 2);
		$("#SlidingSidebarContents").animate({ "height": height, "width": width - 28 }, slideDuration);

		isExtended = 1;
		$("#SlidingSidebarTab").removeClass();
		$("#SlidingSidebarTab").addClass('SlidingSidebarTab' + $("#SlidingSidebar").attr("class") + 'active');

	}
	else {
		$("#SlidingSidebarContents").animate({ "height": 0, "width": 0 }, slideDuration);
		$("#SlidingSidebar").animate({ "width": 28 }, slideDuration);
		isExtended = 0;
		$("#SlidingSidebarTab").removeClass();
		$("#SlidingSidebarTab").addClass('SlidingSidebarTab' + $("#SlidingSidebar").attr("class"));

	}
}

function init() {
	$("#SlidingSidebarTab").click(function () { extendContract() });
}

$(document).ready(function () {
	init();
});