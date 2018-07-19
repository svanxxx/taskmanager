$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

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