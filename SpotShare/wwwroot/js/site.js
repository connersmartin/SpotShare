$("#authBtn").click(function () {
    location.href = 'Auth/Auth?user=true';
});
if (document.cookie.indexOf('spottoke') !== -1) {
    $("#initLink").removeAttr("hidden");
}

$("#makeShare").click(function () {
    $.ajax({
        type: "GET",
        url: "/Share/CreateShare",
        success: function (data) {
            $("#dataContainer").html(data);
        }
    });
})

$("#getShares").click(function () {
    $.ajax({
        type: "GET",
        url: "/Share/StartShare",
        success: function (data) {
            $("#dataContainer").html(data);
        }
    });
})


$("#createSubmit").click(function () {
    var p = new Object();
    p.uri = $("#spotUri").val();
    $.ajax({
        type: "POST",
        url: "/Share/CreateShare",
        data: JSON.stringify(p),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#dataContainer").html(data);
        }
    });
})

function startShare (id,uri) {
    var p = new Object();
    p.id = id;
    p.spotifyuri = uri;
    $.ajax({
        type: "POST",
        url: "/Share/StartShare",
        data: JSON.stringify(p),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#dataContainer").html(data);
        }
    });
}