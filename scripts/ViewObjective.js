function OnClickSeverity(id) {
	window.location.href = window.location.href.substring(0, location.href.lastIndexOf(".aspx") + 5) + "?severity=" + $("#s" + id).text();
}
function LoadObjectivies() {
	if (severities == null || severities.length < 1) {
		setTimeout(LoadObjectivies, 100);
		return;
	}
	var sname = getParameterByName("severity");
	if (sname == null || sname == "")
		return;
	var keyval;
	for (var i = 0; i < severities.length; i++) {
		if (severities[i].val == sname) {
			keyval = severities[i];
			break;
		}
	}
	if (keyval == null)
		return;

	AddObjective({ id: -1, idsever: keyval.key, namesever: keyval.val, x: $(window).width() * 0.05, y: $(window).height() * 0.05, sizex: $(window).width() * 0.9, sizey: $(window).height() * 0.9 });
}
$(function () {
	LoadObjectivies();
	LoadSeverities(OnClickSeverity);
})