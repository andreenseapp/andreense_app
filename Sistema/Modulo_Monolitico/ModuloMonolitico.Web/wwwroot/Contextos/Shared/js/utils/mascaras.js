window.AppUtils = window.AppUtils || {};

window.AppUtils.Mascaras = (function () {
    // === Core Mask Functions ===

    const cpf = (v) => {
        v = v.replace(/\D/g, "");
        if (v.length > 11) v = v.slice(0, 11);
        v = v.replace(/(\d{3})(\d)/, "$1.$2");
        v = v.replace(/(\d{3})(\d)/, "$1.$2");
        v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
        return v;
    };

    const cnpj = (v) => {
        v = v.replace(/\D/g, "");
        if (v.length > 14) v = v.slice(0, 14);
        v = v.replace(/^(\d{2})(\d)/, "$1.$2");
        v = v.replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3");
        v = v.replace(/\.(\d{3})(\d)/, ".$1/$2");
        v = v.replace(/(\d{4})(\d)/, "$1-$2");
        return v;
    };

    const documento = (v) => {
        const apenasDigitos = v.replace(/\D/g, "");
        if (apenasDigitos.length <= 11) return cpf(v);
        return cnpj(v);
    };

    const cep = (v) => {
        v = v.replace(/\D/g, "");
        if (v.length > 8) v = v.slice(0, 8);
        v = v.replace(/(\d{5})(\d)/, "$1-$2");
        return v;
    };

    const telefone = (v) => {
        v = v.replace(/\D/g, "");
        if (v.length > 11) v = v.slice(0, 11);
        if (v.length > 10) {
            v = v.replace(/^(\d{2})(\d{5})(\d{4}).*/, "($1) $2-$3");
        } else if (v.length > 6) {
            v = v.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3");
        } else if (v.length > 2) {
            v = v.replace(/^(\d{2})(\d{0,5})/, "($1) $2");
        }
        return v;
    };

    const moeda = (v) => {
        v = v.replace(/\D/g, "");
        if (v === "") return "";
        let num = (parseInt(v, 10) / 100).toFixed(2);
        return num.replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    };

    const numero = (v) => {
        return v.replace(/\D/g, "");
    };

    // === Binding Automático ===

    const applyMask = (e) => {
        const type = e.target.getAttribute('data-mascara');
        let val = e.target.value;
        
        switch (type) {
            case 'cpf': val = cpf(val); break;
            case 'cnpj': val = cnpj(val); break;
            case 'documento': val = documento(val); break;
            case 'cep': val = cep(val); break;
            case 'telefone': 
            case 'celular': val = telefone(val); break;
            case 'moeda': 
            case 'dinheiro': val = moeda(val); break;
            case 'numero': val = numero(val); break;
        }
        
        e.target.value = val;
    };

    const bindAll = (container = document) => {
        const inputs = container.querySelectorAll('[data-mascara]:not([data-mascara-bound])');
        inputs.forEach(input => {
            input.setAttribute('data-mascara-bound', 'true');
            input.addEventListener('input', applyMask);
            
            // Format initial value if exists
            if (input.value) {
                applyMask({ target: input });
            }
        });
    };

    // Auto-bind on load
    window.addEventListener('DOMContentLoaded', () => bindAll());

    // Expose functions
    return {
        cpf,
        cnpj,
        documento,
        cep,
        telefone,
        moeda,
        numero,
        bindAll
    };
})();
