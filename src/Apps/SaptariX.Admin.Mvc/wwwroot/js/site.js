(function () {
  const sidebarStateKey = "saptarix.admin.sidebar.state";

  function getSidebar() {
    return document.querySelector(".app-sidebar");
  }

  function getSidebarScrollCandidates() {
    const sidebar = getSidebar();
    if (!sidebar) {
      return [];
    }

    return [
      { name: "wrapper", element: sidebar.querySelector(".sidebar-wrapper") },
      { name: "sidebar", element: sidebar }
    ].filter((candidate) => candidate.element);
  }

  function getScrollContainer(preferredName) {
    const candidates = getSidebarScrollCandidates();
    const preferred = candidates.find((candidate) => candidate.name === preferredName);
    if (preferred) {
      return preferred;
    }

    return candidates.find((candidate) => candidate.element.scrollTop > 0)
      || candidates.find((candidate) => candidate.element.scrollHeight > candidate.element.clientHeight + 1)
      || candidates[0]
      || null;
  }

  function readState() {
    try {
      return JSON.parse(sessionStorage.getItem(sidebarStateKey) || "{}");
    } catch {
      return {};
    }
  }

  function writeState(state) {
    try {
      sessionStorage.setItem(sidebarStateKey, JSON.stringify(state));
    } catch {
      // Storage can be unavailable in strict browser modes; navigation must still work.
    }
  }

  function getElementTopWithinContainer(element, container) {
    return element.getBoundingClientRect().top - container.getBoundingClientRect().top;
  }

  function saveSidebarState(sourceLink) {
    const scrollContainer = getScrollContainer();
    if (!scrollContainer) {
      return;
    }

    const { name, element } = scrollContainer;
    const previousState = readState();
    const clickedHref = sourceLink?.getAttribute("href") || previousState.clickedHref || null;
    const clickedTop = sourceLink
      ? getElementTopWithinContainer(sourceLink, element)
      : Number.isFinite(previousState.clickedTop)
        ? previousState.clickedTop
        : null;
    const state = {
      wrapperTop: document.querySelector(".app-sidebar .sidebar-wrapper")?.scrollTop || 0,
      sidebarTop: getSidebar()?.scrollTop || 0,
      scrollContainer: name,
      clickedHref,
      clickedTop
    };

    writeState(state);
  }

  function findRestoreLink(state) {
    const href = state.clickedHref;
    if (href && href !== "#") {
      const escapedHref = window.CSS?.escape ? CSS.escape(href) : href.replaceAll('"', '\\"');
      const clickedMatch = document.querySelector(`.app-sidebar a.nav-link[href="${escapedHref}"]`);
      if (clickedMatch) {
        return clickedMatch;
      }
    }

    return document.querySelector(".app-sidebar a.nav-link.active");
  }

  function restoreSidebarState() {
    const state = readState();
    const scrollContainer = getScrollContainer(state.scrollContainer);
    if (!scrollContainer) {
      return;
    }

    const { element } = scrollContainer;
    const wrapper = document.querySelector(".app-sidebar .sidebar-wrapper");
    const sidebar = getSidebar();

    if (wrapper && Number.isFinite(state.wrapperTop)) {
      wrapper.scrollTop = state.wrapperTop;
    }

    if (sidebar && Number.isFinite(state.sidebarTop)) {
      sidebar.scrollTop = state.sidebarTop;
    }

    const link = findRestoreLink(state);
    if (link && Number.isFinite(state.clickedTop)) {
      const currentTop = getElementTopWithinContainer(link, element);
      element.scrollTop += currentTop - state.clickedTop;
    }
  }

  function scheduleRestore() {
    restoreSidebarState();
    window.requestAnimationFrame(restoreSidebarState);
    [80, 180, 360, 700].forEach((delay) => window.setTimeout(restoreSidebarState, delay));
  }

  document.addEventListener("DOMContentLoaded", () => {
    scheduleRestore();

    getSidebarScrollCandidates().forEach((candidate) => {
      candidate.element.addEventListener("scroll", () => saveSidebarState(), { passive: true });
    });
  });

  document.addEventListener("click", (event) => {
    const sidebarLink = event.target.closest(".app-sidebar a.nav-link[href]");
    if (sidebarLink) {
      saveSidebarState(sidebarLink);
    }
  });

  window.addEventListener("beforeunload", () => saveSidebarState());
})();
