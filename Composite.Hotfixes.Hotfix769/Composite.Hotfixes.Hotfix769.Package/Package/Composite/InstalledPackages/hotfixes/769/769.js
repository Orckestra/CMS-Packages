if (top.PopupBinding && !top.PopupBinding.hasPopupChromeHotfix) {
	top.PopupBinding.newInstance = function (ownerDocument) {
		var element = top.DOMUtil.createElementNS(
			top.Constants.NS_UI,
			"ui:plainpopup",
			ownerDocument
		);
		return top.UserInterface.registerBinding(element, window.PopupBinding);
	};

	top.PopupBinding.hasPopupChromeHotfix = true;
}
