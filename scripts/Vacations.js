var LastUpdatedTT;
var LastUpdatedTR;
function CalcSummary() {
	var hrs = 0;
	$($('#STable > tbody > tr').get()).each(function () {
		var isick = 0;
		var ivac = 0;
		var icells = $(this).children().get().length;
		$($(this).children().get()).each(function () {
			if ($(this).index() != icells - 1) {
				if ($(this).hasClass("vacation"))
					ivac++;
			}
			if ($(this).hasClass("sick"))
				isick++;
		});
		var row = $('#SNames > tbody').children()[$(this).index()];
		if (row != null) {
			$('#SNames tr').eq($(this).index()).find('td').eq(1).text(isick.toString());
			$('#SNames tr').eq($(this).index()).find('td').eq(2).text(ivac.toString());
		}
	});
}
function Check4UpdatesTR() {
	getdata2("GetTRUpdateTime", "", function (mess) {
		if (LastUpdatedTR == null)
			LastUpdatedTR = mess.d;
		else {
			if (LastUpdatedTR != mess.d) {
				LastUpdatedTR = mess.d;
				getdata('GetWorkDays', JSON.stringify({ "year": $('#syear').val() }), function (mess) {
					if (mess.d.length < 1) {
						setTimeout(Check4UpdatesTR, 10000);
						return;
					}
					var tab = document.getElementById("STable");
					for (var i = 2; i < tab.rows.length; i++) {
						var email = document.getElementById("SNames").rows[i].cells[0].getAttribute("title");
						for (var j = 0; j < tab.rows[i].cells.length; j++) {
							
							var ind = -1;
							for (var k = 0; k < mess.d.length; k++){
								if (mess.d[k].email == email && mess.d[k].day == j) {
									ind = k;
									break;
								}
							}

							var cell = tab.rows[i].cells[j];
							if (ind == -1 && cell.classList.contains('work')) {
								cell.classList.remove('work');
								cell.classList.add('empty');
							}
							else if (ind != -1 && !cell.classList.contains('work')) {
								cell.classList.remove('empty');
								cell.classList.add('work');
							}
						}
					}
				})
			}
		}
		setTimeout(Check4UpdatesTR, 10000);
	},
	function () {
		setTimeout(Check4UpdatesTR, 10000);
	})
}
function Check4Updates() {
	getdata2("GetTasksUpdateTime", "", function (mess) {
		if (LastUpdatedTT == null)
			LastUpdatedTT = mess.d;
		else {
			if (LastUpdatedTT != mess.d) {
				window.location.reload();
				return;
			}
		}
		setTimeout(Check4Updates, 10000);
	},
	function () {
		setTimeout(Check4Updates, 10000);
	})
}
$(document).ready(function () {
	$("#SNames td").click(function () {
		var position = $(this).offset();
		$("#rowselector").css(position);
		$("#rowselector").css("visibility", "visible");
		$("#rowselector").height($(this).outerHeight(true) + 1);
	});
	CalcSummary();
	var selector = "#STable tbody tr td.vacation div, #STable tbody tr td.sick div, #STable tbody tr td.unpaid div";
	$(selector).click(function () {
		ShowTask($(this).parent()[0].title);
	})

	var selector = "#STable tbody tr td.empty";
	$(selector).click(function () {
		if (!confirm('Are you sure you want to schedule vacation for this day?')) {
			return;
		}
		var col = $(this).parent().children().index($(this));
		var date = new Date($('#syear').val(), 0, 0);
		date.setDate(date.getDate() + col + 1);
		var email = $("#SNames tbody tr:eq(" + $(this).parent().index() + ") td:eq(0)").attr('title');
		getdata("ScheduleVacation", JSON.stringify({ "useremail": email, "date": date }), function (mess) {
			if (!mess.d)
				alert("No vacations available");
			else {
				ShowTask(mess.d);
			}
		})
	})

	var now = new Date();
	var start = new Date(now.getFullYear(), 0, 0);
	var diff = now - start;
	var oneDay = 365 * 1000 * 60 * 60 * 24;
	var iPercent = Math.floor(100 * diff / oneDay) - 5;
	$('#centered').animate({ scrollLeft: iPercent / 100 * document.getElementById('centered').scrollWidth });

	var year = getParameterByName('year');
	if (year == '')
		year = new Date().getFullYear();

	$('#syear option:contains("' + year + '")').prop('selected', true)
	$("#syear").change(function () {
		var year = "?year=" + $("#syear").val();
		window.location.replace(window.location.href.substring(0, location.href.lastIndexOf(".aspx") + 5) + year);
	});
	LastUpdatedTR = $('#LastUpdatedTR').val();
	setTimeout(Check4Updates, 10000);
	setTimeout(Check4UpdatesTR, 10000);
});