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

// page
const books = document.querySelector(".books");
const paginationContainer = document.querySelector(".pagination");
const itemsPerPage = 9;
let currentPage = 1;

function showPage(page) {
  const items = document.querySelectorAll(".book-item");
  const totalItems = items.length;
  const totalPages = Math.ceil(totalItems / itemsPerPage);

  if (page < 1) page = 1;
  if (page > totalPages) page = totalPages;

  const start = (page - 1) * itemsPerPage;
  const end = start + itemsPerPage;

  items.forEach((item, index) => {
    item.style.display = index >= start && index < end ? "block" : "none";
  });

  currentPage = page;
  updatePagination(totalPages);
}

function updatePagination(totalPages) {
  paginationContainer.innerHTML = "";

  if (totalPages > 1) {
    // Previous Button
    const prevButton = document.createElement("a");
    prevButton.href = "#";
    prevButton.textContent = "«";
    prevButton.classList.add("prev");
    prevButton.classList.toggle("disabled", currentPage === 1);
    paginationContainer.appendChild(prevButton);

    // Page Numbers
    for (let i = 1; i <= totalPages; i++) {
      const pageLink = document.createElement("a");
      pageLink.href = "#";
      pageLink.textContent = i;
      pageLink.classList.add("page");
      if (i === currentPage) {
        pageLink.classList.add("disabled");
      }
      paginationContainer.appendChild(pageLink);
    }

    // Next Button
    const nextButton = document.createElement("a");
    nextButton.href = "#";
    nextButton.textContent = "»";
    nextButton.classList.add("next");
    nextButton.classList.toggle("disabled", currentPage === totalPages);
    paginationContainer.appendChild(nextButton);
  }
}

paginationContainer.addEventListener("click", (event) => {
  if (event.target.classList.contains("page")) {
    showPage(parseInt(event.target.textContent));
  } else if (event.target.classList.contains("prev")) {
    showPage(currentPage - 1);
  } else if (event.target.classList.contains("next")) {
    showPage(currentPage + 1);
  }
});

showPage(currentPage);
