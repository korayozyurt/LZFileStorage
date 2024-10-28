function SwalLoading() {
    Swal.showLoading();
}

function MesajGoster(title, message, status) {
    $(function () {
        Swal.fire(title, message, status);
    });
}

function SwalClose() {
    Swal.close();
}