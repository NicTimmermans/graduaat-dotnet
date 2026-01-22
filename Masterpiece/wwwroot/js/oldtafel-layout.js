console.log("oldtafel-layout.js loaded");

document.addEventListener("DOMContentLoaded", () => {
    const grid = document.querySelector(".js-tafel-grid");
    if (!grid) return;

    // Only intended for Zaal. If buttons aren't present -> do nothing.
    const editBtn = document.getElementById("editModeBtn");
    const saveBtn = document.getElementById("saveLayoutBtn");
    const resetBtn = document.getElementById("resetLayoutBtn");
    if (!editBtn || !saveBtn || !resetBtn) return;

    const scope = (grid.dataset.layoutScope || "zaal").toLowerCase();
    const STORAGE_KEY = `oldtafel-layout-order-${scope}`;

    let isEditMode = false;
    let dragEl = null;

    const getTiles = () => Array.from(grid.querySelectorAll(".tafel-card"));
    const hasTiles = () => getTiles().length > 0;

    function getCurrentOrder() {
        return getTiles()
            .map(el => String(el.dataset.id || "").trim())
            .filter(id => id.length > 0);
    }

    const originalOrder = getCurrentOrder();

    // Apply saved order (SAFE: never clears if no tiles)
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved && hasTiles()) {
        try {
            const order = JSON.parse(saved);
            applyOrder(order);
        } catch (e) {
            console.warn("Kon opgeslagen layout niet parsen:", e);
        }
    }

    editBtn.addEventListener("click", () => {
        isEditMode = !isEditMode;

        grid.classList.toggle("readonly", !isEditMode);

        if (isEditMode) {
            enableDrag();
            saveBtn.style.display = "inline-block";
            resetBtn.style.display = "inline-block";
            editBtn.textContent = "Exit edit mode";
        } else {
            disableDrag();
            saveBtn.style.display = "none";
            resetBtn.style.display = "none";
            editBtn.textContent = "Edit layout";
        }
    });

    saveBtn.addEventListener("click", () => {
        const ids = getCurrentOrder();
        localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));

        const el = document.getElementById("layoutSavedToast");
        if (el && window.bootstrap?.Toast) {
            const toast = bootstrap.Toast.getOrCreateInstance(el, { delay: 1800 });
            toast.show();
        } else {
            alert("Layout saved ✅"); // fallback
        }
    });


    resetBtn.addEventListener("click", () => {
        applyOrder(originalOrder);
        // Optional: also wipe saved
        localStorage.removeItem(STORAGE_KEY);
    });

    function enableDrag() {
        getTiles().forEach(el => {
            el.draggable = true;
            el.addEventListener("dragstart", dragStart);
            el.addEventListener("dragover", dragOver);
            el.addEventListener("dragend", dragEnd);
        });
    }

    function disableDrag() {
        getTiles().forEach(el => {
            el.removeAttribute("draggable");
            el.removeEventListener("dragstart", dragStart);
            el.removeEventListener("dragover", dragOver);
            el.removeEventListener("dragend", dragEnd);
        });
    }

    function dragStart(e) {
        // don't drag when clicking action buttons/links (or any interactive element)
        const interactive = e.target.closest("a, button, input, select, textarea, label");
        if (interactive) {
            e.preventDefault();
            return;
        }

        dragEl = this;
        e.dataTransfer.effectAllowed = "move";
    }

    function dragOver(e) {
        e.preventDefault();

        const target = e.target.closest(".tafel-card");
        if (!target || target === dragEl) return;

        const items = getTiles();
        const fromIndex = items.indexOf(dragEl);
        const toIndex = items.indexOf(target);

        if (fromIndex < 0 || toIndex < 0) return;

        if (fromIndex < toIndex) target.after(dragEl);
        else target.before(dragEl);
    }

    function dragEnd() {
        dragEl = null;
    }

    function applyOrder(orderIds) {
        if (!Array.isArray(orderIds)) return;
        if (!hasTiles()) return;

        const tiles = getTiles();

        // Map by data-id
        const map = new Map();
        tiles.forEach(el => {
            const id = String(el.dataset.id || "").trim();
            if (id) map.set(id, el);
        });

        // If map is empty, DO NOT clear grid (prevents the "zaal empty" bug)
        if (map.size === 0) return;

        // Clear & rebuild safely
        grid.innerHTML = "";

        // Append in requested order
        orderIds.map(String).forEach(id => {
            const el = map.get(String(id));
            if (el) grid.appendChild(el);
        });

        // Append any new/missing tables that weren't in saved order
        map.forEach((el, id) => {
            if (!orderIds.map(String).includes(String(id))) {
                grid.appendChild(el);
            }
        });
    }
});
