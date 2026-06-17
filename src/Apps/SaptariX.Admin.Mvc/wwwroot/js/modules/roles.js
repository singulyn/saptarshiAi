(function () {
  const root = document.querySelector("[data-roles-root]");
  if (!root) {
    return;
  }

  const tokenInput = root.querySelector('input[name="__RequestVerificationToken"]');
  const token = tokenInput ? tokenInput.value : "";
  const listHost = root.querySelector("[data-roles-list-host]");
  const listPanel = root.querySelector("[data-roles-list-panel]");
  const formPanel = root.querySelector("[data-roles-form-panel]");
  const loadingState = root.querySelector("[data-roles-loading]");
  const filterForm = root.querySelector("[data-roles-filter-form]");
  const form = root.querySelector("[data-roles-form]");
  const formTab = document.getElementById("roles-form-tab");
  const listTab = document.getElementById("roles-list-tab");
  const openCreateButton = root.querySelector("[data-roles-open-create]");
  const backToListButton = root.querySelector("[data-roles-back-to-list]");
  const submitButton = root.querySelector("[data-roles-submit]");
  const resetButton = root.querySelector("[data-roles-reset]");
  const cancelEditButton = root.querySelector("[data-roles-cancel-edit]");
  const formHeading = root.querySelector("[data-roles-form-heading]");
  const nameInput = root.querySelector("[data-roles-name]");
  const deleteModalElement = document.getElementById("roleDeleteModal");
  const confirmDeleteButton = root.querySelector("[data-roles-confirm-delete]");
  const permissionsHost = root.querySelector("[data-role-permissions-host]");
  const alertHost = root.querySelector("[data-roles-alert]");

  const state = {
    search: "",
    status: "",
    pageNumber: 1,
    pageSize: 10,
    sortColumn: "Name",
    sortDirection: "asc",
    mode: "create",
    pendingDeleteId: null
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
        Status: state.status,
        PageNumber: state.pageNumber,
        PageSize: state.pageSize,
        SortColumn: state.sortColumn,
        SortDirection: state.sortDirection
      });
      const response = await fetch(url, { headers: { "X-Requested-With": "fetch" } });
      if (!response.ok) {
        throw new Error("Unable to load roles.");
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
    form.querySelector("[data-roles-id]").value = "";
    nameInput.disabled = false;
    if (formTab) {
      formTab.textContent = "Create Role";
    }
    formHeading.textContent = "Create Role";
    submitButton.textContent = "Create Role";
    resetButton.classList.remove("d-none");
    cancelEditButton.classList.add("d-none");
  }

  function setSelectValue(name, value) {
    const field = form.querySelector(`[name="${name}"]`);
    if (!field) {
      return;
    }

    field.value = value;
    field.dispatchEvent(new Event("change", { bubbles: true }));
  }

  function setEditMode(role) {
    state.mode = "edit";
    form.classList.remove("was-validated");
    form.querySelector('[name="Id"]').value = role.id;
    form.querySelector('[name="Name"]').value = role.name || "";
    form.querySelector('[name="DisplayName"]').value = role.displayName || "";
    form.querySelector('[name="Description"]').value = role.description || "";
    setSelectValue("Status", role.status || "Active");
    nameInput.disabled = true;
    if (formTab) {
      formTab.textContent = "Update Role";
    }
    formHeading.textContent = "Update Role";
    submitButton.textContent = "Update Role";
    resetButton.classList.add("d-none");
    cancelEditButton.classList.remove("d-none");
  }

  async function loadRoleForEdit(roleId) {
    try {
      const response = await fetch(`${root.dataset.getUrl}/${roleId}`);
      if (!response.ok) {
        throw new Error("Unable to load role.");
      }
      const role = await response.json();
      setEditMode(role);
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
        throw new Error(result.message || "Unable to save role.");
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

  function openDeleteModal(roleId) {
    state.pendingDeleteId = roleId;
    bootstrap.Modal.getOrCreateInstance(deleteModalElement).show();
  }

  async function softDeleteRole() {
    if (!state.pendingDeleteId) {
      return;
    }

    try {
      confirmDeleteButton.disabled = true;
      const response = await fetch(`${root.dataset.deleteUrl}/${state.pendingDeleteId}`, {
        method: "POST",
        headers: { RequestVerificationToken: token }
      });
      const result = await response.json();
      if (!response.ok || !result.succeeded) {
        throw new Error(result.message || "Unable to delete role.");
      }

      bootstrap.Modal.getOrCreateInstance(deleteModalElement).hide();
      state.pendingDeleteId = null;
      showAlert(result.message, "success");
      await loadList();
    } catch (error) {
      showAlert(error.message, "danger");
    } finally {
      confirmDeleteButton.disabled = false;
    }
  }

  async function openPermissions(roleId) {
    renderPermissionLoading(roleId);
    showPermissionDrawer();
    try {
      const response = await fetch(`${root.dataset.permissionsUrl}/${roleId}`);
      if (!response.ok) {
        throw new Error("Unable to load role permissions.");
      }
      permissionsHost.innerHTML = await response.text();
      showPermissionDrawer();
      updateGroupSelectAllStates();
    } catch (error) {
      showAlert(error.message, "danger");
    }
  }

  function renderPermissionLoading(roleId) {
    permissionsHost.innerHTML = [
      `<div class="offcanvas offcanvas-end user-permission-drawer" tabindex="-1" id="rolePermissionsDrawer" data-role-permissions-drawer data-role-id="${roleId}">`,
      '<div class="offcanvas-header user-permission-header">',
      '<div><p class="section-kicker mb-1">Access Control</p><h2 class="offcanvas-title">Role Permissions</h2></div>',
      '<button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>',
      "</div>",
      '<div class="offcanvas-body user-permission-body">',
      '<div class="users-loading-state"><div class="spinner-border spinner-border-sm text-primary" role="status" aria-hidden="true"></div><span>Loading permissions</span></div>',
      "</div>",
      '<div class="user-permission-footer"><button type="button" class="btn btn-primary btn-sm" disabled>Save Permissions</button><button type="button" class="btn btn-outline-secondary btn-sm" data-bs-dismiss="offcanvas">Cancel</button></div>',
      "</div>"
    ].join("");
  }

  function showPermissionDrawer() {
    const drawer = document.getElementById("rolePermissionsDrawer");
    if (drawer) {
      bootstrap.Offcanvas.getOrCreateInstance(drawer).show();
    }
  }

  function filterPermissions(value) {
    const query = value.trim().toLowerCase();
    permissionsHost.querySelectorAll("[data-role-permission-row]").forEach((row) => {
      const permissionName = row.querySelector("[data-role-permission-checkbox]").dataset.permissionName.toLowerCase();
      row.classList.toggle("d-none", query.length > 0 && !permissionName.includes(query));
    });
  }

  function updateGroupSelectAllStates() {
    permissionsHost.querySelectorAll("[data-role-permission-group]").forEach((group) => {
      const checkboxes = Array.from(group.querySelectorAll("[data-role-permission-checkbox]"));
      const selectAll = group.querySelector("[data-role-permission-select-group]");
      if (!selectAll || checkboxes.length === 0) {
        return;
      }
      const checkedCount = checkboxes.filter((checkbox) => checkbox.checked).length;
      selectAll.checked = checkedCount === checkboxes.length;
      selectAll.indeterminate = checkedCount > 0 && checkedCount < checkboxes.length;
    });
  }

  async function savePermissions() {
    const drawer = document.getElementById("rolePermissionsDrawer");
    const saveButton = permissionsHost.querySelector("[data-role-permission-save]");
    if (!drawer || !saveButton) {
      return;
    }

    const permissions = Array.from(permissionsHost.querySelectorAll("[data-role-permission-checkbox]")).map((checkbox) => ({
      permissionName: checkbox.dataset.permissionName,
      isGranted: checkbox.checked
    }));

    try {
      saveButton.disabled = true;
      const response = await fetch(root.dataset.savePermissionsUrl, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: token
        },
        body: JSON.stringify({
          roleId: drawer.dataset.roleId,
          organizationId: root.dataset.organizationId,
          permissions
        })
      });
      const result = await response.json();
      if (!response.ok || !result.succeeded) {
        throw new Error(result.message || "Unable to save role permissions.");
      }

      showAlert(result.message, "success");
      bootstrap.Offcanvas.getOrCreateInstance(drawer).hide();
      await loadList();
    } catch (error) {
      showAlert(error.message, "danger");
    } finally {
      saveButton.disabled = false;
    }
  }

  filterForm.addEventListener("submit", (event) => {
    event.preventDefault();
    const data = new FormData(filterForm);
    state.search = data.get("Search") || "";
    state.status = data.get("Status") || "";
    state.pageNumber = 1;
    loadList();
  });

  root.querySelector("[data-roles-clear-filters]").addEventListener("click", () => {
    filterForm.reset();
    state.search = "";
    state.status = "";
    state.pageNumber = 1;
    loadList();
  });

  root.addEventListener("click", (event) => {
    const sortButton = event.target.closest("[data-roles-sort]");
    if (sortButton) {
      const column = sortButton.dataset.rolesSort;
      state.sortDirection = state.sortColumn === column && state.sortDirection === "asc" ? "desc" : "asc";
      state.sortColumn = column;
      state.pageNumber = 1;
      loadList();
      return;
    }

    const pageButton = event.target.closest("[data-roles-page]");
    if (pageButton && !pageButton.disabled) {
      state.pageNumber = Number(pageButton.dataset.rolesPage);
      loadList();
      return;
    }

    const actionButton = event.target.closest("[data-roles-action]");
    if (!actionButton) {
      return;
    }

    const roleId = actionButton.dataset.roleId;
    if (actionButton.dataset.rolesAction === "edit") {
      loadRoleForEdit(roleId);
    } else if (actionButton.dataset.rolesAction === "delete") {
      openDeleteModal(roleId);
    } else if (actionButton.dataset.rolesAction === "permissions") {
      openPermissions(roleId);
    }
  });

  permissionsHost.addEventListener("input", (event) => {
    if (event.target.matches("[data-role-permission-search]")) {
      filterPermissions(event.target.value);
    }
  });

  permissionsHost.addEventListener("change", (event) => {
    if (event.target.matches("[data-role-permission-select-group]")) {
      const group = event.target.closest("[data-role-permission-group]");
      group.querySelectorAll("[data-role-permission-checkbox]").forEach((checkbox) => {
        checkbox.checked = event.target.checked;
      });
    }

    if (event.target.matches("[data-role-permission-checkbox]")) {
      updateGroupSelectAllStates();
    }
  });

  permissionsHost.addEventListener("click", (event) => {
    if (event.target.closest("[data-role-permission-save]")) {
      savePermissions();
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
  confirmDeleteButton.addEventListener("click", softDeleteRole);

  setCreateMode();
  showListPanel();
  loadList();
})();
