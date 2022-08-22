﻿import './styles/bonus.scss';

import bonusService from './services/bonusService.js';
import BonusApplyCtrl from './controllers/bonusApplyController.js';
import BonusAuthCtrl from './controllers/bonusAuthController.js';
import BonusCodeCtrl from './controllers/bonusCodeController.js';
import BonusInfoCtrl from './controllers/bonusInfoController.js';
import BonusRegCtrl from './controllers/bonusRegController.js';
import BonusWhatToDoCtrl from './controllers/bonusWhatToDoController.js';
import {
    bonusWhatToDoDirective,
    bonusAuthDirective,
    bonusRegDirective,
    bonusApplyDirective,
    bonusInfoDirective,
    bonusCodeDirective
} from './directives/bonusDirectives.js';

const moduleName = 'bonus';

angular.module(moduleName, [])
    .service('bonusService', bonusService)
    .controller('BonusApplyCtrl', BonusApplyCtrl)
    .controller('BonusAuthCtrl', BonusAuthCtrl)
    .controller('BonusCodeCtrl', BonusCodeCtrl)
    .controller('BonusInfoCtrl', BonusInfoCtrl)
    .controller('BonusRegCtrl', BonusRegCtrl)
    .controller('BonusWhatToDoCtrl', BonusWhatToDoCtrl)
    .directive('bonusWhatToDo', bonusWhatToDoDirective)
    .directive('bonusAuth', bonusAuthDirective)
    .directive('bonusReg', bonusRegDirective)
    .directive('bonusApply', bonusApplyDirective)
    .directive('bonusInfo', bonusInfoDirective)
    .directive('bonusCode', bonusCodeDirective);

export default moduleName;