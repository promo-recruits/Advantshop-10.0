/* @ngInject */
function CompareCtrl(compareService) {
    var ctrl = this;

    ctrl.add = function (offerId, state) {
        return compareService.add(offerId, state);
    };

    ctrl.remove = function (offerId, state) {
        return compareService.remove(offerId, state);
    };

    ctrl.change = function (offerId, state) {

        if (ctrl.isAdded) {
            ctrl.add(offerId, state);
        } else {
            ctrl.remove(offerId, state);
        }
    };

    ctrl.checkStatus = function (offerId) {
        compareService.getStatus(offerId).then(function (isAdded) {
            ctrl.isAdded = isAdded;
        });
    };
};

export default CompareCtrl;