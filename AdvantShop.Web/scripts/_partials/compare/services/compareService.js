/* @ngInject */
function compareService($http) {
    var service = this,
        countObj = {};

    service.add = function (offerId, state) {
        return $http.post('/compare/addtocomparison', { offerId: offerId, rnd: Math.random() }).then(function (response) {

            countObj.count = response.data.Count;
            $(document).trigger("compare.add");
            service.changeCompareControlState(offerId, state);
            return response.data;
        });
    };

    service.remove = function (offerId, state) {
        return $http.get('/compare/removefromcompare', { params: { offerId: offerId, rnd: Math.random() } }).then(function (response) {

            countObj.count = response.data.Count;
            service.changeCompareControlState(offerId, state);
            return response.data;
        });
    };

    service.removeAll = function () {
        return $http.get('/compare/removeallfromcompare').then(function (response) {

            countObj.count = response.data.Count;

            return response.data;
        });
    };

    service.getCountObj = function () {
        return countObj;
    };

    service.getStatus = function (offerId) {
        return $http.get('/compare/getstatus', { params: { offerId: offerId, rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };

    service.addCompareScope = function (id, ctrl) {
        compareService.compareScopeList.set(id, ctrl);
    };

    service.changeCompareControlState = function (id, state) {
        const changedCompareControl = compareService.compareScopeList.get(id);
        if (changedCompareControl != null) {
            changedCompareControl.isAdded = state;
        }
    };
};

compareService.compareScopeList = new Map();

export default compareService;