(function () {
document.addEventListener("DOMContentLoaded", () => {
    const wrapper = document.querySelector(".envoltorio-scanner");
    if (!wrapper) return;

    const urlProcessamento = wrapper.dataset.urlProcessamento;
    const idScanner = document.getElementById("area-scanner")?.dataset.idScanner || "qr-reader";

    inicializarScanner(idScanner, (token) => {
        enviarParaProcessamento(token, urlProcessamento);
    });

    document.getElementById("botao-escanear-novamente")?.addEventListener("click", reiniciarScanner);
});

async function enviarParaProcessamento(payload, urlProcessamento) {
    document.getElementById("area-scanner")?.classList.add("is-hidden");
    const areaResultado = document.getElementById("area-resultado");
    areaResultado?.classList.remove("is-hidden");

    definirResultado("⏳", "Processando...", "Validando seu QR Code...", false);

    try {
        const url = `${urlProcessamento}?handler=ProcessarCodigo&payload=${encodeURIComponent(payload)}`;
        const response = await window.requisitarApp(url, {
            method: "POST",
            headers: {
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        const data = await response.json();

        if (data.sucesso) {
            if (data.tipo === "fidelidade") {
                exibirSucessoFidelidade(data);
            } else if (data.tipo === "cashback") {
                exibirSucessoCashback(data);
            } else {
                definirResultado("✅", "Sucesso!", data.mensagem, true);
            }
        } else {
            definirResultado("❌", "Não foi possível processar", data.mensagem, false);
            document.getElementById("cartao-resultado-interno").className = "cartao-resultado error";
        }
    } catch (error) {
        definirResultado("❌", "Erro de conexão", "Não foi possível processar. Verifique sua conexão e tente novamente.", false);
        document.getElementById("cartao-resultado-interno").className = "cartao-resultado error";
    }
}

function exibirSucessoFidelidade(data) {
    const completou = data.completouCartao;
    const emoji = completou ? "🏆" : "✅";
    const titulo = completou ? "Cartão Completo!" : "Carimbo Registrado!";

    document.getElementById("cartao-resultado-interno").className = "cartao-resultado success";
    definirResultado(emoji, titulo, data.mensagem, true);

    document.getElementById("progresso-fidelidade")?.classList.remove("is-hidden");
    document.getElementById("titulo-cartao").textContent = data.tituloCartao || "Cartão Fidelidade";
    document.getElementById("pontos-acumulados").textContent = data.pontosAcumulados;
    document.getElementById("pontos-necessarios").textContent = data.pontosNecessarios;
    document.getElementById("recompensa-texto").textContent = data.recompensa || "";

    const pct = data.pontosNecessarios > 0
        ? Math.min(100, Math.round(data.pontosAcumulados / data.pontosNecessarios * 100))
        : 0;
    const fill = document.getElementById("preenchimento-progresso");
    fill.classList.remove("progress-mini-bar-fill--empty");
    setTimeout(() => {
        fill.style.width = pct + "%";
    }, 100);

    if (completou) {
        document.getElementById("selo-completo")?.classList.remove("is-hidden");
    }
}

function exibirSucessoCashback(data) {
    document.getElementById("cartao-resultado-interno").className = "cartao-resultado success";
    definirResultado("💰", "Cashback Creditado!", data.mensagem, true);
    document.getElementById("cashback-info")?.classList.remove("is-hidden");

    const valor = data.valorCredito || 0;
    document.getElementById("cashback-valor").textContent =
        valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function definirResultado(emoji, titulo, mensagem) {
    document.getElementById("emoji-resultado").textContent = emoji;
    document.getElementById("titulo-resultado").textContent = titulo;
    document.getElementById("msg-resultado").textContent = mensagem;
}

function reiniciarScanner() {
    window.location.reload();
}

})();
