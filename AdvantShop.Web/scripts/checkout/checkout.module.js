﻿import flatpickrModule from '../../vendors/flatpickr/flatpickr.module.js';
import '../../styles/partials/bonus-card.scss';
import '../../styles/partials/order-history-products.scss';
import '../../styles/views/checkout.scss';

import bonusModule from '../_partials/bonus/bonus.module.js';
import addressModule from '../_partials/address/address.module.js';
import buyOneClickModule from '../_partials/buy-one-click/buyOneClick.module.js';
import paymentModule from '../_partials/payment/payment.module.js';
import shippingModule from '../_partials/shipping/shipping.module.js';
import cardsModule from '../_partials/cards/cards.module.js';
import yandexMaps from '../_common/yandexMaps/yandexMaps.module.js';
import authModule from '../auth/auth.module.js';
import loginOpenIdModule from '../../scripts/_partials/login-open-id/loginOpenId.module.js';


import CheckOutCtrl from './controllers/checkoutController.js';
import checkoutService from './services/checkoutService.js';

const moduleName = 'checkout';

angular.module(moduleName, [flatpickrModule, bonusModule, addressModule, buyOneClickModule, paymentModule, shippingModule, cardsModule, yandexMaps, authModule])
    .service('checkoutService', checkoutService)
    .controller('CheckOutCtrl', CheckOutCtrl);

export default moduleName;