(function () {
  const root = document.querySelector("[data-permissions-root]");
  if (!root) {
    return;
  }

  const tokenInput = root.querySelector('input[name="__RequestVerificationToken"]');
  const token = tokenInput ? tokenInput.value : "";
  const listHost = root.querySelector("[data-permissions-list-host]");
  const listPanel = root.querySelector("[data-permissions-list-panel]");
  const formPanel = root.querySelector("[data-permissions-form-panel]");
  const loadingState = root.querySelector("[data-permissions-loading]");
  const filterForm = root.querySelector("[data-permissions-filter-form]");
  const form = root.querySelector("[data-permissions-form]");
  const formTab = document.getElementById("permissions-form-tab");
  const listTab = document.getElementById("permissions-list-tab");
  const openCreateButton = root.querySelector("[data-permissions-open-create]");
  const backToListButton = root.querySelector("[data-permissions-back-to-list]");
  const submitButton = root.querySelector("[data-permissions-submit]");
  const resetButton = root.querySelector("[data-permissions-reset]");
  const cancelEditButton = root.querySelector("[data-permissions-cancel-edit]");
  const formHeading = root.querySelector("[data-permissions-form-heading]");
  const nameInput = root.querySelector("[data-permissions-name]");
  const alertHost = root.querySelector("[data-permissions-alert]");

  const state = {
    search: "",
    group: "",
    pageNumber: 1,
    pageSize: 10,
    sortColumn: "Name",
    sortDirection: "asc",
    mode: "create"
  };

  function buildUrl(baseUrl, values) {
    const url = new URL(baseUrl, window.location.origin);
    Object.keys(values).forEach((key) => {
      if (values[key] !== undefined && values[key] !== null && values[key] !== "") {
        url.searchParams.set(key, values[key]);
      }
    });
    return url;
  }

  async function loadList() {
    setLoading(true);
    try {
      const url = buildUrl(root.dataset.listUrl, {
        Search: state.search,
        Group: state.group,
        PageNumber: state.pageNumber,
        PageSize: state.pageSize,
        SortColumn: state.sortColumn,
        SortDirection: state.sortDirection
      });
      const response = await fetch(url, { headers: { "X-Requested-With": "fetch" } });
      if (!response.ok) {
        throw new Error("Unable to load permissions.");
      }
      listHost.innerHTML = await response.text();
    } catch (error) {
      showAlert(error.message, "danger");
    } finally {
      setLoading(false);
    }
  }

  function setLoading(isLoading) {
    if (loadingState) {
      loadingState.classList.toggle("d-none", !isLoading);
    }
    listHost.classList.toggle("users-list-busy", isLoading);
  }

  function showAlert(message, tone) {
    if (!alertHost) {
      return;
    }

    alertHost.innerHTML = [
      `<div class="alert alert-${tone} alert-dismissible fade show users-alert" role="alert">`,
      `<span>${escapeHtml(message)}</span>`,
      '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
      "</div>"
    ].join("");
  }

  function escapeHtml(value) {
    return String(value)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function showListPanel() {
    if (listPanel) {
      listPanel.classList.remove("d-none");
    }
    if (formPanel) {
      formPanel.classList.add("d-none");
    }
    if (backToListButton) {
      backToListButton.classList.add("d-none");
    }
    if (openCreateButton) {
      openCreateButton.classList.remove("d-none");
    }
    if (listTab) {
      bootstrap.Tab.getOrCreateInstance(listTab).show();
    }
  }

  function showFormPanel() {
    if (listPanel) {
      listPanel.classList.add("d-none");
    }
    if (formPanel) {
      formPanel.classList.remove("d-none");
    }
    if (backToListButton) {
      backToListButton.classList.remove("d-none");
    }
    if (openCreateButton) {
      openCreateButton.classList.add("d-none");
    }
    if (formTab) {
      bootstrap.Tab.getOrCreateInstance(formTab).show();
    }
  }

  function setCreateMode() {
    state.mode = "create";
    form.reset();
    form.classList.remove("was-validated");
    form.querySelector("[data-permissions-id]").value = "";
    nameInput.disabled = false;
    if (formTab) {
      formTab.textContent = "Create Permission";
    }
    formHeading.textContent = "Create Permission";
    submitButton.textContent = "Create Permission";
    resetButton.classList.remove("d-none");
    cancelEditButton.classList.add("d-none");
  }

  function setEditMode(permission) {
    state.mode = "edit";
    form.classList.remove("was-validated");
    form.querySelector('[name="Id"]').value = permission.id;
    form.querySelector('[name="Name"]').value = permission.name || "";
    form.querySelector('[name="DisplayName"]').value = permission.displayName || "";
    form.querySelector('[name="Group"]').value = permission.group || "";
    form.querySelector('[name="Description"]').value = permission.description || "";
    nameInput.disabled = true;
    if (formTab) {
      formTab.textContent = "Update Permission";
    }
    formHeading.textContent = "Update Permission";
    submitButton.textContent = "Update Permission";
    resetButton.classList.add("d-none");
    cancelEditButton.classList.remove("d-none");
  }

  async function loadPermissionForEdit(permissionId) {
    try {
      const response = await fetch(`${root.dataset.getUrl}/${permissionId}`);
      if (!response.ok) {
        throw new Error("Unable to load permission.");
      }
      const permission = await response.json();
      setEditMode(permission);
      showFormPanel();
    } catch (error) {
      showAlert(error.message, "danger");
    }
  }

  async function submitForm(event) {
    event.preventDefault();
    if (!form.checkValidity()) {
      form.classList.add("was-validated");
      form.reportValidity();
      return;
    }

    const endpoint = state.mode === "edit" ? root.dataset.updateUrl : root.dataset.createUrl;
    const body = new FormData(form);
    try {
      submitButton.disabled = true;
      const response = await fetch(endpoint, {
        method: "POST",
        headers: { RequestVerificationToken: token },
        body
      });
      const result = await response.json();
      if (!response.ok || !result.succeeded) {
        throw new Error(result.message || "Unable to save permission.");
      }

      showAlert(result.message, "success");
      setCreateMode();
      await loadList();
      showListPanel();
    } catch (error) {
      showAlert(error.message, "danger");
    } finally {
      submitButton.disabled = false;
    }
  }

  async function deletePermission(permissionId) {
    if (!window.confirm("Delete this permission?")) {
      return;
    }

    try {
      const response = await fetch(`${root.dataset.deleteUrl}/${permissionId}`, {
        method: "POST",
        headers: { RequestVerificationToken: token }
      });
      const result = await response.json();
      if (!response.ok || !result.succeeded) {
        throw new Error(result.message || "Unable to delete permission.");
      }

      showAlert(result.message, "success");
      await loadList();
    } catch (error) {
      showAlert(error.message, "danger");
    }
  }

  filterForm.addEventListener("submit", (event) => {
    event.preventDefault();
    const data = new FormData(filterForm);
    state.search = data.get("Search") || "";
    state.group = data.get("Group") || "";
    state.pageNumber = 1;
    loadList();
  });

  root.querySelector("[data-permissions-clear-filters]").addEventListener("click", () => {
    filterForm.reset();
    state.search = "";
    state.group = "";
    state.pageNumber = 1;
    loadList();
  });

  root.addEventListener("click", (event) => {
    const sortButton = event.target.closest("[data-permissions-sort]");
    if (sortButton) {
      const column = sortButton.dataset.permissionsSort;
      state.sortDirection = state.sortColumn === column && state.sortDirection === "asc" ? "desc" : "asc";
      state.sortColumn = column;
      state.pageNumber = 1;
      loadList();
      return;
    }

    const pageButton = event.target.closest("[data-permissions-page]");
    if (pageButton && !pageButton.disabled) {
      state.pageNumber = Number(pageButton.dataset.permissionsPage);
      loadList();
      return;
    }

    const actionButton = event.target.closest("[data-permissions-action]");
    if (!actionButton) {
      return;
    }

    const permissionId = actionButton.dataset.permissionId;
    if (actionButton.dataset.permissionsAction === "edit") {
      loadPermissionForEdit(permissionId);
    } else if (actionButton.dataset.permissionsAction === "delete") {
      deletePermission(permissionId);
    }
  });

  if (openCreateButton) {
    openCreateButton.addEventListener("click", () => {
      setCreateMode();
      showFormPanel();
    });
  }

  if (backToListButton) {
    backToListButton.addEventListener("click", () => {
      setCreateMode();
      showListPanel();
    });
  }

  form.addEventListener("submit", submitForm);
  resetButton.addEventListener("click", () => window.setTimeout(() => form.classList.remove("was-validated"), 0));
  cancelEditButton.addEventListener("click", () => {
    setCreateMode();
    showListPanel();
  });
  if (formTab) {
    formTab.addEventListener("click", () => {
      if (state.mode !== "edit") {
        setCreateMode();
      }
    });
  }

  setCreateMode();
  showListPanel();
  loadList();
})();
