function CreateChart() {
	var config = {
		type: "pie",
		data: {
			datasets: [{
				data: [],
				backgroundColor: [],
				label: "Completion chart"
			}],
			labels: []
		},
		options: {
			responsive: true,
			title: {
				display: true,
				position: "bottom",
				text: "Completion chart"
			}
		}
	};
	var chartctx = document.getElementById("chartpie").getContext("2d");
	window.chartpie = new Chart(chartctx, config);
}
function DrawChart($scope) {
	var display = {};
	for (var i = 0; i < $scope.defects.length; i++) {
		var d = $scope.defects[i];
		var val = display[d.DISPO];
		if (val != undefined) {
			val += d.ESTIM;
			display[d.DISPO] = val;
		} else {
			display[d.DISPO] = d.ESTIM;
		}
	}
	var d = window.chartpie.data;
	d.labels = [];
	d.datasets.splice(0, 1);
	var newDataset = {
		backgroundColor: [],
		data: [],
		label: "Completion chart",
	};
	var labels = [];

	window.chartpie.update();

	for (var propertyName in display) {
		newDataset.data.push(display[propertyName]);
		newDataset.backgroundColor.push(getDispoColorById()(propertyName, $scope)["background-color"]);
		labels.push(getDispoNameById()(propertyName, $scope) + " : " + display[propertyName]);
	}
	d.labels = labels;
	d.datasets.push(newDataset);
	window.chartpie.update();
}

$(function () {
	CreateChart();
	var app = angular.module('mpsapplication', []);
	app.filter('getDispoColorById', getDispoColorById);
	app.filter("sumFormat", ["$sce", sumFormat]);

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.isadmin = IsAdmin();
		getDispos($scope, "dispos", $http);
		getUsers($scope, "users", $http);
		$scope.filters = [];
		$scope.defects = [];
		$scope.id = getParameterByName("id");
		var tmp = document.getElementById("trackers").value;
		if (tmp === "") {
			$scope.trackers = [];
		} else {
			$scope.trackers = JSON.parse(tmp);
			if ($scope.id == "" && $scope.trackers.length > 0) {
				$scope.id = $scope.trackers[0].ID;
				window.history.pushState(null, "", replaceUrlParam(location.href, "id", $scope.id));
			}
		}

		if ($scope.isadmin) {
			var prgfltr = StartProgress("Loading filters...");
			$http.post("TrackerService.asmx/getFilters", JSON.stringify({ "user": userID() }))
				.then(function (res) {
					$scope.filters = res.data.d;
					EndProgress(prgfltr);
				});
		}
		$scope.assignToClient = function (track, user) {
			var prgass = StartProgress("Assigning...");
			$http.post("TrackerService.asmx/assignTracker", JSON.stringify({ "id": track, "userid": user }))
				.then(function () {
					for (var i = 0; i < $scope.trackers.length; i++) {
						if ($scope.trackers[i].ID == track) {
							$scope.trackers[i].IDCLIENT = user
							break;
						}
					}
					EndProgress(prgass);
				});
		};
		$scope.delTracker = function (id) {
			var r = confirm("Are you sure you want to delete this tracker?");
			if (r !== true) {
				return;
			}
			var delprg = StartProgress("Deleting...");
			$http.post("TrackerService.asmx/delTracker", JSON.stringify({ "id": id }))
				.then(function (res) {
					$scope.trackers = $scope.trackers.filter(function (item) {
						return (item.ID != id);
					});
					EndProgress(delprg);
				});
		};
		$scope.addTracker = function (id, inputname) {
			var name = prompt("Please enter the name", inputname);
			if (name !== "" && name !== null) {
				$http.post("TrackerService.asmx/newTracker", JSON.stringify({ "name": name, "user": ttUserID(), "filter": id }))
					.then(function (res) {
						$scope.trackers.push(res.data.d);
						EndProgress(prgfltr);
					});
			}
		};
		$scope.loadData = function () {
			if ($scope.id !== "") {
				var taskprg = StartProgress("Loading data...");
				var tr = $scope.trackers.find(function (item) { return item.ID == $scope.id });
				$http.post("TrackerService.asmx/getItems", JSON.stringify({ "filterid": tr.IDFILTER }))
					.then(function (res) {
						$scope.defects = res.data.d;
						EndProgress(prgfltr);
						DrawChart($scope);
					});
				EndProgress(taskprg);
			}
		};
		$scope.loadData();
		$scope.lastloaded = "";
		if ($scope.id != "") {
			$interval(function () {
				var tr = $scope.trackers.find(function (item) { return item.ID == $scope.id });
				$http.post("TrackerService.asmx/getTrackerModified", JSON.stringify({ "id": tr.IDFILTER }))
					.then(function (res) {
						if ($scope.lastloaded == "") {
							$scope.lastloaded = res.data.d;
							return;
						}
						if ($scope.lastloaded != res.data.d) {
							$scope.lastloaded = res.data.d;
							$scope.loadData();
						}
					});
				
			}, 5000);
		}
	}]);
});