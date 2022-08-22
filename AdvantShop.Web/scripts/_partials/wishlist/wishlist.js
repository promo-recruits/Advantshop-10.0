import WishlistService from './services/wishlistService.js';

const moduleName = `wishlist`;

angular.module(moduleName, [])
    .service('wishlistService', WishlistService);

export default moduleName;