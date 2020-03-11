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
	var ttid = element.getAttribute("uniqueid");
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
				}, 1500);
			}
		}
	});
}
function tooltipImg(e) {
	var id = e.target.getAttribute("userid");
	var ttid = e.target.getAttribute("uniqueid");

	var element = document.getElementById("tooltipImgCntrl");
	if (element) {
		if (element.getAttribute("uniqueid") == ttid) {
			return;
		}
		element.remove();
	}
	element = document.createElement("div");
	element.setAttribute("uniqueid", ttid);
	element.id = "tooltipImgCntrl";
	element.style.position = "fixed";
	element.style.opacity = 0.9;
	element.style.left = e.clientX + "px";
	element.style.top = e.clientY + "px";
	element.style.width = "150px";
	element.style.color = "white";
	element.style.zIndex = 9999;
	element.style.padding = 10;
	element.style.backgroundColor = "black";

	var img = document.createElement("img");
	img.style.width = "150px";
	img.style.height = "150px";
	img.style.padding = "2px";
	img.src = "getUserImg.ashx?sz=150&ttid=" + id;

	element.appendChild(img);

	var span = document.createElement("div");
	span.style.backgroundColor = "black";
	span.style.textAlign = "center";
	span.innerHTML = "Loading...";

	fetch("UsersService.asmx/getUser", {
		method: "post",
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			id: id
		})
	}).then(function (response) {
		return response.json();
	}).then(function (json) {
		span.innerHTML = json.d.FULLNAME;
	});

	element.appendChild(span);

	document.body.appendChild(element);
	document.tooltipImgCntrlDate = new Date();
}

function tooltipImgOut() {
	setTimeout(function () {
		if (document.tooltipImgCntrlDate) {
			var diff = (new Date()) - document.tooltipImgCntrlDate;
			var element = document.getElementById("tooltipImgCntrl");
			if (element) {
				if (diff > 1000) {
					element.remove();
				} else {
					tooltipImgOut();
				}
			}
		}
	}, 1000);
}