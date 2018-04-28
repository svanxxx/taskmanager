$(function () {
	$('#startdate').datepicker({
		changeMonth: true,
		changeYear: true,
		showButtonPanel: true,
		dateFormat: 'MM yy',
		onClose: function (dateText, inst) {
			var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
			var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
			var d = new Date(year, month, 1);
			var sd = $.datepicker.formatDate("MM yy", d);
			if ($(this).val() != sd)
				window.location.href = window.location.href.substring(0, location.href.lastIndexOf(".aspx") + 5) + "?date=" + convertDateShort(d);
		}
	});
	$('#startdate').prop('readonly', true);
	var date = getParameterByName("date");
	var d = new Date();
	if (date)
		var d = new Date(date.substring(4, 8), parseInt(date.substring(2, 4)) - 1);
	$('#startdate').val($.datepicker.formatDate("MM yy", d));
});