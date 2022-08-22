; (function (ng) {
    'use strict';

    var yandexMapsService = function (urlHelper, $q) {
        var service = this,
            defer = null;

        // дополнительная функция проверки признаков что янекс.карта уже как-то загружена
        service.isLoadedYandexMap = function () {
            return window.yandexMapLoaded || window.ymaps || document.querySelector('script[src*="api-maps.yandex.ru"]') != null;
        };

        service.loadYandexMap = function (params) {
            if (!defer) {
                defer = $q.defer();
            }

            if (window.yandexMapLoaded) {
                // загрузка api карт уже вызвана
                // defer.resolve() будет вызван по загрузке скрипта
            } else if (window.ymaps) {
                // api карт уже прогружен
                defer.resolve();
            } else {
                if (document.querySelector('script[src*="api-maps.yandex.ru"]') != null) {
                    // уже кем-то запущена загрузка api карт
                    service.waitingInitYmaps();
                } else {
                    // сразу устанавливаем флаг, чтобы больше небыло попыток загрузить
                    window.yandexMapLoaded = true;

                    var script = document.createElement('script');
                    script.onload = function () { service.waitingInitYmaps(); };
                    script.src = 'https://api-maps.yandex.ru/2.1/?' + urlHelper.paramsToString(ng.extend({ lang: 'ru-RU' }, params || {}));
                    document.body.appendChild(script);
                }
            }

            return defer.promise;
        };

        service.waitingInitYmaps = function () {
            // дожидаемся инициализации ymaps
            var delay = 50;
            var maxCount = 3000 / delay;// ждем максимум 3сек
            var countTimer = 0;

            setTimeout(function tick() {
                if (window.ymaps) {
                    defer.resolve();
                    return;
                }

                countTimer++;
                if (countTimer < maxCount) {
                    setTimeout(tick, delay);
                }
            }, delay);
        };

    };

    yandexMapsService.$inject = ['urlHelper', '$q'];

    ng.module('yandexMaps', [])
        .service('yandexMapsService', yandexMapsService);

})(window.angular);