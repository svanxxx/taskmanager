var monthsNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var dispositions;
var priorities;
var ispagevisible = true;

function guid() {
	function s4() {
		return Math.floor((1 + Math.random()) * 0x10000)
		  .toString(16)
		  .substring(1);
	}
	return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
	  s4() + '-' + s4() + s4() + s4();
}
function ShowWaitDlg(bShow) {
	if (bShow !== void 0 && !bShow) {
		$("#waitDlg").empty();
		$("#waitDlg").dialog('destroy');
	}
	else {
		$("<div id='waitDlg'><center><img src='images/process.gif' width='24' style='height: 24px' /><p><h3>Please wait...</h3></p></center></div>").dialog({
			resizable: false,
			closeOnEscape: false,
			draggable: false,
			minHeight: 50,
			minWidth: 50,
			modal: true
		}).siblings('.ui-dialog-titlebar').hide();
	}
}
function pad(num, size) {
	var s = num + "";
	while (s.length < size) s = "0" + s;
	return s;
}
function DateToString(dt) {
	return pad((dt.getMonth() + 1), 2) + "-" + pad(dt.getDate(), 2) + "-" + dt.getFullYear();
}
function StringToDate(st) {
	var vals = st.split('-');
	if (vals.length != 3)
		return new Date();
	return new Date(vals[2], parseInt(vals[0]) - 1, vals[1]);
}
function StartProgress(txt) {
	//doing some cleanup of old progress - some functions may fail leaving progress messages
	var now = new Date();
	$(".loadingprogress").each(function () {
		var createddt = new Date($(this).attr("timestart"));
		if ((now - createddt) > 60000)
			$(this).remove();
	})
	var uuid = guid();
	var messagetext = txt == undefined ? "Loading..." : txt;
	$(document.body).append("<div id='" + uuid + "' class='loadingprogress' timestart='" + now + "'>" + messagetext + "</div>");
	return uuid;
}
function EndProgress(id) {
	$("#" + id).remove();
}
function getParameterByName(name) {
	name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
	var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
		 results = regex.exec(location.search);
	return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
function GetSitePath() {
	return window.location.href.substring(0, location.href.lastIndexOf("/") + 1);
}
function GetPage() {
	return window.location.href.substring(0, location.href.lastIndexOf(".aspx") + 5);
}
function GetPageName() {
	return window.location.href.substring(location.href.lastIndexOf("/") + 1, location.href.lastIndexOf(".aspx") + 5);
}
function senddata(func, params) {
	var webMethod = GetSitePath() + "TRService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function getdata(func, params, proc) {
	var webMethod = GetSitePath() + "TRService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			proc(mess);
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function getdata2(func, params, proc, fproc) {
	var webMethod = GetSitePath() + "TRService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			proc(mess);
		},
		error: function (e) {
			fproc();
		}
	});
}
function convertDateShort(d) {
	function pad(s) { return (s < 10) ? '0' + s : s; }
	return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('');
}
function LaunchTT(tt) {
	var webMethod = GetSitePath() + "TRService.asmx/GetTTURL";
	$.ajax({
		type: "POST",
		url: webMethod,
		data: "{'TTID':'" + tt + "'}",
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			window.open(msg.d);
		},
		error: function (e) {
			if ($("#systemmessages").length) {
				$("#systemmessages").text(e.responseText);
			}
		}
	});
}
function LaunchPlan(user) {
	var webMethod = GetSitePath() + "TRService.asmx/GetUserName";
	$.ajax({
		type: "POST",
		url: webMethod,
		data: "{'email':'" + user + "'}",
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			var url = window.location.href.substring(0, window.location.href.lastIndexOf("/")) + "/EditPlan.aspx?user=" + encodeURI(msg.d);
			window.location.assign(url);
		},
		error: function (e) {
			if ($("#systemmessages").length) {
				$("#systemmessages").text(e.responseText);
			}
		}
	});
}
function ShowTask(tt) {
	var pr = StartProgress();
	getdata("GetTTText", "{'TTID':'" + tt + "'}", function (msg) {
		EndProgress(pr);
		var ttdate = new Date(parseInt(msg.d.date))
		var tttext = msg.d.text;//.replace(/</g, "&lt").replace(/>/g, "&gt").replace(/\n/g, "<br />");
		var stitle = "TT" + tt + " " + msg.d.title;
		$("<div id='ttdlg'><textarea style='resize:none;width:100%;height:100%'>" + tttext + "</textarea></div>").dialog({
			modal: true,
			title: stitle,
			close: function (e) {
				$(this).empty();
				$(this).dialog('destroy');
			},
			buttons: {
				"Show In New Window": function () {
					var win = window.open(GetSitePath() + "ShowTT.aspx?ttid=" + tt, "_blank");
					if (win) {
						//Browser has allowed it to be opened
						win.focus();
						$(this).dialog("close");
					} else {
						//Broswer has blocked it
						alert('Please allow popups for this site');
					}
				},
				Ok: function () {
					var newdate = new Date($("#ttdate").datepicker("getDate"));
					var newdispo = $("#dispselect option:selected").text();
					var newprio = $("#prioselect option:selected").text();
					var newest = $("#ttest").val();
					if (newdate.toDateString() != ttdate.toDateString()
						 || $(this).children("textarea").val() != msg.d.text
						 || newdispo != msg.d.dispo
						 || newprio != msg.d.prio
						 || msg.d.est != newest) {
						senddata("UpdateTask", JSON.stringify({ "TTID": tt, "date": newdate, "est": newest, "dispo": newdispo, "text": $(this).children("textarea").val(), "userid": $.cookie("userid"), "prio": newprio }));
					}
					$(this).dialog("close");
				},
			},
			open: function () {
				$(this).css("background-color", "white");
				var dispotext = "<td id='disptxt'><strong>Disposition:</strong> " + msg.d.dispo + "</td>";
				if (dispositions != null && dispositions.length > 0) {
					dispotext = "<td id='dispselect'><strong>Disposition:</strong>";
					dispotext += "<select id='disposelect'>";
					for (var i = 0; i < dispositions.length; i++) {
						var sel = (msg.d.dispo == dispositions[i].val) ? "selected" : "";
						dispotext += "<option " + sel + " value='" + dispositions[i].key + "'>" + dispositions[i].val + "</option>";
					}
					dispotext += "</select></td>";
				}
				var prioptext = "<td id='pritxt'><strong>Priority:</strong> " + msg.d.prio + "</td>";
				if (priorities != null && priorities.length > 0) {
					prioptext = "<td id='prioselect'><strong>Priority:</strong>";
					prioptext += "<select id='prioselect'>";
					for (var i = 0; i < priorities.length; i++) {
						var sel = (msg.d.prio == priorities[i].val) ? "selected" : "";
						prioptext += "<option " + sel + " value='" + priorities[i].key + "'>" + priorities[i].val + "</option>";
					}
					prioptext += "</select></td>";
				}

				$(this).before("<div align='center'><strong>" + stitle + "</strong></div>" +
									"<table class='taskheader' width='100%'>" +
									"<tr>" +
									"<td><input id='ttdate'</input></td>" +
									"<td><strong>Created by:</strong> " + msg.d.creat_user + "</td>" +
									"<td><strong>Estimated:</strong><input id='ttest' type='number' min='0' max='1000' value='"+msg.d.est+"'></input></td>" +
									"<td><strong>User:</strong> " + msg.d.user + "</td>" +
									dispotext +
									prioptext +
									"</tr>" +
									"</table>");
				$(this).after('<div id="ttctrls" style="background-color:white;width:100%;overflow:auto;"></div>')
				$("#ttdate").datepicker({
					showOn: "button",
					buttonImageOnly: true,
					buttonImage: "http://jqueryui.com/resources/demos/datepicker/images/calendar.gif",
					buttonText: "Select date"
				});
				$("#ttdate").datepicker("setDate", ttdate);
			},
			minHeight: 0,
			width: Math.round($(window).width() * 0.8),
			height: Math.round($(window).height() * 0.8),
		});
	});
}
function ClearTextSelection() {
	if (window.getSelection) {
		if (window.getSelection().empty) {  // Chrome
			window.getSelection().empty();
		} else if (window.getSelection().removeAllRanges) {  // Firefox
			window.getSelection().removeAllRanges();
		}
	} else if (document.selection) {  // IE?
		document.selection.empty();
	}
}
function exportTableToCSV($table, filename) {

	var $rows = $table.find('tr:has(td):visible'),
	// Temporary delimiter characters unlikely to be typed by keyboard
	// This is to avoid accidentally splitting the actual contents
	tmpColDelim = String.fromCharCode(11), // vertical tab character
	tmpRowDelim = String.fromCharCode(0), // null character

	// actual delimiter characters for CSV format
	colDelim = '","',
	rowDelim = '"\r\n"',

	// Grab text from table into CSV formatted string
	csv = '"' + $rows.map(function (i, row) {
		var $row = $(row),
				$cols = $row.find('td');

		return $cols.map(function (j, col) {
			var $col = $(col),
					text = $col.text();

			return text.replace('"', '""'); // escape double quotes

		}).get().join(tmpColDelim);

	}).get().join(tmpRowDelim)
		.split(tmpRowDelim).join(rowDelim)
		.split(tmpColDelim).join(colDelim) + '"',

	// Data URI
	csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

	$(this).attr({
		'download': filename,
		'href': csvData,
		'target': '_blank'
	});
}
function ShowEnterTTDialog() {
	var dlg = $("<div id='ttdlgask'</div>").dialog({
		modal: true,
		title: 'Enter TT ID',
		close: function (e) {
			$(this).empty();
			$(this).dialog('destroy');
		},
		buttons: {
			"OK": function () {
				$.cookie("lastenteredttid", $("#ttidnum").val(), { expires: 365 });
				ShowTask($("#ttidnum").val());
				$(this).dialog("close");
			}
		},
		open: function () {
			if ($("#ttidnum").size() < 1) {
				$(this).html("<input id='ttidnum' type='text'>");
			}
			$("#ttidnum").val($.cookie("lastenteredttid"));
			$("#ttidnum").focus();
			$("#ttidnum").select();
			$('#ttidnum').keyup(function (e) {
				if (e.keyCode == 13) {
					dlg.dialog("option", "buttons")['OK'].apply(dlg);
				}
			});
		},
		minHeight: 0,
		width: Math.round($(window).width() * 0.2),
		height: Math.round($(window).height() * 0.2),
	});
}
var logupdatetime;
function GetEventsLog() {
	if ($("#dialogLOG").dialog("isOpen") == false)
		return;

	getdata("GetTasksUpdateTime", "", function (mess) {
		if (logupdatetime == null || logupdatetime != mess.d) {
			logupdatetime = mess.d;
			getdata("GetEventsLog", "", function (log) {
				$("#logdata").empty();
				var tbl = $("<table id='logtable'></table>");
				$("#logdata").append(tbl);
				for (var i = 0; i < log.d.length; i++) {
					var row = $("<tr></tr>");
					$(tbl).append(row);
					$(row).click(function () {
						ShowTask($($(this).children()[1]).text());
					})
					row.append("<td><img class='pimsmall' src='images/personal/" + log.d[i].person_id + ".jpg''></td>");
					row.append("<td>" + log.d[i].defectnum + "</td>");
					row.append("<td>" + log.d[i].notes + "</td>");
				}
			})
		}
	})
	setTimeout(GetEventsLog, 10000);
}
function ShowLogDialog() {
	$("<div id='dialogLOG'><div id='logdata'>please wait...</div></div>").dialog({
		autoOpen: true,
		title: "Activity Log",
		draggable: true,
		resizable: true,
		height: 500,
		width: 500,
		modal: false,
		open: function (event, ui) {
			var logtop = $.cookie(GetPageName() + "logtop");
			var logleft = $.cookie(GetPageName() + "logleft");

			$(event.target).parent().css('position', 'fixed');
			$(event.target).parent().css('top', logtop == "" ? "500px" : logtop + "px");
			$(event.target).parent().css('left', logleft == "" ? "500px" : logleft + "px");
		},
		dragStop: function (event, ui) {
			$.cookie(GetPageName() + "logtop", ui.offset.top);
			$.cookie(GetPageName() + "logleft", ui.offset.left);
		}
	})
	setTimeout(GetEventsLog, 10000);
}
function ProcessLogOptions() {
	var klogname = GetPageName() + "-showlog";
	if ($.cookie(klogname) == "true")
		ShowLogDialog();

	$(document).bind('keydown', function (e) {
		if (e.keyCode == 188 && event.ctrlKey) {
			ShowEnterTTDialog();
		}
		if (e.keyCode == 76 && event.ctrlKey && event.shiftKey) {
			if ($("#dialogLOG").length == 0) {
				ShowLogDialog();
				$.cookie(klogname, true, { expires: 365 });
				return;
			}
			if ($("#dialogLOG").dialog("isOpen")) {
				$.cookie(klogname, false, { expires: 365 });
				$("#dialogLOG").dialog("close");
			}
			else {
				$.cookie(klogname, true, { expires: 365 });
				$("#dialogLOG").dialog("open");
				setTimeout(GetEventsLog, 10000);
			}
		}
	});
}
$(function () {
	$('head').prepend('<link rel="stylesheet" href="Scripts/bootstrap/css/bootstrap.min.css" type="text/css" />');
	$('head').append('<script type="text/javascript" src="Scripts/bootstrap/js/bootstrap.js"></script>');

	ProcessLogOptions();

	var bootstrapButton = $.fn.button.noConflict(); // return $.fn.button to previously assigned value
	$.fn.bootstrapBtn = bootstrapButton;            // give $().bootstrapBtn the Bootstrap functionality

	$.datepicker.setDefaults({
		firstDay: 1,
		buttonImage: 'http://jqueryui.com/resources/demos/datepicker/images/calendar.gif'
	});

	getdata("LoadDispositions", "", function (msg) {
		dispositions = msg.d;
	})
	getdata("LoadPriorities", "", function (msg) {
		priorities = msg.d;
	})

	$(window).blur(function () {
		ispagevisible = false;
	});
	$(window).focus(function () {
		ispagevisible = true;
	});
})