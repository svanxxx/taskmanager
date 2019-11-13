function estimateDefect(element) {
	if (!IsAdmin()) {
		return;
	}
	var estim = prompt("Please enter new estimation:", element.innerHTML);
	if (!estim || estim === element.innerHTML) {
		return;
	}
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/estimateDefect",
		data: JSON.stringify({ "ttid": element.getAttribute("ttid"), "estim": estim }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			if (mess.d != element.innerHTML) {
				element.innerHTML = mess.d;
			} else {
				var prg = StartProgress("Task is locked!");
				setTimeout(function () {
					EndProgress(prg);
				}, 5000);
			}
		}
	});
}
function orderDefect(element) {
	if (!IsAdmin()) {
		return;
	}
	var order = prompt("Please enter new order:", element.innerHTML);
	if (!order || order === element.innerHTML) {
		return;
	}
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/orderDefect",
		data: JSON.stringify({ "ttid": element.getAttribute("ttid"), "order": order }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			if (mess.d != element.innerHTML) {
				element.innerHTML = mess.d;
			} else {
				var prg = StartProgress("Task is locked!");
				setTimeout(function () {
					EndProgress(prg);
				}, 5000);
			}
		}
	});
}
function assignDefect(element) {
	if (!IsAdmin()) {
		return;
	}
	var r = confirm("Are you sure you want to assign this task?");
	if (r !== true) {
		return;
	}
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/assignDefect",
		data: JSON.stringify({ "ttid": element.getAttribute("ttid"), "userid": element.getAttribute("userid") }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			if (mess.d == "") {
				var prg = StartProgress("Task is locked!");
				setTimeout(function () {
					EndProgress(prg);
				}, 5000);
			} else {
				var prgdone = StartProgress("Task updated! Synchronizing data... Keep continue your work.");
				setTimeout(function () {
					EndProgress(prgdone);
				}, 2000);
			}
		}
	});
}