function OnClickSeverity(id) {
	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/NewObective";
	$.ajax({
		type: "POST",
		url: webMethod,
		data: "{'id':'" + id + "'}",
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			AddObjective(mess.d);
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function LoadObjectivies() {
	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/EnumObectivies";
	$.ajax({
		type: "POST",
		url: webMethod,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			for (var i = 0; i < mess.d.length; i++) {
				AddObjective(mess.d[i]);
			}
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
$(function () {
	LoadObjectivies();
	LoadSeverities(OnClickSeverity);
})