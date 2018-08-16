var _admin = undefined;
function IsAdmin() {
	if (typeof _admin === "undefined") {
		_admin = document.getElementById("isadmin").value;
	}
	return _admin == "True";	
}
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
function getCompById() {
	return function (id, $scope) {
		return $scope.comps.filter(function (x) { return x.ID == id; })[0].DESCR;
	};
}
function getSeveById() {
	return function (id, $scope) {
		var r = $scope.severs.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].DESCR;
		}
		return "";
	};
}
function getUserById() {
	return function (id, $scope) {
		if (!$scope.users) {
			return "";
		}
		var r = $scope.users.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].FULLNAME;
		}
		return "";
	};
}

function getTypes($scope, member, $http) {
	loadReference($scope, member, $http, "types", "gettasktypes");
}
function getDispos($scope, member, $http) {
	loadReference($scope, member, $http, "dispos", "gettaskdispos");
}
function getMPSusers($scope, member, $http) {
	loadReference($scope, member, $http, "mpsusers", "getActiveMPSusers");
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