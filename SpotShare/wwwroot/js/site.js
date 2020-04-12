$("#authBtn").click(function () {
    location.href = 'Share/Auth?user=true';
});
if (document.cookie.indexOf('spottoke') !== -1) {
    $("#initLink").removeAttr("hidden");
}

$("#makeShare").click(function () {
    $.ajax({
        type: "GET",
        url: "/Share/CreateShare",
        success: function (data) {
            $("#createShare").html(data);
        }
    });
})


//Do ajax call for this for better usability
/*
$("#createSubmit").click(function () {
    $.ajax({
        type: "POST",
        url: "/Share/CreateShare"
    })
})
*/