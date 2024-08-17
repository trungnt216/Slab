// header
fetch("header.html")
  .then((response) => response.text())
  .then(
    (data) => (document.getElementById("header-placeholder").innerHTML = data)
  );

//   page-data
const rowsPerPage = 6;
const tablesPerPage = 2;
const mainTable = document.getElementById("main-table");
const tableContainer = document.querySelector(".table-container");
const prevButton = document.getElementById("prev");
const nextButton = document.getElementById("next");
const pageInfo = document.getElementById("page-info");

