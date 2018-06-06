function getDispos() {
	if (localStorage.dispos) {
		return JSON.parse(localStorage.dispos);
	}
	return null;
}
function setDispos(d) {
	localStorage.dispos = JSON.stringify(d);
}