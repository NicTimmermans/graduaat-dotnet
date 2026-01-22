document.addEventListener("DOMContentLoaded", function () {
    const toggles = document.querySelectorAll(".account-toggle, .admin-toggle");

    function closeAll() {
        toggles.forEach(t => {
            const id = t.getAttribute("aria-controls");
            if (!id) return;
            const panel = document.getElementById(id);
            if (panel) panel.classList.remove("show");
        });
    }

    toggles.forEach(toggle => {
        const panelId = toggle.getAttribute("aria-controls");
        if (!panelId) return;

        const panel = document.getElementById(panelId);
        if (!panel) return;

        toggle.addEventListener("click", function (e) {
            e.stopPropagation();
            const willShow = !panel.classList.contains("show");
            closeAll();
            if (willShow) panel.classList.add("show");
        });

        panel.addEventListener("click", e => e.stopPropagation());
    });

    document.addEventListener("click", closeAll);
});

document.addEventListener('DOMContentLoaded', function () {
    const nav = document.querySelector('.masterpiece-main-navbar');
    if (!nav) return;

    function updateNavState() {
        if (window.scrollY <= 10) {
            nav.classList.add('nav-at-top');
        } else {
            nav.classList.remove('nav-at-top');
        }
    }

    updateNavState();
    window.addEventListener('scroll', updateNavState);
});

document.addEventListener('DOMContentLoaded', function () {

    const triggers = document.querySelectorAll('.js-logout-trigger');
    const overlay = document.getElementById('logout-overlay');
    const modal = document.getElementById('logout-modal');
    const btnCancel = document.getElementById('logout-cancel');
    const btnConfirm = document.getElementById('logout-confirm');

    function showLogoutModal() {
        overlay.classList.remove('d-none');
        modal.classList.remove('d-none');
    }

    function hideLogoutModal() {
        overlay.classList.add('d-none');
        modal.classList.add('d-none');
    }

    triggers.forEach(trigger => {
        trigger.addEventListener('click', function (e) {
            e.preventDefault();
            showLogoutModal();
        });
    });

    overlay.addEventListener('click', hideLogoutModal);
    btnCancel.addEventListener('click', hideLogoutModal);

    btnConfirm.addEventListener('click', function () {
        const url = btnConfirm.getAttribute('data-logout-url');
        window.location.href = url;
    });

});