function upadteBuildProgress(builds, buildtime, progress) {
	for (var i = 0; i < builds.length; i++) {
		builds[i].PERCENT = 100;
		if (builds[i].STATUS == progress) {
			let start = new Date(builds[i].DATEBUILD);
			let now = new Date();
			var diffMins = Math.ceil((now - start) / (1000 * 60));
			builds[i].PERCENT = Math.round(diffMins / buildtime * 100.0);
			builds[i].PERCENT = Math.min(99.9, builds[i].PERCENT);
			builds[i].DURATION = diffMins;
		} else {
			let start = new Date(builds[i].DATEBUILD);
			let now = new Date(builds[i].DATEUP);
			var diffMins = Math.ceil((now - start) / (1000 * 60));
			builds[i].DURATION = diffMins;
		}
	}
}
const BuildStatus = {
	undefined: 0,
	progress: 1,
	finishedok: 2,
	cancelled: 3,
	failed: 4,
	notstarted: 5,
}

function InitBuildHelpers($scope, $interval, $http, ttid) {
	$scope.BuildStatus = BuildStatus;
	$scope.fromUTC = function (str) {
		let d = new Date(str);
		return new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()));
	};
	$scope.buildBrokerURL = document.getElementById("buildbroker").value;
	$scope.buildBrokerAPI = document.getElementById("buildbrokerapi").value;
	$scope.buildtime = parseInt(document.getElementById("buildtime").value);
	$scope.buildBrokeHeaders = {
		headers: {
			"X-API-Key": $scope.buildBrokerAPI
		}
	};
	if (!$scope.buildBrokerURL.endsWith("/")) {
		$scope.buildBrokerURL += "/";
	}
	$scope.loadBuilds = function () {
		if ($scope.buildRefresh) {
			$interval.cancel($scope.buildRefresh);
		}
		$scope.buildRefresh = $interval(function () {
			$scope.updatePercent();
		}, 5000);
		if (!Array.isArray($scope.builds)) {
			$scope.builds = [];
		}
		let url = new URL($scope.buildBrokerURL + "byTaskID");
		if (ttid) {
			url.searchParams.set("id", ttid);
		}
		url.searchParams.set("page", $scope.buildsstate.page);
		url.searchParams.set("showby", $scope.buildsstate.showby);
		$http.get(url.toString(), $scope.buildBrokeHeaders)
			.then(function (result) {
				if (JSON.stringify($scope.builds) !== JSON.stringify(result.data)) {
					$scope.builds = result.data.map(function (b) {
						let output = {
							TTID: b.item.parentID,
							MACHINE: b.item.machine,
							STATUSTXT: b.item.statusText,
							STATUS: b.item.status,
							HUMANSTATUS: b.item.humanStatus,
							ID: b.item.id,
							NOTES: b.item.notes,
							COLOR: b.item.color,
							DATEUP: $scope.fromUTC(b.item.dateModified).toLocaleString(),
							DATEBUILD: b.item.status == $scope.BuildStatus.notstarted ? '' : $scope.fromUTC(b.item.dateBuild).toLocaleString(),
							EML: b.item.userEmail,
							DATE: new Date(b.item.dateCreatedOffset).toLocaleString(),
							TESTGUID: b.item.uGuid,
						}
						return output;
					});
					$scope.updatePercent();
					reActivateTooltips();
				}
			});
	};
	$scope.updatePercent = function () {
		upadteBuildProgress($scope.builds, $scope.buildtime, $scope.BuildStatus.progress);
	};
	$scope.abortBuild = function () {
		if (!confirm("Are you sure you want to abort builds for the task?")) {
			return;
		}

		for (var i = 0; i < $scope.builds.length; i++) {
			if ($scope.builds[i].STATUS == $scope.BuildStatus.notstarted || $scope.builds[i].STATUS == $scope.BuildStatus.progress) {
				let url = new URL($scope.buildBrokerURL + "cancel");
				url.searchParams.set("id", $scope.builds[i].ID);
				$http.post(url.toString(), {}, $scope.buildBrokeHeaders)
					.then(function () {
						$scope.loadBuilds();
					});
				return;
			}
		}
		alert("Threre are no waiting for build requests!");
	};
	$scope.conntectToBuildBroker = function (connect) {
		if ($scope.buildEvents) {
			$scope.buildEvents.close();
			$scope.buildEvents = undefined;
		}
		if (connect == true) {
			let url = new URL($scope.buildBrokerURL + "events");
			if (ttid) {
				url.searchParams.set("parent", ttid);
			}
			url.searchParams.set("X-API-Key", $scope.buildBrokerAPI);
			$scope.buildEvents = new EventSource(url.toString());
			$scope.buildEvents.onmessage = function (event) {
				$scope.loadBuilds();
				$scope.$apply();
			};
		}
	};
}