// header
fetch("header.html")
  .then((response) => response.text())
  .then((data) => {
    document.getElementById("header-placeholder").innerHTML = data;

    const menuBtn = document.getElementById("menu-btn");
    if (menuBtn) {
      const menuHead = document.querySelector(".menu_head");
      const header = document.querySelector("header");
      const olay = document.getElementById("olay");

      menuBtn.addEventListener("click", () => {
        menuHead.classList.toggle("active");
        olay.classList.toggle("active");
      });

      olay.addEventListener("click", () => {
        menuHead.classList.remove("active");
        olay.classList.remove("active");
      });
    } else {
      console.error("Phần tử menuBtn không tồn tại trong DOM.");
    }
  })
  .catch((error) => console.error("Lỗi khi tải header:", error));

//   page-data
const rowsPerPage = 6;
const tablesPerPage = 2;
const mainTable = document.getElementById("main-table");
const tableContainer = document.querySelector(".table-container");
const prevButton = document.getElementById("prev");
const nextButton = document.getElementById("next");
const pageInfo = document.getElementById("page-info");
