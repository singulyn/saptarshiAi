(function () {
  let inlineRowSequence = 3;
  let pendingInlineDeleteRow = null;

  function getToastHost() {
    let host = document.querySelector("[data-ui-toast-host]");
    if (!host) {
      host = document.createElement("div");
      host.className = "toast-container position-fixed top-0 end-0 p-3 ui-toast-host";
      host.dataset.uiToastHost = "";
      document.body.appendChild(host);
    }
    return host;
  }

  function escapeHtml(value) {
    return String(value)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function showToast(message, tone) {
    const toneMap = {
      success: "text-bg-success",
      error: "text-bg-danger",
      warning: "text-bg-warning",
      info: "text-bg-info"
    };
    const host = getToastHost();
    const toast = document.createElement("div");
    toast.className = `toast align-items-center border-0 ${toneMap[tone] || "text-bg-secondary"}`;
    toast.role = "status";
    toast.ariaLive = "polite";
    toast.ariaAtomic = "true";
    toast.innerHTML = [
      '<div class="d-flex">',
      `<div class="toast-body">${escapeHtml(message)}</div>`,
      '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>',
      "</div>"
    ].join("");
    host.appendChild(toast);
    bootstrap.Toast.getOrCreateInstance(toast, { delay: 3000 }).show();
    toast.addEventListener("hidden.bs.toast", () => toast.remove());
  }

  function getInlineFields(table) {
    return {
      name: table.querySelector("[data-ui-inline-field='name']"),
      code: table.querySelector("[data-ui-inline-field='code']"),
      status: table.querySelector("[data-ui-inline-field='status']"),
      sortOrder: table.querySelector("[data-ui-inline-field='sortOrder']")
    };
  }

  function getStatusBadge(status) {
    const cssClass = status === "Active" ? "text-bg-success" : status === "Deleted" ? "text-bg-secondary" : "text-bg-secondary";
    return `<span class="badge ${cssClass}">${escapeHtml(status)}</span>`;
  }

  function updateInlineRow(row, values) {
    row.dataset.name = values.name;
    row.dataset.code = values.code;
    row.dataset.status = values.status;
    row.dataset.sortOrder = values.sortOrder;
    row.querySelector("[data-ui-inline-cell='name']").textContent = values.name;
    row.querySelector("[data-ui-inline-cell='code']").textContent = values.code;
    row.querySelector("[data-ui-inline-cell='status']").innerHTML = getStatusBadge(values.status);
    row.querySelector("[data-ui-inline-cell='sortOrder']").textContent = values.sortOrder;
  }

  function resetInlineForm(table) {
    const entityName = table.dataset.uiInlineEntityName || "Entity";
    const fields = getInlineFields(table);
    fields.name.value = "";
    fields.code.value = "";
    fields.status.value = "Active";
    fields.status.dispatchEvent(new Event("change", { bubbles: true }));
    fields.sortOrder.value = "10";
    delete table.dataset.editingRowId;
    table.querySelector("[data-ui-inline-form-title]").textContent = `Create ${entityName}`;
    table.querySelector("[data-ui-inline-submit]").textContent = "Create";
    table.querySelector("[data-ui-inline-cancel]").classList.add("d-none");
  }

  function updateInlineCount(table) {
    const count = table.querySelectorAll("[data-ui-inline-row]").length;
    const label = table.querySelector("[data-ui-inline-record-count]");
    if (label) {
      label.textContent = `${count} ${count === 1 ? "record" : "records"}`;
    }
  }

  function buildInlineRow(values) {
    const createdDate = new Intl.DateTimeFormat("en-GB", {
      day: "2-digit",
      month: "short",
      year: "numeric"
    }).format(new Date()).replace(",", "");
    const id = `role-category-${inlineRowSequence++}`;
    return [
      `<tr data-ui-inline-row data-entity-id="${id}" data-name="${escapeHtml(values.name)}" data-code="${escapeHtml(values.code)}" data-status="${escapeHtml(values.status)}" data-sort-order="${escapeHtml(values.sortOrder)}">`,
      `<td data-ui-inline-cell="name">${escapeHtml(values.name)}</td>`,
      `<td data-ui-inline-cell="code">${escapeHtml(values.code)}</td>`,
      `<td data-ui-inline-cell="status">${getStatusBadge(values.status)}</td>`,
      `<td data-ui-inline-cell="sortOrder">${escapeHtml(values.sortOrder)}</td>`,
      `<td class="sx-nowrap" data-ui-inline-cell="createdDate">${createdDate}</td>`,
      '<td class="sx-action-cell"><div class="sx-action-buttons">',
      '<button class="btn btn-light btn-sm fa-icon-btn" type="button" aria-label="Edit inline" data-ui-inline-edit><i class="fa-solid fa-pen-to-square" aria-hidden="true"></i></button>',
      '<button class="btn btn-light btn-sm fa-icon-btn" type="button" aria-label="Toggle status" data-ui-inline-toggle><i class="fa-solid fa-rotate" aria-hidden="true"></i></button>',
      '<button class="btn btn-light btn-sm fa-icon-btn text-danger" type="button" aria-label="Soft delete" data-bs-toggle="modal" data-bs-target="#uiInlineSoftDeleteModal" data-ui-inline-delete><i class="fa-solid fa-trash" aria-hidden="true"></i></button>',
      "</div></td></tr>"
    ].join("");
  }

  function getSelectOptionValue(option) {
    return Object.prototype.hasOwnProperty.call(option.dataset, "value")
      ? option.dataset.value
      : option.textContent.trim();
  }

  function getSelectOptionLabel(option) {
    return option.dataset.label
      || option.querySelector("strong")?.textContent.trim()
      || option.textContent.trim();
  }

  function getGeneratedOptionMeta(option) {
    if (option.dataset.description || option.dataset.tone) {
      return {
        description: option.dataset.description || "",
        tone: option.dataset.tone || "muted"
      };
    }

    const label = option.textContent.trim();
    const normalized = `${option.value} ${label}`.toLowerCase();

    if (!option.value || normalized.includes("all status") || normalized.startsWith("all ")) {
      return { description: "Include every matching record", tone: "muted" };
    }

    if (normalized.includes("inactive") || normalized.includes("deleted") || normalized.includes("disabled") || normalized.includes("failed")) {
      return { description: "Unavailable or retained for history", tone: "muted" };
    }

    if (normalized.includes("pending") || normalized.includes("draft") || normalized.includes("warning")) {
      return { description: "Waiting for action or review", tone: "warning" };
    }

    if (normalized.includes("active") || normalized.includes("success") || normalized.includes("enabled")) {
      return { description: "Enabled and ready to use", tone: "success" };
    }

    if (normalized.includes("email")) {
      return { description: "Email-formatted value", tone: "info" };
    }

    if (normalized.includes("date")) {
      return { description: "Date picker value", tone: "info" };
    }

    if (normalized.includes("text")) {
      return { description: "Plain text value", tone: "muted" };
    }

    return { description: "Selectable option", tone: "muted" };
  }

  function buildGeneratedOption(option, nativeSelect) {
    const item = document.createElement("button");
    const label = option.textContent.trim();
    const meta = getGeneratedOptionMeta(option);

    item.className = "sx-select-option";
    item.type = "button";
    item.role = "option";
    item.dataset.sxSelectOption = "";
    item.dataset.value = option.value;
    item.dataset.label = label;
    item.disabled = option.disabled;
    item.setAttribute("aria-selected", String(option.selected));

    if (option.selected || nativeSelect.value === option.value) {
      item.classList.add("is-selected");
    }

    const marker = document.createElement("span");
    marker.className = `sx-select-status sx-select-status-${meta.tone}`;
    marker.setAttribute("aria-hidden", "true");

    const text = document.createElement("span");
    const title = document.createElement("strong");
    title.textContent = label;
    text.appendChild(title);

    if (meta.description) {
      const description = document.createElement("small");
      description.textContent = meta.description;
      text.appendChild(description);
    }

    item.append(marker, text);
    return item;
  }

  function syncGeneratedSelectOptions(select) {
    const nativeSelect = select.querySelector("[data-sx-native-select]");
    const menu = select.querySelector("[data-sx-select-menu]");
    if (!nativeSelect || !menu || !Object.prototype.hasOwnProperty.call(nativeSelect.dataset, "sxGeneratedSelect")) {
      return;
    }

    menu.replaceChildren(...Array.from(nativeSelect.options).map((option) => buildGeneratedOption(option, nativeSelect)));
  }

  function updateCustomSelectFromNative(select) {
    const nativeSelect = select.querySelector("[data-sx-native-select]");
    const valueLabel = select.querySelector("[data-sx-select-value]");
    const control = select.querySelector("[data-sx-select-control]");
    if (!nativeSelect) {
      return;
    }

    if (Object.prototype.hasOwnProperty.call(nativeSelect.dataset, "sxGeneratedSelect")) {
      syncGeneratedSelectOptions(select);
    }

    const options = Array.from(select.querySelectorAll("[data-sx-select-option]"));
    const selectedOption = options.find((option) => getSelectOptionValue(option) === nativeSelect.value) || options[0];
    const nativeLabel = nativeSelect.selectedOptions[0]?.textContent.trim() || nativeSelect.value;

    if (valueLabel) {
      valueLabel.textContent = selectedOption ? getSelectOptionLabel(selectedOption) : nativeLabel;
    }

    options.forEach((option) => {
      const isSelected = option === selectedOption;
      option.classList.toggle("is-selected", isSelected);
      option.setAttribute("aria-selected", String(isSelected));
    });

    if (control) {
      control.disabled = nativeSelect.disabled;
      control.setAttribute("aria-disabled", String(nativeSelect.disabled));
    }
  }

  function enhanceNativeSelect(nativeSelect) {
    if (nativeSelect.closest("[data-sx-select]") || nativeSelect.multiple || nativeSelect.size > 1) {
      return;
    }

    const select = document.createElement("div");
    const customClasses = Array.from(nativeSelect.classList)
      .filter((className) => !["form-select", "form-select-sm", "form-select-lg"].includes(className));
    select.className = ["sx-select", "sx-select-auto", nativeSelect.classList.contains("form-select-sm") ? "sx-select-sm" : "", ...customClasses]
      .filter(Boolean)
      .join(" ");
    select.dataset.sxSelect = "";

    const control = document.createElement("button");
    const selectId = nativeSelect.id || nativeSelect.name || `sx-select-${Math.random().toString(36).slice(2)}`;
    const menuId = `${selectId}-menu`;
    control.className = "sx-select-control";
    control.type = "button";
    control.role = "combobox";
    control.setAttribute("aria-haspopup", "listbox");
    control.setAttribute("aria-expanded", "false");
    control.setAttribute("aria-controls", menuId);
    control.dataset.sxSelectControl = "";

    const value = document.createElement("span");
    value.className = "sx-select-value";
    value.dataset.sxSelectValue = "";

    const caret = document.createElement("span");
    caret.className = "sx-select-caret";
    caret.setAttribute("aria-hidden", "true");
    control.append(value, caret);

    const menu = document.createElement("div");
    menu.className = "sx-select-menu";
    menu.id = menuId;
    menu.role = "listbox";
    menu.tabIndex = -1;
    menu.dataset.sxSelectMenu = "";

    nativeSelect.dataset.sxNativeSelect = "";
    nativeSelect.dataset.sxGeneratedSelect = "";
    nativeSelect.classList.add("visually-hidden", "sx-select-native");
    nativeSelect.tabIndex = -1;
    nativeSelect.setAttribute("aria-hidden", "true");

    nativeSelect.parentNode.insertBefore(select, nativeSelect);
    select.append(nativeSelect, control, menu);
    updateCustomSelectFromNative(select);
  }

  function initializeCustomSelects(root) {
    const scope = root || document;
    scope.querySelectorAll("select.form-select:not([data-sx-native-select])").forEach(enhanceNativeSelect);
    scope.querySelectorAll("[data-sx-select]").forEach(updateCustomSelectFromNative);
  }

  function closeCustomSelect(select) {
    select.classList.remove("is-open");
    select.querySelector("[data-sx-select-control]")?.setAttribute("aria-expanded", "false");
  }

  function closeOtherCustomSelects(currentSelect) {
    document.querySelectorAll("[data-sx-select].is-open").forEach((select) => {
      if (select !== currentSelect) {
        closeCustomSelect(select);
      }
    });
  }

  function openCustomSelect(select) {
    closeOtherCustomSelects(select);
    select.classList.add("is-open");
    select.querySelector("[data-sx-select-control]")?.setAttribute("aria-expanded", "true");
  }

  function syncCustomSelect(select, option) {
    const value = getSelectOptionValue(option);
    const label = getSelectOptionLabel(option);
    const nativeSelect = select.querySelector("[data-sx-native-select]");
    const valueLabel = select.querySelector("[data-sx-select-value]");

    if (nativeSelect) {
      nativeSelect.value = value;
      nativeSelect.dispatchEvent(new Event("change", { bubbles: true }));
    }

    if (valueLabel) {
      valueLabel.textContent = label;
    }

    select.querySelectorAll("[data-sx-select-option]").forEach((item) => {
      const isSelected = item === option;
      item.classList.toggle("is-selected", isSelected);
      item.setAttribute("aria-selected", String(isSelected));
    });
  }

  window.SaptariXToast = {
    success(message) {
      showToast(message, "success");
    },
    error(message) {
      showToast(message, "error");
    },
    warning(message) {
      showToast(message, "warning");
    },
    info(message) {
      showToast(message, "info");
    }
  };

  document.addEventListener("click", async (event) => {
    const customSelectControl = event.target.closest("[data-sx-select-control]");
    if (customSelectControl) {
      const select = customSelectControl.closest("[data-sx-select]");
      if (select) {
        select.classList.contains("is-open") ? closeCustomSelect(select) : openCustomSelect(select);
      }
      return;
    }

    const customSelectOption = event.target.closest("[data-sx-select-option]");
    if (customSelectOption) {
      const select = customSelectOption.closest("[data-sx-select]");
      if (select) {
        syncCustomSelect(select, customSelectOption);
        closeCustomSelect(select);
        select.querySelector("[data-sx-select-control]")?.focus();
      }
      return;
    }

    if (!event.target.closest("[data-sx-select]")) {
      closeOtherCustomSelects(null);
    }

    const inlineEditButton = event.target.closest("[data-ui-inline-edit]");
    if (inlineEditButton) {
      const row = inlineEditButton.closest("[data-ui-inline-row]");
      const table = inlineEditButton.closest("[data-ui-inline-create-table]");
      const entityName = table?.dataset.uiInlineEntityName || "Entity";
      if (row && table) {
        const fields = getInlineFields(table);
        fields.name.value = row.dataset.name || "";
        fields.code.value = row.dataset.code || "";
        fields.status.value = row.dataset.status || "Active";
        fields.status.dispatchEvent(new Event("change", { bubbles: true }));
        fields.sortOrder.value = row.dataset.sortOrder || "10";
        table.dataset.editingRowId = row.dataset.entityId || "";
        table.querySelector("[data-ui-inline-form-title]").textContent = `Update ${entityName}`;
        table.querySelector("[data-ui-inline-submit]").textContent = "Update";
        table.querySelector("[data-ui-inline-cancel]").classList.remove("d-none");
      }
      return;
    }

    const inlineCancelButton = event.target.closest("[data-ui-inline-cancel]");
    if (inlineCancelButton) {
      const table = inlineCancelButton.closest("[data-ui-inline-create-table]");
      if (table) {
        resetInlineForm(table);
      }
      return;
    }

    const inlineToggleButton = event.target.closest("[data-ui-inline-toggle]");
    if (inlineToggleButton) {
      const row = inlineToggleButton.closest("[data-ui-inline-row]");
      if (row && row.dataset.status !== "Deleted") {
        const nextStatus = row.dataset.status === "Active" ? "Inactive" : "Active";
        updateInlineRow(row, {
          name: row.dataset.name || "",
          code: row.dataset.code || "",
          status: nextStatus,
          sortOrder: row.dataset.sortOrder || "10"
        });
        window.SaptariXToast.info(`Status changed to ${nextStatus}.`);
      }
      return;
    }

    const inlineDeleteButton = event.target.closest("[data-ui-inline-delete]");
    if (inlineDeleteButton) {
      pendingInlineDeleteRow = inlineDeleteButton.closest("[data-ui-inline-row]");
      return;
    }

    const inlineConfirmDeleteButton = event.target.closest("[data-ui-inline-confirm-delete]");
    if (inlineConfirmDeleteButton) {
      if (pendingInlineDeleteRow) {
        updateInlineRow(pendingInlineDeleteRow, {
          name: pendingInlineDeleteRow.dataset.name || "",
          code: pendingInlineDeleteRow.dataset.code || "",
          status: "Deleted",
          sortOrder: pendingInlineDeleteRow.dataset.sortOrder || "10"
        });
        pendingInlineDeleteRow.classList.add("table-light", "text-muted");
        pendingInlineDeleteRow.querySelectorAll("button").forEach((button) => {
          if (!button.matches("[data-ui-inline-edit]")) {
            button.disabled = true;
          }
        });
        window.SaptariXToast.warning("Record soft deleted.");
      }
      const modalElement = document.getElementById("uiInlineSoftDeleteModal");
      if (modalElement) {
        bootstrap.Modal.getOrCreateInstance(modalElement).hide();
      }
      pendingInlineDeleteRow = null;
      return;
    }

    const copyButton = event.target.closest("[data-ui-copy-code]");
    if (copyButton) {
      const preview = copyButton.closest("[data-ui-code-preview]");
      const code = preview?.querySelector("[data-ui-code-panel] code")?.textContent || "";
      try {
        await navigator.clipboard.writeText(code);
        window.SaptariXToast.success("Snippet copied.");
      } catch {
        window.SaptariXToast.error("Unable to copy snippet.");
      }
      return;
    }

    const toggle = event.target.closest("[data-ui-preview-toggle]");
    if (toggle) {
      const preview = toggle.closest("[data-ui-code-preview]");
      const showCode = toggle.dataset.uiPreviewToggle === "code";
      preview.querySelector("[data-ui-preview-panel]").classList.toggle("d-none", showCode);
      preview.querySelector("[data-ui-code-panel]").classList.toggle("d-none", !showCode);
      preview.querySelectorAll("[data-ui-preview-toggle]").forEach((button) => button.classList.remove("active"));
      toggle.classList.add("active");
      return;
    }

    const toastButton = event.target.closest("[data-ui-toast]");
    if (toastButton) {
      const type = toastButton.dataset.uiToast;
      const message = type === "save" ? "AJAX save completed successfully." : `${type} toast example.`;
      const method = type === "error" ? "error" : type === "warning" ? "warning" : type === "info" ? "info" : "success";
      window.SaptariXToast[method](message);
      return;
    }

    const addRowButton = event.target.closest("[data-ui-add-row]");
    if (addRowButton) {
      const table = addRowButton.closest("[data-ui-input-table]");
      const body = table?.querySelector("[data-ui-input-table-body]");
      const firstRow = body?.querySelector("tr");
      if (body && firstRow) {
        const clone = firstRow.cloneNode(true);
        clone.querySelectorAll("input").forEach((input) => {
          if (input.type === "checkbox") {
            input.checked = false;
          } else {
            input.value = "";
          }
        });
        body.appendChild(clone);
      }
      return;
    }

    const removeRowButton = event.target.closest("[data-ui-remove-row]");
    if (removeRowButton) {
      const row = removeRowButton.closest("tr");
      const body = row?.parentElement;
      if (row && body && body.children.length > 1) {
        row.remove();
      } else {
        window.SaptariXToast.warning("At least one row is required.");
      }
      return;
    }

    const loadingButton = event.target.closest("[data-ui-loading-button]");
    if (loadingButton) {
      const spinner = loadingButton.querySelector("[data-ui-loading-button-spinner]");
      loadingButton.disabled = true;
      spinner?.classList.remove("d-none");
      window.setTimeout(() => {
        loadingButton.disabled = false;
        spinner?.classList.add("d-none");
        window.SaptariXToast.success("Loading state complete.");
      }, 900);
      return;
    }

    const updateModeButton = event.target.closest("[data-ui-set-update-mode]");
    if (updateModeButton) {
      const page = updateModeButton.closest("[data-ui-kit]");
      const tabButton = page?.querySelector("[data-ui-update-tab]");
      const submitButton = page?.querySelector("[data-ui-update-button]");
      if (tabButton && submitButton) {
        tabButton.textContent = "Update User";
        submitButton.textContent = "Update User";
        bootstrap.Tab.getOrCreateInstance(tabButton).show();
      }
    }
  });

  document.addEventListener("keydown", (event) => {
    const control = event.target.closest("[data-sx-select-control]");
    const openSelect = document.querySelector("[data-sx-select].is-open");

    if (control && (event.key === "Enter" || event.key === " " || event.key === "ArrowDown")) {
      event.preventDefault();
      const select = control.closest("[data-sx-select]");
      if (select) {
        openCustomSelect(select);
        select.querySelector("[data-sx-select-option].is-selected, [data-sx-select-option]")?.focus();
      }
      return;
    }

    const option = event.target.closest("[data-sx-select-option]");
    if (option) {
      const select = option.closest("[data-sx-select]");
      const options = Array.from(select?.querySelectorAll("[data-sx-select-option]") || []);
      const currentIndex = options.indexOf(option);

      if (event.key === "Enter" || event.key === " ") {
        event.preventDefault();
        syncCustomSelect(select, option);
        closeCustomSelect(select);
        select.querySelector("[data-sx-select-control]")?.focus();
        return;
      }

      if (event.key === "ArrowDown" || event.key === "ArrowUp") {
        event.preventDefault();
        const nextIndex = event.key === "ArrowDown"
          ? Math.min(options.length - 1, currentIndex + 1)
          : Math.max(0, currentIndex - 1);
        options[nextIndex]?.focus();
        return;
      }
    }

    if (openSelect && event.key === "Escape") {
      event.preventDefault();
      closeCustomSelect(openSelect);
      openSelect.querySelector("[data-sx-select-control]")?.focus();
    }
  });

  document.addEventListener("change", (event) => {
    const nativeSelect = event.target.closest("[data-sx-native-select]");
    if (!nativeSelect) {
      return;
    }

    const select = nativeSelect.closest("[data-sx-select]");
    if (select) {
      updateCustomSelectFromNative(select);
    }
  });

  document.addEventListener("reset", (event) => {
    window.setTimeout(() => {
      event.target.querySelectorAll("[data-sx-select]").forEach(updateCustomSelectFromNative);
    });
  });

  document.addEventListener("submit", (event) => {
    const form = event.target.closest("[data-ui-inline-create-form]");
    if (!form) {
      return;
    }

    event.preventDefault();
    const table = form.closest("[data-ui-inline-create-table]");
    const body = table?.querySelector("[data-ui-inline-table-body]");
    if (!table || !body) {
      return;
    }

    const fields = getInlineFields(table);
    const values = {
      name: fields.name.value.trim(),
      code: fields.code.value.trim(),
      status: fields.status.value,
      sortOrder: fields.sortOrder.value.trim() || "10"
    };

    if (!values.name || !values.code) {
      window.SaptariXToast.error("Name and code are required.");
      return;
    }

    const editingRow = table.dataset.editingRowId
      ? body.querySelector(`[data-entity-id='${table.dataset.editingRowId}']`)
      : null;

    if (editingRow) {
      updateInlineRow(editingRow, values);
      window.SaptariXToast.success("Record updated.");
    } else {
      body.insertAdjacentHTML("beforeend", buildInlineRow(values));
      updateInlineCount(table);
      window.SaptariXToast.success("Record created.");
    }

    resetInlineForm(table);
  });

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => initializeCustomSelects(document));
  } else {
    initializeCustomSelects(document);
  }

  const selectObserver = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      mutation.addedNodes.forEach((node) => {
        if (node.nodeType === Node.ELEMENT_NODE) {
          initializeCustomSelects(node);
        }
      });
    });
  });

  selectObserver.observe(document.documentElement, { childList: true, subtree: true });
})();
