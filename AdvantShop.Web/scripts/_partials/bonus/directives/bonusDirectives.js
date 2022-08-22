function bonusWhatToDoDirective() {
    return {
        restrict: 'A',
        scope: {
            page: '@',
            autorizeBonus: '&',
            changeBonus: '&',
            email: '=',
            city: '=',
            outsideName: '=?',
            outsideSurname: '=?',
            outsidePhone: '=?',
            outsidePatronymic: '=?',
            isShowPatronymic: '&',
            isApply: '&',
            bonusPlus: '@'
        },
        controller: 'BonusWhatToDoCtrl',
        controllerAs: 'bonusWhatToDo',
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/bonus/templates/whatToDo.html'
    };
};

function bonusAuthDirective() {
    return {
        restrict: 'A',
        scope: {
            page: '=',
            callbackSuccess: '&',
            outsidePhone: '=?',
            enablePhoneMask: '<?'
        },
        controller: 'BonusAuthCtrl',
        controllerAs: 'bonusAuth',
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/bonus/templates/auth.html'
    };
};

function bonusRegDirective() {
    return {
        restrict: 'A',
        scope: {
            email: '=',
            city: '=',
            page: '=',
            outsideName: '=?',
            outsideSurname: '=?',
            outsidePhone: '=?',
            outsidePatronymic: '=?',
            isShowPatronymic: '=',
            callbackSuccess: '&',
            agreementDefaultChecked: '<?',
            enablePhoneMask: '<?'
        },
        controller: 'BonusRegCtrl',
        controllerAs: 'bonusReg',
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/bonus/templates/reg.html'
    };
};

function bonusApplyDirective() {
    return {
        restrict: 'A',
        replace: true,
        scope: {
            bonusText: '=',
            changeBonus: '&',
            isApply: '='
        },
        controller: 'BonusApplyCtrl',
        controllerAs: 'bonusApply',
        bindToController: true,
        templateUrl: '/scripts/_partials/bonus/templates/apply.html'
    };
};

function bonusInfoDirective() {
    return {
        restrict: 'A',
        replace: true,
        scope: {
            bonusData: '=',
            isShowPatronymic: '='
        },
        controller: 'BonusInfoCtrl',
        controllerAs: 'bonusInfo',
        bindToController: true,
        templateUrl: '/scripts/_partials/bonus/templates/info.html'
    };
};

function bonusCodeDirective() {
    return {
        restrict: 'A',
        replace: true,
        scope: {},
        controller: 'BonusCodeCtrl',
        controllerAs: 'bonusCode',
        bindToController: true,
        templateUrl: '/scripts/_partials/bonus/templates/code.html'
    };
};

export {
    bonusWhatToDoDirective,
    bonusAuthDirective,
    bonusRegDirective,
    bonusApplyDirective,
    bonusInfoDirective,
    bonusCodeDirective
}
