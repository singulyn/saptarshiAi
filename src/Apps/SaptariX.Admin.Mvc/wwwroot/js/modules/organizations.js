(function () {
  const root = document.querySelector("[data-organizations-root]");
  if (!root) {
    return;
  }

  const rows = Array.from(root.querySelectorAll("[data-organization-row]"));
  const filterForm = root.querySelector("[data-organizations-filter-form]");
  const resetFiltersButton = root.querySelector("[data-organizations-reset-filters]");
  const countLabel = root.querySelector("[data-organizations-count]");
  const emptyState = root.querySelector("[data-organizations-empty]");
  const tableWrapper = root.querySelector("[data-organizations-list-host]");
  const alertHost = root.querySelector("[data-organizations-alert]");
  const form = root.querySelector("[data-organizations-form]");
  const formTab = document.getElementById("organizations-form-tab");
  const listTab = document.getElementById("organizations-list-tab");
  const formHeading = root.querySelector("[data-organizations-form-heading]");
  const submitButton = root.querySelector("[data-organizations-submit]");
  const resetButton = root.querySelector("[data-organizations-reset]");
  const cancelEditButton = root.querySelector("[data-organizations-cancel-edit]");
  const deleteModalElement = document.getElementById("organizationDeleteModal");
  const deleteTarget = root.querySelector("[data-organization-delete-target]");
  const confirmDeleteButton = root.querySelector("[data-organization-confirm-delete]");

  const organizations = {
    platform: {
      id: "platform",
      name: "SaptariX Platform",
      legalName: "SaptariX Platform Pvt Ltd",
      code: "PLATFORM",
      slug: "saptarix-platform",
      type: "Platform",
      industry: "Technology",
      companySize: "51-200",
      status: "Active",
      primaryEmail: "platform@saptarix.local",
      supportEmail: "support@saptarix.local",
      phoneNumber: "+91 90000 00000",
      website: "https://saptarix.local",
      country: "India",
      state: "Karnataka",
      city: "Bengaluru",
      postalCode: "560001",
      addressLine1: "Platform Operations Center",
      addressLine2: "SaaS Foundation Wing",
      gstNumber: "29SXPPLATFORM1Z5",
      cinNumber: "U72900KA2026PTC000001",
      taxId: "SXPPP0001P",
      billingEmail: "billing@saptarix.local",
      currency: "INR",
      timezone: "Asia/Kolkata",
      logoUrl: "",
      brandColor: "#000d0f",
      themePreference: "System",
      currentPlan: "Enterprise",
      billingStatus: "Active",
      trialEndsOn: "",
      isSystem: true
    },
    northwind: {
      id: "northwind",
      name: "Northwind Health",
      legalName: "Northwind Health Services",
      code: "NORTHWIND",
      slug: "northwind-health",
      type: "Customer",
      industry: "Healthcare",
      companySize: "201-500",
      status: "Active",
      primaryEmail: "ops@northwind.example",
      supportEmail: "support@northwind.example",
      phoneNumber: "+91 98888 12001",
      website: "https://northwind.example",
      country: "India",
      state: "Maharashtra",
      city: "Mumbai",
      postalCode: "400001",
      addressLine1: "Northwind Tower",
      addressLine2: "Healthcare Operations",
      gstNumber: "",
      cinNumber: "",
      taxId: "",
      billingEmail: "billing@northwind.example",
      currency: "INR",
      timezone: "Asia/Kolkata",
      logoUrl: "",
      brandColor: "#0f766e",
      themePreference: "Light",
      currentPlan: "Professional",
      billingStatus: "Active",
      trialEndsOn: "",
      isSystem: false
    },
    bluepeak: {
      id: "bluepeak",
      name: "Bluepeak Logistics",
      legalName: "Bluepeak Logistics LLP",
      code: "BLUEPEAK",
      slug: "bluepeak-logistics",
      type: "Partner",
      industry: "Logistics",
      companySize: "51-200",
      status: "Trial",
      primaryEmail: "admin@bluepeak.example",
      supportEmail: "help@bluepeak.example",
      phoneNumber: "+91 97777 12002",
      website: "https://bluepeak.example.com",
      country: "India",
      state: "Delhi",
      city: "New Delhi",
      postalCode: "110001",
      addressLine1: "Bluepeak Hub",
      addressLine2: "Partner Enablement",
      gstNumber: "",
      cinNumber: "",
      taxId: "",
      billingEmail: "finance@bluepeak.example",
      currency: "INR",
      timezone: "Asia/Kolkata",
      logoUrl: "",
      brandColor: "#1d4ed8",
      themePreference: "System",
      currentPlan: "Enterprise",
      billingStatus: "Trial",
      trialEndsOn: "2026-07-13",
      isSystem: false
    },
    atlas: {
      id: "atlas",
      name: "Atlas Retail Group",
      legalName: "Atlas Retail Group",
      code: "ATLAS",
      slug: "atlas-retail",
      type: "Customer",
      industry: "Retail",
      companySize: "11-50",
      status: "Inactive",
      primaryEmail: "it@atlasretail.example",
      supportEmail: "care@atlasretail.example",
      phoneNumber: "+91 96666 12003",
      website: "https://atlasretail.example",
      country: "India",
      state: "Gujarat",
      city: "Ahmedabad",
      postalCode: "380001",
      addressLine1: "Atlas House",
      addressLine2: "",
      gstNumber: "",
      cinNumber: "",
      taxId: "",
      billingEmail: "billing@atlasretail.example",
      currency: "INR",
      timezone: "Asia/Kolkata",
      logoUrl: "",
      brandColor: "#dc6803",
      themePreference: "Light",
      currentPlan: "Starter",
      billingStatus: "Paused",
      trialEndsOn: "",
      isSystem: false
    },
    demo: {
      id: "demo",
      name: "SaptariX Demo Workspace",
      legalName: "SaptariX Demo Workspace",
      code: "DEMO",
      slug: "saptarix-demo",
      type: "Demo",
      industry: "Other",
      companySize: "1-10",
      status: "Suspended",
      primaryEmail: "demo@saptarix.local",
      supportEmail: "demo-support@saptarix.local",
      phoneNumber: "",
      website: "https://demo.saptarix.app",
      country: "India",
      state: "Karnataka",
      city: "Bengaluru",
      postalCode: "",
      addressLine1: "",
      addressLine2: "",
      gstNumber: "",
      cinNumber: "",
      taxId: "",
      billingEmail: "demo-billing@saptarix.local",
      currency: "INR",
      timezone: "Asia/Kolkata",
      logoUrl: "",
      brandColor: "#6941c6",
      themePreference: "Dark",
      currentPlan: "Free",
      billingStatus: "Paused",
      trialEndsOn: "",
      isSystem: false
    }
  };

  const state = {
    mode: "create",
    pendingDeleteId: null
  };

  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
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

  function getField(name) {
    return form ? form.querySelector(`[name="${name}"]`) : null;
  }

  function setField(name, value) {
    const field = getField(name);
    if (!field) {
      return;
    }

    field.value = value ?? "";
    field.dispatchEvent(new Event("change", { bubbles: true }));
  }

  function getOrganization(id) {
    return organizations[id] || null;
  }

  function visibleRows() {
    return rows.filter((row) => !row.classList.contains("d-none"));
  }

  function updateVisibleCount() {
    const count = visibleRows().length;
    if (countLabel) {
      countLabel.textContent = `${count} organization${count === 1 ? "" : "s"}`;
    }
    if (emptyState && tableWrapper) {
      emptyState.classList.toggle("d-none", count > 0);
      tableWrapper.classList.toggle("d-none", count === 0);
    }
  }

  function applyFilters(event) {
    if (event) {
      event.preventDefault();
    }

    const values = new FormData(filterForm);
    const search = String(values.get("Search") || "").trim().toLowerCase();
    const status = values.get("Status");
    const type = values.get("Type");
    const plan = values.get("Plan");
    const industry = values.get("Industry");

    rows.forEach((row) => {
      const matchesSearch = !search || row.dataset.search.includes(search);
      const matchesStatus = !status || row.dataset.status === status;
      const matchesType = !type || row.dataset.type === type;
      const matchesPlan = !plan || row.dataset.plan === plan;
      const matchesIndustry = !industry || row.dataset.industry === industry;

      row.classList.toggle("d-none", !(matchesSearch && matchesStatus && matchesType && matchesPlan && matchesIndustry));
    });

    updateVisibleCount();
  }

  function resetFilters() {
    filterForm.reset();
    filterForm.querySelectorAll("select").forEach((select) => {
      select.dispatchEvent(new Event("change", { bubbles: true }));
    });
    applyFilters();
  }

  function setCreateMode() {
    state.mode = "create";
    if (!form) {
      return;
    }

    form.reset();
    form.classList.remove("was-validated");
    setField("Id", "");
    setField("Country", "India");
    setField("Currency", "INR");
    setField("Timezone", "Asia/Kolkata");
    setField("Status", "Active");
    setField("Industry", "Technology");
    setField("CompanySize", "1-10");
    setField("ThemePreference", "System");
    setField("CurrentPlan", "Enterprise");
    setField("BillingStatus", "Active");

    formTab.textContent = "Create Organization";
    formHeading.textContent = "Create Organization";
    submitButton.textContent = "Create Organization";
    resetButton.classList.remove("d-none");
    cancelEditButton.classList.add("d-none");
  }

  function setEditMode(organization) {
    state.mode = "edit";
    form.classList.remove("was-validated");

    setField("Id", organization.id);
    setField("Name", organization.name);
    setField("LegalName", organization.legalName);
    setField("Code", organization.code);
    setField("Slug", organization.slug);
    setField("OrganizationType", organization.type);
    setField("Industry", organization.industry);
    setField("CompanySize", organization.companySize);
    setField("Status", organization.status);
    setField("PrimaryEmail", organization.primaryEmail);
    setField("SupportEmail", organization.supportEmail);
    setField("PhoneNumber", organization.phoneNumber);
    setField("Website", organization.website);
    setField("Country", organization.country);
    setField("State", organization.state);
    setField("City", organization.city);
    setField("PostalCode", organization.postalCode);
    setField("AddressLine1", organization.addressLine1);
    setField("AddressLine2", organization.addressLine2);
    setField("GstNumber", organization.gstNumber);
    setField("CinNumber", organization.cinNumber);
    setField("TaxId", organization.taxId);
    setField("BillingEmail", organization.billingEmail);
    setField("Currency", organization.currency);
    setField("Timezone", organization.timezone);
    setField("LogoUrl", organization.logoUrl);
    setField("BrandColor", organization.brandColor);
    setField("ThemePreference", organization.themePreference);
    setField("CurrentPlan", organization.currentPlan);
    setField("BillingStatus", organization.billingStatus);
    setField("TrialEndsOn", organization.trialEndsOn);

    formTab.textContent = "Update Organization";
    formHeading.textContent = "Update Organization";
    submitButton.textContent = "Update Organization";
    resetButton.classList.add("d-none");
    cancelEditButton.classList.remove("d-none");
    bootstrap.Tab.getOrCreateInstance(formTab).show();
  }

  function submitForm(event) {
    event.preventDefault();

    if (!form.checkValidity()) {
      form.classList.add("was-validated");
      form.reportValidity();
      return;
    }

    const verb = state.mode === "edit" ? "updated" : "created";
    showAlert(`Organization ${verb} in UI preview only. Backend save is intentionally not wired.`, "success");
    setCreateMode();
    bootstrap.Tab.getOrCreateInstance(listTab).show();
  }

  function updateDrawerSummary(drawer, organization) {
    drawer.querySelectorAll("[data-organization-drawer-avatar]").forEach((avatar) => {
      avatar.textContent = organization.name.substring(0, 1).toUpperCase();
    });
    drawer.querySelectorAll("[data-organization-drawer-name]").forEach((name) => {
      name.textContent = organization.name;
    });
    drawer.querySelectorAll("[data-organization-drawer-meta]").forEach((meta) => {
      meta.textContent = `${organization.code} - ${organization.currentPlan} - ${organization.status}`;
    });
  }

  function openDrawer(kind, organization) {
    const drawer = root.querySelector(`[data-organization-drawer="${kind}"]`);
    if (!drawer) {
      return;
    }

    updateDrawerSummary(drawer, organization);
    bootstrap.Offcanvas.getOrCreateInstance(drawer).show();
  }

  function openDeleteModal(organization) {
    if (organization.isSystem) {
      showAlert("System organization cannot be deleted.", "warning");
      return;
    }

    state.pendingDeleteId = organization.id;
    if (deleteTarget) {
      deleteTarget.textContent = organization.name;
    }
    bootstrap.Modal.getOrCreateInstance(deleteModalElement).show();
  }

  function confirmDelete() {
    if (!state.pendingDeleteId) {
      return;
    }

    const row = rows.find((item) => item.dataset.organizationId === state.pendingDeleteId);
    if (row) {
      row.classList.add("d-none");
      row.dataset.status = "Archived";
    }
    const organization = getOrganization(state.pendingDeleteId);
    bootstrap.Modal.getOrCreateInstance(deleteModalElement).hide();
    showAlert(`${organization ? organization.name : "Organization"} soft-deleted in UI preview only.`, "success");
    state.pendingDeleteId = null;
    updateVisibleCount();
  }

  function handleTableAction(event) {
    const button = event.target.closest("[data-organization-action]");
    if (!button) {
      return;
    }

    const organization = getOrganization(button.dataset.organizationId);
    if (!organization) {
      showAlert("Organization preview data was not found.", "danger");
      return;
    }

    switch (button.dataset.organizationAction) {
      case "view":
        showAlert(`${organization.name} details preview selected.`, "info");
        break;
      case "edit":
        setEditMode(organization);
        break;
      case "apps":
        openDrawer("apps", organization);
        break;
      case "modules":
        openDrawer("modules", organization);
        break;
      case "domains":
        openDrawer("domains", organization);
        break;
      case "settings":
        openDrawer("settings", organization);
        break;
      case "delete":
        openDeleteModal(organization);
        break;
      default:
        break;
    }
  }

  function handleDrawerSearch(event) {
    const search = event.target;
    if (!search.matches("[data-organization-drawer-search]")) {
      return;
    }

    const drawer = search.closest("[data-organization-drawer]");
    const value = search.value.trim().toLowerCase();
    drawer.querySelectorAll("[data-drawer-row]").forEach((row) => {
      row.classList.toggle("d-none", value.length > 0 && !row.textContent.toLowerCase().includes(value));
    });
  }

  function handleDrawerSave(event) {
    const button = event.target.closest("[data-organization-drawer-save]");
    if (!button) {
      return;
    }

    const drawer = button.closest("[data-organization-drawer]");
    const title = drawer.querySelector(".offcanvas-title")?.textContent || "Drawer";
    bootstrap.Offcanvas.getOrCreateInstance(drawer).hide();
    showAlert(`${title} saved in UI preview only.`, "success");
  }

  function handleModulesCommand(event) {
    const selectAll = event.target.closest("[data-organization-select-all-modules]");
    const clearAll = event.target.closest("[data-organization-clear-modules]");
    if (!selectAll && !clearAll) {
      return;
    }

    const checked = Boolean(selectAll);
    root.querySelectorAll("[data-organization-modules-list] input[type='checkbox']").forEach((checkbox) => {
      checkbox.checked = checked;
    });
    updateModuleCounts();
  }

  function updateModuleCounts() {
    const moduleList = root.querySelector("[data-organization-modules-list]");
    if (!moduleList) {
      return;
    }

    const checkboxes = Array.from(moduleList.querySelectorAll("input[type='checkbox']"));
    const selected = checkboxes.filter((checkbox) => checkbox.checked).length;
    const summary = `${selected} of ${checkboxes.length} modules selected`;

    root.querySelectorAll("[data-organization-modules-count], [data-organization-modules-footer-count]").forEach((label) => {
      label.textContent = summary;
    });

    root.querySelectorAll("[data-module-group]").forEach((group) => {
      const groupName = group.dataset.moduleGroup;
      const groupCheckboxes = Array.from(group.querySelectorAll("input[type='checkbox']"));
      const groupSelected = groupCheckboxes.filter((checkbox) => checkbox.checked).length;
      const groupCount = root.querySelector(`[data-module-group-count="${groupName}"]`);
      if (groupCount) {
        groupCount.textContent = `${groupSelected} / ${groupCheckboxes.length}`;
      }
    });
  }

  function handleModuleToggle(event) {
    if (!event.target.matches("[data-organization-modules-list] input[type='checkbox']")) {
      return;
    }

    updateModuleCounts();
  }

  function handleDomainForm(event) {
    const domainForm = event.target.closest("[data-organization-domain-form]");
    if (!domainForm) {
      return;
    }

    event.preventDefault();
    const values = new FormData(domainForm);
    const domain = String(values.get("Domain") || "").trim();
    if (!domain) {
      showAlert("Enter a domain before adding a preview row.", "warning");
      return;
    }

    const domainType = values.get("DomainType") || "Custom";
    const status = values.get("Status") || "Active";
    const body = root.querySelector("[data-organization-domains-body]");
    body.insertAdjacentHTML("beforeend", [
      "<tr>",
      `<td>${escapeHtml(domain)}</td>`,
      `<td><span class="badge text-bg-light users-role-badge">${escapeHtml(domainType)}</span></td>`,
      '<td><div class="form-check form-switch m-0"><input class="form-check-input" type="checkbox" aria-label="Primary domain" /></div></td>',
      '<td><span class="badge text-bg-warning">Pending</span></td>',
      `<td><span class="badge text-bg-${status === "Active" ? "success" : "secondary"}">${escapeHtml(status)}</span></td>`,
      '<td class="text-end"><button class="btn btn-light btn-sm fa-icon-btn text-danger" type="button" data-organization-domain-delete aria-label="Delete domain"><i class="fa-solid fa-trash" aria-hidden="true"></i></button></td>',
      "</tr>"
    ].join(""));
    domainForm.reset();
    domainForm.querySelectorAll("select").forEach((select) => select.dispatchEvent(new Event("change", { bubbles: true })));
    showAlert("Domain preview row added. Backend save is intentionally not wired.", "success");
  }

  function handleDomainDelete(event) {
    const button = event.target.closest("[data-organization-domain-delete]");
    if (!button) {
      return;
    }

    button.closest("tr")?.remove();
    showAlert("Domain preview row removed. Backend delete is intentionally not wired.", "success");
  }

  filterForm?.addEventListener("submit", applyFilters);
  resetFiltersButton?.addEventListener("click", resetFilters);
  tableWrapper?.addEventListener("click", handleTableAction);
  form?.addEventListener("submit", submitForm);
  resetButton?.addEventListener("click", () => setTimeout(setCreateMode, 0));
  cancelEditButton?.addEventListener("click", setCreateMode);
  confirmDeleteButton?.addEventListener("click", confirmDelete);
  root.addEventListener("input", handleDrawerSearch);
  root.addEventListener("click", handleDrawerSave);
  root.addEventListener("click", handleModulesCommand);
  root.addEventListener("change", handleModuleToggle);
  root.addEventListener("submit", handleDomainForm);
  root.addEventListener("click", handleDomainDelete);

  setCreateMode();
  updateModuleCounts();
  updateVisibleCount();
})();
