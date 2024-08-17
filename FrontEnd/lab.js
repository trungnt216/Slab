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


// lab
const itemsPerPage = 4;
let currentPage = 1;

function showPage(page) {
  const items = document.querySelectorAll(".lab a");
  const totalPages = Math.ceil(items.length / itemsPerPage);
  if (page < 1) page = 1;
  if (page > totalPages) page = totalPages;

  items.forEach((item, index) => {
    item.style.display = "none";
    if (index >= (page - 1) * itemsPerPage && index < page * itemsPerPage) {
      item.style.display = "block";
    }
  });

  document.getElementById("page-num").textContent = `Page ${page}`;
  currentPage = page;
  updatePagination(totalPages);
}

function nextPage() {
  showPage(currentPage + 1);
}

function prevPage() {
  showPage(currentPage - 1);
}

function updatePagination(totalPages) {
  const paginationButtons = document.getElementById("pagination-buttons");
  paginationButtons.innerHTML = "";

  for (let i = 1; i <= totalPages; i++) {
    const btn = document.createElement("button");
    btn.textContent = i;
    btn.classList.add("page-btn");
    if (i === currentPage) {
      btn.classList.add("active");
    }
    btn.addEventListener("click", () => showPage(i));
    paginationButtons.appendChild(btn);
  }
}

showPage(currentPage);
