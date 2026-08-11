document.addEventListener('DOMContentLoaded', function () {
    // --- Breadcrumb logic (page-based) ---
    const breadcrumbCurrent = document.getElementById('breadcrumbCurrent');
    const path = window.location.pathname.split('/').pop().replace('.aspx', '');
    if (breadcrumbCurrent && path) {
        breadcrumbCurrent.textContent = path.charAt(0).toUpperCase() + path.slice(1);
    }

    // --- Notification demo ---
    const notifBadge = document.getElementById('notifBadge');
    const notifList = document.getElementById('notifList');
    const demoNotifs = [
        { text: 'Low stock: Item A (2 left)', time: '5m' },
        { text: 'Invoice INV-20251029 processed', time: '1h' }
    ];
    if (notifList) {
        notifList.innerHTML = demoNotifs.map(n =>
            `<div class="dropdown-item small py-1"><strong>${n.text}</strong><div class="text-muted">${n.time} ago</div></div>`
        ).join('');
        notifBadge.textContent = demoNotifs.length;
        notifBadge.classList.remove('d-none');
    }

    // --- Theme toggle ---
    const btnDark = document.getElementById('btnDarkMode');
    const body = document.body;
    const savedTheme = localStorage.getItem('akuraTheme') || 'light';
    if (savedTheme === 'dark') body.classList.add('dark-mode');

    btnDark.addEventListener('click', () => {
        body.classList.toggle('dark-mode');
        localStorage.setItem('akuraTheme', body.classList.contains('dark-mode') ? 'dark' : 'light');
        btnDark.innerHTML = body.classList.contains('dark-mode')
            ? '<i class="fa-solid fa-sun"></i>' : '<i class="fa-solid fa-moon"></i>';
    });

    // --- JS demo check (to confirm link works) ---
    window.demoCheck = () => {
        alert('JS connected ✔');
    };
});

$(function () {
    // Sidebar toggle only, no submenu text hiding
    const savedSidebar = localStorage.getItem("akuraSidebar") || "expanded";
    $("body").toggleClass("sidebar-collapsed", savedSidebar === "collapsed");

    $("#sidebarToggle").click(function () {
        $("body").toggleClass("sidebar-collapsed");
        localStorage.setItem("akuraSidebar", $("body").hasClass("sidebar-collapsed") ? "collapsed" : "expanded");
    });
});