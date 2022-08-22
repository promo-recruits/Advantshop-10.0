/* @ngInject */
function ProductViewModeCtrl($attrs, $element, productViewService, viewList, viewPrefix) {
    const ctrl = this;
    ctrl.$onInit = function (){
        ctrl.isMobile = $attrs.isMobile === 'true';
        ctrl.currentViewList = viewList[$attrs.viewListName || 'desktop'];
        ctrl.currentViewPrefix = viewPrefix[$attrs.viewListName || 'desktop'];
        ctrl.defaultViewMode = $attrs.defaultViewMode;

        if (ctrl.isMobile === true) {
            ctrl.viewName = productViewService.getViewFromCookie('mobile_viewmode', ctrl.currentViewList, ctrl.defaultViewMode);
            $element[0].classList.add('products-view-' + ctrl.currentViewPrefix + ctrl.viewName);
        } else {
            ctrl.viewName = $attrs.current;
        }

        productViewService.addCallback('setView', onChangeMode);

        function onChangeMode(view) {
            view.viewList.forEach(function (item) {
                $element[0].classList.remove('products-view-' + ctrl.currentViewPrefix + item);
            });

            $element[0].classList.add('products-view-' + ctrl.currentViewPrefix + view.viewName);

            ctrl.viewName = view.viewName;
        }
    }
};

export default ProductViewModeCtrl;