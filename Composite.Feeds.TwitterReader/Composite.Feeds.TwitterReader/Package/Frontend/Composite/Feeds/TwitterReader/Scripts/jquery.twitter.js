(function ($) {
	/*
	jquery.twitter.js v1.5
	Last updated: 08 July 2009

	Created by Damien du Toit
	http://coda.co.za/blog/2008/10/26/jquery-plugin-for-twitter

	Licensed under a Creative Commons Attribution-Non-Commercial 3.0 Unported License
	http://creativecommons.org/licenses/by-nc/3.0/
	*/

	$.fn.getTwitter = function (options) {

		$.fn.getTwitter.defaults = {
			userName: null,
			numTweets: 5,
			loaderText: "Loading tweets...",
			slideIn: true,
			slideDuration: 750,
			showHeading: true,
			headingText: "Latest Tweets",
			showProfileLink: true,
			showTimestamp: true
		};

		var o = $.extend({}, $.fn.getTwitter.defaults, options);

		return this.each(function () {
			var c = $(this);

			// hide container element, remove alternative content, and add class
			c.hide().empty().addClass("twitted");

			// add heading to container element
			if (o.showHeading) {
				c.append("<h2>" + o.headingText + "</h2>");
			}

			// add twitter list to container element
			var twitterListHTML = "<ul id=\"" + c.attr("id") + "_update_list\" class=\"twitter_update_list\"><li></li></ul>";
			c.append(twitterListHTML);

			var tl = $("#" + c.attr("id") + "_update_list");

			// hide twitter list
			tl.hide();

			// add preLoader to container element
			var preLoaderHTML = $("<p class=\"preLoader\">" + o.loaderText + "</p>");
			c.append(preLoaderHTML);

			// add Twitter profile link to container element
			if (o.showProfileLink) {
				var profileLinkHTML = "<p class=\"profileLink\"><a href=\"http://twitter.com/" + o.userName + "\">http://twitter.com/" + o.userName + "</a></p>";
				c.append(profileLinkHTML);
			}

			// show container element
			c.show();

			$.getScript("http://api.twitter.com/1/statuses/user_timeline/" + o.userName + ".json?callback=twitterCallback" + c.attr("id") + "&include_rts=1&count=" + o.numTweets, function () {
				// remove preLoader from container element
				$(preLoaderHTML).remove();

				// remove timestamp and move to title of list item
				if (!o.showTimestamp) {
					tl.find("li").each(function () {
						var timestampHTML = $(this).children("a");
						var timestamp = timestampHTML.html();
						timestampHTML.remove();
						$(this).attr("title", timestamp);
					});
				}

				// show twitter list
				if (o.slideIn) {
					// a fix for the jQuery slide effect
					// Hat-tip: http://blog.pengoworks.com/index.cfm/2009/4/21/Fixing-jQuerys-slideDown-effect-ie-Jumpy-Animation
					var tlHeight = tl.data("originalHeight");

					// get the original height
					if (!tlHeight) {
						tlHeight = tl.show().height();
						tl.data("originalHeight", tlHeight);
						tl.hide().css({ height: 0 });
					}

					tl.show().animate({ height: tlHeight }, o.slideDuration);
				}
				else {
					tl.show();
				}

				// add unique class to first list item
				tl.find("li:first").addClass("firstTweet");

				// add unique class to last list item
				tl.find("li:last").addClass("lastTweet");
			});
		});
	};
})(jQuery);

function twitterCallback2(twitters, id) {
	var statusHTML = [];
	for (var i = 0; i < twitters.length; i++) {
		var username = twitters[i].user.screen_name;
		var status = twitters[i].text.replace(/((https?|s?ftp|ssh)\:\/\/[^"\s\<\>]*[^.,;'">\:\s\<\>\)\]\!])/g, function (url) {
			return '<a href="' + url + '">' + url + '</a>';
		}).replace(/\B@([_a-z0-9]+)/ig, function (reply) {
			return reply.charAt(0) + '<a href="http://twitter.com/' + reply.substring(1) + '">' + reply.substring(1) + '</a>';
		});
		statusHTML.push('<li><span>' + status + '</span> <a style="font-size:85%" href="http://twitter.com/' + username + '/statuses/' + twitters[i].id + '">' + relative_time(twitters[i].created_at) + '</a></li>');
	}
	document.getElementById(id + '_update_list').innerHTML = statusHTML.join('');
};

function relative_time(time_value) {
	var values = time_value.split(" ");
	time_value = values[1] + " " + values[2] + ", " + values[5] + " " + values[3];
	var parsed_date = Date.parse(time_value);
	var relative_to = (arguments.length > 1) ? arguments[1] : new Date();
	var delta = parseInt((relative_to.getTime() - parsed_date) / 1000);
	delta = delta + (relative_to.getTimezoneOffset() * 60);

	if (delta < 60) {
		return 'less than a minute ago';
	} else if (delta < 120) {
		return 'about a minute ago';
	} else if (delta < (60 * 60)) {
		return (parseInt(delta / 60)).toString() + ' minutes ago';
	} else if (delta < (120 * 60)) {
		return 'about an hour ago';
	} else if (delta < (24 * 60 * 60)) {
		return 'about ' + (parseInt(delta / 3600)).toString() + ' hours ago';
	} else if (delta < (48 * 60 * 60)) {
		return '1 day ago';
	} else {
		return (parseInt(delta / 86400)).toString() + ' days ago';
	}
}