import './styles/colors-viewer.scss';

import ColorsViewerCtrl from './controllers/colorsViewerController.js';
import { colorsViewerDirective } from './directives/colorsViewerDirectives.js';

const moduleName = 'colorsViewer';

angular.module(moduleName, [])
    .controller('ColorsViewerCtrl', ColorsViewerCtrl)
    .directive('colorsViewer', colorsViewerDirective);
    
export default moduleName;
