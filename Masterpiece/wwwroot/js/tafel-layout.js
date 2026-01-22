document.addEventListener("DOMContentLoaded", () => {
    const grid = document.querySelector(".js-tafel-grid");
    if (!grid) return;

    const editBtn = document.getElementById("editModeBtn");
    const saveBtn = document.getElementById("saveLayoutBtn");
    const resetBtn = document.getElementById("resetLayoutBtn");

    const STORAGE_KEY = "tafel-layout-order";

    let isEditMode = false;
    let dragEl = null;

    // 1. Capture ORIGINAL order as rendered by server
    const originalOrder = getCurrentOrder();

    // 2. Try to apply saved order from localStorage (if any)
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) {
        try {
            const order = JSON.parse(saved);
            applyOrder(order);
        } catch (e) {
            console.warn("Kon opgeslagen layout niet parsen:", e);
        }
    }

    // 3. Toggle edit mode
    if (editBtn) {
        editBtn.addEventListener("click", () => {
            isEditMode = !isEditMode;

            grid.classList.toggle("readonly", !isEditMode);

            if (isEditMode) {
                enableDrag();
                if (saveBtn) saveBtn.style.display = "inline-block";
                if (resetBtn) resetBtn.style.display = "inline-block";
                editBtn.textContent = "Exit edit mode";
            } else {
                disableDrag();
                if (saveBtn) saveBtn.style.display = "none";
                if (resetBtn) resetBtn.style.display = "none";
                editBtn.textContent = "Edit layout";
            }
        });
    }

    // 4. Save button → persist current order to localStorage
    if (saveBtn) {
        saveBtn.addEventListener("click", () => {
            const ids = getCurrentOrder();
            localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
            alert("Layout saved.");
        });
    }

    // 5. Reset button → restore original server order (not saved yet)
    if (resetBtn) {
        resetBtn.addEventListener("click", () => {
            applyOrder(originalOrder);
            // toon save-knop, want er is nu een wijziging t.o.v. huidige layout
            if (saveBtn) saveBtn.style.display = "inline-block";
        });
    }

    // ───────────────────── Helpers ─────────────────────

    function enableDrag() {
        grid.querySelectorAll(".tafel-box").forEach(el => {
            el.draggable = true;

            el.addEventListener("dragstart", dragStart);
            el.addEventListener("dragover", dragOver);
            el.addEventListener("dragend", dragEnd);
        });
    }

    function disableDrag() {
        grid.querySelectorAll(".tafel-box").forEach(el => {
            el.removeAttribute("draggable");

            el.removeEventListener("dragstart", dragStart);
            el.removeEventListener("dragover", dragOver);
            el.removeEventListener("dragend", dragEnd);
        });
    }

    function dragStart(e) {
        dragEl = this;
        e.dataTransfer.effectAllowed = "move";
    }

    function dragOver(e) {
        e.preventDefault();
        const target = e.target.closest(".tafel-box");
        if (!target || target === dragEl) return;

        const items = Array.from(grid.querySelectorAll(".tafel-box"));
        const fromIndex = items.indexOf(dragEl);
        const toIndex = items.indexOf(target);

        if (fromIndex < toIndex) {
            target.after(dragEl);
        } else {
            target.before(dragEl);
        }
    }

    function dragEnd() {
        dragEl = null;
    }

    function getCurrentOrder() {
        return Array.from(grid.querySelectorAll(".tafel-box"))
            .map(el => el.dataset.id);
    }

    function applyOrder(orderIds) {
        const map = new Map();
        grid.querySelectorAll(".tafel-box").forEach(el => {
            map.set(el.dataset.id, el);
        });

        // rebuild grid in given order
        grid.innerHTML = "";

        orderIds.forEach(id => {
            const el = map.get(String(id));
            if (el) {
                grid.appendChild(el);
            }
        });

        // append any tiles not in orderIds (new tables)
        map.forEach((el, id) => {
            if (!orderIds.includes(id)) {
                grid.appendChild(el);
            }
        });
    }
});
