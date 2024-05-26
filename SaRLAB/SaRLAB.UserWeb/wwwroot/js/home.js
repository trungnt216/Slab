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