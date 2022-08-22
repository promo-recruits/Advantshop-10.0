import '../../styles/partials/bonus-card.scss';

import cardsModule from '../_partials/cards/cards.module.js';
import buyOneClickModule from '../_partials/buy-one-click/buyOneClick.module.js';

//для модуля AdvantShop.Module.RelatedProductsInShoppingCart
import carouselModule from '../_common/carousel/carousel.module.js';
import productsCarouselModule from '../_partials/products-carousel/productsCarousel.module.js';
import productViewModule from '../_partials/product-view/productView.module.js';
//конец зависимости модуля

import CartPageCtrl from './controllers/cartPageController.js';

const moduleName = 'cartPage';

angular.module(moduleName, [cardsModule, buyOneClickModule, carouselModule, productsCarouselModule, productViewModule])
    .controller('CartPageCtrl', CartPageCtrl);

export default moduleName;

