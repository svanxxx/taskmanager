function CreateHoursCharts() {
	var config = {
		type: "pie",
		data: {
			datasets: [{
				data: [1, 1],
				backgroundColor: ["Red", "Blue"],
				label: "Total Hours"
			}],
			labels: [
				"Created",
				"Finished"
			],
		},
		options: {
			responsive: true,
			title: {
				display: true,
				position: "bottom",
				text: "Totals:"
			}
		}
	};
	var configbar = {
		type: "bar",
		data: {
			datasets: [{
				data: [1, 2],
				backgroundColor: "Red",
				label: "Created"
			}, {
				data: [1, 2],
				backgroundColor: "Blue",
				label: "Finished"
			}],
			labels: [
				"User 1",
				"User 2"
			]
		},
		options: {
			responsive: true
		}
	};
	var ctx_hourspermonth = document.getElementById("hourspermonth").getContext("2d");
	var ctx_hourspermonthP = document.getElementById("hourspermonthP").getContext("2d");
	window.hourspermonth = new Chart(hourspermonth, config);
	window.hourspermonthP = new Chart(hourspermonthP, configbar);
}
function CreateSickChart() {
	var barChartData = {
		labels: [],
		datasets: [{
			label: "Sick",
			backgroundColor: "red",
			borderColor: "red",
			borderWidth: 1,
			data: []
		}, {
			label: "Vacations",
			backgroundColor: "yellow",
			borderColor: "yellow",
			borderWidth: 1,
			data: []
		}, {
			label: "Unused Vacations",
			backgroundColor: "blue",
			borderColor: "blue",
			borderWidth: 1,
			data: []
		}, {
			label: "Total out of work",
			backgroundColor: "orange",
			borderColor: "orange",
			borderWidth: 1,
			data: []
		}, {
			label: "Total work hours",
			backgroundColor: "gray",
			borderColor: "gray",
			borderWidth: 1,
			data: []
		}]
	};
	var ctx = document.getElementById("sick").getContext("2d");
	window.SickChart = new Chart(ctx, {
		type: "bar",
		data: barChartData,
		options: {
			responsive: true,
			legend: {
				position: "top",
			},
			title: {
				display: true,
				text: "Vacations and sick days"
			}
		}
	});
}
function DrawSickChart(users) {
	var labels = [];
	var sick = [];
	var used = [];
	var free = [];
	var total = [];
	var whours = [];
	for (var i = 0; i < users.length; i++) {
		var u = users[i];
		labels.push(u.LOGIN);
		sick.push(u.sick);
		used.push(u.scheduled);
		free.push(u.unscheduled);
		whours.push(u.whours);
		total.push(u.sick + u.scheduled);
	}
	var d = window.SickChart.data;

	d.labels.pop();
	d.datasets[0].data.pop();
	d.datasets[1].data.pop();
	d.datasets[2].data.pop();
	d.datasets[3].data.pop();
	d.datasets[4].data.pop();
	window.SickChart.update();

	d.labels = labels;
	d.datasets[0].data = sick;
	d.datasets[1].data = used;
	d.datasets[2].data = free;
	d.datasets[3].data = total;
	d.datasets[4].data = whours;
	window.SickChart.update();
}
function DrawHoursCharts(users) {
	var labels = [];
	var createdH = 0;
	var finishedH = 0;
	var arrcreatedH = [];
	var arrfinishedH = [];
	for (var i = 0; i < users.length; i++) {
		var u = users[i];
		labels.push(u.LOGIN);
		arrcreatedH.push(u.createdH);
		arrfinishedH.push(u.finishedH);
		createdH += u.createdH;
		finishedH += u.finishedH;
	}
	var d = window.hourspermonth.data;
	d.datasets[0].data.pop();
	window.hourspermonth.update();
	d.datasets[0].data = [createdH, finishedH];
	window.hourspermonth.update();

	var d = window.hourspermonthP.data;
	d.labels.pop();
	d.datasets[0].data.pop();
	d.datasets[1].data.pop();
	window.hourspermonth.options.title.text = "Total hours: " + (createdH + finishedH);
	window.hourspermonthP.update();
	d.labels = labels;
	d.datasets[0].data = arrcreatedH;
	d.datasets[1].data = arrfinishedH;
	window.hourspermonthP.update();
}
$(function () {

	CreateSickChart();
	CreateHoursCharts();

	var app = angular.module("mpsapplication", []);
	app.controller("mpscontroller", ["$scope", "$http", function ($scope, $http) {

		getDispos($scope, "dispos", $http);
		$scope.isVacationScheduled = function (v) {
			var idxDisp = $scope.dispos.findIndex(function (x) { return x.ID == v.DISPO; });
			return !$scope.dispos[idxDisp].CANNOTSTART;
		}

		$scope.daterep = new Date();
		$scope.daterep.setDate(1);
		$scope.daterep.setMonth(0);
		$scope.daterep.setHours(0, 0, 0, 0);

		$scope.daterepend = new Date();
		$scope.daterepend.setDate(1);
		$scope.daterepend.setMonth(11);
		$scope.daterepend.setHours(0, 0, 0, 0);

		$scope.loadData = function () {
			var usersprg = StartProgress("Loading users...");
			$http.post("trservice.asmx/getMPSusers", JSON.stringify({ "active": true }))
				.then(function (result) {
					$scope.users = result.data.d;
					for (var i = 0; i < $scope.users.length; i++) {
						var u = $scope.users[i];
						u.scheduled = 0;
						u.unscheduled = 0;
						u.whours = 0;
						u.wdays = 0;
						u.sick = 0;
						u.created = 0;
						u.createdH = 0;
						u.finished = 0;
						u.finishedH = 0;
					}

					EndProgress(usersprg);
					var vacsprg = StartProgress("Loading vacations...");
					var repto = new Date($scope.daterepend.getFullYear(), $scope.daterepend.getMonth() + 1, 0);
					var diff = (repto - $scope.daterep) / (24 * 3600 * 1000);
					$http.post("trservice.asmx/EnumCloseVacations", JSON.stringify({ "start": DateToString($scope.daterep), "days": diff }))
						.then(function (result) {
							for (var i = 0; i < result.data.d.length; i++) {
								var v = result.data.d[i];
								for (var j = 0; j < $scope.users.length; j++) {
									var u = $scope.users[j];
									if (u.TTUSERID == v.AUSER) {
										if ($scope.isVacationScheduled(v)) {
											if (v.SICK) {
												u.sick++;
											} else {
												u.scheduled++;
											}
										} else {
											u.unscheduled++;
										}
										break;
									}
								}
							}
							EndProgress(vacsprg);
							DrawSickChart($scope.users);
						})
					var drecsprg = StartProgress("Loading daily reports...");
					$http.post("trservice.asmx/GetTRStatistic", JSON.stringify({ "start": DateToString($scope.daterep), "days": diff }))
						.then(function (result) {
							for (var i = 0; i < result.data.d.length; i++) {
								var v = result.data.d[i];
								for (var j = 0; j < $scope.users.length; j++) {
									var u = $scope.users[j];
									if (u.TTUSERID == v.AUSER) {
										u.whours = v.HOURS;
										break;
									}
								}
							}
							EndProgress(drecsprg);
							DrawSickChart($scope.users);
						})
					$http.post("trservice.asmx/GetStatistics", JSON.stringify({ "start": DateToString($scope.daterep), "days": diff }))
						.then(function (result) {
							for (var i = 0; i < result.data.d.length; i++) {
								var s = result.data.d[i];
								for (var j = 0; j < $scope.users.length; j++) {
									var u = $scope.users[j];
									if (u.TTUSERID == s.TTUSER) {
										if (s.FLAG == 1) {
											u.created = s.CNT;
											u.createdH = s.HOURS;
										} else {
											u.finished = s.CNT;
											u.finishedH = s.HOURS;
										}
										break;
									}
								}
							}
							EndProgress(vacsprg);
							DrawHoursCharts($scope.users);
						})
				});
		}
		$scope.loadData();
	}]);
})