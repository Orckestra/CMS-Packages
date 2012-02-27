//-------------------------------------------------
//                youtube playlist jquery plugin
//-------------------------------------------------

jQuery.fn.ytplaylist = function(_options) {

	// default settings
	var options = jQuery.extend({
		holderId: '_ytvideo',
		playerHeight: '300',
		playerWidth: '450',
		thumbSize: 'small',
		showInline: true,
		autoPlay: true,
		showRelated: true,
		allowFullScreen: true
	}, _options);
	
	var currentVideoHtml = "";

	return this.each(function() {

		var selector = $(this);
		//throw a youtube player in
		function play(id) {
			var autoPlay = "";
			var showRelated = "&rel=0";
			var fullScreen = "";
			if (options.autoPlay) autoPlay = "&autoplay=1";
			if (options.showRelated) showRelated = "&rel=1";
			if (options.allowFullScreen) fullScreen = "&fs=1";

			var html = '';
			html += '<iframe src="http://www.youtube.com/v/' + id + autoPlay + showRelated + fullScreen + '"';
			html += ' height="' + options.playerHeight + '" width="' + options.playerWidth + '"></iframe>';
			window.location.hash = "#show" + id;
			return html;
		};


		//grab a youtube id from a (clean, no querystring) url (thanks to http://jquery-howto.blogspot.com/2009/05/jyoutube-jquery-youtube-thumbnail.html)
		function youtubeid(url) {
			var ytid = url.match("[\\?&]v=([^&#]*)");
			ytid = ytid[1];
			return ytid;
		};

		//load inital video
		var firstVid = selector.children("div:first-child").addClass("currentvideo").children("a").attr("href");
		//$("#" + options.holderId + "").html(play(youtubeid(firstVid)));

		//load video on request
		selector.find(".video-link").click(function() {

			if (options.showLightbox) {
				$("div.currentvideo").removeClass("currentvideo");
				showHtmlLightbox(this, {
					          width: parseInt(options.playerWidth) + 30,
					          height: parseInt(options.playerHeight) + 20,
						  html: play(youtubeid($(this).attr("href"))) 
						});
				$(this).parents("div.video-list-item").addClass("currentvideo");
			}
			else
			{		
				$("div.currentvideo").show();
				$("div.currentvideo").next("object").remove();
				$("div.currentvideo").removeClass("currentvideo");
				$(this).parents("div.video-list-item").addClass("currentvideo").after(play(youtubeid($(this).attr("href"))));
				$("div.currentvideo").hide();
			}
			return false;
		});

	});

};