
function init(contentCss) {
	tinyMCE.init({
		mode: "textareas",
		editor_selector: "mceEditor",
		encoding: "xml",
		entity_encoding: "numeric",
		invalid_elements: "script,meta,iframe",
		valid_elements: "@[id|class|style|title|dir<ltr?rtl|lang|xml::lang],"
  + "a[rel|rev|charset|hreflang|tabindex|accesskey|type|name|href|target|title|class],strong/b,em/i,strike,u,"
  + "#p,-ol[type|compact],-ul[type|compact],-li,br,img[longdesc|usemap|src|border|alt=|title|hspace|vspace|width|height|align],-sub,-sup,"
  + "-blockquote,-table[border=0|cellspacing|cellpadding|width|frame|rules|height|align|summary|bgcolor|background|bordercolor],-tr[rowspan|width|"
  + "height|align|valign|bgcolor|background|bordercolor],tbody,thead,tfoot,#td[colspan|rowspan|width|height|align|valign|bgcolor|background|bordercolor"
  + "|scope],#th[colspan|rowspan|width|height|align|valign|scope],caption,-div,-span,-code,-pre,address,-h1,-h2,-h3,-h4,-h5,-h6,hr[size|noshade],-font[face"
  + "|size|color],dd,dl,dt,cite,abbr,acronym,del[datetime|cite],ins[datetime|cite],object[classid|width|height|codebase|*],param[name|value|_value],embed[type|width"
  + "|height|src|*],map[name],area[shape|coords|href|alt|target],bdo,button,col[align|char|charoff|span|valign|width],colgroup[align|char|charoff|span|"
  + "valign|width],dfn,fieldset,form[action|accept|accept-charset|enctype|method],input[accept|alt|checked|disabled|maxlength|name|readonly|size|src|type|value],"
  + "kbd,label[for],legend,noscript,optgroup[label|disabled],option[disabled|label|selected|value],q[cite],samp,select[disabled|multiple|name|size],small,"
  + "textarea[cols|rows|disabled|name|readonly],tt,var,big",
		// General options
		theme: "advanced",
		plugins: "autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,advlist",
		height: "480",
		// Theme options
		theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,formatselect,fontselect,fontsizeselect",
		theme_advanced_buttons2: "bullist,numlist,|,outdent,indent,blockquote,|,link,unlink,anchor,code,|,insertdate,inserttime,preview,|,forecolor,backcolor,|,tablecontrols,|,hr,removeformat,",
		theme_advanced_buttons3: "",
		//theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
		//theme_advanced_buttons4 : "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak",
		theme_advanced_toolbar_location: "top",
		theme_advanced_toolbar_align: "left",
		theme_advanced_statusbar_location: "bottom",
		theme_advanced_resizing: true,

		// Example content CSS (should be your site CSS)
		content_css: contentCss

	});
}


function setup() {
	var contentHtml = $("#contentReal div:first-child").html();
	$("#contentReal").hide();
	$("#contentMceEditor").show();
	var ed = tinyMCE.get("editablePageContent");
	ed.setContent(contentHtml);
}

function cancelSave() {
	$("#contentMceEditor").hide();
	$("#contentReal").show();
}

function saveThisPage(currentPageID, placeholderId) {
	var ed = tinyMCE.get("editablePageContent");
	ed.setProgressState(1); // Show progress
	var newContent = ed.getContent();
	$.ajax({
		type: "post",
		url: "/SimpleWiki.asmx/SavePageContent",
		data: "{ 'pageId':'" + currentPageID + "', 'placeholderId':'" + placeholderId + "', 'content': '" + escape(newContent) + "'}",
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		success: function (data) {
			ed.setProgressState(0); // Hide progress
			$("#contentMceEditor").hide();
			$("#contentReal div:first-child").html(newContent);
			$("#contentReal").show();
		},
		error: function (m) {
			alert(m.responseText);
			ed.setProgressState(0);
		}
	});
}