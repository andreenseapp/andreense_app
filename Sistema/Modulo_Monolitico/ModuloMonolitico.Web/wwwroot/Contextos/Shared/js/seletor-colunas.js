document.addEventListener("DOMContentLoaded", function () {
    let colunasVisiveis = null;
    try {
        if (window.PreferenciasTabelaJson) {
            colunasVisiveis = JSON.parse(window.PreferenciasTabelaJson);
        }
    } catch (e) {
        console.error("Falha ao parsear AppPreferenciasTabelaJson", e);
    }

    document.querySelectorAll("table[data-seletor-colunas='true']").forEach(table => {
        const idTabela = table.id || 'table-' + Math.random().toString(36).substr(2, 9);
        table.id = idTabela;

        const headers = Array.from(table.querySelectorAll("th"));
        if (headers.length === 0) return;

        const container = document.createElement("div");
        container.className = "seletor-colunas-container";
        container.style.position = "relative";
        container.style.display = "inline-block";
        container.style.marginBottom = "16px";

        const botao = document.createElement("button");
        botao.type = "button";
        botao.className = "botao btn-outline-secondary btn-sm";
        botao.style.cssText = "background: white; border: 1px solid #cbd5e1; border-radius: 6px; padding: 6px 12px; font-size: 13px; font-weight: 600; color: #475569; cursor: pointer; transition: all 0.2s;";
        botao.innerHTML = '<i class="fa-solid fa-table-columns"></i> Colunas';
        
        botao.onmouseover = () => botao.style.background = "#f8fafc";
        botao.onmouseout = () => botao.style.background = "white";
        botao.onclick = () => menu.style.display = menu.style.display === "none" ? "block" : "none";

        const menu = document.createElement("div");
        menu.className = "dropdown-menu shadow-sm";
        menu.style.display = "none";
        menu.style.position = "absolute";
        menu.style.top = "100%";
        menu.style.left = "0";
        menu.style.zIndex = "1000";
        menu.style.background = "white";
        menu.style.border = "1px solid #e2e8f0";
        menu.style.borderRadius = "8px";
        menu.style.padding = "12px";
        menu.style.minWidth = "220px";
        menu.style.marginTop = "4px";
        menu.style.boxShadow = "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)";

        const title = document.createElement("div");
        title.innerText = "Configurar Colunas";
        title.style.fontSize = "12px";
        title.style.fontWeight = "700";
        title.style.color = "#64748b";
        title.style.textTransform = "uppercase";
        title.style.marginBottom = "12px";
        title.style.borderBottom = "1px solid #e2e8f0";
        title.style.paddingBottom = "8px";
        menu.appendChild(title);

        headers.forEach((th, index) => {
            if (th.innerText.trim() === "") return; // Ignora colunas sem texto (ex: ações)
            if (th.hasAttribute("data-coluna-fixa")) return; // Ignora colunas que não podem ser ocultadas

            const label = document.createElement("label");
            label.style.display = "flex";
            label.style.alignItems = "center";
            label.style.cursor = "pointer";
            label.style.marginBottom = "8px";
            label.style.fontSize = "13px";
            label.style.color = "#334155";
            label.style.userSelect = "none";

            const checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.checked = true;
            checkbox.style.marginRight = "10px";
            checkbox.style.cursor = "pointer";
            checkbox.dataset.colIndex = index;

            const chaveLocal = `cols_${window.location.pathname}_${idTabela}_${index}`;
            let estaVisivel = true;

            if (colunasVisiveis !== null && Array.isArray(colunasVisiveis)) {
                estaVisivel = colunasVisiveis.includes(index);
            } else {
                const estadoSalvo = localStorage.getItem(chaveLocal);
                if (estadoSalvo === "hidden") estaVisivel = false;
            }

            if (!estaVisivel) {
                checkbox.checked = false;
                alternarColuna(table, index, false);
            }

            checkbox.onchange = (e) => {
                const checked = e.target.checked;
                alternarColuna(table, index, checked);
                localStorage.setItem(chaveLocal, checked ? "visible" : "hidden");

                salvarPreferenciaNoServidor(menu, headers);
            };

            label.appendChild(checkbox);
            label.appendChild(document.createTextNode(th.innerText.trim()));
            menu.appendChild(label);
        });

        container.appendChild(botao);
        container.appendChild(menu);

        table.parentNode.insertBefore(container, table);

        document.addEventListener("click", (e) => {
            if (!container.contains(e.target)) {
                menu.style.display = "none";
            }
        });
    });

    function alternarColuna(table, index, estaVisivel) {
        const headers = table.querySelectorAll("thead th");
        if (headers[index]) {
            headers[index].style.display = estaVisivel ? "" : "none";
        }
        
        const rows = table.querySelectorAll("tbody tr, tfoot tr");
        rows.forEach(row => {
            const cells = Array.from(row.children); 
            if (cells[index]) {
                cells[index].style.display = estaVisivel ? "" : "none";
            }
        });
    }

    let atrasoSalvar = null;
    function salvarPreferenciaNoServidor(menu, headers) {
        clearTimeout(atrasoSalvar);
        atrasoSalvar = setTimeout(() => {
            const visibleCols = [];
            headers.forEach((h, i) => {
                const chk = menu.querySelector(`input[data-col-index='${i}']`);
                if (chk && chk.checked) visibleCols.push(i);
                if (!chk) visibleCols.push(i); 
            });

            window.requisitarApp('/api/preferenciatela/salvar', {
                method: 'POST',
                headers: { 
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({
                    TelaIdentificador: window.IdentificadorTela || window.location.pathname,
                    ColunasVisiveisJson: JSON.stringify(visibleCols)
                })
            }).catch(err => console.error("Erro ao salvar preferência no servidor:", err));
        }, 800); // debounce
    }
});
