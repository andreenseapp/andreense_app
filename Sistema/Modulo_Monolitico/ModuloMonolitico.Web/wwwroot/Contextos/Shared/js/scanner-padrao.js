function inicializarScanner(idElemento, aoSucesso) {
        const html5QrcodeScanner = new Html5QrcodeScanner(
            idElemento, 
            { 
                fps: 10, 
                qrbox: {width: 250, height: 250},
                aspectRatio: 1.0
            },
            
        html5QrcodeScanner.render((decodedText, decodedResult) => {
            html5QrcodeScanner.clear().then(() => {
                aoSucesso(decodedText, decodedResult);
            });
        });
        
        return html5QrcodeScanner;
    }

