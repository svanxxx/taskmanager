function GetDate() {
	return StringToDate(getParameterByName("date"));
}
function UpdateURL() {
	var params = "";
	var urldate = getParameterByName("date");
	var date;
	if (urldate == "") {
		date = GetDate();
	}
	else {
		date = StringToDate(urldate);//update for pad foramat
	}
	urldate = "?date=" + DateToString(date);

	document.getElementById("date").valueAsDate = date;
	//	$("#date").valueAsDate = date;

	params += urldate;

	window.history.pushState("default date redirect", "default date redirect", GetPage() + params);
}
function ProcessDay(col, users) {
	for (var iuser = 0; iuser < users.length; iuser++) {
		var usr = users[iuser];
		var text = usr.notes == "" ? "TT" + usr.vacation : usr.notes;
		var div = usr.notes == "" ? "<div class=vacation>" : "<div class=normal>";
		text = div + text.replace(/\n+/g, "<br />") + "</div>";
		text = text.replace(/\TT(\d+)/g, function (a, b) {
			return "<mark onclick='{ShowTask(" + b + ");}'>" + a + "</mark>";
		})

		$($("#maintbl tbody tr[email='" + usr.user + "'] td")[col]).html(text);
	}
}
function ShowHideMore(el) {
	var el1 = $($(el).parents()[0]).children()[0];
	var el2 = $($(el).parents()[0]).children()[1];
	var b = $(el1).is(":visible") == true;
	if (b) {
		$(el1).hide();
		$(el2).show();
	} else {
		$(el2).hide();
		$(el1).show();
	}
}
function ProcessPlan(plan) {
	if (plan.length < 1)
		return;

	var text = "";
	var smalltext = "";
	var bbig = false;
	for (var ipl = 0; ipl < plan.length; ipl++) {
		var pl = plan[ipl];
		if (pl.iorder == "")
			continue;
		text += "<mark onclick='{ShowTask(" + pl.ttid + ");}'>TT" + pl.ttid + "</mark>" + " " + pl.summary + "<br />";
		if (ipl > 5 && bbig == false) {
			smalltext = text;
			bbig = true;
		}
	}

	var div = "<div>";
	if (bbig == true) {
		div += "<div>" + smalltext + "</div>";
		div += "<div style='display:none'>" + text + "</div>";
		div += "<mark onclick='{ShowHideMore(this);}'>...</mark>";
	} else {
		div += text;
	}
	div += "</div>";

	$($("#maintbl tbody tr[email='" + plan[0].usr + "'] td")[3]).html(div);
}
function LoadPlanData(userid) {
	var idplan = StartProgress("User: " + userid);
	getdata("GetPlanData", JSON.stringify({ "userid": userid , "count" : 50}), function (plan) {
		ProcessPlan(plan.d);
		EndProgress(idplan);
	})
}
function LoadUsers() {
	var idusers = StartProgress();
	getdata("GetUsers", "", function (mess) {
		EndProgress(idusers);
		for (var iuser = 0; iuser < mess.d.length; iuser++) {
			var usrtd = $("<td nowrap>");
			var yeste = $("<td>");
			var today = $("<td>");
			var tomor = $("<td>");
			var uid = mess.d[iuser].id;
			$("#maintbl tbody").append($("<tr email='" + mess.d[iuser].email + "' userid='" + mess.d[iuser].id + "'>").append(usrtd).append(yeste).append(today).append(tomor));
			$(usrtd).text(mess.d[iuser].name);
			$(usrtd).attr("usr", mess.d[iuser].name);
			$(usrtd).addClass("user");
			$(usrtd).click(function () {
				window.location.replace(GetSitePath() + "editplan.aspx?user=" + $(this).attr("usr"));
			});

				LoadPlanData(mess.d[iuser].id);
		}
		var idtoday = StartProgress("Loading today...");
		getdata("GetDailyReport", JSON.stringify({ "date": DateToString(GetDate()) }), function (mess) {
			ProcessDay(2, mess.d);
			EndProgress(idtoday);
		})
		var nextdate = GetDate();
		nextdate.setDate(nextdate.getDate() - 1);
		var idtomorrow = StartProgress("Loading yesterday...");
		getdata("GetDailyReport", JSON.stringify({ "date": DateToString(nextdate) }), function (mess) {
			ProcessDay(1, mess.d);
			EndProgress(idtomorrow);
		})
	});
}
$(function () {
	UpdateURL();
	LoadUsers();
	$("#date").focus();
	$("#form1").submit(function (e) {
		e.preventDefault();
		HightlightTasks()
	});
	$.isReady = false;
})