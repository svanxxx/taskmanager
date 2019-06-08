function upadteBuildProgress(builds, buildtime) {
	for (var i = 0; i < builds.length; i++) {
		builds[i].PERCENT = 100;
		if (builds[i].DATEBUILD !== "") {
			var time = builds[i].DATEBUILD.split(" ")[1];
			var d = StringToTime(time);
			var now = new Date();
			d.setFullYear(now.getFullYear());
			d.setDate(1);//not to reset last day of month.
			d.setMonth(now.getMonth());
			d.setDate(now.getDate());
			var timeDiff = Math.abs(now.getTime() - d.getTime());
			var diffMins = Math.ceil(timeDiff / (1000 * 60));
			builds[i].PERCENT = Math.round(diffMins / buildtime * 100.0);
			if (builds[i].STARTED) {
				builds[i].DURATION = diffMins;
			}
		}
	}
}
