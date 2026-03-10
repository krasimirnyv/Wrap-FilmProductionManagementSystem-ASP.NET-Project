// Dismiss alert automatically after 5 seconds
setTimeout(function() {
    var alert = document.querySelector('.alert');
    if (alert) {
        var bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    }
}, 5000);

