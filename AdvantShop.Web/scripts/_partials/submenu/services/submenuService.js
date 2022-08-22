; (function (ng) {
    'use strict';

    var _currentActiveSubmenuContainer;

    var submenuService = function ($document) {
        const service = this,
            mouseLocs = [];

        service.startSpyMove = function () {
            $document[0].addEventListener('mousemove', memory);
        };

        service.stopSpyMove = function () {
            $document[0].removeEventListener('mousemove', memory);
            mouseLocs.length = 0;
        };

        service.getMouseLocs = function () {
            return mouseLocs;
        };

        service.addCurrentSubmenuContainer = function (submenuContainer) {
            if (_currentActiveSubmenuContainer !== submenuContainer) {
                service.closeAnotherMenu(_currentActiveSubmenuContainer);
                _currentActiveSubmenuContainer = submenuContainer;
            }
        };

        service.closeAnotherMenu = function (currentActiveSubmenuContainer) {
            var _container = currentActiveSubmenuContainer || _currentActiveSubmenuContainer;

            if (_container != null) {
                _container.deactiveAll();
                _container.getBlockOrientation().style.zIndex = 0;
                return _container;
            }        
        };

        function memory(e) {
            mouseLocs.push({ x: e.pageX, y: e.pageY });

            if (mouseLocs.length > 3) {
                mouseLocs.shift();
            }
        };
    };

    angular.module('submenu')
      .service('submenuService', submenuService);

    submenuService.$inject = ['$document'];

})(window.angular);