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

    var visibleRowCount = 0; // Counter for visible rows

    for (i = 0; i < tr.length; i++) {
        tr[i].style.display = "none";
        td = tr[i].getElementsByTagName("td");
        for (j = 0; j < td.length; j++) {
            if (td[j]) {
                txtValue = td[j].textContent || td[j].innerText;
                if (txtValue.toLowerCase().indexOf(filter) > -1) {
                    tr[i].style.display = "";
                    visibleRowCount++;
                    // Update the index cell with the new order
                    tr[i].getElementsByTagName("td")[0].innerText = visibleRowCount;
                    break;
                }
            }
        }
    }
}

function sortTable(tableId, columnIndex, isDate = false) {
    var table = document.getElementById(tableId);
    var tableBody = table.querySelector("tbody");
    var rows = Array.from(tableBody.rows);
    var isAsc = tableBody.getAttribute('data-sort') === 'asc';

    rows.sort(function (a, b) {
        var x = a.cells[columnIndex].innerText.trim();
        var y = b.cells[columnIndex].innerText.trim();

        if (isDate) {
            x = new Date(x);
            y = new Date(y);
            return isAsc ? x - y : y - x;
        } else if (!isNaN(x) && !isNaN(y)) {
            // Compare numbers
            x = parseFloat(x);
            y = parseFloat(y);
            return isAsc ? x - y : y - x;
        } else {
            // Compare text
            x = x.toLowerCase();
            y = y.toLowerCase();
            if (x < y) return isAsc ? -1 : 1;
            if (x > y) return isAsc ? 1 : -1;
            return 0;
        }
    });

    // Append sorted rows back to the table
    rows.forEach(function (row, index) {
        // Update the index cell to the new order
        row.cells[0].innerText = index + 1;
        tableBody.appendChild(row);
    });

    tableBody.setAttribute('data-sort', isAsc ? 'desc' : 'asc');
}
