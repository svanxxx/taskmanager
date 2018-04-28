var defclr = "#B0B0B0";
var clrsnum = 4;
var defsize = 30;

var staticday1;
var staticday2;
var staticusers;
var staticstatus;

function GetDate() {
	return StringToDate(getParameterByName("date"));
}
function GetTTPosition(row, now) {
	var position = $("#maintbl tbody tr:eq(" + row + ")").offset();
	position.left = $(document).width();

	var start = new Date(GetDate().getTime());
	var diff = now - start;
	var oneDay = 1000 * 60 * 60 * 24;
	var day = Math.floor(diff / oneDay) + 1;
	var daypart = diff / oneDay - Math.floor(diff / oneDay);

	var td = $("#maintbl tbody tr:eq(" + row + ") td:eq(" + day + ")");
	if (td.length == 0)
		return position;

	while (td.hasClass("weekend") || td.children(0).hasClass("vacation")) {
		day++;
		var inc = new Date(now.getTime() + oneDay);
		now.setTime(inc.getTime());
		td = $("#maintbl tbody tr:eq(" + row + ") td:eq(" + day + ")");
		if (td.length == 0)
			return position;
	}
	position = $(td).offset();
	position.left += defsize * daypart;
	return position;
}
function ProcessUser(iuser, row) {
	var id = StartProgress("User: " + iuser);
	getdata("GetPlanData", JSON.stringify({ "userid": iuser, "count": 80 }), function (plan) {
		EndProgress(id);
		var oneDay = 1000 * 60 * 60 * 24;
		var now = new Date();
		now.setHours(0, 0, 0, 0);

		var delselector;
		for (var i = 0; i < plan.d.length; i++) {
			if (i == 0) {
				$("div[tt4user='" + iuser + "']").remove();
			}
			if (plan.d[i].idsev == null || plan.d[i].idsev == "" || plan.d[i].idsev == 0 || plan.d[i].iorder == "")
				continue;

			var pos1 = GetTTPosition(row, now);
			now = new Date(now.getTime() + plan.d[i].est / 8 * oneDay);
			var pos2 = GetTTPosition(row, now);

			var ttid = plan.d[i].ttid;
			$(document.body).append("<div id='" + ttid + "'></div>");
			var tt = $("#" + ttid);
			$(tt).addClass("tt");
			$(tt).addClass("iddisp" + plan.d[i].iddisp);
			$(tt).css("opacity", 0.7);
			$(tt).css("top", pos1.top);
			$(tt).css("left", pos1.left);
			$(tt).css("position", "absolute");
			$(tt).css("background-color", defclr);
			$(tt).width(pos2.left - pos1.left);
			$(tt).height(defsize);
			$(tt).attr("idsev", plan.d[i].idsev);
			$(tt).attr("iorder", plan.d[i].iorder);
			$(tt).attr("tt4user", iuser);
			$(tt).prop("title", ttid + " " + plan.d[i].summary + " (" + plan.d[i].est + ")" + " - " + plan.d[i].est + " - " + plan.d[i].sev);
			$(tt).dblclick(function () {
				ShowTask($(this).attr("id"));
			});
		}
		ColorTasks();
		HightlightTasks();
	})
}
function ProcessUsers(users) {
	for (var iuser = 0; iuser < users.length; iuser++) {
		ProcessUser(users[iuser].id, iuser);
	}
}
function ColorTasks() {
	$(".tt").css("background-color", defclr);
	for (var i = 0; i < clrsnum; i++) {
		var pref = i + 1;
		var sev = parseInt(getParameterByName("sev" + pref));
		if (sev > 0)
			$(".tt[idsev=" + sev + "]").css("background-color", $("#sev" + pref).parents(".clselector").css("background-color"));
	}
}
function UpdateURL() {
	var params = "";
	var urldate = getParameterByName("date");
	var date = new Date();
	date.setDate(date.getDate() - 7);
	if (urldate != "") {
		date = StringToDate(urldate);//update for pad foramat
	}
	urldate = "?date=" + DateToString(date);

	params += urldate;

	var sev1 = getParameterByName("sev1");
	var sev2 = getParameterByName("sev2");
	var sev3 = getParameterByName("sev3");
	var sev4 = getParameterByName("sev4");

	$(".sevselector").each(function () {
		if ($(this).find("option").length > 2 && $(this).find(":selected").length > 0) {
			if ($(this).attr("id") == "sev1")
				sev1 = $(this).find(":selected").attr("key");
			else if ($(this).attr("id") == "sev2")
				sev2 = $(this).find(":selected").attr("key");
			else if ($(this).attr("id") == "sev3")
				sev3 = $(this).find(":selected").attr("key");
			else if ($(this).attr("id") == "sev4")
				sev4 = $(this).find(":selected").attr("key");
		}
	})
	window.history.pushState("default date redirect", "default date redirect", GetPage() + params + "&sev1=" + sev1 + "&sev2=" + sev2 + "&sev3=" + sev3 + "&sev4=" + sev4);
}
function Check4Updates() {
	getdata("GetTasksUpdateTime", "", function (mess) {
		if (staticstatus == null)
			staticstatus = mess.d;
		else {
			if (staticstatus != mess.d) {
				staticstatus = mess.d;
				ReLoadData();
			}
		}
	})
	setTimeout(Check4Updates, 60000);
}
function ColorUsers(users) {
	$($("tr[email] td")[0]).removeClass("online");
	$($("tr[email] td")[0]).removeClass("vacation");
	for (var i = 0; i < users.length; i++) {
		if (users[i].online == "1")
			$($("tr[email='" + users[i].email + "'] td")[0]).addClass("online");
		else if (users[i].vacations_count != "0")
			$($("tr[email='" + users[i].email + "'] td")[0]).addClass("vacnorm");
	}
}
function Check4Online() {
	getdata("GetUsers", "", function (mess) {
		ColorUsers(mess.d);
	})
	setTimeout(Check4Online, 60000);
}
function ReLoadData() {
	LoadData(staticday1, staticday2, staticusers);
}
function LoadWorkDays(days1, days2) {
	getdata("GetWorkDaysFromDate", JSON.stringify({ "date": DateToString(days1) }), function (workdays) {
		$("#maintbl tbody tr div.workdayused").removeClass("workdayused");
		var dstart = GetDate();
		var mstodays = 1000 * 3600 * 24;
		$("#maintbl tbody tr").each(function () {
			var email = $(this).attr("email");
			for (var iday = 0; iday < workdays.d.length; iday++) {
				if (workdays.d[iday].email == email) {
					var dent = StringToDate(workdays.d[iday].date);
					var diffDays = 1 + Math.ceil(Math.abs(dent.getTime() - dstart.getTime()) / mstodays);
					var daycell = $(this).find("td:eq(" + diffDays + ")");
					var daydiv = $(daycell).find("div");
					$(daydiv).addClass("workdayused");
				}
			}
		})
	})
}
function LoadData(days1, days2, users) {

	staticday1 = new Date(days1.getTime());
	staticday2 = new Date(days2.getTime());
	staticusers = users;

	getdata("GetOutDays", JSON.stringify({ "datefrom": DateToString(days1), "dateto": DateToString(days2) }), function (outdays) {
		$("#maintbl tbody tr div.vacation").removeClass("vacation");
		$("#maintbl tbody tr div.vacnorm").removeClass("vacnorm");
		$("#maintbl tbody tr div.vacsick").removeClass("vacsick");
		$("#maintbl tbody tr div.vacunp").removeClass("vacunp");

		var dstart = GetDate();
		var mstodays = 1000 * 3600 * 24;
		$("#maintbl tbody tr").each(function () {
			var email = $(this).attr("email");
			for (var iday = 0; iday < outdays.d.length; iday++) {
				if (outdays.d[iday].usr == email) {
					var dent = StringToDate(outdays.d[iday].dateenter);
					var diffDays = 1 + Math.ceil(Math.abs(dent.getTime() - dstart.getTime()) / mstodays);
					var daycell = $(this).find("td:eq(" + diffDays + ")");
					var daydiv = $(daycell).find("div");
					var doadd = true;
					while ($(daydiv).hasClass("vacation") || $(daycell).hasClass("weekend")) {
						diffDays++;
						daycell = $(this).find("td:eq(" + diffDays + ")");
						if (daycell.length < 1) {
							doadd = false;
							break;
						}
						daydiv = $(daycell).find("div");
					}
					if (doadd) {
						$(daydiv).addClass("vacation");
						$(daydiv).prop("title", outdays.d[iday].ttid);
						var sumup = outdays.d[iday].summary.toUpperCase()
						if (sumup.indexOf("SICK") > -1)
							$(daydiv).addClass("vacsick");
						else if (sumup.indexOf("UNPAID") > -1)
							$(daydiv).addClass("vacunp");
						else
							$(daydiv).addClass("vacnorm");
						$(daydiv).off("click").on("click", function () {
							ShowTask($(this).attr("title"));
						})
					}
				}
			}
		})
		ProcessUsers(users);

		var selector = "#maintbl tbody tr td div.day:not(.vacation)";
		$(selector).off("click").on("click", function () {
			if (!confirm('Are you sure you want to schedule vacation for this day?')) {
				return;
			}
			var date = new Date(parseInt($(this).attr("date")));
			var email = $($(this).parent()).parent().attr("email");
			getdata("ScheduleVacation", JSON.stringify({ "useremail": email, "date": date }), function (mess) {
				if (!mess.d)
					alert("No vacations available");
				else {
					ReLoadData();
					ShowTask(mess.d);
				}
			})
		})
	})
}
function Load2Do() {
	var pr = StartProgress("Checking 4 new tasks...")
	getdata("Get2Do", "", function (mess) {
		var pers = mess.d;
		$("div[id='redhint']").remove();
		$.each(pers, function (index, per) {
			if (per.name != "") {
				$("#maintbl tbody tr td:contains('" + per.name + "')").append("<div id='redhint' class='undone'>" + per.count + "</div>");
			}
		});
		EndProgress(pr);
	})
	setTimeout(Load2Do, 10000);
}
function LoadUsers() {
	var idusers = StartProgress("Loading users...");
	getdata("GetUsers", "", function (mess) {
		EndProgress(idusers);

		var days1 = new Date(GetDate().getTime());
		var days2 = new Date(days1.getFullYear() + 1, days1.getMonth(), days1.getDate());

		$("#maintbl thead").append($('<tr>')
			.append($('<td>').text(""))
		)
		$("#maintbl thead").append($('<tr>')
			.append($('<td>').text("Name"))
		)

		for (var iuser = 0; iuser < mess.d.length; iuser++) {
			var usrtd = $("<td nowrap>");
			$("#maintbl tbody").append($("<tr email='" + mess.d[iuser].email + "'>").append(usrtd));
			$(usrtd).text(mess.d[iuser].name);
			$(usrtd).attr("usr", mess.d[iuser].name);
			$(usrtd).addClass("user");
			$(usrtd).click(function () {
				window.location.replace(GetSitePath() + "editplan.aspx?user=" + $(this).attr("usr"));
			});
		}
		Load2Do();

		var monthsrow = $("#maintbl thead tr")[0];
		var daysrow = $("#maintbl thead tr")[1];
		var currday = 0;
		var prevmnth = "";
		var workday = new Date(days1.getTime());
		while (workday.getTime() != days2.getTime()) {
			var dclass = workday.getDay() > 5 || workday.getDay() < 1 ? "weekend" : "workday";
			var mnth = monthsNames[workday.getMonth()];

			$(daysrow).append($("<td class='" + dclass + "'>")
				.append($("<div class='day'>").text(workday.getDate()))
			)

			$(monthsrow).append($("<td class='" + dclass + "'>")
				.append($("<div class='day'>").text(""))
			)

			var daysel = $("#maintbl thead tr:first td:eq(" + (currday + 1) + ")");
			var offset = $(daysel).offset();

			if ($("#" + mnth).length == 0) {
				$(document.body).append("<div id='" + mnth + "'>" + mnth + "</div>");
				var mn = $("#" + mnth);
				$(mn).addClass("month");
				$(mn).css("top", offset.top);
				$(mn).css("left", offset.left);
				$(mn).css("position", "absolute");
				$(mn).width($(daysel).outerWidth());
				$(mn).height(defsize);
				$(mn).css('border', '1px solid black');
				prevmnth = mnth;
			}
			else {
				var mn = $("#" + prevmnth);
				$(mn).width($(daysel).offset().left + $(daysel).outerWidth() - $(mn).offset().left);
			}

			$('#maintbl tbody tr').each(function () {
				$(this).append($("<td class='" + dclass + "'>")
					.append($("<div class='day' date='" + workday.getTime() + "'>"))
				)
			})
			workday.setDate(workday.getDate() + 1);
			currday++;
		}

		LoadData(days1, days2, mess.d);
		LoadWorkDays(days1, days2);
		ColorUsers(mess.d);
		setTimeout(Check4Updates, 60000);
		setTimeout(Check4Online, 60000)
	});
}
function LoadSeverities() {
	$(".sevselector").each(function () {
		$(this).append($("<option></option>")
			 .attr("value", "Unused")
			 .attr("key", -1)
			 .text("Unused")
			);
		$(this).on('change', function () {
			UpdateURL();
			ColorTasks();
		});
	});

	var idsevs = StartProgress("Loading severities...");
	getdata("LoadSeverities", "", function (mess) {
		for (var isev = 0; isev < mess.d.length; isev++) {
			$(".sevselector").each(function () {
				$(this).append($("<option></option>")
					 .attr("value", mess.d[isev].val)
					 .attr("key", mess.d[isev].key)
					 .text(mess.d[isev].val));
			});
		}

		var sev1 = parseInt(getParameterByName("sev1")); if (isNaN(sev1)) sev1 = -1;
		var sev2 = parseInt(getParameterByName("sev2")); if (isNaN(sev2)) sev2 = -1;
		var sev3 = parseInt(getParameterByName("sev3")); if (isNaN(sev3)) sev3 = -1;
		var sev4 = parseInt(getParameterByName("sev4")); if (isNaN(sev4)) sev4 = -1;
		if (sev1 > 0)
			$("#sev1 option[key=" + sev1 + "]").attr("selected", "selected");
		if (sev2 > 0)
			$("#sev2 option[key=" + sev2 + "]").attr("selected", "selected");
		if (sev3 > 0)
			$("#sev3 option[key=" + sev3 + "]").attr("selected", "selected");
		if (sev4 > 0)
			$("#sev4 option[key=" + sev4 + "]").attr("selected", "selected");

		EndProgress(idsevs);
	})
}
function HightlightTasks() {
	$("div.filtered").removeClass("filtered");
	var text = $("#ttfilter").val();
	var re = RegExp(text, "i");
	if (text != "")
		$("div.tt[title]").filter(function () { return re.test(this.title) }).addClass("filtered");
}
$(function () {
	UpdateURL();
	LoadSeverities();
	LoadUsers();
	$('#tttoggle').change(function () {
		$(".tt").toggle();
	})
	$("#ttfilter").focus();
	$("#form1").submit(function (e) {
		e.preventDefault();
		HightlightTasks()
	});
})