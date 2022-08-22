import '../../../vendors/threesixty/styles/threesixty.css';
import '../../../vendors/threesixty/threesixty.js';

import RotateCtrl from './controllers/rotateController.js';
import { rotateDirective } from './directives/rotateDirectives.js';

const moduleName = 'rotate';

angular.module(moduleName, [])
    .controller('RotateCtrl', RotateCtrl)
    .directive('rotate', rotateDirective);

export default moduleName;