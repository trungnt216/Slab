$(document).ready(function () {
    let selectedIds = [];
    checkEmptyTable();
/*    updateDeleteButtonState();*/


    // handle click delete item
    $('.delete').on('click', function () {
        let id = $(this).data('id');
        selectedIds.push(id);

        checkEmptyTable();
    })
    
    $('.btn-custom-delete-cancel').on('click', function () {
        selectedIds = [];

        checkEmptyTable();
    })

    // handle row checkbox change
    $('.selectRow').on('change', function () {
        let id = $(this).data('id');
        if ($(this).is(':checked')) {
            if (!selectedIds.includes(id)) {
                selectedIds.push(id);
            }
        } else {
            selectedIds = selectedIds.filter(item => item !== id);
        }
        updateDeleteButtonState();
        checkEmptyTable();
    });

    // handle select all checkbox change
    $('#selectAll').on('change', function () {
        selectedIds = [];
        if ($(this).is(':checked')) {
            $('.selectRow').each(function () {
                $(this).prop('checked', true);
                selectedIds.push($(this).data('id'));
            });
        } else {
            $('.selectRow').prop('checked', false);
        }
        updateDeleteButtonState();
    });

    // handle delete confirmation in modal
    $('.btn-custom-delete').on('click', function () {
        if (selectedIds.length > 0) {
            let url = $(this).attr("href");
            $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ ids: selectedIds }),
                success: function () {
                    loadContent(window.location.pathname);
                },
                error: function (xhr, status, error) {
                    console.error('Deletion failed:', error);
                }
            });
            updateDeleteButtonState();
        } else {
            alert('Please select at least one banner to delete.');
        }
    });

    function loadContent(url) {
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                var subContent = $(data).find("#sub-content").html();
                $("#content .home-container").html(subContent);
                history.pushState(null, "", url);

                $(data).find("script[src]").each(function () {
                    var script = document.createElement("script");
                    script.src = this.src;
                    document.head.appendChild(script);
                });
            },
            error: function (xhr, status, error) {
                console.log("Đã xảy ra lỗi: " + error);
            },
        });
    }

    // check empty table
    function checkEmptyTable() {
        var rowCount = $(".table tbody tr").length;

        if (rowCount === 0) {
            $(".table").after("<p class='noti-table'>Không có dữ liệu!</p>");
        } else {
            $(".table").next("p").remove();
        }
    }

    // update state btn delete
/*    function updateDeleteButtonState() {
        if (selectedIds.length > 0) {
            $('#deleteSelected').prop('disabled', false);
        } else {
            $('#deleteSelected').prop('disabled', true);
        }
    }*/
});