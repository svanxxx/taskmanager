var g_filter;
var g_stimelimit;
var g_selector;
var taskslastupdate;

function CheckForTTUpdates() {
	getdata("GetTasksUpdateTime", "", function (mess) {
		if (taskslastupdate == null)
			taskslastupdate = mess.d;
		else {
			if (taskslastupdate != mess.d && $(".changed").length < 1) {
				window.location.reload();
				return;
			}
		}
	})
	setTimeout(CheckForTTUpdates, 10000);
}
function RemovePlan() {
	var user = $.cookie("userid");
	if (user == null || user == '')
		return;

	var skiptasks = "";
	$('#TTasks tr.selected').each(function () {
		skiptasks += (skiptasks == "" ? "" : ",") + $(this).find("td:first").html();
	})

	getdata("UpdatePlan", JSON.stringify({ "user": user, "tasks": null, "skiptasks": skiptasks, "modifiedtasks": skiptasks }), function () {
		window.location.reload();
	});
}
function UpdatePlan() {
	var user = $.cookie("userid");
	if (user == null || user == '')
		return;

	var plantasks = "";
	var skiptasks = "";
	var modifiedtasks = "";
	var go = false;
	$($('#TTasks > tbody > tr').get().reverse()).each(function () {
		var td = $(this).find("td:first");
		if (!go && td.parent().children()[1].innerHTML == "*")
			go = true;
		if ($(this).hasClass("changed"))
			modifiedtasks = modifiedtasks + "," + td.html();
		if (go)
			plantasks = td.html() + "," + plantasks;
		else
			skiptasks = td.html() + "," + skiptasks;
	})

	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/UpdatePlan";
	var parameters = "{'user':'" + user + "','tasks':'" + plantasks + "','skiptasks':'" + skiptasks + "','modifiedtasks':'" + modifiedtasks + "'}";

	$.ajax({
		type: "POST",
		url: webMethod,
		data: parameters,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			location.reload();
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function CalcSummary() {
	var hrs = 0;
	$($('#TTasks > tbody > tr').get()).each(function () {
		if ($(this).children()[1].innerHTML == "*")
			hrs += parseInt($(this).children()[2].innerHTML);
	});
	$('#statusbar').text("Total: " + hrs + " (hrs), " + hrs / 8 + " (days), " + hrs / 8 / 5 + " (weeks), " + hrs / 8 / 5 / 4 + " (months)");
}
function DragHelper() {
	var selected = $('#TTasks tr.selected');
	if (selected.length === 0) {
		selected = $(this).addClass('selected');
	}
	var container = $('<div/>').attr('id', 'draggingContainer');
	container.append(selected.clone().removeClass("selected"));
	return container;
}
function RowClickHelper() {
	if ($(this).hasClass("selected"))
		$(this).toggleClass("selected");
	else
		$(this).addClass("selected");
}
function DropHelper(event, ui) {
	var rows = ui.helper.children();
	for (var i = 0; i < rows.length; i++) {
		var row = rows[i];

		$(row).click(RowClickHelper)
		$(row).draggable({ helper: DragHelper });
		SetDroppable(row);

		$(this).before(row);
		$(row).switchClass("ini", "changed");
		$(row).children()[1].innerHTML = "*";
		if ($(row).hasClass("selected"))
			$(row).toggleClass("selected");
		$(this).switchClass("over", "ini");
	}
	$(".selected").remove();
}
function SetDroppableBin() {
	$("#droptdbin").droppable({
		activeClass: "ui-state-default",
		hoverClass: "ui-state-hover",
		accept: ":not(.ui-sortable-helper)",
		drop: function (event, ui) {
			var rows = ui.helper.children();
			for (var i = 0; i < rows.length; i++) {
				var row = rows[i];
				var selector = "tr td:contains('" + $(row).children()[0].innerHTML + "')";
				$("tr td:contains('" + $(row).children()[0].innerHTML + "')").each(function () {
					$(this).parent().children()[1].innerHTML = "";
					$($(this).parent().children()[1]).css("background-color", "");
					$(this).parent().toggleClass("selected");
				});
			}
			$("#droptdbin").css("background-color", "");
		},
		over: function () {
			$(this).css("background-color", "red");
		},
	});
}
function SetDroppableTT() {
	$("#droptdtt").droppable({
		activeClass: "ui-state-default",
		hoverClass: "ui-state-hover",
		accept: ":not(.ui-sortable-helper)",
		drop: function (event, ui) {
			var rows = ui.helper.children();
			for (var i = 0; i < rows.length; i++) {
				var row = rows[i];
				var tt = $(row).children()[0].innerHTML;
				LaunchTT(tt);
			}
			$("#droptdtt").css("background-color", "");
		},
		over: function () {
			$(this).css("background-color", "red");
		},
	});
}
function SetDroppable(selector) {
	$(selector).droppable({
		activeClass: "ui-state-default",
		hoverClass: "ui-state-hover",
		accept: ":not(.ui-sortable-helper)",
		drop: DropHelper,
		over: function () {
			if ($(this).hasClass("ini"))
				$(this).switchClass("ini", "over");
			else
				$(this).switchClass("changed", "changedover");
		},
		out: function () {
			if ($(this).hasClass("over"))
				$(this).switchClass("over", "ini");
			else
				$(this).switchClass("changedover", "changed");
		},
	});
}
function Load2Do() {
	var webMethod = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "TRService.asmx/Get2Do";
	$.ajax({
		type: "POST",
		url: webMethod,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (msg) {
			var pers = msg.d;
			$("div[id='redhint']").remove();
			$.each(pers, function (index, per) {
				$('#UsersTbl tr td[title="' + per.name + '"]').append("<div id='redhint' class='undone'>" + per.count + "</div>");
			});
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
	setTimeout(Load2Do, 10000);
}
function CheckTasks() {
	if ($("div[id='redhint']").length > 0) {
		if ($(document).prop('title') != "Edit Plan !!!New tasks!!!)")
			$(document).prop('title', "Edit Plan !!!New tasks!!!)");
		else
			$(document).prop('title', "Edit Plan");

		$($("div[id='redhint']").get()).each(function () {
			if ($(this).hasClass("undone"))
				$(this).switchClass("undone", "undonerev");
			else
				$(this).switchClass("undonerev", "undone");
		})
		$("#TTasks tr td:nth-child(5):contains('a.')").each(function () {
			var r = $($(this).parent()[0]);
			if ($(r).children()[1].innerHTML != "*") {
				if ($(r).hasClass("newtask1"))
					$(r).switchClass("newtask1", "newtask2");
				else {
					if ($(r).hasClass("newtask2"))
						$(r).switchClass("newtask2", "newtask1");
					else
						$(r).addClass("newtask2");
				}
			}
		})
	}
	else
		$(document).prop('title', "Edit Plan");

	setTimeout(CheckTasks, 1000);
}
function CheckSelector() {
	var selector = $.cookie("selector");
	if (!selector) //check for null - opera does not work
		selector = "";

	if (selector == g_selector)
		return;

	g_selector = selector;

	$("#TTasks > tbody > tr > td").css('background-color', '');
	var sum = 0
	var checkedrows = [];
	if (selector != "") {
		$("#TTasks > tbody > tr:visible > td").each(function () {
			if ($(this).text().toUpperCase().indexOf(selector.toUpperCase()) > -1) {
				$($(this).parents()[0]).children(":gt(1)").css('background-color', '#66FF66');
				var i = $($(this).parents()[0]).index();
				if (checkedrows.indexOf(i) == -1) {
					checkedrows.push(i);
					sum += parseInt($($(this).parents()[0]).children()[2].innerHTML);
				}
			}
		});
	}
	$("#selectorsum").text("= " + sum + "(hrs)");
}
function CheckFilter() {
	var filter = $.cookie("filter");
	if (!filter)
		filter = "";

	var stimelimit = $.cookie("timelimit");
	if (!stimelimit)
		stimelimit = "";

	if (stimelimit == g_stimelimit && filter == g_filter)
		return;

	g_stimelimit = stimelimit;
	g_filter = filter;

	if (filter == "" && timelimit == "") {
		$("#TTasks tr").show();
		$("#filtersum").text("");
		return;
	}

	var timelimit = parseInt(stimelimit);
	var hrs = 0;
	var i = 0;
	var arr2show = [];

	$("#TTasks tr td:contains('" + filter + "')").each(function () {
		i = $($(this).parent()[0]).index();
		if (arr2show.indexOf(i) == -1)
			arr2show.push(i);
	});

	$("#TTasks tr").each(function () {
		if (arr2show.indexOf(($(this).index())) == -1)
			$(this).hide();
		else {
			var cur = parseInt($(this).children()[2].innerHTML);
			if (hrs >= timelimit && timelimit != "")
				$(this).hide();
			else {
				hrs += cur;
				$(this).show();
			}
		}
	});

	$("#filtersum").text("= " + hrs + "(hrs)");
}
function FilterData() {
	$.cookie("filter", $('#filter').val(), { expires: 365 });
	$.cookie("timelimit", $('#timelimit').val().replace(/[^0-9\.]/g, ''), { expires: 365 });
	$.cookie("selector", $('#selector').val(), { expires: 365 });
	CheckFilter();
	CheckSelector();
	setTimeout(FilterData, 1000);
}
function LoadFilters() {
	$('#filter').val($.cookie("filter"));
	$('#timelimit').val($.cookie("timelimit"));
	$('#selector').val($.cookie("selector"));
	FilterData()
	setTimeout(FilterData, 1000);
}
$(function () {
	jQuery.expr[':'].contains = function (a, i, m) {
		return jQuery(a).text().toUpperCase()
			 .indexOf(m[3].toUpperCase()) >= 0;
	};
	if (getParameterByName("showby") != "")
		$('#showbyCombo').val(getParameterByName("showby"));

	$('#showbyCombo').on('change', function () {
		var uname = getParameterByName("user");
		var uparam = "";
		if (uname != "")
			uparam = "user=" + uname + "&";
		location.replace(GetPage() + "?" + uparam + "showby=" + (this.value));
	});
	$("#TTasks tr").click(RowClickHelper)
	$("#TTasks tr").addClass("ini");
	$("#TTasks tbody tr").draggable({ helper: DragHelper });
	$("#TTasks tbody tr td").mousedown(function (e) {
		if (e.which == 2) {
			ShowTask($(this).parent().eq(0).children().eq(0).text());
			e.preventDefault();
			return false;
		}
	});
	$("#updateplan").click(UpdatePlan);
	$("#removeplan").click(RemovePlan);

	SetDroppable("#TTasks tbody tr");
	SetDroppableBin();
	SetDroppableTT();
	Load2Do();
	CheckTasks();
	$("#UsersTbl img").click(function () {
		if ($(this).parent().hasClass("seluser")){
			location.reload();
			return;
		}
		$("#UsersList").val($(this).parent().attr('title')).change();
	})
	$('#UsersTbl > tbody > tr > td').hover(
		function () {
			$(this).css('background-color', 'blue');
		},
		function () {
			$(this).css('background-color', '');
		}
	);
	$('#selector').focus();
	$("#export").on('click', function (event) {
		// CSV
		exportTableToCSV.apply(this, [$('#TTasks'), 'tasks.csv']);
		// IF CSV, don't do event.preventDefault() or return false
		// We actually need this to be a typical hyperlink
	});
	$("#pagelink").click(function () {
		window.prompt("Copy to clipboard: Ctrl+C, Enter", window.location.href);
	});
	$("#toolsbar").hide();
	$("#tools").click(function () {
		if ($("#toolsbar").is(":visible"))
			$("#toolsbar").hide();
		else
			$("#toolsbar").show();
	});
	LoadFilters();
	CheckSelector();
	CalcSummary();
	setTimeout(CheckForTTUpdates, 10000);
});