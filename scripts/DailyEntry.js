var mouseX;
var mouseY;
var LastTasksUpdate = "";
var SelectedTask = "";
var TextChecked = false;
var g_oldEditVal;

function TTID() {
	return SelectedTask.substring(0, SelectedTask.indexOf("(")).trim();
}
function SetTaskStatus(stat, refadd, refrepl) {
	senddata("SetTaskStatus", "{'ttid':'" + TTID() + "', 'stat':'" + stat + "', 'refadd':'" + refadd + "', 'refrepl':'" + refrepl + "', 'userid':'" + $.cookie("userid") + "'}");
	UpdatePlan();
	if (stat != 3 && stat != 10){ // when finished - do not remove from work today
		var newtext = [];
		var tt = TTID();
		$.each($("#TodayText").val().split('\n'), function (index, line) {
			if (line.indexOf(tt) == -1) {
				var tr = line.trim();
				if (tr != "")
					newtext.push(line.trim());
			}
		});
		$("#TodayText").val(newtext.join('\n'));
	}
}
function UpdatePlan() {
	var user = $.cookie("userid");
	if (!user)
		return;

	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/GetPlan";
	var params = "{userid:" + user + "}";
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			if ($("#TomorrowPlan").size() > 0) {
				$("#TomorrowPlan").empty();
				$.each(msg.d, function (index, line) {
					$("#TomorrowPlan").append("<div class='dropdowncontainer'>" + line + "</div>");
				});
			}
			MarkPlan(false);
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function Check4Newtasks() {
	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/GetTasksUpdateTime";
	$.ajax({
		type: "POST",
		url: webMethod,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			if (LastTasksUpdate != msg.d) {
				LastTasksUpdate = msg.d;
				UpdatePlan();
			}
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
	if ($.cookie("AutoTT") == 'true' && IsToday()) {
		getdata("GetUpdatedTasks", "{'userid':'" + $.cookie("userid") + "'}", function (msg) {
			if (msg.d != null) {
				var lines = $("#TodayText").val().split('\n');
				var arrayLength = lines.length;
				var newtext = "";
				var prefix = "Updated tasks: ";
				var updated = false;
				for (var i = 0; i < arrayLength; i++) {
					if (lines[i].substring(0, prefix.length) == prefix) {
						lines[i] = prefix + msg.d;
						updated = true;
					}
				}
				if (!updated) {
					lines.push(prefix + msg.d);
				}
				$("#TodayText").val(lines.join("\n"));
			}
		})
		getdata("GetScheduledTasks", "{'userid':'" + $.cookie("userid") + "'}", function (msg) {
			if (msg.d != null) {
				var lines = $("#TodayText").val().split('\n');
				var arrayLength = lines.length;
				var newtext = "";
				var prefix = "Scheduled tasks: ";
				var updated = false;
				for (var i = 0; i < arrayLength; i++) {
					if (lines[i].substring(0, prefix.length) == prefix) {
						lines[i] = prefix + msg.d;
						updated = true;
					}
				}
				if (!updated) {
					lines.push(prefix + msg.d);
				}
				$("#TodayText").val(lines.join("\n"));
			}
		})
	}
	setTimeout(Check4Newtasks, 60000);
}
function MarkPlan(timer) {
	if (timer == true)
		setTimeout(function () { MarkPlan(true);}, 1000);

	if (timer && TextChecked)
		return;

	if ($("#TomorrowPlan").size() < 1 || $("#TomorrowPlan").children().length < 1)
		return;

	var lines = $("#TodayText").val().split('\n');
	$($("#TomorrowPlan").children().get()).each(function () {
		var i;
		var bFind = false;
		for (i = 0; i < lines.length; i++){
			if (lines[i].length < 7)
				continue;
			if (lines[i].substring(0, 2) != "TT")
				continue;
			var tt = lines[i].substring(2, 7);
			if ($(this).text().indexOf(tt) > -1) {
				bFind = true;
				break;
			}
		}
		if (bFind)
			$(this).addClass("currenttask");
		else
			$(this).removeClass("currenttask");
	});
	TextChecked = true;
}
function IsToday() {
	var urldate = getParameterByName('date');
	var vals = urldate.split('-');
	if (vals.length != 3)
		return false;
	var repdt = new Date(vals[2], parseInt(vals[0]) - 1, vals[1])
	var currdt = new Date();
	currdt.setHours(0, 0, 0, 0);
	return currdt.valueOf() == repdt.valueOf();
}
function CheckCheckinTimer(){
	var curr = new Date($.now());
	var currtime = new Date();
	var intext = $("#mTimeIN").val();
	if (intext == "") {
		intext = ("0" + (currtime.getHours())).slice(-2) + ':' + ("0" + (currtime.getMinutes())).slice(-2);
	}
	var In = intext.split(':');
	if (In.length)
	var diff = 9 - (curr.getHours() - parseInt(In[0]) + (curr.getMinutes() - parseInt(In[1])) / 60 + (curr.getSeconds() / 60 / 60));
	var diffin = 9 - diff;
	diff = Math.max(diff, 0);
	diffin = Math.max(diffin, 0);
	var min = 60 * (diff - parseInt(diff));
	var minin = 60 * (diffin - parseInt(diffin));
	$("#progressmess").text("Left: " + parseInt(diff) + "h " + parseInt(min) + "m " + parseInt(60 * (min - parseInt(min))) + "s");
	$("#progressmessdown").text("Time in: " + parseInt(diffin) + "h " + parseInt(minin) + "m " + parseInt(60 * (minin - parseInt(minin))) + "s");
	$(".progress-bar").attr("style", "width: " + 100 * (9 - diff) / 9 + "%");
	$(".progress-bar").text(parseInt(100 * (9 - diff) / 9) + "%");

	if ($("#RunTime").is(':checked')) {
		var urldate = getParameterByName('date');
		var user = $.cookie("userid");
		if (urldate && user) {
			var vals = urldate.split('-');
			if (vals.length == 3) {
				var repdt = new Date(vals[2], parseInt(vals[0]) - 1, vals[1])
				var currdt = new Date();
				var currtime1 = new Date();
				currtime1.setHours(8, 0, 0, 0);
				var currtime2 = new Date();
				currtime2.setHours(22, 0, 0, 0);
				currdt.setHours(0, 0, 0, 0);
				if (currdt.valueOf() == repdt.valueOf() && currtime > currtime1 && currtime < currtime2) {
					var txt = ("0" + (currtime.getHours())).slice(-2) + ':' + ("0" + (currtime.getMinutes())).slice(-2);
					if (txt != $('#mTimeOut').val()) {
						senddata('UpdateCheckOut', "{'userid':'" + user + "'}");
						$('#mTimeOut').val(txt);
					}
				}
			}
		}
	}
	setTimeout(CheckCheckinTimer, 1000);
}
function InitMenu(){
	$(document).contextmenu({
		delegate: ".dropdowncontainer",
		preventContextMenuForPopup: true,
		preventSelect: true,
		taphold: true,
		menu: [
			{title: "View Task", cmd: "taskView", uiIcon: "ui-icon-newwin"},
			{title: "----"},
			{title: "Add task to today''s report", cmd: "taskAdd", uiIcon: "ui-icon-plus"},
			{title: "Set task status to PROCESS", cmd: "taskProcess", uiIcon: "ui-icon-blank"},
			{title: "Set task status to SCHEDULED", cmd: "taskScheduled", uiIcon: "ui-icon-blank"},
			{title: "Set task status to FINISHED", cmd: "taskFinished", uiIcon: "ui-icon-blank"},
			{title: "Set task status to 'On BST Test'", cmd: "taskOnBSTTest", uiIcon: "ui-icon-blank"},
			],
		// Handle menu selection to implement a fake-clipboard
		select: function(event, ui) {
			var $target = ui.target;
			SelectedTask = $target.text().substring($target.text().indexOf("TT"));
			switch(ui.cmd){
			case "taskAdd":
				$("#TodayText").val($("#TodayText").val() + "\n" + SelectedTask);
				MarkPlan(false);
				break;
			case "taskProcess":
				SetTaskStatus(10, "P", "");
				MarkPlan(false);
				break;
		   case "taskScheduled":
				SetTaskStatus(1, "S", "");
				MarkPlan(false);
				break;
			case "taskFinished":
				SetTaskStatus(3, "F", "");
				MarkPlan(false);
				break;
			case "taskOnBSTTest":
				SetTaskStatus(14, "B", "");
				MarkPlan(false);
				break;
			case "taskView":
				var tt  = $target.text();
				tt = tt.substring(tt.indexOf("TT") + 2, tt.indexOf("TT") + 7);
				ShowTask(tt);
				break;
			}
			// Optionally return false, to prevent closing the menu now
		}
	});
}
function InitLayout(){
	$("body").layout({
		stateManagement__enabled:false,
		livePaneResizing:			true,
		togglerLength_open:		0,
		resizable:					false,
		spacing_open:				0,
		center__resizable:		true,
		center__spacing_open:	5,
		south__resizable:			true,
		south__spacing_open:		5,
		south__size:				.4,
		});
	$("#PlanContainer").layout({
		stateManagement__enabled:false,
		livePaneResizing:			true,
		togglerLength_open:		0,
		resizable:					false,
		spacing_open:				0,
		north__size:				30,
		});
}
function SetPlanToggleSwitch(AssignedFlag) {
	var ctrlTomorrow = document.getElementById('TomorrowText');
	var ctrlPlan = document.getElementById('TomorrowPlan');
	var ctrlAssBtn = document.getElementById('PlanAssignedButton');
	var ctrlPlnBtn = document.getElementById('PlanMyButton');

	if (AssignedFlag == 1) {
		ctrlAssBtn.style.backgroundColor = "#00FFFF";
		ctrlPlnBtn.style.backgroundColor = "white";
		ctrlTomorrow.style.display = "none";
		ctrlPlan.style.display = "inline-block";
	}
	else {
		ctrlAssBtn.style.backgroundColor = "white";
		ctrlPlnBtn.style.backgroundColor = "#00FFFF";
		ctrlTomorrow.style.display = "inline-block";
		ctrlPlan.style.display = "none";
	}
}
function TogglePlan() {
	if (document.getElementById('TomorrowText').style.display == "inline-block") {
		SetPlanToggleSwitch(1);
	}
	else {
		SetPlanToggleSwitch(0);
	}
}
function init_body() {
	var isShift = false;
	var isCtrl = false;
	document.onkeyup = function (e) {
		if (e.which == 16) isShift = false;
		if (e.which == 17) isCtrl = false;
	}
	document.onkeydown = function (e) {
		if (e.which == 16) isShift = true;
		if (e.which == 17) isCtrl = true;
		if (e.which == 83 && isShift == true && isCtrl == true) {
			//run code for shift+S -- ie, save!
			$("#Save").trigger("click");
			return true;
		}
	}

	if (document.getElementById('TomorrowText').value == "") {
		SetPlanToggleSwitch(1);
	}
	else {
		SetPlanToggleSwitch(0);
	}
	InitLayout();
}

$(function () {
	if (getParameterByName('date') == "") {
		var cd = new Date();
		window.history.pushState('Todays report', 'Daily Entry', window.location.href.substring(0, location.href.indexOf(".aspx") + 5) + '?date=' + ("0" + (cd.getMonth() + 1)).slice(-2) + '-' + ("0" + cd.getDate()).slice(-2) + '-' + cd.getFullYear());
	}

	setTimeout(Check4Newtasks, 1000);
	setTimeout(function () { MarkPlan(true); }, 5000);

	$("#mTimeIN,#mTimeOut,#mBreakTime").each(function () {
		$(this).focus(function () {
			g_oldEditVal = $(this).val();
		});
	});
	$("#mTimeIN,#mTimeOut,#mBreakTime").each(function () {
		$(this).change(function () {
			var val = $(this).val().split(":");
			if (2 != val.length || val[0] < 0 || 23 < val[0] || val[1] < 0 || 59 < val[1])
				$(this).val(g_oldEditVal);
		});
	});

	$("#InBtn").click(function () {
		var today = new Date();
		$('#mTimeIN').val(today.getHours() + ":" + today.getMinutes());
		return false;
	})
	$("#OutBtn").click(function () {
		var today = new Date();
		$('#mTimeOut').val(today.getHours() + ":" + today.getMinutes());
		return false;
	})

	$(document).mousemove(function (e) {
		mouseX = e.pageX;
		mouseY = e.pageY;
	});
	$("#TodayText").keyup(function () {
		TextChecked = false;
	})
	$(document).on("mousedown", ".dropdowncontainer", function (e) {
		if (e.which == 2) {
			var tt  = $(this).text();
			tt = tt.substring(tt.indexOf("TT") + 2, tt.indexOf("TT") + 7);
			ShowTask(tt);
			e.preventDefault();
			return false;
		}
	});

	$("#RunTime").attr('checked', $.cookie("PerformRunTimeCheckin") == 'true');
	$("#RunTime").click(function () {
		$.cookie("PerformRunTimeCheckin", $("#RunTime").is(':checked').toString(), { expires: 365 });
	})
	$("#AutoTT").attr('checked', $.cookie("AutoTT") == 'true');
	$("#AutoTT").click(function () {
		$.cookie("AutoTT", $("#AutoTT").is(':checked').toString(), { expires: 365 });
	})

	var cname = "_RTAJAX_CopyCheckBox" + $.cookie("userid");
	var cpy = $.cookie(cname) == 'true' || $.cookie(cname) == 'on';
	$("#copycheck").attr('checked', cpy);
	$("#copycheck").click(function () {
		$.cookie(cname, $("#copycheck").is(':checked').toString(), { expires: 365 });
	})
	$("#todaybtn").click(function () {
		var cpy = $.cookie(cname) == 'true' || $.cookie(cname) == 'on';
		getdata("AddToday", "{'userid':'" + $.cookie("userid") + "', 'copy':'" + cpy + "'}", function () {
			window.location.replace(GetPage());
		});
	})
	$("#discbtn").click(function () {
		location.reload();
	})
	$("#newrbtn").click(function () {
		var cpy = $.cookie(cname) == 'true' || $.cookie(cname) == 'on';
		var date = getParameterByName("date");
		getdata("NewRec", "{'userid':'" + $.cookie("userid") + "', 'copy':'" + cpy + "', 'date':'"+date+"'}", function () {
			location.reload();
		});
	})
	$("#delebtn").click(function () {
		var date = getParameterByName("date");
		getdata("DelRec", "{'userid':'" + $.cookie("userid") + "', 'date':'" + date + "'}", function () {
			location.reload();
		});
	})

	$("#datepicker").datepicker({
		onSelect: function (dateText) {
			var date = $(this).datepicker('getDate');
			window.location.replace(GetPage() + "?date=" + $.datepicker.formatDate('mm-dd-yy', date));
		}
	});
	var parsedDate = $.datepicker.parseDate('mm-dd-yy', getParameterByName("date"));
	$("#datepicker").datepicker("setDate", parsedDate);

	setTimeout(CheckCheckinTimer, 1000);

	InitMenu();
	var oldSaveCliclFn = $("#Save").click;
	$("#Save").click(function () {
		ShowWaitDlg();
		oldSaveCliclFn();
	})

})