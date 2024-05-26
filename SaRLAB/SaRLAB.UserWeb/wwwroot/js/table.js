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

function searchTable() {
    var input, filter, table, tr, td, i, j, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toLowerCase();
    table = document.querySelector(".table tbody");
    tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        tr[i].style.display = "none";
        td = tr[i].getElementsByTagName("td");
        for (j = 0; j < td.length; j++) {
            if (td[j]) {
                txtValue = td[j].textContent || td[j].innerText;
                if (txtValue.toLowerCase().indexOf(filter) > -1) {
                    tr[i].style.display = "";
                    break;
                }
            }
        }
    }
}

function sortTable(column) {
    var table = document.getElementById("tableBody");
    var rows = Array.from(table.rows);
    var isAsc = table.getAttribute('data-sort') === 'asc';
    var columnIndex = column === 'UpdateTime' ? 6 : 7;

    rows.sort(function (a, b) {
        var x = new Date(a.cells[columnIndex].innerText);
        var y = new Date(b.cells[columnIndex].innerText);
        return isAsc ? x - y : y - x;
    });

    rows.forEach(function (row) {
        table.appendChild(row);
    });

    table.setAttribute('data-sort', isAsc ? 'desc' : 'asc');
}
