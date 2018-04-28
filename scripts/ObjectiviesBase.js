var severities;
var taskslastupdate;
var objslastupdate;
var objPinGrid = 35;

function ShowDispoDetails(div) {
	var idsever = $(div).attr("idsever");
	var iddisp = $(div).attr("iddisp");
	$("<div id='sevdispdialog'></div>").dialog({
		modal: true,
		title: "Disposition Details",
		width: "auto",
		open: function () {
			getdata("GetFilteredTTShort", "{'idsever':'" + idsever + "','iddisp':'" + iddisp + "'}", function (mess) {
				var table = $("<table></table>");
				$("#sevdispdialog").append(table);
				table.css("margin-left", "auto");
				table.css("margin-right", "auto");
				var users = [];
				for (var i = 0; i < mess.d.length; i++) {
					var bgot = false;
					for (var j = 0; j < users.length; j++) {
						if (users[j].id == mess.d[i].userid) {
							bgot = true;
							break;
						}
					}
					if (bgot == false)
						users.push({ id: mess.d[i].userid, name: mess.d[i].username });
				}
				var row = $("<tr></tr>");
				table.append(row);
				for (var i = 0; i < users.length; i++) {
					var td = $("<td></td>");
					row.append(td);
					var img = $("<img></img>");
					td.append(img);
					img.attr("src", "Images/Personal/" + users[i].id + ".JPG");
					img.attr("title", users[i].name)
					img.addClass("pim");
				}
				var datarow = $("<tr></tr>");
				table.append(datarow);
				for (var i = 0; i < users.length; i++) {
					var tdtt = $("<td></td>");
					datarow.append(tdtt);
					for (var j = 0; j < mess.d.length; j++) {
						if (mess.d[j].userid == users[i].id) {
							var div = $("<div></div>");
							tdtt.append(div);
							var ttidtext = mess.d[j].ttid;
							div.text(ttidtext);
							div.attr("title", mess.d[j].summary);
							div.css("cursor", "pointer");
							div.mousedown(function (e) {
								ShowTask($(this).text());
								e.preventDefault();
								return false;
							});
						}
					}
				}
			})
		},
		close: function (e) {
			$(this).empty();
			$(this).dialog('destroy');
		},
		buttons: {
			Ok: function () {
				$(this).dialog("close");
			}
		},
	});
}
function LoadObjData(id) {
	var selector = ".severitycontainer#" + id;
	var parent = $(selector);
	var wascalculated = $(selector).attr("wascalculated");
	var idsever = parent.attr("idsever");
	var namesever = parent.attr("namesever");
	getdata("LoadSummary2Severity", "{'id':'" + idsever + "'}", function (mess2) {
		var oldinternals = $('<div>').append(parent.clone()).html();
		parent.empty();
		var i, sum = 0;
		for (i = 0; i < mess2.d.length; i++) {
			sum += parseInt(mess2.d[i].est);
		}
		for (i = 0; i < mess2.d.length; i++) {
			var disp = mess2.d[i].disp;
			var iddisp = mess2.d[i].iddisp;
			var effort = mess2.d[i].num + "(" + mess2.d[i].est + ")";
			var summary1 = $("<div></div>");
			var summary2 = $("<div></div>");
			var el = $("<div></div>");
			parent.append(el);
			el.height((100 * mess2.d[i].est / sum) + '%');
			if (disp.toUpperCase().indexOf("REJECT") != -1)
				el.addClass("reject");
			if (disp.toUpperCase().indexOf("PROCESS") != -1)
				el.addClass("process");
			if (disp.toUpperCase().indexOf("FINISHED") != -1)
				el.addClass("finished");
			if (disp.toUpperCase().indexOf("TESTED") != -1)
				el.addClass("tested");
			if (disp.toUpperCase().indexOf("BST") != -1)
				el.addClass("onbst");
			if (disp.toUpperCase().indexOf("SCHEDULE") != -1)
				el.addClass("scheduled");
			el.css('cursor', 'pointer');
			el.attr('disposition', disp);
			el.attr("iddisp", iddisp);
			el.attr("idsever", idsever);
			el.attr('estimated', mess2.d[i].est);
			var table = $("<table class='tabledisp'></table>");
			el.append(table);
			var row = $("<tr class='trdisp'></tr>");
			table.append(row);
			var td = $("<td class='tddisp'></td>");
			row.append(td);
			td.append(summary1);
			td.append(summary2);
			summary1.text(disp);
			summary2.text(effort);
			el.click(function () {
				window.location.href = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "ttrep.aspx?severity=" + namesever + "&disposition=" + $(this).attr('disposition');
			})
			$(el).mousedown(function (e) {
				ShowDispoDetails(this);
				e.preventDefault();
				return false;
			});
		}
		RecalcContainer($(selector));
		var newinternals = $('<div>').append(parent.clone()).html();
		if (newinternals != oldinternals && wascalculated != null) {
			Blink(selector, 10);
			var dir = window.location.href.substring(0, location.href.lastIndexOf("/") + 1);
			//$("body").append("<embed type='audio/wav' src='" + dir + "/Sounds/VOLTAGE.mp3' autostart='true' loop='false' width='2' height='0'></embed>");
			var audioElement = document.createElement('audio');
			audioElement.setAttribute('src', dir + '/Sounds/VOLTAGE.mp3');
			audioElement.setAttribute('autoplay', 'autoplay');
			//audioElement.load()
			$.get();
			audioElement.addEventListener("load", function () {
				audioElement.play();
			}, true);
		}
		$(selector).attr("wascalculated", "1");
	})
}
function AddObjective(mess) {
	$("<div class='severitycontainer' emailed = '" + mess.emailed + "' id='" + mess.id + "' idsever='" + mess.idsever + "' namesever='" + mess.namesever + "'></div>").dialog({
		modal: false,
		autoOpen: true,
		title: mess.namesever,
		width: mess.sizex,
		height: mess.sizey,
		position: [mess.x, mess.y],
		resizable: true,
		closeOnEscape: false,
		open: function () {
			var t = $(this).parent();
			t.offset({ top: mess.y, left: mess.x });
			LoadObjData(mess.id);
		},
		close: function (e) {
			if (mess.id != -1) {
				senddata("ObectiveDelete", "{'id':'" + mess.id + "'}");
			}
			$(this).empty();
			$(this).dialog('destroy');
		},
		dragStop: function (event, ui) {
			if (mess.id != -1) {
				ui.offset.left = Math.round(ui.offset.left / objPinGrid) * objPinGrid;
				ui.offset.top = Math.round(ui.offset.top / objPinGrid) * objPinGrid;
				var t = $(this).parent();
				t.offset({ top: ui.offset.top, left: ui.offset.left });
				senddata("ObectiveOffset", "{'id':'" + mess.id + "', 'x':'" + ui.offset.left + "', 'y':'" + ui.offset.top + "'}");
			}
		},
		resizeStop: function (event, ui) {
			if (mess.id != -1) {
				ui.size.height = Math.round(ui.size.height / objPinGrid) * objPinGrid;
				ui.size.width = Math.round(ui.size.width / objPinGrid) * objPinGrid;
				$(this).dialog("option", "height", ui.size.height);
				$(this).dialog("option", "width", ui.size.width);
				senddata("ObectiveSize", "{'id':'" + mess.id + "', 'x':'" + ui.size.width + "', 'y':'" + ui.size.height + "'}");
			}
		}
	});
}
function LoadSeverities(clickfn) {
	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/LoadSeverities";
	$.ajax({
		type: "POST",
		url: webMethod,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			severities = mess.d;
			var i;
			for (i = 0; i < severities.length; i++) {
				$("#drop").append("<a href='#' id='s" + severities[i].key + "'>" + severities[i].val + "</a><br/>");
				$("#s" + severities[i].key).addClass("trigger");
				$("#s" + severities[i].key).click(function () {
					clickfn($(this).attr("id").substring(1));
				})
			}
			$('#trigger').click(function (event) {
				event.stopPropagation();
				$('#drop').css('z-index', 3000);
				$('#drop').css('top', $(this).offset().top + $(this).height());
				$('#drop a').each(function () {
					if ($('span.ui-dialog-title:contains("' + $(this).text() + '")').length > 0) {
						$(this).css("background-color", "red");
					}
					else {
						$(this).css("background-color", "");
					}
				})
				$('#drop').toggle();
			});
			$(document).click(function () {
				$('#drop').hide();
			});
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function SetIsVisible(disp, val) {
	var a = window.location.href, b = a.lastIndexOf("/"), c = a.lastIndexOf(".aspx");
	disp = a.substring(b + 1, c + 5) + disp;
	$.cookie(disp, val ? "1" : "0");
}
function GetIsVisilble(disp) {
	var a = window.location.href, b = a.lastIndexOf("/"), c = a.lastIndexOf(".aspx");
	disp = a.substring(b + 1, c + 5) + disp;
	var val = $.cookie(disp);
	if (val == null || val == '') {
		$.cookie(disp, 0, { expires: 365 });
		val = "1";
		$.cookie(disp, val);
	}
	return val == "1";
}
function CheckElVisible(el) {
	var vis = GetIsVisilble($(el).attr("class"));
	if (!vis && $(el).is(":visible"))
		$(el).hide();
	else if (vis && $(el).is(":hidden"))
		$(el).show();
}
function CheckFilterLabels() {
	$("#bstlegend td").each(function () {
		if (GetIsVisilble($(this).attr("class"))) {
			if ($(this).text().indexOf("+") == -1)
				$(this).text($(this).text().replace("-", "") + "+");
		}
		else {
			if ($(this).text().indexOf("-") == -1)
				$(this).text($(this).text().replace("+", "") + "-");
		}
	})
}
function LoadFilter() {
	CheckFilterLabels();
	$("#bstlegend tr").css('cursor', 'pointer');
	$("#bstlegend").on("click", "td", function () {
		var disp = $(this).attr("class");
		SetIsVisible(disp, !GetIsVisilble(disp));
		RecalcContainers();
		CheckFilterLabels();
	});
}
function RecalcContainer(container) {
	var sum = 0;
	$(container).children("div").each(function () {
		CheckElVisible($(this));
	});
	$(container).children("div").each(function () {
		if ($(this).is(":visible"))
			sum += parseInt($(this).attr("estimated"));
	});
	$(container).children("div").each(function () {
		if ($(this).is(":visible")) {
			var es = parseInt($(this).attr("estimated"));
			$(this).css("height", (100 * es / sum) + "%");
		}
	});
	$(container).find("img").remove();
	if (sum == 0) {
		if ($(container).attr("emailed") == "1"){
			$(container).append('<img class="mailinformer" src="images/nail.png"/>');
		}
		else {
			$(container).append('<img class="mailinformer" src="images/mail.png"/>');
			var el = $(container).find("img");
			$(el).unbind("click")
			$(el).click(function () {
				var idsev = $(container).attr("idsever");
				if (confirm("Are you sure you want to send email?")) {
					var id = $(container).attr("id");
					$(container).find("img:first").attr("src", "images/process.gif");
					getdata("EmailSeverity", "{'idsever':'" + idsev + "', 'userid':'" + $.cookie("userid") + "'}", function () {
						$(container).find("img:first").attr("src", "images/nail.png");
					});
				}
				else {
					if (confirm("Close severity without sending an email?")) {
						$(container).find("img:first").attr("src", "images/process.gif");
						getdata("CloseSeverity", "{'idsever':'" + idsev + "'}", function () {
							$(container).find("img:first").attr("src", "images/nail.png");
						});
					}
				}
			})
		}
	}
	else {
		if ($(container).attr("emailed") == "1")
			senddata("SkipEmail2Severity", "{'idsever':'" + $(container).attr("idsever") + "'}");
	}
}
function Blink(container, times) {
	if (times == 1){
		$(container).css("background-color", "transparent");
		return;
	}
	if ($(container).css("background-color") == "rgb(0, 0, 255)")
		$(container).css("background-color", "rgb(255, 0, 0)");
	else
		$(container).css("background-color", "rgb(0, 0, 255)");
	var dec = times - 1;
	setTimeout(function () { Blink(container, dec) }, 1000);
}
function RecalcContainers() {
	$(".severitycontainer").each(function (i, elem) {
		RecalcContainer(this);
	})
}
function CheckForTTUpdates() {
	getdata("GetTasksUpdateTime", "", function (mess) {
		if (taskslastupdate == null)
			taskslastupdate = mess.d;
		else {
			if (taskslastupdate != mess.d) {
				taskslastupdate = mess.d;
				$(".severitycontainer").each(function () {
					LoadObjData($(this).attr("id"));
				})
			}
		}
	})
	getdata("GetObjUpdateTime", "", function (mess) {
		if (objslastupdate == null)
			objslastupdate = mess.d;
		else {
			if (objslastupdate != mess.d) {
				objslastupdate = mess.d;
				$(".severitycontainer").each(function () {
					var cont = $(this);
					getdata("GetObective", "{'id':'" + $(this).attr("id") + "'}", function (mess2) {
						cont.attr("emailed", mess2.d.emailed);
						RecalcContainer(cont);
					})
				})
			}
		}
	})
	setTimeout(CheckForTTUpdates, 10000);
}
$(function () {
	LoadFilter();
	setTimeout(CheckForTTUpdates, 10000);
})