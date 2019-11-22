function estimateDefect(element) {
	if (!IsAdmin()) {
		return;
	}
	var estim = prompt("Please enter new estimation:", element.innerHTML);
	if (!estim || estim === element.innerHTML) {
		return;
	}
	waitForProcess();
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/estimateDefect",
		data: JSON.stringify({ "ttid": element.getAttribute("ttid"), "estim": estim }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			waitForProcessEnd();
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
	waitForProcess();
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/orderDefect",
		data: JSON.stringify({ "ttid": element.getAttribute("ttid"), "order": order }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			waitForProcessEnd();
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
	var ttid = element.getAttribute("ttid");
	$("#selectuser")[0].setAttribute("ttid", ttid);
	$("#selectuser").modal("show");
	return;
}
function assignDefectUser(element) {
	if (!IsAdmin()) {
		return;
	}
	$("#selectuser").modal("hide");
	waitForProcess();
	$.ajax({
		type: "POST",
		url: GetSitePath() + "DefectService.asmx/assignDefect",
		data: JSON.stringify({ "ttid": $("#selectuser")[0].getAttribute("ttid"), "userid": element.getAttribute("userid") }),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			waitForProcessEnd();
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