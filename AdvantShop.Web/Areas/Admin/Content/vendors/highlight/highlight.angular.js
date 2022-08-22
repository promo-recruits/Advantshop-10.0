(function (ng) {
    'use strict';

    ng.module('highlight', ['oc.lazyLoad'])
        .directive('highlight', ['$ocLazyLoad', 'urlHelper', function ($ocLazyLoad, urlHelper) {
            return {
                link: function (scope, element, attrs) {
                    $ocLazyLoad.load('../areas/admin/content/vendors/highlight/styles/vs.css')
                        .then(function () {
                            var code = element[0];
                            var worker = new Worker(urlHelper.getAbsUrl('../areas/admin/content/vendors/highlight/highlightWorker.js'));
                            worker.onmessage = function (event) { code.innerHTML = event.data; };
                            worker.postMessage({ scriptSrc: urlHelper.getAbsUrl('../areas/admin/content/vendors/highlight/highlight.pack.js'), code: code.textContent });
                        });
                }
            };
        }]);
})(window.angular);