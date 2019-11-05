function estimageDefect(element) {
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