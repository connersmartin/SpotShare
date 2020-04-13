if (document.cookie.indexOf('spottoke') !== -1) {
    $("#initLink").removeAttr("hidden");
}
function startShare(id, uri) {
    var params = new Object();
    params.id = id;
    params.spotifyuri = uri;
    $.ajax({
        type: "POST",
        url: "/Share/StartShare",
        data: JSON.stringify(params),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#dataContainer").html(data);
        }
    });
}

    $("#authBtn").click(function () {
        location.href = 'Auth/Auth?user=true';
    })

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


    function submitCreate () {
        
        $.ajax({
            type: "POST",
            url: "/Share/ShareCreate?id=" + $("#spotUri").val(),            
            success: function (data) {
                $("#dataContainer").html(data);
            }
        });
    }