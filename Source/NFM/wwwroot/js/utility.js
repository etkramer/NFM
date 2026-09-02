HTMLElement.blur = (obj) => obj.blur();
HTMLElement.setPointerCapture = (obj, pointerId) => obj.setPointerCapture(pointerId);
HTMLElement.releasePointerCapture = (obj, pointerId) => obj.releasePointerCapture(pointerId);

HTMLElement.getValue = (obj) => obj.value;
HTMLElement.setValue = (obj, val) => (obj.value = val);

HTMLElement.getClientWidth = (obj) => obj.clientWidth;
HTMLElement.getClientHeight = (obj) => obj.clientHeight;

HTMLElement.getBounds = (obj) => {
    const rect = obj.getBoundingClientRect();
    return { x: rect.x, y: rect.y, w: rect.width, h: rect.height, viewportHeight: window.innerHeight };
};

HTMLElement.getStyle = (obj, prop) => obj.style[prop];
HTMLElement.setStyle = (obj, prop, value) => (obj.style[prop] = value);

HTMLElement.getChild = (obj, idx) => obj.children[idx];

var setBodyCursor = (value) => (document.body.style.cursor = value);
var blurActiveElement = () => document.activeElement?.blur();

function addResizeCallback(obj, callbackObj, callbackName) {
    new ResizeObserver(() => callbackObj.invokeMethodAsync(callbackName)).observe(obj);
}

/*
 * Viewports are holes in the page - the host composites a swapchain behind each one, and needs to
 * know where they are. Nothing observes both position and size, so the rects are polled per frame
 * and only reported when they actually move.
 */
const viewports = new Map();

function registerViewport(element, id) {
    const entry = { element, rect: null };
    viewports.set(id, entry);

    // Moves rather than enters - the pointer is often already inside by the time a viewport registers.
    element.addEventListener("pointermove", () => postHover(id));
    element.addEventListener("pointerleave", () => postHover(null));

    if (viewports.size === 1) {
        requestAnimationFrame(pollViewports);
    }
}

function unregisterViewport(id) {
    if (viewports.delete(id)) {
        if (hoveredViewport === id) {
            hoveredViewport = null;
        }

        chrome.webview.postMessage({ kind: "viewportRemoved", id });
    }
}

let hoveredViewport = null;

function postHover(id) {
    if (id === hoveredViewport) {
        return;
    }

    hoveredViewport = id;
    chrome.webview.postMessage({ kind: "viewportHover", id });
}

function pollViewports() {
    for (const [id, entry] of viewports) {
        const bounds = entry.element.getBoundingClientRect();
        const rect = {
            x: Math.round(bounds.x),
            y: Math.round(bounds.y),
            w: Math.round(bounds.width),
            h: Math.round(bounds.height),
        };

        if (
            entry.rect === null ||
            entry.rect.x !== rect.x ||
            entry.rect.y !== rect.y ||
            entry.rect.w !== rect.w ||
            entry.rect.h !== rect.h
        ) {
            entry.rect = rect;
            chrome.webview.postMessage({ kind: "viewportRect", id, ...rect });
        }
    }

    if (viewports.size > 0) {
        requestAnimationFrame(pollViewports);
    }
}

/*
 * The engine reads the keyboard through the page rather than the window, so that focused text
 * fields keep their input instead of also driving the camera.
 */
function isEditable(target) {
    return (
        target instanceof HTMLInputElement ||
        target instanceof HTMLTextAreaElement ||
        target?.isContentEditable === true
    );
}

function postKey(event, down) {
    if (isEditable(event.target) || event.repeat) {
        return;
    }

    chrome.webview.postMessage({ kind: "key", code: event.code, down });
}

document.addEventListener("keydown", (e) => postKey(e, true));
document.addEventListener("keyup", (e) => postKey(e, false));
