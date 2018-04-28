$(function () {
	$(".machine").click(function (e) {
		window.location.replace(GetPage() + "?machine=" + $(this).text());
		e.stopPropagation();
	});
	$(".back").click(function (e) {
		window.location.replace(GetPage());
		e.stopPropagation();
	});
	$(".find").click(function (e) {
		window.location.replace(GetPage() + "?find=1");
		e.stopPropagation();
	});
	$(".maction").click(function (e) {
		window.location.replace(GetPage() + "?machine=" + getParameterByName("machine") + "&" + $(this).attr('id') + "=1");
		e.stopPropagation();
	});
})