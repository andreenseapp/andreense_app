window.AppUtils = window.AppUtils || {};

window.AppUtils.Formatadores = (function () {
    const moedaBRL = (valor) => {
        if (valor === null || valor === undefined) return '';
        let num = typeof valor === 'string' ? parseFloat(valor.replace(',', '.')) : valor;
        if (isNaN(num)) return valor;
        
        return num.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    };

    const dataBR = (dataString) => {
        if (!dataString) return '';
        const d = new Date(dataString);
        if (isNaN(d.getTime())) return dataString;
        
        return d.toLocaleDateString('pt-BR');
    };
    
    const dataHoraBR = (dataString) => {
        if (!dataString) return '';
        const d = new Date(dataString);
        if (isNaN(d.getTime())) return dataString;
        
        return d.toLocaleString('pt-BR');
    };

    return {
        moedaBRL,
        dataBR,
        dataHoraBR
    };
})();
