var _callsettname = "JColResizer0";
function getTabKey() {
	return GetPageName() + _callsettname;
}
$(function () {
	var tables = $(".table-colresizable");
	if (tables.length > 0) {
		var data = localStorage[_callsettname];
		if (data === undefined) {
			data = "36;69;40;108;41;1152;82;78;61;58;63;77;1865";
		}
		sessionStorage[_callsettname] = data;
		tables.colResizable({
			liveDrag: true,
			postbackSafe: true,
			onResize: function (target) {
				setTimeout(function () {
					localStorage[getTabKey()] = sessionStorage[_callsettname];
				}, 1000);
			}
		});
	}
});