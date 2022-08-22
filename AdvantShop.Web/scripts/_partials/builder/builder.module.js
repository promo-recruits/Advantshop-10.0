import './styles/builder.scss';
import './styles/new-builder.scss';
import './styles/new-builder-theme.scss';
import '../../../styles/common/sidebar.scss';
import '../../../styles/common/progressbar-css.scss';
import '../../../styles/common/spinner.scss';

import '../../../Areas/Admin/Content/src/_shared/switch-on-off/switchOnOff.js';

import cmStatModule from '../../../Areas/Admin/Content/src/_shared/cm-stat/cmStat.module.js';
import helpTriggerModule from '../../../Areas/Admin/Content/src/_partials/help-trigger/helpTrigger.module.js';
import uiAceTextareaModule from '../../../Areas/Admin/Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';
import colorsViewerModule from '../colors-viewer/colorsViewer.module.js';
import '../../../Areas/Admin/Content/src/csseditor/csseditor.js';


import tabsModule from '../../_common/tabs/tabs.module.js';

import BuilderCtrl from './controllers/BuilderController.js';
import { NewBuilderController, BuilderOtherSettingsController } from './controllers/NewBuilderController.js';
import {
    builderTriggerDirective,
    builderStylesheetDirective,
    newBuilderTriggerDirective,
    builderTriggerOtherSettingsDirective
} from './directives/builderDirectives.js';
import builderService from './services/builderService.js';

const moduleName = 'builder';

angular.module(moduleName, ['sidebarsContainer', tabsModule, cmStatModule, helpTriggerModule, uiAceTextareaModule, 'csseditor', colorsViewerModule])
    .constant('builderTypes', {
        colorScheme: 'colorScheme',
        theme: 'theme',
        background: 'background'
    })
    .service('builderService', builderService)
    .controller('BuilderCtrl', BuilderCtrl)
    .controller('NewBuilderCtrl', NewBuilderController)
    .controller('BuilderOtherSettingsCtrl', BuilderOtherSettingsController)
    .directive('builderTrigger', builderTriggerDirective)
    .directive('builderStylesheet', builderStylesheetDirective)
    .directive('newBuilderTrigger', newBuilderTriggerDirective)
    .directive('builderTriggerOtherSettings', builderTriggerOtherSettingsDirective);

export default moduleName;