function CreateSickChart() {
	var labels = [];
	var sick = [];
	var used = [];
	var free = [];
	var total = [];
	var barChartData = {
		labels: labels,
		datasets: [{
			label: 'Sick',
			backgroundColor: "red",
			borderColor: "red",
			borderWidth: 1,
			data: sick
		}, {
			label: 'Vacations',
			backgroundColor: "yellow",
			borderColor: "yellow",
			borderWidth: 1,
			data: used
		}, {
			label: 'Unused Vacations',
			backgroundColor: "blue",
			borderColor: "blue",
			borderWidth: 1,
			data: free
		}, {
			label: 'Total out of work',
			backgroundColor: "orange",
			borderColor: "orange",
			borderWidth: 1,
			data: total
		}]
	};
	var ctx = document.getElementById("sick").getContext('2d');
	window.SickChart = new Chart(ctx, {
		type: 'bar',
		data: barChartData,
		options: {
			responsive: true,
			legend: {
				position: 'top',
			},
			title: {
				display: true,
				text: 'Vacations and sick days'
			}
		}
	});
}
function DrawSick(users) {
	var labels = [];
	var sick = [];
	var used = [];
	var free = [];
	var total = [];
	for (var i = 0; i < users.length; i++) {
		var u = users[i];
		labels.push(u.LOGIN);
		sick.push(u.sick);
		used.push(u.scheduled);
		free.push(u.unscheduled);
		total.push(u.sick + u.scheduled);
	}
	var d = window.SickChart.data;

	d.labels.pop();
	d.datasets[0].data.pop();
	d.datasets[1].data.pop();
	d.datasets[2].data.pop();
	d.datasets[3].data.pop();
	window.SickChart.update();

	d.labels = labels;
	d.datasets[0].data = sick;
	d.datasets[1].data = used;
	d.datasets[2].data = free;
	d.datasets[3].data = total;
	window.SickChart.update();
}
$(function () {

	CreateSickChart();

	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

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
						u.sick = 0;
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
							DrawSick($scope.users);
						})
				});
		}

		$scope.loadData();

		var ctx_hourspermonth = document.getElementById("hourspermonth").getContext('2d');
		var ctx_hourspermonthP = document.getElementById("hourspermonthP").getContext('2d');

		var ctx_countpermonth = document.getElementById("countpermonth").getContext('2d');
		var ctx_countpermonthP = document.getElementById("countpermonthP").getContext('2d');


		var config = {
			type: 'pie',
			data: {
				datasets: [{
					data: [
						1,
						2,
						3,
						4,
						5,
					],
					backgroundColor: [
						'Red',
						'Orange',
						'Yellow',
						'Green',
						'Blue'
					],
					label: 'Dataset 1'
				}],
				labels: [
					'Red',
					'Orange',
					'Yellow',
					'Green',
					'Blue'
				]
			},
			options: {
				responsive: true
			}
		};
		var configbar = {
			type: 'bar',
			data: {
				datasets: [{
					data: [
						1,
						2,
						3,
						4,
						5,
					],
					backgroundColor: [
						'Red',
						'Orange',
						'Yellow',
						'Green',
						'Blue'
					],
					label: 'Dataset 1'
				}],
				labels: [
					'Red',
					'Orange',
					'Yellow',
					'Green',
					'Blue'
				]
			},
			options: {
				responsive: true
			}
		};


		window.hourspermonth = new Chart(hourspermonth, config);
		window.hourspermonthP = new Chart(hourspermonthP, configbar);

		window.countpermonth = new Chart(countpermonth, config);
		window.countpermonthP = new Chart(countpermonthP, configbar);
	}]);
})