; (function (ng, body) {
    'use strict';

    var HarmonicaTileCtrl = function ($element, $scope, domService) {

        var ctrl = this;

        ctrl.isVisibleTile = true;

        ctrl.$onInit = function () {
            ctrl.links = ctrl.harmonicaCtrl.getLinks();

            ctrl.cssClasses = ctrl.harmonicaCtrl.getCssClassesForTile();

            ctrl.harmonicaCtrl.saveTileScope(ctrl);

            $element[0].addEventListener('mouseenter', function (event) {
                ctrl.tileActive(event);
                $scope.$digest();
            });

            $element[0].addEventListener('mouseleave', function (event) {
                ctrl.tileDeactive(event);
                $scope.$digest();
            });

            $element[0].addEventListener('click', function (event) {
                ctrl.tileClick(event)
                $scope.$digest();
            });
        };

        ctrl.tileActive = function (event) {
            //if (document.body.offsetWidth <= document.body.offsetWidth) {
            //    document.body.style.overflowX = 'hidden';
            //}

            event.stopPropagation();

            ctrl.hoverTileSubmenu = true;
            ctrl.submenuInvert = false;

            //ctrl.checkSubmenuOrientation(submenu);

            ctrl.isVisibleTileSubmenu = true;
        };

        ctrl.tileDeactive = function (event) {

            event.stopPropagation();

            ctrl.hoverTileSubmenu = false;
            ctrl.isVisibleTileSubmenu = false;

            //document.body.style.overflowX = 'auto';
        };


        ctrl.clickOut = function (event) {
            if (domService.closest(event.target, '.js-harmonica-tile') == null) {
                ctrl.hoverTileSubmenu = false;
                ctrl.submenuInvert = false;
                ctrl.isVisibleTileSubmenu = false;
            }
        };


        ctrl.tileClick = function (event) {
            ctrl.isVisibleTileSubmenu === true ? ctrl.tileDeactive(event) : ctrl.tileActive(event);
        };
    }

    HarmonicaTileCtrl.$inject = ['$element', '$scope', 'domService'];

    ng.module('harmonica')
    .controller('HarmonicaTileCtrl', HarmonicaTileCtrl);

})(angular, document.body);