// header
fetch("header.html")
  .then((response) => response.text())
  .then(
    (data) => (document.getElementById("header-placeholder").innerHTML = data)
  );

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
