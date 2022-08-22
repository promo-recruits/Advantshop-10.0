import LozadAdvCtrl from './lozadAdv.ctrl.js';
import lozadAdvConstants from './lozadAdv.constants.js';
import LozadAdv from './lozadAdv.directive.js';

const moduleName = `lozadAdv`;

angular.module(moduleName, [])
    .constant(`lozadAdvDefault`, lozadAdvConstants)
    .controller(`LozadAdvCtrl`, LozadAdvCtrl)
    .directive(`lozadAdv`, LozadAdv);
     

export default moduleName;
