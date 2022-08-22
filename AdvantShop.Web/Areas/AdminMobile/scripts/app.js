; (function (ng, body) {

    'use strict';

    /*убираем BFCache в FF*/
    window.addEventListener('unload', function () { });

    ng.module('app', [
                      //'templatesCache',
                      'ngCookies',
                      'home',
                      'graphics',
                      'windowExt',
                      'ordersList',
                      'orderItem',
                      'tasksView',
                      'taskView',
                      'leads',
                      'sidebar',
                      'angular-flatpickr',
                      'dom'
    ])
        .config(['$provide', '$compileProvider', '$cookiesProvider', '$httpProvider', '$localeProvider', 'ngFlatpickrDefaultOptions', function ($provide, $compileProvider, $cookiesProvider, $httpProvider, $localeProvider , ngFlatpickrDefaultOptions) {

           var date = new Date(),
               currentYear = date.getFullYear();

           date.setFullYear(currentYear + 1);

           //#region compile debug
           $compileProvider.debugInfoEnabled(false);
           //#endregion

           //#region set cookie expires
           $cookiesProvider.defaults.expires = date;
           $cookiesProvider.defaults.path = '/';

           if (window.location.hostname !== 'localhost' && window.location.hostname !== 'server' &&
              !/^(?!0)(?!.*\.$)((1?\d?\d|25[0-5]|2[0-4]\d)(\.|$)){4}$/.test(window.location.hostname)) {
               $cookiesProvider.defaults.domain = '.' + window.location.hostname.replace('www.', '');
           }

           //#endregion

           //#region ie10 bug validation

           $provide.decorator('$sniffer', ['$delegate', function ($sniffer) {
               var msie = parseInt((/msie (\d+)/.exec(angular.lowercase(navigator.userAgent)) || [])[1], 10);
               var _hasEvent = $sniffer.hasEvent;
               $sniffer.hasEvent = function (event) {
                   if (event === 'input' && msie === 10) {
                       return false;
                   }
                   _hasEvent.call(this, event);
               }
               return $sniffer;
           }]);

           //#endregion

           //#region prepera ajax url in absolute path
           var basePath = document.getElementsByTagName('base')[0].getAttribute('href'),
               regex = new RegExp('^(?:[a-z]+:)?//', 'i');


           $httpProvider.useApplyAsync(true);

           var tokens = document.getElementsByName("__RequestVerificationToken");
           if (tokens.length > 0) {
               $httpProvider.defaults.headers.post['__RequestVerificationToken'] = tokens[0].value;
           }
           $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

           $httpProvider.interceptors.push(function () {
               return {
                   'request': function (config) {

                       var urlOld = config.url,
                           template;

                       if (regex.test(config.url) === false) {

                           if (config.url.charAt(0) === '/') {
                               config.url = config.url.substring(1);
                           }

                           config.url = basePath + config.url;
                       }

                       //for templates
                       if (urlOld != config.url && ng.isObject(config.cache) && config.cache.get(urlOld) != null) {
                           template = config.cache.get(urlOld);
                           config.cache.remove(urlOld);
                           config.cache.put(config.url, template);
                       }

                       //config.headers['Pragma'] = 'no-cache';
                       //config.headers['Expires'] = '-1';
                       //config.headers['Cache-Control'] = 'no-cache, no-store';

                       return config;
                   }
               };
           });

           //#endregion
            var localeId = $localeProvider.$get().id;

           ngFlatpickrDefaultOptions.locale = localeId.split('-')[0];
           ngFlatpickrDefaultOptions.disableMobile = true;
       }]);

})(window.angular, document.body);


