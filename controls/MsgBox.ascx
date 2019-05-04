<%@ Control Language="C#" AutoEventWireup="true" %>

<script>
	function msgBox(title, mess, func) {
		document.getElementById("msgBoxTitle").innerHTML = title;
		var msg = document.getElementById("msgBoxMess");
		msg.value = mess;
		var el = document.getElementById("msgBoxOK"),
		elClone = el.cloneNode(true);
		el.parentNode.replaceChild(elClone, el);//all listeners are gone now
		document.getElementById("msgBoxOK").addEventListener("click", function () {
			func(document.getElementById("msgBoxMess").value);
		});
		$("#msgBox").modal();
		setTimeout(function () { msg.focus(); }, 250);
	};
</script>

<div class="modal fade" id="msgBox">
	<div class="modal-dialog modal-dialog-centered">
		<div class="modal-content">
			<div class="modal-header">
				<h4 class="modal-title" id="msgBoxTitle"></h4>
				<button type="button" class="close" data-dismiss="modal">&times;</button>
			</div>
			<div class="modal-body">
				<div class="form-group">
					<textarea id="msgBoxMess" class="form-control" rows="5"></textarea>
				</div>
			</div>
			<div class="modal-footer">
				<button id="msgBoxOK" type="button" class="btn btn-success" data-dismiss="modal">Ok</button>
				<button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>
			</div>
		</div>
	</div>
</div>
