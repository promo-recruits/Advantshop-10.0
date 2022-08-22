; (function (ng) {
    'use strict';

    ng.module('blocksConstructor')
        .directive('blocksConstructorMain', function () {
            return {
                restrict: 'EA',
                scope: true,
                controller: 'BlocksConstructorMainCtrl',
                controllerAs: 'blocksConstructorMain',
                bindToController: true
            }
        });


    ng.module('blocksConstructor')
        .directive('blocksConstructorSelect', ['$parse', '$document', 'blocksConstructorService', function ($parse, $document, blocksConstructorService) {
            return {
                restrict: 'EA',
                link: function (scope, element, attrs, ctrl) {
                    element.on('click', function () {

                        var blocksConstructorMain = blocksConstructorService.getMain();

                        blocksConstructorMain.hideModals();

                        var keyFn = function (event) {
                            if (event.keyCode === 27) {
                                blocksConstructorService.deactivateSelectMode();
                                blocksConstructorMain.showModals();
                                $document[0].removeEventListener('keyup', keyFn);
                                scope.$apply();
                            }
                        };

                        $document[0].addEventListener('keyup', keyFn);

                        blocksConstructorService.activateSelectMode()
                            .then(function (blockSelected) {
                                blocksConstructorMain.showModals();
                                if (attrs.blocksConstructorSelect != null && attrs.blocksConstructorSelect.length > 0) {
                                    $parse(attrs.blocksConstructorSelect)(scope, { blockSelected: blockSelected });
                                }
                            });
                    });
                }
            };
        }]);

    ng.module('blocksConstructor')
        .directive('blocksConstructorContainer', function () {
            return {
                require: {
                    blocksConstructorMain: '^^blocksConstructorMain'
                },
                restrict: 'EA',
                scope: true,
                controller: 'BlocksConstructorContainerCtrl',
                controllerAs: 'blocksConstructorContainer',
                bindToController: true
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructor', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/blocksConstructor.html',
            controller: 'BlocksConstructorCtrl',
            transclude: true,
            bindings: {
                landingpageId: '@',
                blockId: '<?',
                name: '@',
                type: '@',
                sortOrder: '<',
                isShowReverse: '<?',
                isShowOptions: '<?',
                templateCustom: '<?'
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructorModalNewBlock', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/modalNew.html',
            controller: 'BlocksConstructorNewBlockCtrl',
            bindings: {
                modalData: '<',
                onApply: '&',
                onApplyByCategories: '&',
                onRemoveByCategories: '&',
                experemental: '<?'
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructorModalSettingsBlock', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/modalSettings.html',
            controller: 'BlocksConstructorSettingsBlockCtrl',
            bindings: {
                modalData: '<',
                onApply: '&',
                onFixedBackground: '&',
                onCancel: '&',
                inProgress: '=?'
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructorModalAddSubblock', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/modalAddSubblock.html',
            controller: 'BlocksConstructorAddSubblockCtrl',
            bindings: {
                modalData: '<',
                onApply: '&',
                onCancel: '&',
                onInit: '&',
            }
        });

    ng.module('blocksConstructor')
        //.component('blocksConstructorButtonSettings', {
        //    templateUrl: function ($element, $attrs) {
        //        return $attrs.linkMode === "true" ? 'areas/landing/frontend/_common/blocks-constructor/templates/linkSettings.html' : "areas/landing/frontend/_common/blocks-constructor/templates/buttonSettings.html"
        //    },
        //    templateUrl: "areas/landing/frontend/_common/blocks-constructor/templates/buttonSettings.html",
        //    controller: 'BlocksConstructorButtonSettingsCtrl',
        //    bindings: {
        //        buttonOptions: '=',
        //        isVisibility: '=',
        //        commonOptions: '=?',
        //        lpId: '<?',
        //        lpBlockId: '<?',
        //        formExclude: '<?',
        //        paymentExclude: '<?',
        //        formSettings: '=?',
        //        linkMode: '<?'
        //    }
        //});
        .directive('blocksConstructorButtonSettings', function () {
            return {
                templateUrl: function (element, attrs) {
                    return attrs.linkMode === 'true' ? 'areas/landing/frontend/_common/blocks-constructor/templates/linkSettings.html' : 'areas/landing/frontend/_common/blocks-constructor/templates/buttonSettings.html';
                },
                controller: 'BlocksConstructorButtonSettingsCtrl',
                controllerAs: '$ctrl',
                bindToController: true,
                scope: {
                    buttonOptions: '=',
                    isVisibility: '=',
                    commonOptions: '=?',
                    lpId: '<?',
                    lpBlockId: '<?',
                    formExclude: '<?',
                    linkExclude: '<?',
                    hideFormSettings: '<?',
                    paymentExclude: '<?',
                    formSettings: '=?',
                    linkMode: '<?'
                }
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructorButtonSettingsModalTrigger', {
            controller: 'BlocksConstructorButtonSettingsModalCtrl',
            bindings: {
                buttonOptions: '=',
                isVisibility: '=',
                commonOptions: '=?',
                lpId: '<?',
                formExclude: '<?',
                hideFormSettings: '<?',
                paymentExclude: '<?',
                onApply: '&',
                linkMode: '<?'
            }
        });

    ng.module('blocksConstructor')
        .component('blocksConstructorFormSettings', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/formSettings.html',
            controller: 'BlocksConstructorFormSettingsCtrl',
            transclude: true,
            bindings: {
                commonOptions: '=',
                buttonOptions: '=',
                formSettings: '=?'
            }
        });

    ng.module('blocksConstructor')
        .directive('blocksConstructorFormSettingsTab', function () {
            return {
                require: {
                    blocksConstructorFormSettings: '^blocksConstructorFormSettings'
                },
                scope: true,
                controller: 'BlocksConstructorFormSettingsTabCtrl',
                controllerAs: 'blocksConstructorFormSettingsTab',
                bindToController: true
            };
        });

    ng.module('blocksConstructor')
        .component('blockConstructorFormSettingsPlaceholder', {
            controller: ['$compile', '$element', function ($compile, $element) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    var child = ng.element(document.createElement('div'));

                    child.html(ctrl.content);

                    $element.append(child);

                    $compile(child)(ctrl.scope);
                };
            }],
            transclude: true,
            bindings: {
                content: '<',
                scope: '<'
            }
        });

    ng.module('blocksConstructor')
        .component('blockConstructorCountdown', {
            controller: 'SubBlockCountdownViewConstructorController',
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/countdown.html',
            bindings: {
                date: '<',
                onChange: '&'
            }
        });

    ng.module('blocksConstructor')
        .component('blockConstructorGoals', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/goals.html',
            bindings: {
                yaMetrikaEventName: '=',
                gaEventCategory: '=',
                gaEventAction: '=',
                goalsEnabled: '='
            }
        });

    ng.module('blocksConstructor')
        .component('blockConstructorUpsell', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/partials/_upsell.html',
            bindings: {
                value: '=',
                items: '<'
            }
        });

    ng.module('blocksConstructor')
        .component('blockConstructorMenu', {
            templateUrl: 'areas/landing/frontend/_common/blocks-constructor/templates/partials/_menu.html',
            bindings: {
                modalData: '<',
                menuData: '<',
                hasPicture: '<?',
                rangeOptions: '<',
                onSelectBlock: '&'
            }
        });

})(window.angular);