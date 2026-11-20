(function () {
  'use strict';

  var fotoPrincipal = document.getElementById('foto-principal');
  var miniaturas = document.querySelectorAll('.miniatura');

  miniaturas.forEach(function (min) {
    min.addEventListener('click', function () {
      if (fotoPrincipal) fotoPrincipal.src = this.dataset.origem;
      miniaturas.forEach(function (m) { m.classList.remove('miniatura--ativa'); });
      this.classList.add('miniatura--ativa');
    });
  });

  var mapaEl = document.getElementById('mapa-anuncio');
  if (mapaEl && typeof L !== 'undefined') {
    var lat = parseFloat(mapaEl.dataset.latitude);
    var lng = parseFloat(mapaEl.dataset.longitude);

    if (!isNaN(lat) && !isNaN(lng)) {
      var mapa = L.map('mapa-anuncio', { scrollWheelZoom: false }).setView([lat, lng], 15);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 18
      }).addTo(mapa);

      L.marker([lat, lng])
        .addTo(mapa)
        .bindPopup(mapaEl.dataset.titulo + '<br><small>' + mapaEl.dataset.bairro + '</small>')
        .openPopup();
    }
  }
})();
