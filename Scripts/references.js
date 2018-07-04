function loadReference($scope, member, $http, localmember, functionname) {
	var m = localmember + "_storageversion";
	var curr = document.getElementById("referenceid").value;
	if (localStorage[m] != curr) {
		localStorage.removeItem(localmember);
		localStorage[m] = curr;
	}

	if (localStorage[localmember]) {
		$scope[member] = JSON.parse(localStorage[localmember]);
	} else {
		if (!("loaders" in $scope)) {
			$scope["loaders"] = 0;
		}
		var prgtypes = StartProgress("Loading " + localmember + "..."); 
		$scope["loaders"]++;
		$http.post("trservice.asmx/" + functionname, JSON.stringify({}))
			.then(function (result) {
				$scope[member] = result.data.d;
				localStorage[localmember] = JSON.stringify($scope[member]);
				EndProgress(prgtypes); $scope["loaders"]--;
			});
	}
}
function getTypes($scope, member, $http) {
	loadReference($scope, member, $http, "types", "gettasktypes");
}
function getDispos($scope, member, $http) {
	loadReference($scope, member, $http, "dispos", "gettaskdispos");
}
function getDispoColorById() {
	return function (id, $scope) {
		var col = ($scope.dispos && $scope.dispos.length > 0) ? $scope.dispos.filter(function (x) { return x.ID == id; })[0].COLOR : "white";
		return { "background-color": col };
	};
}
function getDispoById() {
	return function (id, $scope) {
		return $scope.dispos.filter(function (x) { return x.ID == id; })[0].DESCR;
	};
}
function getUsers($scope, member, $http) {
	loadReference($scope, member, $http, "users", "gettaskusers");
}
function getPriorities($scope, member, $http) {
	loadReference($scope, member, $http, "priorities", "gettaskpriorities");
}
function getSevers($scope, member, $http) {
	loadReference($scope, member, $http, "severs", "gettasksevers");
}
function getProducts($scope, member, $http) {
	loadReference($scope, member, $http, "products", "gettaskproducts");
}
function getComps($scope, member, $http) {
	loadReference($scope, member, $http, "comps", "gettaskcomps");
}