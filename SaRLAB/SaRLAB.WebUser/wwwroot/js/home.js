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
document.querySelectorAll('.dropdown > a').forEach(menu => {
    menu.addEventListener('click', function (e) {
        e.preventDefault();
        let submenu = this.nextElementSibling;
        if (submenu.style.display === 'block') {
            submenu.style.display = 'none';
        } else {
            submenu.style.display = 'block';
        }
    });
});

document.addEventListener('click', function (e) {
    let isClickInside = document.querySelector('.nav').contains(e.target);
    if (!isClickInside) {
        document.querySelectorAll('.dropdown-menu').forEach(submenu => {
            submenu.style.display = 'none';
        });
    }
});

var deleteUrl = '';
function openDeleteModal(element) {
    deleteUrl = element.getAttribute("data-url");
    document.getElementById('deleteModal').style.display = 'block';
}

function closeDeleteModal() {
    document.getElementById('deleteModal').style.display = 'none';
}

document.getElementById('confirmDeleteButton').onclick = function () {
    window.location.href = deleteUrl;
};

window.onclick = function (event) {
    var modal = document.getElementById('deleteModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const overlay = document.createElement('div');
    overlay.classList.add('overlay');
    document.body.appendChild(overlay);
}