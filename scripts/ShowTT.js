$(function () {
	var ttid = getParameterByName("ttid");
	if (!ttid) {
		ShowEnterTTDialog();
	}
	else
		ShowTask(ttid);
})