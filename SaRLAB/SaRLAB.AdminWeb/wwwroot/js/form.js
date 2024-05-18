/*$(document).ready(function () {
    $("form").submit(function (event) {
        event.preventDefault();

        var actionUrl = $(this).attr("action");
        var method = $(this).attr("method");

        var formData = new FormData($(this)[0]);

        $.ajax({
            url: actionUrl,
            type: method,
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                var subContent = $(data).find("#sub-content").html();
                $("#content .home-container").html(subContent);
                history.pushState(null, "", url);
            },
            error: function (xhr, status, error) {
                console.error("Đã xảy ra lỗi:", error);
            }
        });
    });
});
*/