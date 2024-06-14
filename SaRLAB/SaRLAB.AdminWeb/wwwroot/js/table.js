document.addEventListener('DOMContentLoaded', function () {
    var popupNotification = document.getElementById('popupNotification');
    if (popupNotification.textContent.trim() !== "") {
        popupNotification.classList.add('show');

        setTimeout(function () {
            popupNotification.classList.remove('show');
            popupNotification.classList.add('hide');
            popupNotification.addEventListener('transitionend', function () {
                popupNotification.style.display = 'none';
                popupNotification.classList.remove('hide');
            }, { once: true });
        }, 1000);
    }
});

var deleteUrl = '';
var selectedIds = [];

document.getElementById('selectAll').addEventListener('change', function () {
    var checkboxes = document.querySelectorAll('.selectRow');
    for (var checkbox of checkboxes) {
        checkbox.checked = this.checked;
    }
});

document.getElementById('deleteSelected').addEventListener('click', function () {
    selectedIds = [];
    var checkboxes = document.querySelectorAll('.selectRow:checked');
    for (var checkbox of checkboxes) {
        selectedIds.push(checkbox.getAttribute('data-id'));
    }

    if (selectedIds.length > 0) {
        document.getElementById('deleteModal').style.display = 'block';
    } else {
        alert("Vui lòng chọn ít nhất một mục để xóa.");
    }
});


// Mở modal xóa cho từng item
function openDeleteModal(element) {
    deleteUrl = element.getAttribute("data-url");
    selectedIds = [];
    document.getElementById('deleteModal').style.display = 'block';
}


function closeDeleteModal() {
    document.getElementById('deleteModal').style.display = 'none';
}

// Xác nhận xóa
document.getElementById('confirmDeleteButton').onclick = function () {
    if (selectedIds.length > 0) {
        // Gửi yêu cầu xóa nhiều mục
        fetch('@Url.Action("DeleteMultipleBanners", "Configuration")', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Ids: selectedIds })
        }).then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                alert("Có lỗi xảy ra khi xóa các mục.");
            }
        });
    } else if (deleteUrl) {
        // Gửi yêu cầu xóa một mục
        window.location.href = deleteUrl;
    }
};


window.onclick = function (event) {
    var modal = document.getElementById('deleteModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}