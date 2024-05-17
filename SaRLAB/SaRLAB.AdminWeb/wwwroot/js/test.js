﻿$(document).ready(function () {
    // handle click subject
    $(".subject a").on("click", function (e) {
        e.preventDefault();

        $(".subject a").removeClass("active");

        $(this).addClass("active");
        var target = $(this).data("target");

        $(".subject-content").children().hide();
        $(target).removeClass("d-none");
        $(target).show();
    });

    // handle click item in collapse by subject
    $("a.item").on("click", function (event) {
        event.preventDefault();

        $("a.item").removeClass("active");
        $(this).addClass("active");

        var url = $(this).attr("href");

        loadContent(url);
    });

    // handle click for btn
    $("a.btn-custom-create").on("click", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");
        loadContent(url);
    });

    $("a.btn-custom-edit").on("click", function (e) {
        e.preventDefault();

        var url = $(this).attr("href") + "/" + $(this).data("id");
        loadContent(url);
    });
    $("a.btn-custom-edit-by-email").on("click", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");
        loadContent(url);
    });


    $("a.btn-custom-return").on("click", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");
        loadContent(url);
    });

    // load content using ajax
    function loadContent(url) {
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                var subContent = $(data).find("#sub-content").html();
                $("#content .home-container").html(subContent);
                history.pushState(null, "", url);

                console.log("test" + $(data).find("script[src]"));
                $('script[src]').remove();

                $(data).find("script[src]").each(function () {
                    var src = $(this).attr("src");
                    $.getScript(src + '?t=' + new Date().getTime());
                });
            },
            error: function (xhr, status, error) {
                console.log("Đã xảy ra lỗi: " + error);
            },
        });
    }
});