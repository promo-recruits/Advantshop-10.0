; (function (ng) {
    'use strict';
    
    ng.module('readmore')
      .component('readmore', {
          controller: 'ReadmoreCtrl',
          template: ['$element', '$attrs', function ($element, $attrs) {
              return $element.closest('[data-inplace-rich]').length == 0
                  ? '<div data-ng-class="{\'readmore-expanded\' : $ctrl.expanded, \'readmore-collapsed\' : !$ctrl.expanded}"><div class="readmore-content" data-ng-style="{maxHeight: $ctrl.maxHeight, transitionDuration: $ctrl.speed}"><div class="js-readmore-inner-content" ' + ($attrs.content == null ? 'data-ng-transclude' : 'ng-bind-html="$ctrl.content"') + '></div></div><div class="readmore-controls" data-ng-if="$ctrl.isActive"><a class="readmore-link" href="" data-ng-click="$ctrl.switch($ctrl.expanded)">{{$ctrl.text | translate}}</a></div></div>'
                  : '';
          }],
          transclude: true,
          bindings: {
              expanded: '<?',
              maxHeight: '<?',
              content: '<?',
              speed: '@',
              moreText: '@',
              lessText: '@',
          }
      });

})(window.angular);