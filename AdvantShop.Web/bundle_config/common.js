import appDependency from '../scripts/appDependency.js';

import '../fonts/fonts.store.css';
import '../fonts/fonts.icons.css';
import '../vendors/flexboxgrid/flexboxgrid.scss';
import '../vendors/flexboxgrid/ext/flexboxgridExt.scss';
import '../node_modules/normalize.css/normalize.css';

import '../styles/general.scss';

import '../styles/common/buttons.scss';
import '../styles/common/headers.scss';
import '../styles/common/icons.scss';
import '../styles/common/custom-input.scss';
import '../styles/common/inputs.scss';
import '../styles/common/block.scss';
import '../styles/common/forms.scss';
import '../styles/common/links.scss';
import '../styles/common/tables.scss';
import '../styles/common/validation.scss';
import '../styles/common/connector.scss';
import '../styles/common/social.scss';
import '../styles/common/social-widgets.scss';
import '../styles/common/accordion-css.scss';
import '../styles/common/sidebar.scss';

import '../styles/partials/bonus-card.scss';
import '../styles/partials/header.scss';
import '../styles/partials/menu-dropdown.scss';
import '../styles/partials/menu-header.scss';
import '../styles/partials/menu-general.scss';
import '../styles/partials/price.scss';
import '../styles/partials/toolbar-top.scss';
import '../styles/partials/footer.scss';
import '../styles/partials/footer-menu.scss';
import '../styles/partials/recentlyView.scss';
import '../styles/partials/gift.scss';
import '../styles/partials/toolbar-bottom.scss';
import '../styles/partials/stickers.scss';


import '../node_modules/jquery/dist/jquery.js';
import '../vendors/jquery/jquery.passive.js';

import '../node_modules/angular/angular.js';
import '../node_modules/angular-cookies/angular-cookies.js';
appDependency.addItem('ngCookies');

import '../node_modules/angular-sanitize/angular-sanitize.js';
appDependency.addItem('ngSanitize');

import '../node_modules/angular-translate/dist/angular-translate.js';
appDependency.addItem('pascalprecht.translate');

import '../vendors/qazy/qazyOpt.directive.js';
appDependency.addItem('qazy');

import '../node_modules/angularjs-toaster/toaster';
import '../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import '../node_modules/sweetalert2/dist/sweetalert2.css';
import '../vendors/sweet-alert/ext/styles/sweet-alert2.ext.css';
import '../node_modules/sweetalert2/dist/sweetalert2.js';

Sweetalert2.mixin({
    cancelButtonText: 'Отмена',
    confirmButtonText: 'ОK',
    allowOutsideClick: false,
    buttonsStyling: false,
    confirmButtonClass: 'btn btn-small btn-buy',
    cancelButtonClass: 'btn btn-confirm btn-small',
    useRejections: true
});

import '../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../node_modules/oclazyload/dist/ocLazyLoad.min.js';
import '../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import '../vendors/autofocus/autofocus.js';
appDependency.addItem('autofocus');

import maskModule from '../scripts/_common/mask/mask.module.js'
appDependency.addItem(maskModule);

import '../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

import '../vendors/magnific-popup/styles/main.css';
import '../vendors/magnific-popup/magnific-popup.min.js';

import '../scripts/_partials/magnificPopupWrapper/styles/mfp-custom.scss';
import '../scripts/_partials/magnificPopupWrapper/magnificPopupWrapper.js';
import '../scripts/_partials/magnificPopupWrapper/directives/magnificPopupWrapperDirective.js';
appDependency.addItem('magnificPopup');


import * as Ladda from '../node_modules/ladda/dist/ladda-themeless.min.css';

import '../node_modules/ladda/js/ladda.js';
import '../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import '../scripts/search/search.module.js';
appDependency.addItem('search');

import '../scripts/_partials/submenu/submenu.module.js';
appDependency.addItem('submenu');

import '../scripts/_partials/rootMenu/rootMenu.module.js';
appDependency.addItem('rootMenu');

import '../scripts/_partials/cart/cart.module.js';
appDependency.addItem('cart');

import '../scripts/_partials/zone/zone.module.js';
appDependency.addItem('zone');

import '../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import '../scripts/_common/autocompleter/autocompleter.module.js';
appDependency.addItem('autocompleter');

import compareModule from '../scripts/_partials/compare/compare.module.js';
appDependency.addItem(compareModule);

import '../scripts/_common/harmonica/harmonica.module.js';
appDependency.addItem('harmonica');

import '../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import '../scripts/_common/popover/popover.module.js';
import '../scripts/_common/popover/popover.module.js';
appDependency.addItem('popover');

import '../scripts/_common/readmore/readmore.module.js';
appDependency.addItem('readmore');

import '../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

import '../scripts/_common/scrollToTop/scrollToTop.module.js';
appDependency.addItem('scrollToTop');

import '../scripts/_common/transformer/transformer.module.js';
appDependency.addItem('transformer');

import '../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import '../scripts/_common/module/module.module.js';
appDependency.addItem('module');

import '../scripts/_common/validation/validation.module.js';
appDependency.addItem('validation');

import '../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

import '../scripts/_common/mouseoverClassToggler/mouseoverClassToggler.module.js';
appDependency.addItem('mouseoverClassToggler');

import '../scripts/_common/hunter/hunter.module.js';

//import '../styles/partials/mobileOverlap.scss';
//import '../scripts/_mobile/mobileOverlap.js';
//appDependency.addItem('mobileOverlap');

//import '../scripts/_partials/cookies-policy/cookiesPolicy.module.js';
//appDependency.addItem('cookiesPolicy');

import '../scripts/_partials/wishlist/wishlist.module.js';
appDependency.addItem('wishlist');

//import '../scripts/_partials/currency/currency.module.js';
//appDependency.addItem('currency');

//import countdownModule from '../scripts/_common/countdown/countdown.module.js';
//appDependency.addItem(countdownModule);

import lozadAdvModule from '../scripts/_common/lozad-adv/lozadAdv.module.js';
appDependency.addItem(lozadAdvModule);

//to do добавил костыли для модулей
//storereviews page

//import reviewsModule from '../scripts/_partials/reviews/reviews.module.js';
//appDependency.addItem(reviewsModule);

//import ratingModule from '../scripts/_common/rating/rating.module.js';
//appDependency.addItem(ratingModule);

////shippingpayment 
//import checkoutModule from '../scripts/checkout/checkout.module.js';
//appDependency.addItem(checkoutModule);

////blog
//import newsModule from '../scripts/news/news.module.js';
//appDependency.addItem(newsModule);
////end to do

import sidebarsContainerModule from './sidebarsContainer.js';
appDependency.addItem(sidebarsContainerModule); //for templates which use it

import '../styles/snippets.scss';

import '../scripts/app.js';

//Для совместимости с 4.0
import '../styles/theme.scss';
//Для совместимости с 4.0

import breadcrumbsModule from '../scripts/_common/breadCrumbs/breadCrumbs.module.js';

appDependency.addItem(breadcrumbsModule);

//import setCssCustomPropsModule from '../scripts/_common/setCssCustomProps/setCssCustomProps.module.js'; // НЕ УДАЛЯТЬ используется в Modern шаблоне (удалить из шаблона если добавлять в движок)
//appDependency.addItem(setCssCustomPropsModule); // фича добавляет своства элемента в css переменную