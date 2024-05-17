$(document).ready(function () {
    function initializeFormSubmit() {
        $("form").off("submit").on("submit", function (event) {
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

                    // Remove all existing scripts
                    $('script').remove();

                    $(data).find("script[src]").each(function () {
                        var script = document.createElement("script");
                        script.src = this.src;
                        document.head.appendChild(script);
                    });

                    // Reinitialize event handlers after content load
                    initializeFormSubmit();
                },
                error: function (xhr, status, error) {
                    console.error("Đã xảy ra lỗi:", error);
                }
            });
        });
    }

    // Initialize form submit handler on document ready
    initializeFormSubmit();
});
