function DropDown(dropdownId, hoverClass, mouseOffDelay) {
	if (dropdown = document.getElementById(dropdownId)) {
		var listItems = dropdown.getElementsByTagName("li");
		for (var i = 0; i < listItems.length; i++) {
			if ("ontouchstart" in window || "ontouch" in window) {
				listItems[i].ontouchstart = function () { this.className = AddClass(this); };
				listItems[i].ontouchend = function () { var that = this; setTimeout(function () { that.className = RemoveClass(that); }, mouseOffDelay); this.className = that.className; };
			}
			else {
				listItems[i].onmouseover = function () { this.className = AddClass(this); };
				listItems[i].onmouseout = function () { var that = this; setTimeout(function () { that.className = RemoveClass(that); }, mouseOffDelay); this.className = that.className; };
			}
			var anchor = listItems[i].getElementsByTagName("a");
			anchor = anchor[0];
			anchor.onfocus = function () { On(this.parentNode); };
			anchor.onblur = function () { Off(this.parentNode); };
		}
	}

	function On(li) {
		if (li.nodeName == "LI") {
			li.className = AddClass(li);
			On(li.parentNode.parentNode);
		}
	}

	function Off(li) {
		if (li.nodeName == "LI") {
			li.className = RemoveClass(li);
			Off(li.parentNode.parentNode);
		}
	}

	function AddClass(li) {
		return li.className + " " + hoverClass;
	}

	function RemoveClass(li) {
		return li.className.replace(hoverClass, "");
	}

}