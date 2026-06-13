(function () {
  const root = document.querySelector("[data-users-root]");
  if (!root) {
    return;
  }

  const tokenInput = root.querySelector('input[name="__RequestVerificationToken"]');
  const token = tokenInput ? tokenInput.value : "";
  const listHost = root.querySelector("[data-users-list-host]");
  const loadingState = root.querySelector("[data-users-loading]");
  const filterForm = root.querySelector("[data-users-filter-form]");
  const form = root.querySelector("[data-users-form]");
  const formTab = document.getElementById("users-form-tab");
  const listTab = document.getElementById("users-list-tab");
  const submitButton = root.querySelector("[data-users-submit]");
  const resetButton = root.querySelector("[data-users-reset]");
  const cancelEditButton = root.querySelector("[data-users-cancel-edit]");
  const formHeading = root.querySelector("[data-users-form-heading]");
  const passwordFields = root.querySelectorAll("[data-users-create-only]");
  const passwordInput = root.querySelector("[data-users-password]");
  const confirmPasswordInput = root.querySelector("[data-users-confirm-password]");
  const deleteModalElement = document.getElementById("userDeleteModal");
  const confirmDeleteButton = root.querySelector("[data-users-confirm-delete]");
  const permissionsHost = root.querySelector("[data-user-permissions-host]");
  const alertHost = root.querySelector("[data-users-alert]");

  const state = {
    search: "",
    status: "",
    pageNumber: 1,
    pageSize: 10,
    sortColumn: "CreatedDate",
    sortDirection: "desc",
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
        throw new Error("Unable to load users.");
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
    if (listHost) {
      listHost.classList.toggle("users-list-busy", isLoading);
    }
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

  function setCreateMode() {
    state.mode = "create";
    form.reset();
    form.classList.remove("was-validated");
    form.querySelector("[data-users-id]").value = "";
    clearFieldErrors();
    formTab.textContent = "Create User";
    formHeading.textContent = "Create User";
    submitButton.textContent = "Create User";
    resetButton.classList.remove("d-none");
    cancelEditButton.classList.add("d-none");
    passwordFields.forEach((field) => field.classList.remove("d-none"));
    passwordInput.disabled = false;
    confirmPasswordInput.disabled = false;
    passwordInput.required = true;
    confirmPasswordInput.required = true;
  }

  function setSelectValue(name, value) {
    const field = form.querySelector(`[name="${name}"]`);
    if (!field) {
      return;
    }

    field.value = value;
    field.dispatchEvent(new Event("change", { bubbles: true }));
  }

  function setEditMode(user) {
    state.mode = "edit";
    clearFieldErrors();
    form.classList.remove("was-validated");
    form.querySelector('[name="Id"]').value = user.id;
    form.querySelector('[name="FirstName"]').value = user.firstName || "";
    form.querySelector('[name="LastName"]').value = user.lastName || "";
    form.querySelector('[name="Email"]').value = user.email || "";
    form.querySelector('[name="MobileNumber"]').value = user.mobileNumber || "";
    setSelectValue("Role", user.role || "Member");
    setSelectValue("Status", user.status || "Active");
    passwordInput.value = "";
    confirmPasswordInput.value = "";
    passwordInput.required = false;
    confirmPasswordInput.required = false;
    passwordInput.disabled = true;
    confirmPasswordInput.disabled = true;
    passwordFields.forEach((field) => field.classList.add("d-none"));
    formTab.textContent = "Update User";
    formHeading.textContent = "Update User";
    submitButton.textContent = "Update User";
    resetButton.classList.add("d-none");
    cancelEditButton.classList.remove("d-none");
  }

  function clearFieldErrors() {
    root.querySelectorAll("[data-users-field-error]").forEach((field) => {
      field.textContent = "";
    });
  }

  async function loadUserForEdit(userId) {
    try {
      const response = await fetch(`${root.dataset.getUrl}/${userId}`);
      if (!response.ok) {
        throw new Error("Unable to load user.");
      }
      const user = await response.json();
      setEditMode(user);
      bootstrap.Tab.getOrCreateInstance(formTab).show();
    } catch (error) {
      showAlert(error.message, "danger");
    }
  }

  async function submitForm(event) {
    event.preventDefault();
    clearFieldErrors();

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
        throw new Error(result.message || "Unable to save user.");
      }

      showAlert(result.message, "success");
      setCreateMode();
      await loadList();
      bootstrap.Tab.getOrCreateInstance(listTab).show();
    } catch (error) {
      showAlert(error.message, "danger");
    } finally {
      submitButton.disabled = false;
    }
  }

  async function softDeleteUser() {
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
        throw new Error(result.message || "Unable to delete user.");
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

  function openDeleteModal(userId) {
    state.pendingDeleteId = userId;
    bootstrap.Modal.getOrCreateInstance(deleteModalElement).show();
  }

  async function openPermissions(userId) {
    renderPermissionLoading(userId);
    showPermissionDrawer();
    try {
      const response = await fetch(`${root.dataset.permissionsUrl}/${userId}`);
      if (!response.ok) {
        throw new Error("Unable to load permissions.");
      }
      permissionsHost.innerHTML = await response.text();
      showPermissionDrawer();
      updateGroupSelectAllStates();
    } catch (error) {
      showAlert(error.message, "danger");
    }
  }

  function renderPermissionLoading(userId) {
    permissionsHost.innerHTML = [
      `<div class="offcanvas offcanvas-end user-permission-drawer" tabindex="-1" id="userPermissionsDrawer" data-user-permissions-drawer data-user-id="${userId}">`,
      '<div class="offcanvas-header user-permission-header">',
      '<div><p class="section-kicker mb-1">Access Control</p><h2 class="offcanvas-title">User Permissions</h2></div>',
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
    const drawer = document.getElementById("userPermissionsDrawer");
    if (drawer) {
      bootstrap.Offcanvas.getOrCreateInstance(drawer).show();
    }
  }

  function filterPermissions(value) {
    const query = value.trim().toLowerCase();
    permissionsHost.querySelectorAll("[data-permission-row]").forEach((row) => {
      const permissionName = row.querySelector("[data-permission-checkbox]").dataset.permissionName.toLowerCase();
      row.classList.toggle("d-none", query.length > 0 && !permissionName.includes(query));
    });
  }

  function updateGroupSelectAllStates() {
    permissionsHost.querySelectorAll("[data-permission-group]").forEach((group) => {
      const checkboxes = Array.from(group.querySelectorAll("[data-permission-checkbox]"));
      const selectAll = group.querySelector("[data-permission-select-group]");
      if (!selectAll || checkboxes.length === 0) {
        return;
      }
      const checkedCount = checkboxes.filter((checkbox) => checkbox.checked).length;
      selectAll.checked = checkedCount === checkboxes.length;
      selectAll.indeterminate = checkedCount > 0 && checkedCount < checkboxes.length;
    });
  }

  async function savePermissions() {
    const drawer = document.getElementById("userPermissionsDrawer");
    const saveButton = permissionsHost.querySelector("[data-permission-save]");
    if (!drawer || !saveButton) {
      return;
    }

    const permissions = Array.from(permissionsHost.querySelectorAll("[data-permission-checkbox]")).map((checkbox) => ({
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
          userId: drawer.dataset.userId,
          organizationId: root.dataset.organizationId,
          permissions
        })
      });
      const result = await response.json();
      if (!response.ok || !result.succeeded) {
        throw new Error(result.message || "Unable to save permissions.");
      }

      showAlert(result.message, "success");
      bootstrap.Offcanvas.getOrCreateInstance(drawer).hide();
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

  root.querySelector("[data-users-clear-filters]").addEventListener("click", () => {
    filterForm.reset();
    state.search = "";
    state.status = "";
    state.pageNumber = 1;
    loadList();
  });

  root.addEventListener("click", (event) => {
    const sortButton = event.target.closest("[data-users-sort]");
    if (sortButton) {
      const column = sortButton.dataset.usersSort;
      state.sortDirection = state.sortColumn === column && state.sortDirection === "asc" ? "desc" : "asc";
      state.sortColumn = column;
      state.pageNumber = 1;
      loadList();
      return;
    }

    const pageButton = event.target.closest("[data-users-page]");
    if (pageButton && !pageButton.disabled) {
      state.pageNumber = Number(pageButton.dataset.usersPage);
      loadList();
      return;
    }

    const actionButton = event.target.closest("[data-users-action]");
    if (!actionButton) {
      return;
    }

    const userId = actionButton.dataset.userId;
    if (actionButton.dataset.usersAction === "edit") {
      loadUserForEdit(userId);
    } else if (actionButton.dataset.usersAction === "delete") {
      openDeleteModal(userId);
    } else if (actionButton.dataset.usersAction === "permissions") {
      openPermissions(userId);
    }
  });

  permissionsHost.addEventListener("input", (event) => {
    if (event.target.matches("[data-permission-search]")) {
      filterPermissions(event.target.value);
    }
  });

  permissionsHost.addEventListener("change", (event) => {
    if (event.target.matches("[data-permission-select-group]")) {
      const group = event.target.closest("[data-permission-group]");
      group.querySelectorAll("[data-permission-checkbox]").forEach((checkbox) => {
        checkbox.checked = event.target.checked;
      });
    }

    if (event.target.matches("[data-permission-checkbox]")) {
      updateGroupSelectAllStates();
    }
  });

  permissionsHost.addEventListener("click", (event) => {
    if (event.target.closest("[data-permission-save]")) {
      savePermissions();
    }
  });

  form.addEventListener("submit", submitForm);
  resetButton.addEventListener("click", () => {
    window.setTimeout(() => {
      clearFieldErrors();
      form.classList.remove("was-validated");
      passwordInput.required = true;
      confirmPasswordInput.required = true;
    }, 0);
  });
  cancelEditButton.addEventListener("click", setCreateMode);
  formTab.addEventListener("click", () => {
    if (state.mode !== "edit") {
      setCreateMode();
    }
  });
  confirmDeleteButton.addEventListener("click", softDeleteUser);

  setCreateMode();
  loadList();
})();
