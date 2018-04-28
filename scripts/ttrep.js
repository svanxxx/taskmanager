function ApplyFilter() {
	var severity = "severity=" + $("#TTSeverity").val().replace("&", "%26");
	var user = "&repusr=" + $("#TTUser").val().replace("&", "%26");
	var disposition = "&disposition=" + $("#TTDisposition").val().replace("&", "%26");
	var stext = "&stext=" + encodeURIComponent($("#stext").val());
	var datefilter = "";
	if ($("#startdatecheck").is(":checked")) {
		var d = new Date($("#startdate").val());
		datefilter = "&date=" + convertDateShort(d);
	}
	window.location.replace(window.location.href.substring(0, location.href.lastIndexOf(".aspx") + 5) + "?" + severity + datefilter + disposition + user + stext);
}
function CalcSummary() {
	var sum = 0;
	var arr2disp = [];
	var arr2dispsum = [];
	var disp = getParameterByName("disposition");
	if (disp != "") {
		arr2dispsum.push(0);
		arr2disp.push(disp);
	}

	$("#TTTable tbody tr").each(function () {
		var tdsev = $(this).children().eq(5);
		var tdhrs = $(this).children().eq(3);
		sum += Number(tdhrs.html());
		if (disp != "")
			arr2dispsum[0] += Number(tdhrs.html());
		else {
			if (arr2disp.indexOf(tdsev.html()) == -1) {
				arr2disp.push(tdsev.html());
				arr2dispsum.push(0);
			}
			arr2dispsum[arr2disp.indexOf(tdsev.html())] += Number(tdhrs.html());
		}
	})

	$("#totalhours").append("<table id='sumtable'></table>");
	var table = $("#totalhours").children();
	table.append("<tr><td></td><td>hrs</td><td>days</td><td>months</td></tr>");
	table.append("<tr><td>Total:</td><td>" + sum + "</td><td>" + sum / 8 + "</td><td>" + sum / 8 / 20 + "</td></tr>");

	var i;
	for (i = 0; i < arr2disp.length; ++i) {
		table.append("<tr><td>" + arr2disp[i].substring(0, arr2disp[i].indexOf(" ")) +
			":</td><td>" + arr2dispsum[i] + "</td><td>" + arr2dispsum[i] / 8 + "</td><td>" + arr2dispsum[i] / 8 / 20 + "</td></tr>");
	}
	$("#sumtable tr td").css("border", "1px solid black");
	$("#sumtable tr td").attr("align", "right");

}
$(function () {
	$("#TTSeverity").change(function () {
		ApplyFilter();
	});
	$("#TTUser").change(function () {
		ApplyFilter();
	});
	$("#TTDisposition").change(function () {
		ApplyFilter();
	});
	$("#startdatecheck").change(function () {
		ApplyFilter();
	});
	$("#export").on('click', function (event) {
		// CSV
		exportTableToCSV.apply(this, [$('#TTTable'), 'tasks.csv']);
		// IF CSV, don't do event.preventDefault() or return false
		// We actually need this to be a typical hyperlink
	});
	$("#startdate").datepicker({
		changeMonth: true,
		changeYear: true,
		showButtonPanel: true,
		dateFormat: "MM yy",
		onClose: function (dateText, inst) {
			var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
			var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
			var d = new Date(year, month, 1);
			var sd = $.datepicker.formatDate("MM yy", d);
			if ($(this).val() != sd)
				$(this).val(sd);
			$("#startdatecheck").prop("checked", true);
			ApplyFilter();
		}
	});
	$("#stext").val(getParameterByName("stext"));
	$("#stext").keyup(function (e) {
		var code = e.which; // recommended to use e.which, it's normalized across browsers
		if (code == 13) {
			e.preventDefault();
			ApplyFilter();
		}
	});
	$("#startdate").prop("readonly", true);
	var date = getParameterByName("date");
	$("#startdatecheck").prop("checked", date);
	var d = new Date();
	if (date)
		var d = new Date(date.substring(4, 8), parseInt(date.substring(2, 4)) - 1);
	$("#startdate").val($.datepicker.formatDate("MM yy", d));

	var summary = $.cookie("summary");
	if (!summary) //check for null - opera does not work
		summary = "none";
	$("#summary").css("display", summary);
	$("#more").click(function () {
		if ($("#summary").css("display") == "block")
			$("#summary").css("display", "none");
		else
			$("#summary").css("display", "block");
		$.cookie("summary", $("#summary").css("display"), { expires: 365 });
	})
	CalcSummary();
	$("#TTTable tbody tr td:nth-child(2)").each(function () {
		$(this).css('cursor', 'pointer');
		$(this).click(function () {
			LaunchTT($(this).text());
		})
	});
	var emailindex = $("td:contains('@resnet.com')").eq(0).index();
	emailindex++;
	$("#TTTable tbody tr td:nth-child(" + emailindex + ")").each(function () {
		$(this).css('cursor', 'pointer');
		$(this).click(function () {
			LaunchPlan($(this).text());
		})
	});
	$("#TTTable tbody tr td").mousedown(function (e) {
		if (e.which == 2) {
			ShowTask($(this).parent().eq(0).children().eq(1).text());
			return false;
		}
	});
})