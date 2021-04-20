 if(window.PopupBinding) {
   window.PopupBinding.newInstance = function ( ownerDocument ) {

 	var element = DOMUtil.createElementNS ( Constants.NS_UI, "ui:plainpopup", ownerDocument );
 	return UserInterface.registerBinding ( element, window.PopupBinding );
   }
 }