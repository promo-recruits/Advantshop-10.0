import './styles/shipping.scss';

import ShippingService from './services/shipping.service.js';
import ShippingListCtrl from './controllers/shippingListController.js';
import ShippingVariantsCtrl from './controllers/shippingVariantsController.js';
import ShippingTemplateCtrl from './controllers/shippingTemplateController.js';
import {
    shippingListDirective,
    shippingTemplateDirective,
    shippingVariantsDirective
} from './directives/shippingDirectives.js';

const moduleName = 'shipping';

angular.module(moduleName, [])
    .service('shippingService', ShippingService)
    .controller('ShippingListCtrl', ShippingListCtrl)
    .controller('ShippingTemplateCtrl', ShippingTemplateCtrl)
    .controller('ShippingVariantsCtrl', ShippingVariantsCtrl)
    .directive('shippingList', shippingListDirective)
    .directive('shippingTemplate', shippingTemplateDirective)
    .directive('shippingVariants', shippingVariantsDirective);

export default moduleName;