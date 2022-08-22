const HTTP = new WeakMap();
const countObj = {};

class WishlistService {
    /* @ngInject */
    constructor($http,) {
        HTTP.set(this, $http);
    }

    add(offerId, state) {
        return HTTP.get(this).post(`wishlist/wishlistadd`, { offerId: offerId, rnd: Math.random() }).then((response) => {
            countObj.count = response.data.Count;
            $(document).trigger(`add_to_wishlist`);
            this.changeWishlistControlState(offerId, state);
            return response.data;
        });
    }

    remove(offerId, state) {
        return HTTP.get(this).post(`wishlist/wishlistremove`, { offerId: offerId, rnd: Math.random() }).then((response) => {

            countObj.count = response.data.Count;
            this.changeWishlistControlState(offerId, state);
            return response.data;
        });
    }

    getCountObj() {
        return countObj;
    }

    getStatus(offerId) {
        return HTTP.get(this).get(`/wishlist/getstatus`, { params: { offerId: offerId, rnd: Math.random() } }).then((response) => {
            return response.data;
        });
    }

    addWishlistScope(id, ctrl) {
        WishlistService.wishlistsScopeList.set(id, ctrl);
    }

    changeWishlistControlState(id, state) {
        const changedWishlistControl = WishlistService.wishlistsScopeList.get(id);
        if (changedWishlistControl != null) {
            changedWishlistControl.isAdded = state;
        }
    }
}

WishlistService.wishlistsScopeList = new Map();

export default WishlistService;



