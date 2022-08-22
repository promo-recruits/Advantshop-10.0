; (function (ng) {
    'use strict';

    /*
     * <help-trigger class="ng-cloak" data-title="фывфывфывфыв">
            <strong>sdasfasdas</strong><br />
            <i>asdfsdasd</i>
        </help-trigger>
     */
    var increment = 1;

    ng.module('helpTrigger')
        .directive('helpTrigger', ['$sce', '$templateRequest', '$compile', '$templateCache', '$parse', 'urlHelper', function ($sce, $templateRequest, $compile, $templateCache, $parse, urlHelper) {
            return {
                controller: 'HelpTriggerCtrl',
                bindToController: true,
                controllerAs: '$ctrl',
                transclude: true,
                //scope: {
                //    title: '@',
                //    useTemplate: '<?'
                //},
                scope: true,
                link: function (scope, element, attrs, ctrl, transclude) {

                    ctrl.title = attrs.title;
                    ctrl.useTemplate = attrs.useTemplate ? attrs.useTemplate === 'true' : false;
                    ctrl.helpAppendToBody = attrs.helpAppendToBody != null ? $parse(attrs.helpAppendToBody)(scope) : true;
                    ctrl.classes = attrs.classes != null ? $parse(attrs.classes)(scope) : '';
                    

                    $templateRequest(urlHelper.getAbsUrl('areas/admin/content/src/_partials/help-trigger/templates/help-trigger.html', true))
                        .then(function (tpl) {
                            var innerEl = document.createElement('div'),
                                uiPopoverEl,
                                clone,
                                container;
                            ctrl.innerPopoverContentClass = "js-help-trigger-content-" + increment;
                            innerEl.innerHTML = tpl;

                            uiPopoverEl = innerEl.querySelector('.help-trigger-icon');

                            clone = transclude().clone();

                            container = document.createElement('div');

                            for (var i = 0; i < clone.length; i++) {
                                container.appendChild(clone[i]);
                            }
                            uiPopoverEl.setAttribute('popover-class', ctrl.innerPopoverContentClass + " " + ctrl.classes);


                            if (ctrl.useTemplate === true) {
                                uiPopoverEl.setAttribute('uib-popover-template', "'helpTrigger_" + increment + ".html'");
                                $templateCache.put('helpTrigger_' + increment + '.html', container.innerHTML);
                            } else {
                                uiPopoverEl.setAttribute('uib-popover-html', "$ctrl.content");
                                ctrl.content = $sce.trustAsHtml(ng.element('<div />').append(clone).html());
                            }

                            $compile(element.html(innerEl).contents())(scope);

                            increment++;
                        });
                }
            };
        }]);



})(window.angular);