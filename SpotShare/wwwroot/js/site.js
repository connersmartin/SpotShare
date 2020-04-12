$("#authBtn").click(function () {
    location.href = 'Share/Auth';
});
if (document.cookie.indexOf('spotauthtoke') !== -1) {
    $("#initLink").removeAttr("hidden");
}