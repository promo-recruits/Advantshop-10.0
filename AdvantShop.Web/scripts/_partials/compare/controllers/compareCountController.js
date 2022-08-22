/* @ngInject */
function CompareCountCtrl(compareService) {
        var ctrl = this;

        ctrl.countObj = compareService.getCountObj();
};


export default CompareCountCtrl;