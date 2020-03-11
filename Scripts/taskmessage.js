function Linkify(inputText) {
	//URLs starting with http://, https://, or ftp://
	var replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
	var replacedText = inputText.replace(replacePattern1, '<a href="$1" target="_blank">$1</a>');

	//URLs starting with www. (without // before it, or it'd re-link the ones done above)
	var replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
	replacedText = replacedText.replace(replacePattern2, '$1<a href="http://$2" target="_blank">$2</a>');

	//Change email addresses to mailto:: links
	var replacePattern3 = /(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})/gim;
	replacedText = replacedText.replace(replacePattern3, '<a href="mailto:$1">$1</a>');

	//Change email addresses to mailto:: links
	var replacePattern4 = /TT\d+/gim;
	replacedText = replacedText.replace(replacePattern4, function (match, $1, $2, offset, original) {
		return "<a href='showtask.aspx?ttid=" + match.substring(2) + ">" + match.toUpperCase() + "</a>";
	});

	return replacedText;
}
class TaskMessage extends HTMLElement {
	connectedCallback() {

		var lines = this.innerHTML.trim().split("\n");
		for (var i = 0; i < lines.length; i++) {
			lines[i] = Linkify(lines[i]);
		}
		lines = "<span>" + lines.join("</span><br/><span>") + "</span>";
		var color = !this.clr ? "#9E9E9E" : this.clr;

		this.innerHTML =
			"<div class='toast mb-1' data-autohide='false' style='max-width:100%;box-shadow: 5px 5px 13px 0px " + color + ";'>" +
			"<div class='toast-header'>" +
			"<img src='getUserImg.ashx?id=" + this.userid + "&sz=20'/>&nbsp;" +
			"<span class='mr-auto text-primary'>" + this.user + "</span>" +
			"<small class='text-muted'>" + this.time + "</small>" +
			"</div>" +
			"<div class='toast-body'>" +
			lines +
			"</div>" +
			"</div>";
	}
	get time() {
		return this.getAttribute('time');
	}
	set time(val) {
		this.setAttribute('time', val);
	}
	get user() {
		return this.getAttribute('user');
	}
	set user(val) {
		this.setAttribute('user', val);
	}
	get userid() {
		return this.getAttribute('userid');
	}
	set userid(val) {
		this.setAttribute('userid', val);
	}
	get clr() {
		return this.getAttribute('clr');
	}
	set clr(val) {
		this.setAttribute('clr', val);
	}
}
$(function () {
	customElements.define('task-message', TaskMessage);
});