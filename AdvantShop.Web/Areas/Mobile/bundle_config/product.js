import appDependency from '../../../scripts/appDependency.js';

import productViewModule from '../../../scripts/_partials/product-view/productView.module.js';

import uiBootstrapModule from '../../../vendors/ui-bootstrap-custom/ui-bootstrap.module.js';

import carouselModule from '../../../scripts/_common/carousel/carousel.module.js';
import ratingModule from '../../../scripts/_common/rating/rating.module.js';
import rotateModule from '../../../scripts/_common/rotate/rotate.module.js';
import videosModule from '../../../scripts/_partials/videos/videos.module.js';


import customOptionsModule from '../../../scripts/_partials/custom-options/customOptions.module.js';
import colorsViewerModule from '../../../scripts/_partials/colors-viewer/colorsViewer.module.js';
import sizesViewerModule from '../../../scripts/_partials/sizes-viewer/sizesViewer.module.js';
import buyOneClickModule from '../../../scripts/_partials/buy-one-click/buyOneClick.module.js';

import tabsModule from '../../../scripts/_common/tabs/tabs.module.js';
import compareModule from '../../../scripts/_partials/compare/compare.module.js';
import reviewsModule from '../../../scripts/_partials/reviews/reviews.module.js';
import shippingModule from '../../../scripts/_partials/shipping/shipping.module.js';

import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';

import '../../../styles/partials/gallery.scss';
import '../../../styles/partials/product-color.scss';
import '../../../styles/partials/properties.scss';
import '../../../styles/partials/bonus-card.scss';
import '../styles/views/product.scss';

import ProductCtrl from '../../../scripts/product/controllers/productController.js';
import productService from '../../../scripts/product/services/productService.js';

import '../../../styles/partials/stickers.scss';

//import countdownModule from '../../../scripts/_common/countdown/countdown.module.js';

import '../../../styles/partials/social-share42.scss';

const moduleName = 'product';

const deps = [
    uiBootstrapModule,
    tabsModule,
    ratingModule,
    carouselModule,
    productViewModule,
    rotateModule,
    compareModule,
    customOptionsModule,
    colorsViewerModule,
    sizesViewerModule,
    reviewsModule,
    shippingModule,
    buyOneClickModule,
    videosModule,
    photoViewerModule
];

angular.module(moduleName, deps)
    .controller('ProductCtrl', ProductCtrl)
    .service('productService', productService);

appDependency.addItem(moduleName);
//appDependency.addItem(countdownModule);

export default moduleName;