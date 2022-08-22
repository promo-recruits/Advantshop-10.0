import './styles/orderHistory.scss';

import OrderHistoryCtrl from './controllers/orderHistoryController.js';
import {
    orderHistoryDirective,
    orderHistoryItemsDirective,
    orderHistoryDetailsDirective,
    orderPayDirective
} from './directives/orderDirectives.js';
import orderService from './services/orderService.js';
import OrderPayCtrl from "./controllers/orderPayController";

const moduleName = 'order';

angular.module('order', [])
    .service('orderService', orderService)
    .controller('OrderHistoryCtrl', OrderHistoryCtrl)
    .controller('OrderPayCtrl', OrderPayCtrl)
    .directive('orderHistory', orderHistoryDirective)
    .directive('orderHistoryItems', orderHistoryItemsDirective)
    .directive('orderHistoryDetails', orderHistoryDetailsDirective)
    .directive('orderPay', orderPayDirective);

export default moduleName;