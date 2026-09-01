/*
 * The dock frame (splits, tab strips, empty content slots) is rendered by Blazor; the panels
 * themselves live in one flat, absolutely-positioned stack that never changes shape. This file
 * keeps each panel aligned to its slot, and owns both drags so that resizing and re-docking
 * never round-trip through a render.
 */

const EDGE_FRACTION = 0.25;
const CARET_WIDTH = 2;
const MIN_RATIO = 0.05;
const DRAG_THRESHOLD = 4;

let root = null;
let host = null;
let overlay = null;
let frameHandle = 0;

const placements = new Map();

export function init(rootElement, hostRef) {
    root = rootElement;
    host = hostRef;
    overlay = root.querySelector(".dock-overlay");

    root.addEventListener("pointerdown", onPointerDown);
    frameHandle = requestAnimationFrame(sync);
}

export function dispose() {
    cancelAnimationFrame(frameHandle);
    root?.removeEventListener("pointerdown", onPointerDown);

    placements.clear();
    root = host = overlay = null;
}

/*
 * Panels are positioned from their slot's measured rect, so a tab can move anywhere in the tree
 * without Blazor ever unmounting the component. Nothing observes both position and size, so rects
 * are polled and only written back when they actually move.
 */
function sync() {
    frameHandle = requestAnimationFrame(sync);

    if (root === null) {
        return;
    }

    const origin = root.getBoundingClientRect();
    const slots = new Map();

    for (const slot of root.querySelectorAll(".dock-slot")) {
        slots.set(slot.dataset.slot, slot);
    }

    for (const panel of root.querySelectorAll(".dock-panel")) {
        const id = panel.dataset.tab;
        const slot = slots.get(id);

        // An unplaced panel is display:none, so a hidden viewport measures as an empty rect and
        // the host stops compositing a swapchain behind it.
        if (slot === undefined) {
            if (placements.get(id) !== null) {
                placements.set(id, null);
                panel.style.display = "none";
            }

            continue;
        }

        const bounds = slot.getBoundingClientRect();
        const rect = {
            x: Math.round(bounds.x - origin.x),
            y: Math.round(bounds.y - origin.y),
            w: Math.round(bounds.width),
            h: Math.round(bounds.height),
        };

        const previous = placements.get(id);
        if (
            previous &&
            previous.x === rect.x &&
            previous.y === rect.y &&
            previous.w === rect.w &&
            previous.h === rect.h
        ) {
            continue;
        }

        placements.set(id, rect);
        panel.style.display = "flex";
        panel.style.left = `${rect.x}px`;
        panel.style.top = `${rect.y}px`;
        panel.style.width = `${rect.w}px`;
        panel.style.height = `${rect.h}px`;
    }
}

function onPointerDown(event) {
    // Middle click closes a tab, and would otherwise start Chromium's autoscroll.
    if (event.button === 1) {
        const tab = event.target.closest(".dock-tab");

        if (tab !== null) {
            event.preventDefault();
            host.invokeMethodAsync("OnClosed", tab.dataset.tab);
        }

        return;
    }

    if (event.button !== 0) {
        return;
    }

    const handle = event.target.closest(".dock-handle");
    if (handle !== null) {
        beginResize(event, handle);
        return;
    }

    const tab = event.target.closest(".dock-tab");
    if (tab !== null) {
        beginTabDrag(event, tab);
    }
}

/*
 * The ratio lives in a custom property the frame's flex rules read, so a resize touches one style
 * declaration per frame and never re-renders.
 */
function beginResize(event, handle) {
    const split = handle.parentElement;
    const isHorizontal = split.classList.contains("horizontal");
    const bounds = split.getBoundingClientRect();
    const total =
        (isHorizontal ? bounds.width : bounds.height) -
        (isHorizontal ? handle.offsetWidth : handle.offsetHeight);

    if (total <= 0) {
        return;
    }

    const origin = isHorizontal ? bounds.x : bounds.y;
    let ratio = parseFloat(split.style.getPropertyValue("--ratio"));

    const onMove = (move) => {
        const position = (isHorizontal ? move.clientX : move.clientY) - origin;
        ratio = clamp(position / total, MIN_RATIO, 1 - MIN_RATIO);
        split.style.setProperty("--ratio", ratio);
    };

    const onUp = () => {
        handle.removeEventListener("pointermove", onMove);
        handle.removeEventListener("pointerup", onUp);
        handle.removeEventListener("lostpointercapture", onUp);
        document.body.style.cursor = "";

        host.invokeMethodAsync("OnResized", split.dataset.node, ratio);
    };

    handle.setPointerCapture(event.pointerId);
    handle.addEventListener("pointermove", onMove);
    handle.addEventListener("pointerup", onUp);
    handle.addEventListener("lostpointercapture", onUp);
    document.body.style.cursor = isHorizontal ? "ew-resize" : "ns-resize";

    event.preventDefault();
}

function beginTabDrag(event, tab) {
    const start = { x: event.clientX, y: event.clientY };
    let ghost = null;
    let target = null;

    const onMove = (move) => {
        if (ghost === null) {
            if (Math.hypot(move.clientX - start.x, move.clientY - start.y) < DRAG_THRESHOLD) {
                return;
            }

            ghost = createGhost(tab);
        }

        ghost.style.transform = `translate(${move.clientX + 12}px, ${move.clientY + 12}px)`;

        target = hitTest(move.clientX, move.clientY);
        drawOverlay(target);
    };

    const onUp = () => {
        tab.removeEventListener("pointermove", onMove);
        tab.removeEventListener("pointerup", onUp);
        tab.removeEventListener("lostpointercapture", onUp);

        ghost?.remove();
        overlay.style.display = "none";

        if (ghost !== null && target !== null) {
            host.invokeMethodAsync(
                "OnDropped",
                tab.dataset.tab,
                target.node,
                target.zone,
                target.index
            );
        }
    };

    tab.setPointerCapture(event.pointerId);
    tab.addEventListener("pointermove", onMove);
    tab.addEventListener("pointerup", onUp);
    tab.addEventListener("lostpointercapture", onUp);
}

function createGhost(tab) {
    const ghost = document.createElement("div");
    ghost.className = "dock-ghost";
    ghost.textContent = tab.querySelector(".dock-tab-name").textContent;
    root.appendChild(ghost);

    return ghost;
}

/*
 * A drop over a tab strip inserts between tabs; anywhere else the group's rect is split into a
 * center region and four edge bands.
 */
function hitTest(x, y) {
    const strip = elementAt(x, y, ".dock-tab-strip");
    if (strip !== null) {
        return {
            node: strip.dataset.node,
            zone: "Center",
            index: insertionIndex(strip, x),
            strip: true,
        };
    }

    const group = elementAt(x, y, ".dock-group");
    if (group === null) {
        return null;
    }

    const bounds = group.querySelector(".dock-slot").getBoundingClientRect();
    const u = (x - bounds.x) / bounds.width;
    const v = (y - bounds.y) / bounds.height;

    // The nearest edge wins, so the bands meet along the rect's diagonals.
    const distances = [
        { zone: "Left", value: u },
        { zone: "Right", value: 1 - u },
        { zone: "Top", value: v },
        { zone: "Bottom", value: 1 - v },
    ];

    const nearest = distances.reduce((a, b) => (b.value < a.value ? b : a));
    const zone = nearest.value < EDGE_FRACTION ? nearest.zone : "Center";

    return { node: group.dataset.node, zone, index: -1, strip: false };
}

function elementAt(x, y, selector) {
    for (const element of root.querySelectorAll(selector)) {
        const bounds = element.getBoundingClientRect();

        if (x >= bounds.x && x <= bounds.right && y >= bounds.y && y <= bounds.bottom) {
            return element;
        }
    }

    return null;
}

function insertionIndex(strip, x) {
    const tabs = [...strip.querySelectorAll(".dock-tab")];

    for (let i = 0; i < tabs.length; i++) {
        const bounds = tabs[i].getBoundingClientRect();

        if (x < bounds.x + bounds.width / 2) {
            return i;
        }
    }

    return tabs.length;
}

function drawOverlay(target) {
    if (target === null) {
        overlay.style.display = "none";
        return;
    }

    const group = root.querySelector(`.dock-group[data-node="${target.node}"]`);
    const origin = root.getBoundingClientRect();

    // Over a tab strip the drop is an insertion, so the caret marks where the tab would land.
    if (target.strip) {
        const strip = group.querySelector(".dock-tab-strip");
        const bounds = strip.getBoundingClientRect();
        const tabs = [...strip.querySelectorAll(".dock-tab")];

        const edge =
            target.index < tabs.length
                ? tabs[target.index].getBoundingClientRect().x
                : tabs.at(-1)?.getBoundingClientRect().right ?? bounds.x;

        place(
            edge - origin.x - CARET_WIDTH / 2,
            bounds.y - origin.y,
            CARET_WIDTH,
            bounds.height,
            true
        );

        return;
    }

    const bounds = group.querySelector(".dock-slot").getBoundingClientRect();
    let { x, y, width, height } = {
        x: bounds.x - origin.x,
        y: bounds.y - origin.y,
        width: bounds.width,
        height: bounds.height,
    };

    switch (target.zone) {
        case "Left":
            width /= 2;
            break;
        case "Right":
            width /= 2;
            x += width;
            break;
        case "Top":
            height /= 2;
            break;
        case "Bottom":
            height /= 2;
            y += height;
            break;
    }

    place(x, y, width, height, false);
}

function place(x, y, width, height, isCaret) {
    overlay.style.display = "block";
    overlay.classList.toggle("caret", isCaret);
    overlay.style.transform = `translate(${x}px, ${y}px)`;
    overlay.style.width = `${width}px`;
    overlay.style.height = `${height}px`;
}

function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max);
}
